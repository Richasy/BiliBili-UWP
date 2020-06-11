using BiliBili_Lib.Models.BiliBili.Account;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Widgets;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Share.Sub.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FollowPage : Page, IRefreshPage
    {
        private ObservableCollection<FollowTag> TabCollection = new ObservableCollection<FollowTag>();
        private List<TempFollowPart> FollowList = new List<TempFollowPart>();
        private bool _isInit = false;
        private bool _isRequesting = false;
        private int _currentTab = int.MinValue;
        private AccountService _accountService = App.BiliViewModel._client.Account;
        public FollowPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "我的关注";
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            await Refresh();
            _isInit = true;
            base.OnNavigatedTo(e);
        }

        private void Reset()
        {
            TabCollection.Clear();
            FollowList.Clear();
            _isRequesting = false;

            HolderContainer.Visibility = Visibility.Collapsed;
            HolderBlock.Visibility = Visibility.Collapsed;
        }
        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            var tabs = await _accountService.GetMyFollowTagsAsync();
            if (tabs != null && tabs.Count > 0)
            {
                foreach (var item in tabs)
                {
                    TabCollection.Add(item);
                    var newItem = new TempFollowPart()
                    {
                        TagId = item.tagid,
                        Page = 1,
                        Users = new ObservableCollection<RelationUser>()
                    };
                    FollowList.Add(newItem);
                }
                _currentTab = TabCollection.First().tagid;
                var first = FollowList.First();
                UserListView.ItemsSource = first.Users;
                TabListView.SelectedIndex = 0;
                await LoadFollowUser();
            }
            else
            {
                HolderContainer.Visibility = Visibility.Visible;
            }
            LoadingRing.IsActive = false;
        }

        private async Task LoadFollowUser(bool isIncrease = false)
        {
            if (_isRequesting)
                return;
            var source = FollowList.Where(p => p.TagId == _currentTab).FirstOrDefault();
            var tab = TabCollection.Where(p => p.tagid == _currentTab).FirstOrDefault();
            if (source != null && tab!=null)
            {
                if (tab.count <= source.Users.Count)
                    return;
                _isRequesting = true;
                if (isIncrease)
                    source.Page += 1;
                var users = await _accountService.GetMyFollowUserAsync(_currentTab, source.Page);
                if (users != null && users.Count > 0)
                    users.ForEach(p => source.Users.Add(p));
            }
            _isRequesting = false;
            HolderBlock.Visibility = source.Users.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void UnfollowButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AsyncButton;
            var data = btn.DataContext as RelationUser;
            btn.IsLoading = true;
            bool result = await _accountService.UnfollowUser(data.mid);
            if(result)
            {
                var source = FollowList.Where(p => p.TagId == _currentTab).FirstOrDefault();
                var tab = TabCollection.Where(p => p.tagid == _currentTab).FirstOrDefault();
                source.Users.Remove(data);
                tab.count -= 1;
                new TipPopup("已取关").ShowMessage();
            }
            else
            {
                new TipPopup("取关失败，稍后再试试吧").ShowError();
            }
        }

        private async void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FollowTag;
            if (_currentTab != item.tagid)
            {
                _currentTab = item.tagid;
                var source = FollowList.Where(p => p.TagId == _currentTab).FirstOrDefault();
                UserListView.ItemsSource = source.Users;
                if (source.Users.Count == 0)
                {
                    LoadingRing.IsActive = true;
                    await LoadFollowUser();
                    LoadingRing.IsActive = false;
                }
            }
        }

        private void UserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RelationUser;
            App.AppViewModel.NavigateToSubPage(typeof(DetailPage), item.mid);
        }


        public class TempFollowPart
        {
            public int TagId { get; set; }
            public ObservableCollection<RelationUser> Users { get; set; }
            public int Page { get; set; }
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                LoadingBar.Visibility = Visibility.Visible;
                await LoadFollowUser(true);
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
