using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
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
    public sealed partial class FavoriteContainerPage : Page,IRefreshPage
    {
        private ObservableCollection<FavoriteItem> FavoriteCollection = new ObservableCollection<FavoriteItem>();
        private string _type = "";
        private AccountService _account = App.BiliViewModel._client.Account;
        private int pn = 1;
        private bool _isRequesting = false;
        public FavoriteContainerPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if(e.Parameter!=null && e.Parameter is string type)
            {
                _type = type;
                if (type == "Favorite")
                    App.AppViewModel.CurrentSubPageControl.SubPageTitle = "我的收藏夹";
                else
                    App.AppViewModel.CurrentSubPageControl.SubPageTitle = "我的收集列表";
                await Refresh();
            }
            base.OnNavigatedTo(e);
        }
        public async Task Refresh()
        {
            FavoriteCollection.Clear();
            pn = 1;
            LoadingRing.IsActive = true;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                await LoadMoreFavoriteItem();
            }
            LoadingRing.IsActive = false;
        }

        private async Task LoadMoreFavoriteItem(bool isIncrease=false)
        {
            if (_isRequesting)
                return;
            _isRequesting = true;
            List<FavoriteItem> items = new List<FavoriteItem>();
            if (isIncrease)
                pn += 1;
            if (_type == "Favorite")
                items = await _account.GetFavoritesAsync(Convert.ToInt32(BiliTool.mid), pn);
            else
                items = await _account.GetCollectListAsync(Convert.ToInt32(BiliTool.mid), pn);
            if(items!=null && items.Count > 0)
            {
                items.ForEach(p => FavoriteCollection.Add(p));
            }
            HolderText.Visibility = FavoriteCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            _isRequesting = false;
        }

        private void FavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteItem;
            App.AppViewModel.NavigateToSubPage(typeof(FavoriteDetailPage), item);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                LoadingBar.Visibility = Visibility.Visible;
                await LoadMoreFavoriteItem(true);
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
