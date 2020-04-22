using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BangumiPage : Page, IRefreshPage, IPlayerPage
    {
        private ObservableCollection<Episode> BangumiPartCollection = new ObservableCollection<Episode>();
        private ObservableCollection<BangumiStyle> TagCollection = new ObservableCollection<BangumiStyle>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private BangumiDetail _detail = null;
        private Episode _currentPart = null;
        private int bangumiId = 0;
        private bool isEp = false;
        public static BangumiPage Current;
        public BangumiPage()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("VideoConnectedAnimation");
                if (anim != null)
                {
                    anim.TryStart(VideoPlayer);
                }
                App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
                App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.Bangumi;
                if (e.Parameter is int aid)
                {
                    bangumiId = aid;
                    isEp = false;
                    await Refresh();
                }
                else if (e.Parameter is Tuple<int, bool> data)
                {
                    bangumiId = data.Item1;
                    isEp = data.Item2;
                    await Refresh();
                }
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VideoPlayer.Close();
            App.AppViewModel.CurrentVideoPlayer = null;
            App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.None;
            if (_currentPart != null)
                await _animeService.AddVideoHistoryAsync(_currentPart.aid, _currentPart.id, _currentPart.cid, VideoPlayer.CurrentProgress);
            Reset();
            base.OnNavigatedFrom(e);
        }
        private void Reset()
        {
            _detail = null;
            _currentPart = null;
            BangumiPartCollection.Clear();
            TagCollection.Clear();

            CoinButton.IsCheck = false;

            TitleBlock.Text = "--";
            PlayCountBlock.Text = "-";
            DanmukuCountBlock.Text = "-";
            ReplyCountBlock.Text = "-";
            RepostCountBlock.Text = "-";
            BasicInfoBlock.Text = "-";
            Section1Title.Text = "--";
            Section1Content.Text = "--";
            Section2Content.Text = "--";
            Section2Title.Text = "--";

            DescriptionBlock.Text = "--";
            CoverImage.Source = null;
            ScoreBlock.Text = "--";
            ScoreCountBlock.Text = "0人";
            RatingContainer.Visibility = Visibility.Visible;

            FollowButton.Style = UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = "追番";
        }
        public async Task Refresh()
        {
            Reset();
            var tip = new WaitingPopup("加载视频中...");
            tip.ShowPopup();
            var detail = await _animeService.GetBangumiDetailAsync(bangumiId, isEp);
            if (detail != null && detail.season_id > 0)
            {
                _detail = detail;
                InitDetail();
                CheckCoin();
                await VideoPlayer.Init(_detail, _currentPart);
            }
            tip.HidePopup();
        }

        private void InitDetail()
        {
            TitleBlock.Text = _detail.title;
            PlayCountBlock.Text = _detail.stat.play;
            DanmukuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmakus);
            RepostCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            ReplyCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);

            DescriptionBlock.Text = _detail.evaluate;
            ToolTipService.SetToolTip(DescriptionBlock, _detail.evaluate);

            if (_detail.rating != null)
            {
                ScoreBlock.Text = _detail.rating.score.ToString();
                ScoreCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.rating.count) + "人";
            }
            else
                RatingContainer.Visibility = Visibility.Collapsed;

            BasicInfoBlock.Text = $"{_detail.type_desc}\n{_detail.publish.release_date_show}\n{_detail.publish.time_length_show}";

            CoinButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.coins);

            CoverImage.Source = new BitmapImage(new Uri(_detail.cover + "@200w.jpg"));

            Section1Title.Text = _detail.actor.title;
            Section1Content.Text = _detail.actor.info;
            Section2Title.Text = _detail.staff.title;
            Section2Content.Text = _detail.staff.info;

            _detail.episodes.ForEach(p => BangumiPartCollection.Add(p));

            if (isEp)
            {
                for (int i = 0; i < _detail.episodes.Count; i++)
                {
                    var part = _detail.episodes[i];
                    if (part.id == bangumiId)
                    {
                        _currentPart = part;
                        PartListView.SelectedIndex = i;
                        PartListView.ScrollIntoView(part);
                        break;
                    }
                }
                bangumiId = _detail.season_id;
                
            }
            else if (_detail.user_status.progress != null)
            {
                _currentPart = _detail.episodes.Where(p => p.id == _detail.user_status.progress.last_ep_id).FirstOrDefault();
                PartListView.SelectedIndex = Convert.ToInt32(_detail.user_status.progress.last_ep_index) - 1;
                PartListView.ScrollIntoView(_currentPart);
            }

            if (_currentPart == null)
            {
                _currentPart = _detail.episodes.First();
                PartListView.SelectedIndex = 0;
            }
            if (_detail.styles != null && _detail.styles.Count > 0)
            {
                _detail.styles.ForEach(p => TagCollection.Add(p));
            }

            if (_detail.user_status != null)
            {
                FollowButton.Style = _detail.user_status.follow == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
                FollowButton.Text = _detail.user_status.follow == 1 ? "已追番" : "追番";
            }
        }
        public void RemovePlayer()
        {
            VideoContainer.Children.Clear();
        }
        public void InsertPlayer()
        {
            if (VideoContainer.Children.Count == 0)
                VideoContainer.Children.Add(App.AppViewModel.CurrentVideoPlayer);
            App.AppViewModel.CurrentVideoPlayer.Focus(FocusState.Programmatic);
        }

        private void VideoPlayer_FullWindowChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoFullScreen(e);
        }

        private void VideoPlayer_CinemaChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCinema(e);
        }
        private async void CoinCountButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int coin = Convert.ToInt32(btn.Tag.ToString());
            CoinButton.IsEnabled = false;
            bool result = await App.BiliViewModel._client.Video.GiveCoinToVideoAsync(_currentPart.aid, coin, false);
            if (result)
                new TipPopup("成功投币！").ShowMessage();
            else
                new TipPopup("投币失败").ShowError();
            CoinButton.IsCheck = result;
            CoinButton.IsEnabled = true;
        }

        private void CoinButton_Click(object sender, EventArgs e)
        {
            if (CoinButton.IsCheck)
                return;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FlyoutBase.ShowAttachedFlyout(CoinButton);
            }
        }
        private void ReplyCountBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", _currentPart.aid.ToString());
            param.Add("type", "1");
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Sub.ReplyPage), param);
        }

        private async void CheckCoin()
        {
            bool isCoin = await _animeService.CheckUserCoinAsync(_currentPart.id);
            CoinButton.IsCheck = isCoin;
        }

        private async void PartListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Episode;
            if (item.cid != _currentPart.cid)
            {
                _currentPart = item;
                await VideoPlayer.Init(_detail, _currentPart);
            }
        }
        private void TagListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as BangumiStyle;
            //App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Sub.Channel.TagDetailPage), item.tag_id);
        }
        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            if (_detail.user_status.follow == 1)
            {
                result = await _animeService.UnfollowBangumiAsync(_detail.season_id);
                if (result)
                {
                    _detail.user_status.follow = 0;
                    new TipPopup("已取消追番").ShowMessage();
                }
            }
            else
            {
                result = await _animeService.FollowBangumiAsync(_detail.season_id);
                if (result)
                {
                    _detail.user_status.follow = 1;
                    new TipPopup("追番成功").ShowMessage();
                }
            }
            FollowButton.Style = _detail.user_status.follow == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = _detail.user_status.follow == 1 ? "已追番" : "追番";
            if (!result)
                new TipPopup("操作失败").ShowError();
        }

        private void VideoPlayer_CompactOverlayChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCompactOverlay(e);
        }

        private void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Close();
            App.AppViewModel.PlayVideoSeparate(_detail, _currentPart);
        }
    }
}
