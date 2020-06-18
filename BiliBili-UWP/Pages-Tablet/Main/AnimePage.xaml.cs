using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Controls;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AnimePage : Page
    {
        private ObservableCollection<AnimeModuleItem> HotCollection = new ObservableCollection<AnimeModuleItem>();
        private AnimeService _animeService = App.BiliViewModel._client.Anime;
        private ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        private TabletBangumiDetailBlock _bangumiBlock = App.AppViewModel.CurrentBangumiDetailBlock;
        private bool isJP = true;
        private bool _isInit = false;
        public AnimePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DetailContainer.Children.Add(_bangumiBlock);
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is bool _isJP)
            {
                isJP = _isJP;

            }
            await Refresh();
            _isInit = true;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DetailContainer.Children.Remove(_bangumiBlock);
            _bangumiBlock.Visibility = Visibility.Collapsed;
            TabletMainPage.Current.HideBackgroundImage();
            App.AppViewModel.CurrentVideoPlayer = null;
            _bangumiBlock.VideoPlayer.Close();
            base.OnNavigatingFrom(e);
        }
        private void Reset()
        {
            HotCollection.Clear();
            RegionCollection.Clear();
            _bangumiBlock.Visibility = Visibility.Collapsed;
            HoldContainer.Visibility = Visibility.Visible;
            HotContainer.SelectedIndex = -1;

            foreach (var item in App.BiliViewModel.RegionCollection)
            {
                if (item.is_bangumi == 1 && (item.name == "番剧" || item.name == "国创"))
                    RegionCollection.Add(item);
            }

            RegionListView.SelectedIndex = isJP ? 0 : 1;
        }
        public async Task Refresh()
        {
            Reset();
            await LoadHotVideo();
        }

        private async Task LoadHotVideo()
        {
            LoadingBar.Visibility = Visibility.Visible;
            HoldContainer.Visibility = Visibility.Visible;
            _bangumiBlock.Visibility = Visibility.Collapsed;
            if (App.AppViewModel.CurrentVideoPlayer != null)
                App.AppViewModel.CurrentVideoPlayer.Close();
            int hot_id = isJP ? 119 : 124;
            HotCollection.Clear();
            var items = await _animeService.GetAnimeSectionExchange(1, hot_id);
            if (items != null && items.Count > 0)
            {
                items.ForEach(p => HotCollection.Add(p));
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
        private async void HotContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = HotContainer.SelectedItem as AnimeModuleItem;
            if (item == null)
            {
                _bangumiBlock.Visibility = Visibility.Collapsed;
                HoldContainer.Visibility = Visibility.Visible;
                return;
            }
            HoldContainer.Visibility = Visibility.Collapsed;
            _bangumiBlock.Visibility = Visibility.Visible;
            await _bangumiBlock.Init(item.oid, false);
        }

        private async void RegionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionContainer;
            isJP = item.name == "番剧";
            await LoadHotVideo();
        }

        private void MyFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteAnimePage));
        }
    }
}
