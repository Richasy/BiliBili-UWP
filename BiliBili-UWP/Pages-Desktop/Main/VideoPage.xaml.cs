using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using BiliBili_UWP.Models.UI.Others;
using NSDanmaku.Helper;
using SYEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.System.Display;
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
    public sealed partial class VideoPage : Page, IRefreshPage, IPlayerHost
    {
        private ObservableCollection<VideoPart> VideoPartCollection = new ObservableCollection<VideoPart>();
        private VideoDetail _detail = null;
        private ObservableCollection<VideoRelated> RelatedCollection = new ObservableCollection<VideoRelated>();
        private ObservableCollection<VideoTag> TagCollection = new ObservableCollection<VideoTag>();
        private ObservableCollection<Staff> StaffCollection = new ObservableCollection<Staff>();
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private ObservableCollection<VideoDetail> PlayBackupCollection = new ObservableCollection<VideoDetail>();

        private int _currentPartId = 0;
        private int videoId = 0;
        private string bvId = "";
        public static VideoPage Current;
        private List<FavoriteItem> _tempFavorites = new List<FavoriteItem>();
        private string _fromSign = "";
        private string _lastSelectPartType = "row";
        private bool _isPlayList = false;
        private bool _isCurrently = false;


        public VideoPage()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App._watch.Start();
            PageContainer.Visibility = Visibility.Collapsed;
            if (e.Parameter != null)
            {
                App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
                App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.Video;
                PlayBackupCollection.Clear();
                if (e.Parameter is Tuple<int, string> data)
                {
                    int aid = data.Item1;
                    _fromSign = data.Item2;
                    videoId = aid;
                    PlayListContainer.Visibility = Visibility.Collapsed;
                    _isPlayList = false;
                }
                else if (e.Parameter is Tuple<int, List<VideoDetail>> videoList)
                {
                    PlayListContainer.Visibility = Visibility.Visible;
                    videoList.Item2.ForEach(p => PlayBackupCollection.Add(p));
                    videoId = videoList.Item1;
                    _isPlayList = true;
                    _fromSign = "";
                }
                else if (e.Parameter is VideoActiveArgs args)
                {
                    if (args.aid > 0)
                    {
                        videoId = args.aid;
                        bvId = "";
                    }
                    else if (!string.IsNullOrEmpty(args.bvid))
                    {
                        videoId = 0;
                        bvId = args.bvid;
                    }
                    _currentPartId = args.cid;
                    PlayListContainer.Visibility = Visibility.Collapsed;
                    _isPlayList = false;
                    _fromSign = "";
                }
                if (_isCurrently && App.AppViewModel.IsVideoPageInit)
                    await Refresh();
            }
            _isCurrently = true;
        }

        private async void ConnectAnimation_Completed(ConnectedAnimation sender, object args)
        {
            sender = null;
            if (!App.AppViewModel.IsVideoPageInit)
                await Refresh();
            App.AppViewModel.IsVideoPageInit = true;
            UpdateLayout();
        }
        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            if (videoId > 0 && _currentPartId > 0)
                await _videoService.AddVideoHistoryAsync(videoId, _currentPartId, VideoPlayer.CurrentProgress);
            VideoPlayer.Close();
            if (e.SourcePageType != typeof(VideoPage))
                _isCurrently = false;
            App.AppViewModel.CurrentVideoPlayer = null;
            App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.None;
            Reset();
            App.AppViewModel.CurrentPagePanel.CheckSubReplyPage();
            base.OnNavigatingFrom(e);
        }
        private void Reset()
        {
            _detail = null;
            VideoPartCollection.Clear();
            RelatedCollection.Clear();
            TagCollection.Clear();
            StaffCollection.Clear();

            LikeButton.IsCheck = false;
            CoinButton.IsCheck = false;
            FavoriteButton.IsCheck = false;
            AutoLoopSwitch.IsOn = false;
            VideoPlayer.AutoLoop = false;

            PlayCountBlock.Text = "-";
            DanmukuCountBlock.Text = "-";
            CommentButton.Text = "评论";
            RepostButton.Text = "-";

            DescriptionBlock.Text = "--";
            UPAvatar.ProfilePicture = null;
            UPNameBlock.Text = "-";

            SelectLikeCheckBox.IsChecked = true;
            FavoriteListView.ItemsSource = null;

            FollowButton.Style = UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = "关注";

            if (_isPlayList)
            {
                int index = PlayBackupCollection.IndexOf(PlayBackupCollection.Where(p => p.aid == videoId).FirstOrDefault());
                PlayListContainer.ShowListView.SelectedIndex = index;
                if (index != -1)
                    PlayListContainer.ShowListView.ScrollIntoView(PlayBackupCollection[index], ScrollIntoViewAlignment.Leading);
            }
        }
        public async Task Refresh()
        {
            if (PageContainer.Visibility == Visibility.Collapsed)
                PageContainer.Visibility = Visibility.Visible;
            Reset();
            var tip = new WaitingPopup("加载视频中...");
            tip.ShowPopup();
            var detail = await _videoService.GetVideoDetailAsync(videoId, _fromSign, bvId);
            if (detail != null && detail.aid > 0)
            {
                _detail = detail;
                if (InitDetail())
                    await VideoPlayer.Init(_detail, _currentPartId);
            }
            App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, 0, 1);
            tip.HidePopup();
        }

        private bool InitDetail()
        {
            if (!string.IsNullOrEmpty(_detail.redirect_url))
            {
                var result = BiliTool.GetResultFromUri(_detail.redirect_url);
                videoId = 0;
                _currentPartId = 0;
                App.AppViewModel.CurrentPagePanel.RemoveMainHistory(Models.Enums.AppMenuItemType.VideoPlayer);
                if (result.Type == UriType.Bangumi)
                {
                    new TipPopup("正在转到专题...").ShowMessage();
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(result.Param), null, true);
                }
                return false;
            }
            if (_isPlayList)
                VideoPlayer.IsAutoReturnWhenEnd = false;
            else
                VideoPlayer.IsAutoReturnWhenEnd = _detail.pages.Count <= 1;
            TitleBlock.Text = _detail.title;
            ToolTipService.SetToolTip(TitleBlock, _detail.title);
            videoId = _detail.aid;
            bvId = _detail.bvid;
            PlayCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.view);
            DanmukuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmaku);
            RepostButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            CommentButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);
            BVBlock.Text = _detail.bvid;
            AVBlock.Text = _detail.aid.ToString();

            DescriptionBlock.Text = _detail.desc;

            LikeButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.like);
            CoinButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.coin);
            FavoriteButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.favorite);

            DateBlock.Text = AppTool.GetReadDateString(_detail.pubdate);

            UPAvatar.ProfilePicture = string.IsNullOrEmpty(_detail.owner.face) ? null : new BitmapImage(new Uri(_detail.owner.face + "@50w.jpg")) { DecodePixelWidth = 40 };
            UPNameBlock.Text = _detail.owner.name;

            _detail.pages.ForEach(p => VideoPartCollection.Add(p));
            PartListView.SelectedIndex = PartGridView.SelectedIndex = 0;
            PartContainer.Visibility = _detail.pages.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            if (_detail.tag != null && _detail.tag.Count > 0)
            {
                TagListView.Visibility = Visibility.Visible;
                _detail.tag.ForEach(p => TagCollection.Add(p));
            }
            else
            {
                TagListView.Visibility = Visibility.Collapsed;
            }

            if (_detail.staff != null && _detail.staff.Count > 0)
            {
                _detail.staff.ForEach(p => StaffCollection.Add(p));
                SingleUserContainer.Visibility = Visibility.Collapsed;
                StaffContainer.Visibility = Visibility.Visible;
            }
            else
            {
                StaffContainer.Visibility = Visibility.Collapsed;
                SingleUserContainer.Visibility = Visibility.Visible;
            }

            if (_detail.req_user != null)
            {
                LikeButton.IsCheck = _detail.req_user.like != 0;
                CoinButton.IsCheck = _detail.req_user.coin != 0;
                FavoriteButton.IsCheck = _detail.req_user.favorite != 0;
                FollowButton.Style = _detail.req_user.attention == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
                FollowButton.Text = _detail.req_user.attention == 1 ? "已关注" : "关注";
            }

            if (_detail.relates != null && _detail.relates.Count > 0)
            {
                _detail.relates.Where(p => p.@goto == "av").Take(10).ToList().ForEach(p => RelatedCollection.Add(p));
            }
            CheckLikeHoldState();
            return true;
        }

        private async void RelateVideoContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as VideoRelated;
            videoId = video.aid;
            _fromSign = "main.ugc-video-detail.0.0";
            App.AppViewModel.CurrentPagePanel.CheckSubReplyPage();
            await Refresh();
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
        private void CheckLikeHoldState()
        {
            LikeButton.CanHolding = !(LikeButton.IsCheck && CoinButton.IsCheck && FavoriteButton.IsCheck);
        }
        private async void LikeButton_Click(object sender, EventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                bool isLike = !LikeButton.IsCheck;
                LikeButton.IsEnabled = false;
                bool result = await _videoService.LikeVideoAsync(_detail.aid, isLike);
                if (result)
                {
                    string prompt = isLike ? "已点赞" : "已取消点赞";
                    _detail.req_user.like = isLike ? 1 : 0;
                    new TipPopup(prompt).ShowMessage();
                    LikeButton.IsCheck = !LikeButton.IsCheck;
                    UpdateVideoInfo();
                }
                else
                    new TipPopup("点赞操作失败").ShowError();
                LikeButton.IsEnabled = true;
                CheckLikeHoldState();
            }
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

        private async void FavoriteButton_Click(object sender, EventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FavoriteContainer.Visibility = Visibility.Collapsed;
                FavoriteProgressBar.Visibility = Visibility.Visible;
                FlyoutBase.ShowAttachedFlyout(FavoriteButton);
                var favoriteList = await _videoService.GetFavoritesAsync(_detail.aid, App.BiliViewModel._client.Account.Me.mid);
                FavoriteListView.ItemsSource = favoriteList;
                FavoriteContainer.Visibility = Visibility.Visible;
                FavoriteProgressBar.Visibility = Visibility.Collapsed;
                _tempFavorites = favoriteList;
            }
        }

        private async void CoinCountButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            int coin = Convert.ToInt32(btn.Tag.ToString());
            CoinButton.IsEnabled = false;
            bool result = await _videoService.GiveCoinToVideoAsync(_detail.aid, coin, Convert.ToBoolean(SelectLikeCheckBox.IsChecked));
            if (result)
            {
                if (Convert.ToBoolean(SelectLikeCheckBox.IsChecked))
                {
                    LikeButton.IsCheck = true;
                }
                new TipPopup("成功投币！").ShowMessage();
                UpdateVideoInfo();
                CoinFlyout.Hide();
            }
            else
                new TipPopup("投币失败").ShowError();
            CoinButton.IsCheck = result;
            CoinButton.IsEnabled = true;
            CheckLikeHoldState();
        }

        private async void FavoriteSureButton_Click(object sender, RoutedEventArgs e)
        {
            var removedList = new List<string>();
            var addList = new List<string>();
            var selectedItems = FavoriteListView.SelectedItems;
            foreach (var item in selectedItems)
            {
                var fav = item as FavoriteItem;
                if (fav.fav_state == 0)
                    addList.Add(fav.id.ToString());
            }
            foreach (var item in _tempFavorites)
            {
                if (item.fav_state == 1 && !selectedItems.Any(p => (p as FavoriteItem).id == item.id))
                {
                    removedList.Add(item.id.ToString());
                }
            }
            if (addList.Count > 0 || removedList.Count > 0)
            {
                FavoriteSureButton.IsLoading = true;
                bool result = await _videoService.AddVideoToFavoriteAsync(_detail.aid, addList, removedList);
                FavoriteSureButton.IsLoading = false;
                if (result)
                {
                    FavoriteFlyout.Hide();
                    FavoriteButton.IsCheck = selectedItems.Count > 0;
                    new TipPopup("已更改收藏夹").ShowMessage();
                    UpdateVideoInfo();
                }
                else
                {
                    new TipPopup("收藏失败").ShowError();
                }
            }
            CheckLikeHoldState();
        }

        private async void PartListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoPart;
            if (item.cid != _currentPartId)
            {
                _currentPartId = item.cid;
                await VideoPlayer.Init(_detail, _currentPartId);
            }
        }

        private void TagListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoTag;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.TagDetailPage), item.tag_id);
        }

        private void StaffListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Staff;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), item.mid);
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            bool result = false;
            if (_detail.req_user.attention == 1)
            {
                result = await App.BiliViewModel._client.Account.UnfollowUser(_detail.owner.mid);
                if (result)
                {
                    _detail.req_user.attention = -999;
                    new TipPopup("已取消关注").ShowMessage();
                }
            }
            else
            {
                result = await App.BiliViewModel._client.Account.FollowUser(_detail.owner.mid);
                if (result)
                {
                    _detail.req_user.attention = 1;
                    new TipPopup("关注成功").ShowMessage();
                }
            }
            FollowButton.Style = _detail.req_user.attention == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = _detail.req_user.attention == 1 ? "已关注" : "关注";
            if (!result)
                new TipPopup("操作失败").ShowError();
        }

        private void VideoPlayer_CompactOverlayChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCompactOverlay(e);
        }

        private async void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Close();
            App.AppViewModel.PlayVideoSeparate(_detail, _currentPartId, !_isPlayList);
            if (_isPlayList)
            {
                var dialog = new ConfirmDialog("您为一个视频开启了单独窗口播放，是否在当前页继续播放下一个视频？");
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await PlayNextVideo();
                }
            }
        }

        private void SingleUserContainer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var accId = _detail.owner.mid;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), accId);
        }

        private async void PlayListContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoDetail;
            videoId = item.aid;
            await Refresh();
        }

        private async void VideoPlayer_MediaEnded(object sender, int e)
        {
            if (_isPlayList)
            {
                await PlayNextVideo();
            }
        }
        private async Task PlayPreviousVideo()
        {
            int index = PlayBackupCollection.IndexOf(PlayBackupCollection.Where(p => p.aid == videoId).FirstOrDefault());
            if (index <= 0)
                new TipPopup("已经是列表里的第一个视频了").ShowMessage();
            else
            {
                var item = PlayBackupCollection[index - 1];
                videoId = item.aid;
                await Refresh();
            }
        }

        private async Task PlayNextVideo()
        {
            int index = PlayBackupCollection.IndexOf(PlayBackupCollection.Where(p => p.aid == videoId).FirstOrDefault());
            if (index >= PlayBackupCollection.Count - 1)
                new TipPopup("播放列表内的视频已经全部播放完啦~").ShowMessage();
            else
            {
                var item = PlayBackupCollection[index + 1];
                videoId = item.aid;
                await Refresh();
            }
        }

        private void VideoPlayer_PartSwitched(object sender, int e)
        {
            PartListView.SelectedIndex = PartGridView.SelectedIndex = e;
            PartListView.ScrollIntoView(VideoPartCollection[e], ScrollIntoViewAlignment.Leading);
        }

        private async void LikeButton_Hold(object sender, bool e)
        {
            if (e)
            {
                if (App.BiliViewModel.CheckAccoutStatus())
                {
                    bool result = await _videoService.TripleVideoAsync(_detail.aid);
                    if (result)
                    {
                        LikeButton.IsCheck = true;
                        CoinButton.IsCheck = true;
                        FavoriteButton.IsCheck = true;
                        CoinButton.ShowBubble();
                        FavoriteButton.ShowBubble();
                        new TipPopup("已一键三连~").ShowMessage();
                        UpdateVideoInfo();
                    }
                    else
                    {
                        new TipPopup("一键三连失败QAQ").ShowError();
                    }
                }
            }
        }

        private async void UpdateVideoInfo()
        {
            var data = await _videoService.GetVideoSlimAsync(_detail.aid);
            LikeButton.Text = AppTool.GetNumberAbbreviation(data.like);
            CoinButton.Text = AppTool.GetNumberAbbreviation(data.coin);
            FavoriteButton.Text = AppTool.GetNumberAbbreviation(data.favorite);
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", _detail.aid.ToString());
            param.Add("type", "1");
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.ReplyPage), param);
        }

        private void ShareDynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            string content = _detail.title + "\n" + (_detail.desc ?? "");
            App.AppViewModel.ShowRepostPopup(content, _detail);
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
            request.Data.Properties.Title = _detail.title;
            request.Data.Properties.Description = _detail.desc ?? "";
            if (!string.IsNullOrEmpty(_detail.desc))
                request.Data.SetText(_detail.desc);
            request.Data.SetWebLink(new Uri($"https://www.bilibili.com/video/{_detail.bvid}"));
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(_detail.pic)));
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
            if (App.AppViewModel.IsVideoPageInit)
                await Refresh();
            if (!string.IsNullOrEmpty(App.AppViewModel.ConnectAnimationName) && !App.AppViewModel.IsVideoPageInit)
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation(App.AppViewModel.ConnectAnimationName);
                if (anim != null)
                {
                    anim.Completed -= ConnectAnimation_Completed;
                    anim.Completed += ConnectAnimation_Completed;
                    anim.TryStart(VideoHolder);
                }
                App.AppViewModel.ConnectAnimationName = "";
            }
            else if (!App.AppViewModel.IsVideoPageInit)
            {
                await Refresh();
                App.AppViewModel.IsVideoPageInit = true;
            }
        }

        private void AutoLoopSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!App.AppViewModel.IsVideoPageInit)
                return;
            VideoPlayer.AutoLoop = AutoLoopSwitch.IsOn;
        }

        private async void VideoPlayer_PreviousVideoRequest(object sender, EventArgs e)
        {
            if (_isPlayList)
            {
                await PlayPreviousVideo();
            }
        }

        private async void VideoPlayer_NextVideoRequest(object sender, EventArgs e)
        {
            if (_isPlayList)
            {
                await PlayNextVideo();
            }
        }
    }
}
