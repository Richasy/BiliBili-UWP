using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Account;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Share.Sub.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page,IRefreshPage
    {
        private ObservableCollection<ArchiveVideo> VideoCollection = new ObservableCollection<ArchiveVideo>();
        private ObservableCollection<Topic> DynamicCollection = new ObservableCollection<Topic>();
        private AccountService _accountService = App.BiliViewModel._client.Account;
        private TopicService _topicService = App.BiliViewModel._client.Topic;
        private int _uid = 0;
        private User _user = null;
        private bool _isVideoRequesting = false;
        private bool _isDynamicRequesting = false;
        private int _videoPage = 1;
        private int _dynamicPage = 1;
        private int _videoCount = 0;
        private string _dynamicOffset = "0";
        public DetailPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "用户信息";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if(e.Parameter!=null)
            {
                if(e.Parameter is int uid)
                {
                    if (_uid != uid)
                    {
                        _uid = uid;
                        await Refresh();
                    }
                }
                else if(e.Parameter is Tuple<int,bool> pack)
                {
                    if (_uid != pack.Item1)
                    {
                        _uid = pack.Item1;
                        await Refresh();
                        await SwitchToDynamic();
                    }
                }
            }
        }
        private void Reset()
        {
            UserAvatar.ProfilePicture = null;
            PendantImage.Source = null;
            PendantImage.Visibility = Visibility.Collapsed;
            LogoutButton.Visibility = Visibility.Collapsed;
            FollowButton.Visibility = Visibility.Collapsed;
            FollowButton.IsEnabled = false;
            UserNameBlock.Text = "--";
            LikeBlock.Text = "--";
            FollowBlock.Text = "--";
            FanBlock.Text = "--";
            SignBlock.Text = "--";
            _isDynamicRequesting = false;
            _isVideoRequesting = false;
            _videoPage = 1;
            _dynamicPage = 1;
            _videoCount = 0; 
            _dynamicOffset = "0";

            VideoButton.IsChecked = true;
            DynamicButton.IsChecked = false;

            VideoContainer.Visibility = Visibility.Visible;
            DynamicContainer.Visibility = Visibility.Collapsed;
            VideoHolderText.Visibility = Visibility.Collapsed;
            DynamicHolderText.Visibility = Visibility.Collapsed;
            HolderContainer.Visibility = Visibility.Collapsed;

            DynamicCollection.Clear();
            VideoCollection.Clear();
        }
        public async Task Refresh()
        {
            LoadingRing.IsActive = true;
            Reset();
            var detail = await _accountService.GetUserSpaceAsync(_uid);
            if (detail != null)
            {
                InitDetail(detail);
            }
            LoadingRing.IsActive = false;
        }

        private async void InitDetail(UserResponse detail)
        {
            var acc = detail.card;
            _user = acc;
            UserAvatar.ProfilePicture = new BitmapImage(new Uri(acc.face + "@50w.jpg"));
            PendantImage.Visibility = Visibility.Collapsed;
            if (acc.pendant != null)
            {
                PendantImage.Visibility = Visibility.Visible;
                PendantImage.Source = acc.pendant.image;
            }
            FanBlock.Text = AppTool.GetNumberAbbreviation(acc.fans);
            FollowBlock.Text = AppTool.GetNumberAbbreviation(acc.attention);
            LikeBlock.Text = AppTool.GetNumberAbbreviation(acc.likes.like_num);

            CheckFollowButtonStatus();

            UserNameBlock.Text = acc.name;
            SignBlock.Text = acc.sign;
            LevelImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Level/level_{acc.level_info.current_level}.png"));

            if (detail.archive.count > 0)
            {
                _videoCount = detail.archive.count;
                var videos = detail.archive.item;
                videos.ForEach(p => VideoCollection.Add(p));
            }
            else
            {
                await LoadMoreVideo();
            }
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                bool result = false;
                if (_user.relation.is_follow == 1)
                {
                    result = await _accountService.UnfollowUser(Convert.ToInt32(_user.mid));
                }
                else
                {
                    result = await _accountService.FollowUser(Convert.ToInt32(_user.mid));
                }
                if (result)
                {
                    _user.relation.is_follow = _user.relation.is_follow == 0 ? 1 : 0;
                    CheckFollowButtonStatus();
                }
                else
                    new TipPopup("操作失败").ShowError();
            }
        }

        private void CheckFollowButtonStatus()
        {
            if (App.BiliViewModel.IsLogin && App.BiliViewModel._client.Account.Me.mid == _uid)
            {
                FollowButton.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
            }
            else
            {
                FollowButton.Visibility = Visibility.Visible;
                LogoutButton.Visibility = Visibility.Collapsed;
            }
            FollowButton.Style = _user.relation.is_follow == 1 ? UIHelper.GetStyle("DefaultAsyncButtonStyle") : UIHelper.GetStyle("PrimaryAsyncButtonStyle");
            if (_user.relation.is_follow == 1 && _user.relation.is_followed == 1)
                FollowButton.Text = "已互关";
            else
                FollowButton.Text = _user.relation.is_follow == 0 ? "关注" : "已关注";
            FollowButton.IsEnabled = true;
        }

        private async Task LoadMoreVideo(bool isIncrease=false)
        {
            if (!_isVideoRequesting)
            {
                if (isIncrease)
                    _videoPage += 1;
                _isVideoRequesting = true;
                var response = await _accountService.GetUserArchiveAsync(_uid, _videoPage);
                if (response != null && response.count>0)
                {
                    _videoCount = response.count;
                    response.item.ForEach(p => VideoCollection.Add(p));
                }
                VideoHolderText.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                _isVideoRequesting = false;
            }
        }

        private async Task LoadMoreDynamic(bool isIncrease = false)
        {
            if (!_isDynamicRequesting)
            {
                if (isIncrease)
                    _dynamicPage += 1;
                _isDynamicRequesting = true;
                var response = await _topicService.GetUserSpaceDynamicAsync(_uid, _dynamicPage,_dynamicOffset);
                if (response != null)
                {
                    _dynamicOffset = response.Item1;
                    if(response.Item2!=null && response.Item2.Count > 0)
                    {
                        response.Item2.ForEach(p => DynamicCollection.Add(p));
                    }
                }
                DynamicHolderText.Visibility = DynamicCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                _isDynamicRequesting = false;
            }
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            bool isVideo = Convert.ToBoolean(VideoButton.IsChecked);
            var ele = sender as ScrollViewer;
            if (isVideo)
            {
                if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
                {
                    if(VideoCollection.Count< _videoCount)
                        await LoadMoreVideo(true);
                }
            }
            else
            {
                if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
                {
                    if(!string.IsNullOrEmpty(_dynamicOffset))
                        await LoadMoreDynamic(true);
                }
            }
        }

        private async void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            DynamicButton.IsChecked = false;
            VideoContainer.Visibility = Visibility.Visible;
            DynamicContainer.Visibility = Visibility.Collapsed;
            if (VideoCollection.Count == 0)
            {
                LoadingRing.IsActive = true;
                await LoadMoreVideo();
                LoadingRing.IsActive = false;
            }
        }
        private async void DynamicButton_Click(object sender, RoutedEventArgs e)
        {
            await SwitchToDynamic();
        }

        private async Task SwitchToDynamic()
        {
            VideoButton.IsChecked = false;
            DynamicButton.IsChecked = true;
            VideoContainer.Visibility = Visibility.Collapsed;
            DynamicContainer.Visibility = Visibility.Visible;
            if (DynamicCollection.Count == 0)
            {
                LoadingRing.IsActive = true;
                await LoadMoreDynamic();
                LoadingRing.IsActive = false;
            }
        }

        private void VideoListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as ArchiveVideo;
            if (data.@goto == "av")
            {
                var ana = BiliTool.GetResultFromUri(data.uri);
                if (ana.Type == BiliBili_Lib.Enums.UriType.Bangumi)
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(ana.Param), null, true);
                else
                    App.AppViewModel.PlayVideo(Convert.ToInt32(data.param));
            }
            else if (data.@goto == "bangumi")
                App.AppViewModel.PlayBangumi(Convert.ToInt32(data.param), null, true);
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ConfirmDialog("退出提醒", "是否退出当前登录的账户？");
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                App.BiliViewModel.ClearAccountInformation();
                await Refresh();
            }
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            TopicCard card = (TopicCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }

        private void VideoListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoPanel card = (DefaultVideoPanel)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
