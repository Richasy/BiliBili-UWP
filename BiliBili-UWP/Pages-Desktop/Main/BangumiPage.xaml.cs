using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BangumiPage : Page, IRefreshPage, IPlayerHost
    {
        private ObservableCollection<Episode> BangumiPartCollection = new ObservableCollection<Episode>();
        private ObservableCollection<BangumiStyle> TagCollection = new ObservableCollection<BangumiStyle>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private BangumiDetail _detail = null;
        private Episode _currentPart = null;
        private int bangumiId = 0;
        private bool isEp = false;
        private string _lastSelectPartType = "row";
        public static BangumiPage Current;
        private bool _isInit = false;
        private bool _isCurrently = false;
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
                App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
                App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.Bangumi;
                if (e.Parameter is int aid)
                {
                    bangumiId = aid;
                    isEp = false;
                }
                else if (e.Parameter is Tuple<int, bool> data)
                {
                    bangumiId = data.Item1;
                    isEp = data.Item2;
                }
                if (_isCurrently && _isInit)
                    await Refresh();
            }
            _isCurrently = true;
            
        }

        private async void ConnectAnimation_Completed(ConnectedAnimation sender, object args)
        {
            sender = null;
            UpdateLayout();
            if (!_isInit)
            {
                await Refresh();
                _isInit = true;
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.SourcePageType != typeof(BangumiPage))
                _isCurrently = false;
            if (_currentPart != null)
                await _animeService.AddVideoHistoryAsync(_currentPart.aid, _currentPart.cid, _currentPart.id, VideoPlayer.CurrentProgress, _detail.season_id);
            VideoPlayer.Close(); 
            App.AppViewModel.CurrentVideoPlayer = null;
            App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.None;
            App.AppViewModel.CurrentPagePanel.CheckSubReplyPage();
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
            CommentButton.Text = "评论";
            RepostButton.Text = "-";
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
            Section1Container.Visibility = Visibility.Visible;
            Section2Container.Visibility = Visibility.Visible;

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
                CheckCoin();
                if (await InitDetail())
                {
                    await VideoPlayer.Init(_detail, _currentPart);
                }
            }
            tip.HidePopup();
            App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, 0, 1);
        }

        private async Task<bool> InitDetail()
        {
            TitleBlock.Text = _detail.title;
            PlayCountBlock.Text = _detail.stat.play;
            DanmukuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmakus);
            RepostButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            CommentButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);

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
            if (string.IsNullOrEmpty(_detail.actor.info))
            {
                Section1Container.Visibility = Visibility.Collapsed;
            }
            Section2Title.Text = _detail.staff.title;
            Section2Content.Text = _detail.staff.info;
            if (string.IsNullOrEmpty(_detail.staff.info))
            {
                Section2Container.Visibility = Visibility.Collapsed;
            }

            _detail.episodes.ForEach(p => BangumiPartCollection.Add(p));
            if (isEp)
            {
                for (int i = 0; i < _detail.episodes.Count; i++)
                {
                    var part = _detail.episodes[i];
                    if (part.id == bangumiId)
                    {
                        _currentPart = part;
                        PartListView.SelectedIndex = PartGridView.SelectedIndex = i;
                        PartListView.ScrollIntoView(part, ScrollIntoViewAlignment.Leading);
                        PartGridView.ScrollIntoView(part, ScrollIntoViewAlignment.Leading);
                        break;
                    }
                }
                bangumiId = _detail.season_id;
            }
            else if (_detail.user_status.progress != null)
            {
                _currentPart = _detail.episodes.Where(p => p.id == _detail.user_status.progress.last_ep_id).FirstOrDefault();
                if (_currentPart != null)
                {
                    int lastIndex = _detail.episodes.IndexOf(_currentPart);
                    PartListView.SelectedIndex = PartGridView.SelectedIndex = lastIndex - 1 < -1 ? -1 : lastIndex;
                    PartListView.ScrollIntoView(_currentPart, ScrollIntoViewAlignment.Leading);
                    PartGridView.ScrollIntoView(_currentPart, ScrollIntoViewAlignment.Leading);
                }
            }

            if (_currentPart == null && _detail.episodes.Count > 0)
            {
                _currentPart = _detail.episodes.First();
                PartListView.SelectedIndex = PartGridView.SelectedIndex = 0;
            }
            if (_detail.styles != null && _detail.styles.Count > 0)
            {
                TagListView.Visibility = Visibility.Visible;
                _detail.styles.ForEach(p => TagCollection.Add(p));
            }
            else
            {
                TagListView.Visibility = Visibility.Collapsed;
            }

            CheckFollowButton();

            if (_detail.limit != null)
            {
                await new ConfirmDialog(_detail.limit.content).ShowAsync();
                return false;
            }
            return true;
        }
        private void CheckFollowButton()
        {
            if (_detail.user_status != null)
            {
                string followString = "已追剧";
                string unfollowString = "追剧";
                if (_detail.type == 1 || _detail.type == 2 || _detail.type == 4)
                {
                    followString = "已追番";
                    unfollowString = "追番";
                }
                FollowButton.Style = _detail.user_status.follow == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
                FollowButton.Text = _detail.user_status.follow == 1 ? followString : unfollowString;
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
            App.AppViewModel.CurrentVideoPlayer.ResetDanmakuStatus();
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
            {
                new TipPopup("成功投币！").ShowMessage();
                CoinFlyout.Hide();
            }
            else
                new TipPopup("投币失败").ShowError();
            CoinButton.IsCheck = result;
            CoinButton.IsEnabled = true;
        }

        private void CoinButton_Click(object sender, EventArgs e)
        {
            if (CoinButton.IsCheck || _currentPart == null)
                return;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FlyoutBase.ShowAttachedFlyout(CoinButton);
            }
        }

        private async void CheckCoin()
        {
            if (_currentPart != null)
            {
                bool isCoin = await _animeService.CheckUserCoinAsync(_currentPart.id);
                CoinButton.IsCheck = isCoin;
            }
        }

        private async void PartListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Episode;
            if (item.cid != _currentPart?.cid)
            {
                _currentPart = item;
                await VideoPlayer.Init(_detail, _currentPart);
            }
        }
        private void TagListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as BangumiStyle;
            string param = item.url.Split("?").Last().TrimStart('?');
            QueryString args = QueryString.Parse(param);
            var list = new List<KeyValueModel>();
            if (args.Count() > 0)
            {
                foreach (var query in args)
                {
                    list.Add(new KeyValueModel(query.Name, query.Value));
                }
                list.Add(new KeyValueModel("season_type", _detail.type.ToString()));
            }
            if (list.Count > 0)
            {
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Anime.IndexPage), list);
            }
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
            CheckFollowButton();
            if (!result)
                new TipPopup("操作失败").ShowError();
        }

        private void VideoPlayer_CompactOverlayChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCompactOverlay(e);
        }

        private void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            if (_currentPart != null)
            {
                VideoPlayer.Close();
                App.AppViewModel.PlayVideoSeparate(_detail, _currentPart);
            }
        }

        private void VideoPlayer_PartSwitched(object sender, int e)
        {
            PartListView.SelectedIndex= PartGridView.SelectedIndex = e;
            PartListView.ScrollIntoView(BangumiPartCollection[e], ScrollIntoViewAlignment.Leading);
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPart != null)
            {
                var param = new Dictionary<string, string>();
                param.Add("oid", _currentPart.aid.ToString());
                param.Add("type", "1");
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.ReplyPage), param);
            }
        }

        private void ShareDynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            string content = _currentPart.share_copy;
            App.AppViewModel.ShowRepostPopup(content, _detail, _currentPart);
        }

        private void ShareDataButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = _currentPart.share_copy;
            request.Data.Properties.Description = _detail.evaluate;
            if (!string.IsNullOrEmpty(_detail.evaluate))
                request.Data.SetText(_detail.evaluate);
            request.Data.SetWebLink(new Uri(_currentPart.share_url));
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(_currentPart.cover)));
        }

        private void RepostButton_Click(object sender, EventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
        }
        private void PartDsplayButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            string tag = btn.Tag.ToString();
            if (tag == _lastSelectPartType)
                return;
            _lastSelectPartType = tag;
            if (tag == "grid")
            {
                SingleRowButton.IsChecked = false;
                GridViewButton.IsChecked = true;
                PartListView.Visibility = Visibility.Collapsed;
                PartGridView.Visibility = Visibility.Visible;
                PartGridView.SelectedIndex = PartListView.SelectedIndex;
            }
            else
            {
                SingleRowButton.IsChecked = true;
                GridViewButton.IsChecked = false;
                PartListView.Visibility = Visibility.Visible;
                PartGridView.Visibility = Visibility.Collapsed;
                PartListView.SelectedIndex = PartGridView.SelectedIndex;
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isInit)
                await Refresh();
            if (!string.IsNullOrEmpty(App.AppViewModel.ConnectAnimationName))
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation(App.AppViewModel.ConnectAnimationName);
                if (anim != null)
                {
                    anim.Completed -= ConnectAnimation_Completed;
                    anim.Completed += ConnectAnimation_Completed;
                    anim.TryStart(VideoPlayer);
                }
                App.AppViewModel.ConnectAnimationName = "";
            }
            else if (!_isInit)
            {
                await Refresh();
                _isInit = true;
            }
        }
    }
}
