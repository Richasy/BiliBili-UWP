using BiliBili_Lib.Models.BiliBili;
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
using Windows.System;
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
    public sealed partial class AnimePage : Page, IRefreshPage
    {
        private bool _isInit = false;
        private ObservableCollection<AnimeModuleItem> BannerCollection = new ObservableCollection<AnimeModuleItem>();
        private ObservableCollection<AnimeModuleItem> HotCollection = new ObservableCollection<AnimeModuleItem>();
        private ObservableCollection<AnimeModuleItem> RankCollection = new ObservableCollection<AnimeModuleItem>();

        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private bool isJP = true;
        public AnimePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is bool _isJP)
            {
                isJP = _isJP;
            }
            TitleBlock.Text = isJP ? "番剧" : "国创";
            await Refresh();
            _isInit = true;
        }
        private void Reset()
        {
            BannerCollection.Clear();
            HotCollection.Clear();
            RankCollection.Clear();

            BannerContainer.Visibility = Visibility.Collapsed;
            HotContainer.Visibility = Visibility.Collapsed;
            RankContainer.Visibility = Visibility.Collapsed;
        }
        public async Task Refresh()
        {
            Reset();
            LoadingBar.Visibility = Visibility.Visible;
            var data = await _animeService.GetAnimeSquareAsync(isJP);
            if (data != null && data.Count > 0)
            {
                //Banner
                var banners = data.Where(p => p.style == "banner").FirstOrDefault();
                if (banners != null)
                {
                    BannerContainer.Visibility = Visibility.Visible;
                    banners.items.ForEach(p => BannerCollection.Add(p));
                    BannerContainer.HolderVisibility = BannerCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                int hot_id = isJP ? 119 : 124;
                var hot = data.Where(p => p.module_id == hot_id).FirstOrDefault();
                if (hot != null)
                {
                    HotContainer.Visibility = Visibility.Visible;
                    hot.items.ForEach(p => HotCollection.Add(p));
                    HotContainer.HolderVisibility = HotCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
                var rank = data.Where(p => p.style == "rank").FirstOrDefault();
                if (rank != null)
                {
                    RankContainer.Visibility = Visibility.Visible;
                    foreach (var item in rank.items)
                    {
                        item.cards = item.cards.Take(3).ToList();
                        for (int i = 0; i < item.cards.Count; i++)
                        {
                            item.cards[i].render_sign = $"ms-appx:///Assets/Rank/ic_live_rank_{i + 1}.png";
                        }
                        RankCollection.Add(item);
                    }
                    RankContainer.HolderVisibility = RankCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private void SubRegionButton_Click(object sender, RoutedEventArgs e)
        {
            string name = isJP ? "番剧" : "国创";
            var param = App.BiliViewModel.RegionCollection.Where(p => p.name == name).FirstOrDefault();
            if (param != null)
            {
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.SubRegionPage), param);
            }
        }

        private void IndexButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Anime.IndexPage), 1);
        }

        private void TimelineButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Anime.TimelinePage), isJP ? 2 : 3);
        }

        private void BannerListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var banner = e.ClickedItem as AnimeModuleItem;
            if (banner.oid > 0)
                App.AppViewModel.PlayBangumi(banner.oid, sender, true);
            else
                App.AppViewModel.ShowWebPopup(banner.title, banner.link);
        }

        private void HotContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AnimeModuleItem;
            App.AppViewModel.PlayBangumi(item.oid, sender);
        }

        private async void HotContainer_RefreshButtonClick(object sender, EventArgs e)
        {
            var btn = sender as AsyncButton;
            btn.IsLoading = true;
            int hot_id = isJP ? 119 : 124;
            var items = await _animeService.GetAnimeSectionExchange(1, hot_id);
            if (items != null && items.Count > 0)
            {
                var others = items.Where(p => !HotCollection.Contains(p)).Take(6).ToList();
                if (others.Count() > 0)
                {
                    HotCollection.Clear();
                    RandomSortList(others).ForEach(p => HotCollection.Add(p));
                }
            }
            btn.IsLoading = false;
        }
        private List<AnimeModuleItem> RandomSortList(List<AnimeModuleItem> items)
        {
            var random = new Random();
            var newList = new List<AnimeModuleItem>();
            foreach (var item in items)
            {
                newList.Insert(random.Next(newList.Count + 1), item);
            }
            return newList;
        }

        private void RankVideo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var video = (sender as FrameworkElement).DataContext as Card;
            App.AppViewModel.PlayBangumi(video.oid, sender);
        }

        private void MyFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                App.AppViewModel.CurrentSidePanel.SetSelectedItem(Models.Enums.AppMenuItemType.MyFavorite);
                App.AppViewModel.CurrentPagePanel.NavigateToPage(Models.Enums.AppMenuItemType.MyFavorite);
            } 
        }
    }
}
