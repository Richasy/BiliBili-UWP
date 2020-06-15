using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class TabletVideoDetailBlock : UserControl, IPlayerHost
    {
        private ObservableCollection<VideoPart> VideoPartCollection = new ObservableCollection<VideoPart>();
        public VideoDetail _detail = null;
        private ObservableCollection<VideoTag> TagCollection = new ObservableCollection<VideoTag>();
        private ObservableCollection<Staff> StaffCollection = new ObservableCollection<Staff>();
        private ObservableCollection<VideoDetail> PlayBackupCollection = new ObservableCollection<VideoDetail>();
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private int _currentPartId = 0;
        private int videoId = 0;
        private string bvId = "";
        private List<FavoriteItem> _tempFavorites = new List<FavoriteItem>();
        public VideoPlayer VideoPlayer;
        public TabletVideoDetailBlock()
        {
            this.InitializeComponent();
        }
        public void Reset()
        {
            _detail = null;
            VideoPartCollection.Clear();
            TagCollection.Clear();
            StaffCollection.Clear();

            LikeButton.IsCheck = false;
            CoinButton.IsCheck = false;
            FavoriteButton.IsCheck = false;

            PlayCountBlock.Text = "-";
            DanmakuCountBlock.Text = "-";
            CommentButton.Text = "-";
            RepostButton.Text = "-";

            DescriptionBlock.Text = "--";
            UPAvatar.ProfilePicture = null;
            UPNameBlock.Text = "-";

            SelectLikeCheckBox.IsChecked = true;
            FavoriteListView.ItemsSource = null;

            if (PlayBackupCollection.Count > 0)
            {
                int index = PlayBackupCollection.IndexOf(PlayBackupCollection.Where(p => p.aid == videoId).FirstOrDefault());
                PlayListContainer.ShowListView.SelectedIndex = index;
                if (index != -1)
                    PlayListContainer.ShowListView.ScrollIntoView(PlayBackupCollection[index], ScrollIntoViewAlignment.Leading);
            }
        }

        public async Task Init(int startId, List<VideoDetail> playList)
        {
            PlayListContainer.Visibility = Visibility.Visible;
            playList.ForEach(p => PlayBackupCollection.Add(p));
            videoId = startId;
            await Init(startId);
        }

        public void ClearPlayList()
        {
            PlayBackupCollection.Clear();
            PlayListContainer.Visibility = Visibility.Collapsed;
        }

        public async Task Init(int aid, int partId = 0, string bvId = "")
        {
            LoadingRing.IsActive = true;
            MyVideoPlayer.Close();
            DetailContainer.Visibility = Visibility.Collapsed;
            var detail = await _videoService.GetVideoDetailAsync(aid, bvId: bvId);
            LoadingRing.IsActive = false;
            DetailContainer.Visibility = Visibility.Visible;
            if (detail != null)
            {
                await Init(detail, partId);
            }
        }

        public async Task Init(VideoDetail detail, int partId = 0)
        {
            Reset();
            App.AppViewModel.CurrentPlayerType = Models.Enums.PlayerType.Video;
            App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
            _detail = detail;
            _currentPartId = partId;
            TabletMainPage.Current.SetBackgroundImage(_detail.pic);
            if (!string.IsNullOrEmpty(_detail.redirect_url))
            {
                var result = BiliTool.GetResultFromUri(_detail.redirect_url);
                videoId = 0;
                _currentPartId = 0;
                if (result.Type == UriType.Bangumi)
                {
                    throw new InvalidDataException(result.Param);
                }
            }
            TitleBlock.Text = detail.title;
            ToolTipService.SetToolTip(TitleBlock, _detail.title);
            videoId = _detail.aid;
            bvId = _detail.bvid;
            PlayCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.view);
            DanmakuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmaku);
            RepostButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            CommentButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);
            BVBlock.Text = _detail.bvid;
            AVBlock.Text = _detail.aid.ToString();

            DescriptionBlock.Text = _detail.desc;

            LikeButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.like);
            CoinButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.coin);
            FavoriteButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.favorite);

            PublishBlock.Text = AppTool.GetReadDateString(_detail.pubdate);

            UPAvatar.ProfilePicture = string.IsNullOrEmpty(_detail.owner.face) ? null : new BitmapImage(new Uri(_detail.owner.face + "@50w.jpg")) { DecodePixelWidth = 40 };
            UPNameBlock.Text = _detail.owner.name;

            if (_detail.pages != null)
            {
                _detail.pages.ForEach(p => VideoPartCollection.Add(p));
                PartGridView.SelectedIndex = 0;
            }
            PartContainer.Visibility = _detail.pages.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            if (_detail.tag != null && _detail.tag.Count > 0)
            {
                TagGridView.Visibility = Visibility.Visible;
                _detail.tag.ForEach(p => TagCollection.Add(p));
            }
            else
            {
                TagGridView.Visibility = Visibility.Collapsed;
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
            }

            await MyVideoPlayer.Init(_detail, _currentPartId);

            CheckLikeHoldState();
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

        private async void PartGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoPart;
            if (item.cid != _currentPartId)
            {
                _currentPartId = item.cid;
                await MyVideoPlayer.Init(_detail, _currentPartId);
            }
        }

        private void TagGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoTag;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.TagDetailPage), item.tag_id);
        }

        private void StaffListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Staff;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), item.mid);
        }
        private async void UpdateVideoInfo()
        {
            var data = await _videoService.GetVideoSlimAsync(_detail.aid);
            LikeButton.Text = AppTool.GetNumberAbbreviation(data.like);
            CoinButton.Text = AppTool.GetNumberAbbreviation(data.coin);
            FavoriteButton.Text = AppTool.GetNumberAbbreviation(data.favorite);
        }
        private void CommentButton_Click(object sender, EventArgs e)
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

        private void VideoPlayer_CompactOverlayChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCompactOverlay(e);
        }

        private void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            MyVideoPlayer.Close();
            App.AppViewModel.PlayVideoSeparate(_detail, _currentPartId, false);
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

        private void MyVideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            VideoPlayer = MyVideoPlayer;
        }
        private void VideoPlayer_PartSwitched(object sender, int e)
        {
            PartGridView.SelectedIndex = e;
            PartGridView.ScrollIntoView(VideoPartCollection[e], ScrollIntoViewAlignment.Leading);
        }
        private void SingleUserContainer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var accId = _detail.owner.mid;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), accId);
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

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1000)
            {
                VisualStateManager.GoToState(this, "Narrow", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Wide", true);
            }
        }

        private async void MyVideoPlayer_MediaEnded(object sender, int e)
        {
            if (PlayBackupCollection.Count > 0)
            {
                await PlayNextVideo();
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
                await Init(item.aid);
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
                await Init(item.aid);
            }
        }

        private async void PlayListContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoDetail;
            if (item.aid != videoId)
            {
                await Init(item.aid);
            }
        }

        private async void MyVideoPlayer_PreviousVideoRequest(object sender, EventArgs e)
        {
            if (PlayBackupCollection.Count>0)
            {
                await PlayPreviousVideo();
            }
        }

        private async void MyVideoPlayer_NextVideoRequest(object sender, EventArgs e)
        {
            if (PlayBackupCollection.Count > 0)
            {
                await PlayNextVideo();
            }
        }
    }
}
