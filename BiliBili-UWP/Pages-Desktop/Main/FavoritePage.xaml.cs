using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Favorites;
using BiliBili_Lib.Service;
using BiliBili_UWP.Models.UI.Interface;
using Microsoft.Toolkit.Uwp.Helpers;
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

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritePage : Page, IRefreshPage
    {
        private ObservableCollection<FavoriteItem> MyFavoriteCollection = new ObservableCollection<FavoriteItem>();
        private ObservableCollection<FavoriteItem> MyCollectCollection = new ObservableCollection<FavoriteItem>();
        private ObservableCollection<FavoriteAnime> MyAnimeCollection = new ObservableCollection<FavoriteAnime>();
        private ObservableCollection<FavoriteAnime> MyCinemaCollection = new ObservableCollection<FavoriteAnime>();
        private bool isInit = false;
        public static FavoritePage Current;
        private AccountService _account = App.BiliViewModel._client.Account;
        public FavoritePage()
        {
            this.InitializeComponent();
            Current = this;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (isInit || e.NavigationMode == NavigationMode.Back)
                return;
            await Refresh();
            isInit = true;
        }
        private void Reset()
        {
            MyFavoriteCollection.Clear();
            MyCollectCollection.Clear();
            MyAnimeCollection.Clear();
            MyCinemaCollection.Clear();
        }
        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                var tasks = new List<Task>();
                var task1 = Task.Run(async () =>
                {
                    var favorite = await _account.GetMyMainlyFavoritesAsync();
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        if (favorite != null)
                        {
                            favorite.Item1.ForEach(p => MyFavoriteCollection.Add(p));
                            favorite.Item2.ForEach(p => MyCollectCollection.Add(p));
                        }
                    });
                });
                var task2 = Task.Run(async () =>
                {
                    var data = await _account.GetMyFavoriteAnimeAsync();
                    if (data != null && data.Item2.Count > 0)
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            data.Item2.ForEach(p => MyAnimeCollection.Add(p));
                        });
                    }
                });
                var task3 = Task.Run(async () =>
                {
                    var data = await _account.GetMyFavoriteCinemaAsync();
                    if (data != null && data.Item2.Count > 0)
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                        {
                            data.Item2.ForEach(p => MyCinemaCollection.Add(p));
                        });
                    }
                });
                tasks.Add(task1);
                tasks.Add(task2);
                tasks.Add(task3);
                await Task.WhenAll(tasks);
                MyFavoriteListView.HolderVisibility = MyFavoriteCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                MyCollectListView.HolderVisibility = MyCollectCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                MyAnimeListView.HolderVisibility = MyAnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                MyCinemaListView.HolderVisibility = MyCinemaCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            LoadingRing.IsActive = false;
        }
        private void MyFavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteItem;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteDetailPage), item);
        }

        private void MyAnimeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FavoriteAnime;
            var con = (sender as ListView).ContainerFromItem(item);
            if (item.progress != null)
                App.AppViewModel.PlayBangumi(item.progress.last_ep_id, con, true);
            else
                App.AppViewModel.PlayBangumi(item.season_id, con);
        }

        private void MyAnimeListView_RefreshButtonClick(object sender, EventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteAnimePage), "anime");
        }

        private void MyCinemaListView_RefreshButtonClick(object sender, EventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteAnimePage), "cinema");
        }
        public void RemoveBangumi(string type, FavoriteAnime data)
        {
            if (type == "cinema")
            {
                MyCinemaCollection.Remove(data);
                MyAnimeListView.HolderVisibility = MyAnimeCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                MyAnimeCollection.Remove(data);
                MyCinemaListView.HolderVisibility = MyCinemaCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void MyFavoriteListView_RefreshButtonClick(object sender, EventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteContainerPage), "Favorite");
        }

        private void MyCollectListView_RefreshButtonClick(object sender, EventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteContainerPage), "Collect");
        }
    }
}
