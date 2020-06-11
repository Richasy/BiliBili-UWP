using BiliBili_Lib.Models.BiliBili.Favorites;
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
    public sealed partial class FavoriteAnimePage : Page, IRefreshPage
    {
        private ObservableCollection<FavoriteAnime> AnimeCollection = new ObservableCollection<FavoriteAnime>();
        private int _page = 1;
        private int _total = 0;
        private bool _isInit = false;
        private bool _isRequesting = false;
        private string _type = "";
        private AccountService _account = App.BiliViewModel._client.Account;
        public FavoriteAnimePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is string t)
            {
                _type = t;
            }
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = _type == "cinema" ? "我的追剧" : "我的追番";
            await Refresh();
        }

        private void Reset()
        {
            AnimeCollection.Clear();
            _page = 1;
            _total = 0;
            _isInit = false;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
            _isRequesting = false;
        }

        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            _isInit = false;
            await LoadAnime();
            _isInit = true;
            LoadingRing.IsActive = false;
        }

        private async Task LoadAnime(bool isIncrease = false)
        {
            if (_isRequesting || (_isInit && _total <= AnimeCollection.Count))
                return;
            if (!LoadingRing.IsActive)
                LoadingBar.Visibility = Visibility.Visible;
            if (isIncrease)
                _page += 1;
            _isRequesting = true;
            Tuple<int, List<FavoriteAnime>> data;
            if (_type == "cinema")
            {
                data = await _account.GetMyFavoriteCinemaAsync(_page);

            }
            else
            {
                data = await _account.GetMyFavoriteAnimeAsync(_page);
            }
            if (data != null)
            {
                _total = data.Item1;
                if (data.Item2!=null && data.Item2.Count > 0)
                {
                    data.Item2.ForEach(p => AnimeCollection.Add(p));
                }
            }
            HolderText.Visibility = AnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility = Visibility.Collapsed;
            _isRequesting = false;
        }

        private void AnimeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteAnime;
            if (item.progress != null)
                App.AppViewModel.PlayBangumi(item.progress.last_ep_id, null, true);
            else
                App.AppViewModel.PlayBangumi(item.season_id);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadAnime(true);
            }
        }

        private async void UnfollowButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            var data = btn.DataContext as FavoriteAnime;
            if (data != null && App.BiliViewModel.CheckAccoutStatus())
            {
                btn.IsEnabled = false;
                bool result = await App.BiliViewModel._client.Anime.UnfollowBangumiAsync(data.season_id);
                if (result)
                {
                    AnimeCollection.Remove(data);
                    if (Pages.Main.FavoritePage.Current != null && App.AppViewModel.CurrentPageType==typeof(Pages.Main.FavoritePage))
                        Pages.Main.FavoritePage.Current.RemoveBangumi(_type, data);
                    new TipPopup("操作成功").ShowMessage();
                }
                else
                {
                    new TipPopup("操作失败").ShowError();
                }
            }
            HolderText.Visibility = AnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
