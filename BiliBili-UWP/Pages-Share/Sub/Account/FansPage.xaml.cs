using BiliBili_Lib.Models.BiliBili.Account;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
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
    public sealed partial class FansPage : Page,IRefreshPage
    {
        private ObservableCollection<FanUser> FansCollection = new ObservableCollection<FanUser>();
        private int _page = 1;
        private long _reversion = 0;
        private int _total = 0;
        private bool _isInit = false;
        private bool _isRequesting = false;
        private AccountService _accountService = App.BiliViewModel._client.Account;
        public FansPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "我的粉丝";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            await Refresh();
        }

        private void Reset()
        {
            FansCollection.Clear();
            _page = 1;
            _reversion = 0;
            _total = 0;
            _isRequesting = false;
        }

        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            _isInit = false;
            await LoadFans();
            _isInit = true;
            LoadingRing.IsActive = false;
        }

        private async Task LoadFans(bool isIncrease = false)
        {
            if (_isRequesting || (_isInit && _total == FansCollection.Count))
                return;
            if (isIncrease)
                _page += 1;
            _isRequesting = true;
            var data = await _accountService.GetMyFansAsync(_page, _reversion);
            if (data != null)
            {
                _reversion = data.re_version;
                _total = data.total;
                TotalBlock.Text = $"共 {AppTool.GetNumberAbbreviation(_total)} 个粉丝";
                if(data.list!=null && data.list.Count>0)
                    data.list.ForEach(p => FansCollection.Add(p));
            }
            _isRequesting = false;
            HolderText.Visibility = FansCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AsyncButton;
            var data = btn.DataContext as FanUser;
            btn.IsLoading = true;
            bool result = await _accountService.FollowUser(data.mid);
            if (result)
            {
                btn.Visibility = Visibility.Collapsed;
                new TipPopup("已关注").ShowMessage();
            }
            btn.IsLoading = false;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                LoadingBar.Visibility = Visibility.Visible;
                await LoadFans(true);
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }

        private void UserListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FanUser;
            App.AppViewModel.NavigateToSubPage(typeof(DetailPage), item.mid);
        }
    }
}
