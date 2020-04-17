using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI.Others;
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
        private ObservableCollection<Tuple<int, string>> QualityCollection = new ObservableCollection<Tuple<int, string>>();
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
        private bool _isCatchPointer = false;
        private bool _isMergeSameDanmaku = false; //合并相同弹幕
        private int _maxDanmakuNumber = 0;
        public bool IsFocus = false;
        public VideoTransportControls MTC;

        public event EventHandler<bool> FullWindowChanged;

        public VideoPlayer()
        {
            this.InitializeComponent();
            _danmaTimer.Interval = TimeSpan.FromSeconds(1);
            _danmaTimer.Tick += DanmuTimer_Tick;
            DanmakuControls = VideoMTC.DanmakuControls;
            MTC = VideoMTC;
        }

        public void Init()
        {
            ErrorContainer.Visibility = Visibility.Collapsed;
            bool isAutoPlay = AppTool.GetBoolSetting(Settings.IsAutoPlay);
            mediaElement.AutoPlay = isAutoPlay;
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
            else
            {
                _player.MediaEnded += Media_Ended;
                mediaElement.SetMediaPlayer(_player);
            }
            if (DanmakuControls != null)
                DanmakuControls.ClearAll();
            _danmaTimer.Start();
        }

        private void Media_Ended(MediaPlayer sender, object args)
        {
            if (VideoMTC.IsFullWindow)
                VideoMTC.IsFullWindow = false;
        }

        public async void RefreshVideoSource(int videoId, int partId)
        {
            Init();
            LoadingBar.Visibility = Visibility.Visible;
            var _mediaPlayer = mediaElement.MediaPlayer;
            if (_playData == null)
            {
                var data = await _videoService.GetVideoPlayAsync(videoId, partId, _currentQn);
                if (data != null)
                {
                    _playData = data;
                    for (int i = 0; i < _playData.accept_quality.Count; i++)
                    {
                        QualityCollection.Add(new Tuple<int, string>(_playData.accept_quality[i], _playData.accept_description[i]));
                    }
                    _currentQn = QualityCollection.First().Item1;
                }
            }
            if (_playData != null)
            {
                DanmakuList = (await _danmakuParse.ParseBiliBili(Convert.ToInt64(partId)));
                MediaSource mediaSource = null;
                if (_playData is VideoPlayDash)
                    mediaSource = await HandleDashSource();
                else
                    mediaSource = await HandleFlvSource(videoId);
                _tempSource = mediaSource;
                var currentTs = _mediaPlayer.TimelineControllerPositionOffset;
                _mediaPlayer.Source = new MediaPlaybackItem(mediaSource);
                _mediaPlayer.TimelineControllerPositionOffset = currentTs;

                VideoMTC.IsInit = false;
                VideoMTC.IsPlaying = mediaElement.AutoPlay;
                VideoMTC.IsInit = true;
            }
            else
            {
                ErrorContainer.Visibility = Visibility.Visible;
            }
            mediaElement.Focus(FocusState.Programmatic);
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void DanmuTimer_Tick(object sender, object e)
        {
            if (_pointerHoldCount >= 5 && _isCatchPointer && Window.Current.CoreWindow.PointerCursor!=null)
            {
                Window.Current.CoreWindow.PointerCursor = null;
            }
            if (_pointerHoldCount < 5)
                _pointerHoldCount++;
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

        public void Pause()
        {
            VideoMTC.IsPlaying = false;
            _danmaTimer.Stop();
        }

        public void Resume()
        {
            VideoMTC.IsPlaying = true;
            _danmaTimer.Start();
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
            var mediaSource = await _videoService.CreateMediaSourceAsync(video, audio);
            return mediaSource;
        }
        private async Task<MediaSource> HandleFlvSource(int videoId)
        {
            var playList = new Playlist(PlaylistTypes.NetworkHttp);
            var data = _playData as VideoPlayFlv;
            List<string> urls = new List<string>();
            data.durl.ForEach(p =>
            {
                urls.Add(p.url);
                playList.Append(p.url, 0, p.length / 1000);
            });
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

        private void mediaElement_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            _pointerHoldCount = 0;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 0);
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
    }
}
