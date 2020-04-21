using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using NSDanmaku.Helper;
using SYEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
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
    public sealed partial class PlayerPage : Page, IRefreshPage
    {
        private ObservableCollection<VideoPart> VideoPartCollection = new ObservableCollection<VideoPart>();
        private VideoDetail _detail = null;
        private ObservableCollection<VideoRelated> RelatedCollection = new ObservableCollection<VideoRelated>();
        private ObservableCollection<VideoTag> TagCollection = new ObservableCollection<VideoTag>();
        private ObservableCollection<Staff> StaffCollection = new ObservableCollection<Staff>();
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private int _currentPartId = 0;
        private int videoId = 0;
        public static PlayerPage Current;
        private List<FavoriteItem> _tempFavorites = new List<FavoriteItem>();

        public PlayerPage()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter is int aid)
            {
                var anim = ConnectedAnimationService.GetForCurrentView().GetAnimation("VideoConnectedAnimation");
                if (anim != null)
                {
                    anim.TryStart(VideoPlayer);
                }
                if (aid == videoId)
                    return;
                else
                {
                    videoId = aid;
                    await Refresh();
                }
                App.AppViewModel.CurrentVideoPlayer = VideoPlayer;
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            VideoPlayer.Pause();
            App.AppViewModel.CurrentVideoPlayer = null;
            await _videoService.AddVideoHistoryAsync(videoId, _currentPartId, VideoPlayer.CurrentProgress);
            base.OnNavigatedFrom(e);
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

            PlayCountBlock.Text = "-";
            DanmukuCountBlock.Text = "-";
            ReplyCountBlock.Text = "-";
            RepostCountBlock.Text = "-";

            DescriptionBlock.Text = "--";
            UPAvatar.ProfilePicture = null;
            UPNameBlock.Text = "-";

            SelectLikeCheckBox.IsChecked = true;
            FavoriteListView.ItemsSource = null;

            FollowButton.Style = UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            FollowButton.Text = "关注";
        }
        public async Task Refresh()
        {
            Reset();
            var tip = new WaitingPopup("加载视频中...");
            tip.ShowPopup();
            var detail = await _videoService.GetVideoDetailAsync(videoId);
            if (detail != null)
            {
                _detail = detail;
                InitDetail();
                await VideoPlayer.Init(_detail,_currentPartId);
            }
            tip.HidePopup();
        }

        private void InitDetail()
        {
            TitleBlock.Text = _detail.title;
            PlayCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.view);
            DanmukuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmaku);
            RepostCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.share);
            ReplyCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.reply);

            DescriptionBlock.Text = _detail.desc;
            
            LikeButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.like);
            CoinButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.coin);
            FavoriteButton.Text = AppTool.GetNumberAbbreviation(_detail.stat.favorite);

            UPAvatar.ProfilePicture = string.IsNullOrEmpty(_detail.owner.face) ? null : new BitmapImage(new Uri(_detail.owner.face + "@50w.jpg")) { DecodePixelWidth = 40 };
            UPNameBlock.Text = _detail.owner.name;

            _detail.pages.ForEach(p => VideoPartCollection.Add(p));
            PartListView.SelectedIndex = 0;
            _currentPartId = _detail.pages.First().cid;
            PartListView.Visibility = _detail.pages.Count > 1 ? Visibility.Visible : Visibility.Collapsed;

            if(_detail.tag!=null && _detail.tag.Count > 0)
            {
                _detail.tag.ForEach(p => TagCollection.Add(p));
            }

            if(_detail.staff!=null && _detail.staff.Count > 0)
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
                CoinButton.IsCheck = _detail.req_user.like != 0;
                FavoriteButton.IsCheck = _detail.req_user.like != 0;
                FollowButton.Style = _detail.req_user.attention == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
                FollowButton.Text = _detail.req_user.attention == 1 ? "已关注" : "关注";
            }

            if(_detail.relates!=null && _detail.relates.Count > 0)
            {
                _detail.relates.Where(p => p.@goto == "av").Take(10).ToList().ForEach(p => RelatedCollection.Add(p));
            }
        }

        private async void RelateVideoContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as VideoRelated;
            videoId = video.aid;
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
        }

        private void VideoPlayer_FullWindowChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoFullScreen(e);
        }

        private void VideoPlayer_CinemaChanged(object sender, bool e)
        {
            App.AppViewModel.PlayVideoCinema(e);
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
                    new TipPopup(prompt).ShowMessage();
                    LikeButton.IsCheck = !LikeButton.IsCheck;
                }
                else
                    new TipPopup("点赞操作失败").ShowError();
                LikeButton.IsEnabled = true;
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
                    LikeButton.IsCheck = true;
                new TipPopup("成功投币！").ShowMessage();
            } 
            else
                new TipPopup("投币失败").ShowError();
            CoinButton.IsCheck = result;
            CoinButton.IsEnabled = true;
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
                if(item.fav_state==1 && !selectedItems.Any(p=>(p as FavoriteItem).id == item.id))
                {
                    removedList.Add(item.id.ToString());
                }
            }
            if(addList.Count>0 || removedList.Count > 0)
            {
                FavoriteSureButton.IsLoading = true;
                bool result = await _videoService.AddVideoToFavoriteAsync(_detail.aid, addList, removedList);
                FavoriteSureButton.IsLoading = false;
                if (result)
                {
                    FavoriteFlyout.Hide();
                    FavoriteButton.IsCheck = selectedItems.Count > 0;
                    new TipPopup("已更改收藏夹").ShowMessage();
                }
                else
                {
                    new TipPopup("收藏失败").ShowError();
                }
            }
        }

        private void ReplyCountBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", _detail.aid.ToString());
            param.Add("type", "1");
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Sub.ReplyPage), param);
        }

        private async void PartListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoPart;
            if (item.cid != _currentPartId)
            {
                _currentPartId = item.cid;
                await VideoPlayer.Init(_detail,_currentPartId);
            }
        }

        private void TagListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoTag;
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Sub.Channel.TagDetailPage), item.tag_id);
        }

        private void StaffListView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void DescriptionBlock_IsTextTrimmedChanged(TextBlock sender, IsTextTrimmedChangedEventArgs args)
        {
            if (DescriptionBlock.IsTextTrimmed)
                ToolTipService.SetToolTip(DescriptionBlock, _detail.desc);
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void VideoPlayer_SeparateButtonClick(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Close();
            App.AppViewModel.PlayVideoSeparate(_detail, _currentPartId);
        }
    }
}
