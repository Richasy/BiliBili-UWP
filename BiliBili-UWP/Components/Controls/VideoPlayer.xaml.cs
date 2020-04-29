using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Others;
using Microsoft.Toolkit.Uwp.Helpers;
using NSDanmaku.Helper;
using NSDanmaku.Model;
using SYEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.System;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class VideoPlayer : UserControl
    {
        private VideoPlayBase _playData = null;
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private ObservableCollection<Tuple<int, string>> QualityCollection = new ObservableCollection<Tuple<int, string>>();
        private ObservableCollection<Choice> ChoiceCollection = new ObservableCollection<Choice>();
        private List<DanmakuColor> DanmakuColors = DanmakuColor.GetColorList();
        private DanmakuParse _danmakuParse = new DanmakuParse();
        private int _currentQn = 64;
        private MediaPlayer _player = new MediaPlayer();
        private MediaSource _tempSource = null;
        private NSDanmaku.Controls.Danmaku DanmakuControls = null;
        private List<DanmakuModel> DanmakuList = new List<DanmakuModel>();
        private List<string> ShieldTextLish = new List<string>(); //屏蔽关键字列表
        private List<string> SendDanmakuList = new List<string>();
        private DispatcherTimer _danmaTimer = new DispatcherTimer();
        private int _pointerHoldCount = 0; // 光标保持不动的持续时间
        private int _heartBeatCount = 0;
        private bool _isCatchPointer = false;
        private bool _isMergeSameDanmaku = false; //合并相同弹幕
        private int _maxDanmakuNumber = 0;
        public bool IsFocus = false;
        public VideoTransportControls MTC;
        private DisplayRequest dispRequest = null;
        private InteractionVideo _interaction = null;

        private int _videoId = 0;
        private int _partId = 0;
        private Episode _bangumiPart = null;

        private VideoDetail _videoDetail = null;
        private BangumiDetail _bangumiDetail = null;

        public bool isBangumi = false;

        public event EventHandler<bool> FullWindowChanged;
        public event EventHandler<bool> CinemaChanged;
        public event EventHandler<bool> CompactOverlayChanged;
        public event RoutedEventHandler SeparateButtonClick;
        public event EventHandler MTCLoaded;

        private bool _isChoiceHandling = false;
        private bool _isMTCShow = false;

        public int CurrentProgress
        {
            get => Convert.ToInt32(_player.PlaybackSession.Position.TotalSeconds);
        }

        public VideoPlayer()
        {
            this.InitializeComponent();
            _danmaTimer.Interval = TimeSpan.FromSeconds(1);
            _danmaTimer.Tick += DanmuTimer_Tick;
            DanmakuControls = VideoMTC.DanmakuControls;
            MTC = VideoMTC;
        }

        public async Task Init(VideoDetail detail, int cid = 0)
        {
            _videoDetail = detail;
            _videoId = detail.aid;
            isBangumi = false;
            if (cid == 0)
            {
                if (detail != null && detail.pages != null)
                    cid = detail.pages.First().cid;
            }
            if (detail.interaction != null)
            {
                await InitInteraction(cid, 0);
            }
            else
            {
                Reset();
                await RefreshVideoSource(cid);
            }
        }

        public async Task Init(BangumiDetail detail, Episode part)
        {
            isBangumi = true;
            _bangumiDetail = detail;
            _bangumiPart = part;
            Reset();
            await RefreshVideoSource(part);
        }

        private void Reset()
        {
            ErrorContainer.Visibility = Visibility.Collapsed;
            bool isAutoPlay = AppTool.GetBoolSetting(Settings.IsAutoPlay);
            mediaElement.AutoPlay = isAutoPlay;
            bool isShowDanmaku = AppTool.GetBoolSetting(Settings.IsDanmakuOpen);
            DanmakuVisibilityButton.Content = isShowDanmaku ? "" : "";
            DanmakuColorItemsControl.ItemsSource = DanmakuColors;
            ColorTextBox.Text = "#FFFFFF";
            DefaultFontSizeRadio.IsChecked = true;
            ModeComboBox.SelectedIndex = 0;
            ColorViewBorder.Background = new SolidColorBrush(Windows.UI.Colors.White);
            DanmakuBox.Text = "";
            _maxDanmakuNumber = Convert.ToInt32(AppTool.GetLocalSetting(Settings.MaxDanmuNumber, "200"));
            _playData = null;
            QualityCollection.Clear();
            DanmakuList.Clear();
            SendDanmakuList.Clear();
            if (mediaElement.MediaPlayer != null)
            {
                mediaElement.MediaPlayer.Pause();
                mediaElement.MediaPlayer.TimelineControllerPositionOffset = TimeSpan.FromSeconds(0);
                if (_tempSource != null)
                {
                    _tempSource.Dispose();
                }
            }
            _player = new MediaPlayer();
            _player.MediaEnded += Media_Ended;
            mediaElement.SetMediaPlayer(_player);
            if (DanmakuControls != null)
                DanmakuControls.ClearAll();
            _danmaTimer.Start();
        }

        private async void Media_Ended(MediaPlayer sender, object args)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                VideoMTC.IsPlaying = false;
                if (_videoDetail.interaction != null)
                {
                    //互动视频
                    if (ChoiceCollection.Count > 0)
                    {
                        if (ChoiceCollection.Count == 1)
                            await InitInteraction(ChoiceCollection.First().cid, ChoiceCollection.First().id);
                        else
                            ChoiceItemsControl.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (VideoMTC.IsFullWindow)
                        VideoMTC.IsFullWindow = false;
                    else if (VideoMTC.IsCinema)
                        VideoMTC.IsCinema = false;
                }
            });
        }

        public async Task RefreshVideoSource(int partId)
        {
            LoadingBar.Visibility = Visibility.Visible;
            var _mediaPlayer = mediaElement.MediaPlayer;
            if (_playData == null || _partId != partId)
            {
                _partId = partId;
                var data = await _videoService.GetVideoPlayAsync(_videoId, partId, _currentQn);
                if (data != null)
                {
                    _playData = data;
                    for (int i = 0; i < _playData.accept_quality.Count; i++)
                    {
                        QualityCollection.Add(new Tuple<int, string>(_playData.accept_quality[i], _playData.accept_description[i]));
                    }
                    VideoMTC._qualityListView.SelectedIndex = -1;
                    _currentQn = QualityCollection.First().Item1;
                    VideoMTC._qualityListView.SelectedIndex = 0;
                }
            }
            if (_playData != null)
            {
                await LoadDanmaku();
                MediaSource mediaSource = null;
                if (_playData is VideoPlayDash)
                    mediaSource = await HandleDashSource();
                else
                    mediaSource = await HandleFlvSource(_videoId);
                if (mediaSource != null)
                {
                    var offset = TimeSpan.FromSeconds(_mediaPlayer.PlaybackSession.Position.TotalSeconds);
                    var other = _tempSource;
                    _tempSource = mediaSource;
                    _mediaPlayer.Source = new MediaPlaybackItem(mediaSource);
                    if (offset.TotalSeconds > 0)
                        _player.PlaybackSession.Position = offset;
                    other?.Dispose();

                    VideoMTC.IsInit = false;
                    VideoMTC.IsPlaying = mediaElement.AutoPlay;
                    VideoMTC.IsInit = true;
                    if (mediaElement.AutoPlay)
                        Resume();
                }
                else
                    ErrorContainer.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorContainer.Visibility = Visibility.Visible;
            }
            mediaElement.Focus(FocusState.Programmatic);
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        public async Task RefreshVideoSource(Episode part)
        {
            if (part == null)
            {
                ErrorContainer.Visibility = Visibility.Visible;
                return;
            }
            LoadingBar.Visibility = Visibility.Visible;
            if (_playData == null || _bangumiPart.id != part.id)
            {
                _bangumiPart = part;
                var data = await _animeService.GetBangumiPlayAsync(_bangumiDetail.type, part.cid, _currentQn);
                if (data != null)
                {
                    _playData = data;
                    for (int i = 0; i < _playData.accept_quality.Count; i++)
                    {
                        QualityCollection.Add(new Tuple<int, string>(_playData.accept_quality[i], _playData.accept_description[i]));
                    }
                    _currentQn = QualityCollection.First().Item1;
                    VideoMTC.QualitySelectIndex = 0;
                }
            }
            if (_playData != null)
            {
                await LoadDanmaku();
                MediaSource mediaSource = null;
                if (_playData is VideoPlayDash)
                    mediaSource = await HandleDashSource();
                else
                    mediaSource = await HandleFlvSource(_videoId, true);
                var other = _tempSource;
                _tempSource = mediaSource;
                _player.Source = new MediaPlaybackItem(mediaSource);
                other?.Dispose();

                VideoMTC.IsInit = false;
                VideoMTC.IsPlaying = mediaElement.AutoPlay;
                VideoMTC.IsInit = true;
                Resume();
            }
            else
            {
                ErrorContainer.Visibility = Visibility.Visible;
            }
            mediaElement.Focus(FocusState.Programmatic);
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private async Task LoadDanmaku()
        {
            bool isShow = AppTool.GetBoolSetting(Settings.IsDanmakuOpen);
            DanmakuList.Clear();
            if (isShow)
            {
                if (isBangumi)
                    DanmakuList = await _danmakuParse.ParseBiliBili(Convert.ToInt64(_bangumiPart.cid));
                else
                    DanmakuList = await _danmakuParse.ParseBiliBili(Convert.ToInt64(_partId));
            }
        }

        private void DanmuTimer_Tick(object sender, object e)
        {
            if (_pointerHoldCount >= 3)
            {
                if (_isCatchPointer && Window.Current.CoreWindow.PointerCursor != null)
                    Window.Current.CoreWindow.PointerCursor = null;
                if (_isMTCShow)
                {
                    _isMTCShow = false;
                    VideoMTC.Hide();
                }
            }
            if (_pointerHoldCount < 3)
                _pointerHoldCount++;
            if (_heartBeatCount >= 10)
            {
                HeartBeat();
                _heartBeatCount = 0;
            }
            else
                _heartBeatCount++;
            try
            {
                if (DanmakuControls == null)
                    return;
                if (mediaElement.MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing && DanmakuList.Count > 0)
                {
                    int nowDanmaNum = 0;
                    var currentPosition = mediaElement.MediaPlayer.PlaybackSession.Position.TotalSeconds;
                    var tempList = DanmakuList.Where(p => p.time > currentPosition && p.time - currentPosition < 1).ToList();
                    if (tempList.Count > 0)
                    {
                        foreach (var item in tempList)
                        {
                            if (nowDanmaNum >= _maxDanmakuNumber && _maxDanmakuNumber != 0)
                                break;
                            if (!IsTextShouldShield(item.text))
                            {
                                switch (item.location)
                                {
                                    case DanmakuLocation.Top:
                                        DanmakuControls.AddTopDanmu(item, false);
                                        break;
                                    case DanmakuLocation.Bottom:
                                        DanmakuControls.AddBottomDanmu(item, false);
                                        break;
                                    case DanmakuLocation.Position:
                                        DanmakuControls.AddPositionDanmu(item);
                                        break;
                                    default:
                                        DanmakuControls.AddRollDanmu(item, false);
                                        break;
                                }
                                nowDanmaNum++;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private async void HeartBeat()
        {
            if (isBangumi)
                await _animeService.AddVideoHistoryAsync(_bangumiPart.aid, _bangumiPart.id, _bangumiPart.cid, CurrentProgress);
            else
                await _videoService.AddVideoHistoryAsync(_videoId, _partId, CurrentProgress);
        }

        public void Pause()
        {
            VideoMTC.IsPlaying = false;
            _danmaTimer.Stop();
            if (dispRequest != null)
            {
                dispRequest.RequestRelease();
                dispRequest = null;
            }
        }

        public void Resume()
        {
            VideoMTC.IsPlaying = true;
            _danmaTimer.Start();
            if (dispRequest == null)
            {
                // 用户观看视频，需要保持屏幕的点亮状态
                dispRequest = new DisplayRequest();
                dispRequest.RequestActive(); // 激活显示请求
            }
        }

        public void Close()
        {
            Pause();
            DanmakuList.Clear();
            if (_tempSource != null)
                _tempSource.Dispose();
        }

        private async Task<MediaSource> HandleDashSource()
        {
            bool isHevc = AppTool.GetBoolSetting(Settings.IsUseHevc, false);
            var data = _playData as VideoPlayDash;
            int codecId = isHevc ? 12 : 7;
            var video = data.dash.video.FirstOrDefault(p => p.id == _currentQn && p.codecid == codecId);
            if (video == null && codecId == 12)
                video = data.dash.video.FirstOrDefault(p => p.id == _currentQn);
            if (video == null)
                video = data.dash.video.OrderByDescending(p => p.id).FirstOrDefault(p => p.codecid == 7);
            var audio = data.dash.audio.FirstOrDefault();
            MediaSource source = null;
            if (isBangumi)
                source = await _animeService.CreateMediaSourceAsync(video, audio);
            else
                source = await _videoService.CreateMediaSourceAsync(video, audio);
            return source;
        }
        private async Task<MediaSource> HandleFlvSource(int videoId, bool isBangumi = false)
        {
            var playList = new Playlist(PlaylistTypes.NetworkHttp);
            var data = _playData as VideoPlayFlv;
            List<string> urls = new List<string>();
            data.durl.ForEach(p =>
            {
                urls.Add(p.url);
                playList.Append(p.url, 0, p.length / 1000);
            });
            string prefix = isBangumi ? "https://www.bilibili.com/bangumi/play/ep" : "https://www.bilibili.com/video/av";
            playList.NetworkConfigs = CreatePlaylistNetworkConfigs("https://www.bilibili.com/video/av" + videoId + "/");
            var mediaSouce = MediaSource.CreateFromUri(await playList.SaveAndGetFileUriAsync());
            return mediaSouce;
        }

        private PlaylistNetworkConfigs CreatePlaylistNetworkConfigs(string referer)
        {
            PlaylistNetworkConfigs config = new PlaylistNetworkConfigs();
            config.DownloadRetryOnFail = true;
            config.HttpCookie = string.Empty;
            config.UniqueId = string.Empty;
            config.HttpUserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
            config.HttpReferer = referer;
            return config;
        }

        /// <summary>
        /// 弹幕是否该被屏蔽（不显示）
        /// </summary>
        /// <param name="text">弹幕文本</param>
        /// <param name="location">弹幕位置</param>
        /// <returns></returns>
        private bool IsTextShouldShield(string text, DanmakuLocation location = DanmakuLocation.Other)
        {
            if (ShieldTextLish.Any(s => text.Contains(s, StringComparison.OrdinalIgnoreCase)))
                return true;
            if (_isMergeSameDanmaku && SendDanmakuList.Contains(text + location))
                return true;
            return false;
        }

        private void VideoMTC_DanmakuLoaded(object sender, NSDanmaku.Controls.Danmaku e)
        {
            DanmakuControls = e;
            VideoMTC.MediaPlayerElement = mediaElement;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DanmakuControls != null)
                DanmakuControls.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height) };
        }

        private void VideoMTC_FullWindowChanged(object sender, bool e)
        {
            FullWindowChanged?.Invoke(this, e);
        }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _isCatchPointer = true;
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            _isCatchPointer = false;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void VideoMTC_PlayButtonClick(object sender, bool e)
        {
            if (e)
                _danmaTimer.Start();
            else
                _danmaTimer.Stop();
            this.Focus(FocusState.Programmatic);
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("已获取焦点");
            IsFocus = true;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("失去焦点");
            IsFocus = false;
        }

        private void VideoMTC_CinemaChanged(object sender, bool e)
        {
            CinemaChanged?.Invoke(this, e);
        }

        private async void VideoMTC_QualityChanged(object sender, int e)
        {
            if (_currentQn != e)
            {
                _currentQn = e;
                await RefreshVideoSource(_partId);
            }
        }

        private void VideoMTC_CompactOverlayButtonClick(object sender, bool e)
        {
            CompactOverlayChanged?.Invoke(this, e);
        }

        private void VideoMTC_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            SeparateButtonClick?.Invoke(this, e);
        }
        public Visibility CinemaButtonVisibility
        {
            get { return (Visibility)GetValue(CinemaButtonVisibilityProperty); }
            set { SetValue(CinemaButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CinemaButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CinemaButtonVisibilityProperty =
            DependencyProperty.Register("CinemaButtonVisibility", typeof(Visibility), typeof(VideoPlayer), new PropertyMetadata(Visibility.Visible));

        public Visibility FullWindowButtonVisibility
        {
            get { return (Visibility)GetValue(FullWindowButtonVisibilityProperty); }
            set { SetValue(FullWindowButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FullWindowButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FullWindowButtonVisibilityProperty =
            DependencyProperty.Register("FullWindowButtonVisibility", typeof(Visibility), typeof(VideoPlayer), new PropertyMetadata(Visibility.Visible));

        public Visibility CompactOverlayButtonVisibility
        {
            get { return (Visibility)GetValue(CompactOverlayButtonVisibilityProperty); }
            set { SetValue(CompactOverlayButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompactOverlayButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompactOverlayButtonVisibilityProperty =
            DependencyProperty.Register("CompactOverlayButtonVisibility", typeof(Visibility), typeof(VideoPlayer), new PropertyMetadata(Visibility.Visible));

        public Visibility SeparateButtonVisibility
        {
            get { return (Visibility)GetValue(SeparateButtonVisibilityProperty); }
            set { SetValue(SeparateButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SeparateButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparateButtonVisibilityProperty =
            DependencyProperty.Register("SeparateButtonVisibility", typeof(Visibility), typeof(VideoPlayer), new PropertyMetadata(Visibility.Visible));

        public int ChoiceRows
        {
            get { return (int)GetValue(ChoiceRowsProperty); }
            set { SetValue(ChoiceRowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChoiceRows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChoiceRowsProperty =
            DependencyProperty.Register("ChoiceRows", typeof(int), typeof(VideoPlayer), new PropertyMetadata(1));



        private void VideoMTC_Loaded(object sender, RoutedEventArgs e)
        {
            MTCLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async Task InitInteraction(int cid, int edgeId)
        {
            if (_isChoiceHandling)
                return;
            _isChoiceHandling = true;
            Reset();
            var data = await _videoService.GetInteractionVideoAsync(_videoId, _videoDetail.interaction.graph_version, edgeId);
            ChoiceItemsControl.Visibility = Visibility.Collapsed;
            await RefreshVideoSource(cid);
            if (data != null)
            {
                ChoiceCollection.Clear();
                _interaction = data;
                if (_interaction.edges.questions != null)
                {
                    var choices = _interaction.edges.questions.First().choices;
                    choices.ForEach(p => ChoiceCollection.Add(p));
                }
            }
            _isChoiceHandling = false;
        }

        private async void Choice_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Choice;
            await InitInteraction(data.cid, data.id);
        }

        private void ChoiceItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ((sender as ItemsControl).ItemsPanelRoot as ItemsWrapGrid).ItemWidth = e.NewSize.Width / 2;
        }

        public Visibility DanmakuBarVisibility
        {
            get { return (Visibility)GetValue(DanmakuBarVisibilityProperty); }
            set { SetValue(DanmakuBarVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DanmakuBarVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DanmakuBarVisibilityProperty =
            DependencyProperty.Register("DanmakuBarVisibility", typeof(Visibility), typeof(VideoPlayer), new PropertyMetadata(Visibility.Visible));

        private async void DanmakuVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            bool isShowDanmaku = AppTool.GetBoolSetting(Settings.IsDanmakuOpen);
            isShowDanmaku = !isShowDanmaku;
            AppTool.WriteLocalSetting(Settings.IsDanmakuOpen, isShowDanmaku.ToString());
            DanmakuVisibilityButton.Content = isShowDanmaku ? "" : "";
            await LoadDanmaku();
        }

        private async void SendDanmakuButton_Click(object sender, RoutedEventArgs e)
        {
            await SendDanmaku();
        }

        private async void DanmakuBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await SendDanmaku();
            }
        }

        private async Task SendDanmaku()
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            if (_playData == null || _player.PlaybackSession.PlaybackState == MediaPlaybackState.Opening)
            {
                new TipPopup("请等待视频加载完成").ShowMessage();
                return;
            }
            string text = DanmakuBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                SendDanmakuButton.IsEnabled = false;
                DanmakuBox.IsEnabled = false;
                string color = "";
                if (!string.IsNullOrEmpty(ColorTextBox.Text))
                    color = UIHelper.GetDanmakuColor(ColorHelper.ToColor(ColorTextBox.Text));
                else
                    color = UIHelper.GetDanmakuColor(Windows.UI.Colors.White);
                string fontSize = Convert.ToBoolean(DefaultFontSizeRadio.IsChecked) ? "25" : "20";
                string mode = (ModeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
                bool result = false;
                double progress = _player.PlaybackSession.Position.TotalMilliseconds;
                if (isBangumi)
                    result = await _videoService.SendDanmakuAsync(text, _bangumiPart.aid, _bangumiPart.cid, progress, color, fontSize, mode);
                else
                    result = await _videoService.SendDanmakuAsync(text, _videoId, _partId, progress, color, fontSize, mode);
                if (result)
                {
                    DanmakuBox.Text = string.Empty;
                    int fontSizeNum = Convert.ToInt32(fontSize);
                    if (mode == "1")
                        DanmakuControls.AddRollDanmu(new DanmakuModel { color = ColorHelper.ToColor(ColorTextBox.Text), size = fontSizeNum, text = text }, true);
                    else if (mode == "4")
                        DanmakuControls.AddBottomDanmu(new DanmakuModel { color = ColorHelper.ToColor(ColorTextBox.Text), size = fontSizeNum, text = text }, true);
                    else if (mode == "5")
                        DanmakuControls.AddTopDanmu(new DanmakuModel { color = ColorHelper.ToColor(ColorTextBox.Text), size = fontSizeNum, text = text }, true);
                    else
                        DanmakuControls.AddRollDanmu(new DanmakuModel { color = ColorHelper.ToColor(ColorTextBox.Text), size = fontSizeNum, text = text }, true);
                }
                else
                {
                    new TipPopup("发送失败").ShowError();
                }
                // send
                SendDanmakuButton.IsEnabled = true;
                DanmakuBox.IsEnabled = true;
            }
        }

        private void UserControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _pointerHoldCount = 0;
            if (!_isMTCShow)
            {
                _isMTCShow = true;
                VideoMTC.Show();
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
        }

        private void DanmakuColor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as DanmakuColor;
            ColorTextBox.Text = data.Color.Color.ToString();
            ColorViewBorder.Background = data.Color;
        }
    }
}
