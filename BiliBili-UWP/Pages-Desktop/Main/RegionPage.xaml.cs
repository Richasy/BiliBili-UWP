using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Core;
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
    public sealed partial class RegionPage : Page, IRefreshPage
    {
        private bool _isInit = false;
        private ObservableCollection<RegionBanner> BannerCollection = new ObservableCollection<RegionBanner>();
        private ObservableCollection<WebVideo> RankCollection = new ObservableCollection<WebVideo>();
        private ObservableCollection<RegionVideo> VideoCollection = new ObservableCollection<RegionVideo>();

        private VideoService _regionService = App.BiliViewModel._client.Video;
        private RegionContainer _region;
        private bool _isRecommendRequesting = false;
        private int ctime = 0;
        private double _scrollOffset = 0;
        public RegionPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = ScrollViewerBottomHandle;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = ScrollViewerChanged;
            RecommendVideoView.DesiredWidth = 220 + ((App.AppViewModel.BasicFontSize - 14) * 5);
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is RegionContainer _con)
            {
                if (_region != null && _con.tid == _region.tid)
                    return;
                _region = _con;
            }
            TitleBlock.Text = _region.name;
            
            await Refresh();
            
            _isInit = true;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = null;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = null;
            base.OnNavigatingFrom(e);
        }
        private void Reset()
        {
            ctime = 0;
            _scrollOffset = 0;
            BannerContainer.Visibility = Visibility.Collapsed;
            RankContainer.Visibility = Visibility.Collapsed;

            VideoCollection.Clear();
            BannerCollection.Clear();
            RankCollection.Clear();
        }
        public async Task Refresh()
        {
            if (_isRecommendRequesting)
                return;
            VideoLoadingRing.IsActive = true;
            Reset();
            _isRecommendRequesting = true;
            var square = await _regionService.GetRegionSquareAsync(_region.tid, ctime);
            var rank = await _regionService.GetRegionRankAsync(_region.tid);
            if (square.Item1 != null)
            {
                square.Item1.ForEach(p => BannerCollection.Add(p));
                BannerContainer.Visibility = BannerCollection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
            ctime = square.Item2;
            if (square.Item3 != null && square.Item3.Count > 0)
            {
                square.Item3.ForEach(p => VideoCollection.Add(p));
            }
            else
                HolderText.Visibility = Visibility.Visible;
            if (rank != null && rank.Count > 0)
            {
                rank = rank.Take(10).ToList();
                rank.ForEach(p => RankCollection.Add(p));
                RankContainer.Visibility = Visibility.Visible;
            }
            _isRecommendRequesting = false;
            VideoLoadingRing.IsActive = false;
        }

        private void SubRegionButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.SubRegionPage), _region);
        }

        private async void ScrollViewerBottomHandle()
        {
            if (_isRecommendRequesting)
                return;
            _isRecommendRequesting = true;
            VideoLoadingBar.Visibility = Visibility.Visible;
            var videos = await _regionService.GetRegionSquareAsync(_region.tid, ctime);
            ctime = videos.Item2;
            if (videos.Item3 != null && videos.Item3.Count > 0)
            {
                videos.Item3.ForEach(p => VideoCollection.Add(p));
            }
            VideoLoadingBar.Visibility = Visibility.Collapsed;
            _isRecommendRequesting = false;
        }

        private async void BannerListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var banner = e.ClickedItem as RegionBanner;
            await Launcher.LaunchUriAsync(new Uri(banner.uri));
        }

        private void RecommendVideoView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as RegionVideo;
            var ele = RecommendVideoView.ContainerFromItem(video);
            App.AppViewModel.PlayVideo(Convert.ToInt32(video.param),ele);
        }

        private void RankContainer_ItemClick(object sender, ItemClickEventArgs e)
        {
            var video = e.ClickedItem as WebVideo;
            var view = sender as ListView;
            var ele = view.ContainerFromItem(video);
            App.AppViewModel.PlayVideo(Convert.ToInt32(video.aid),ele);
        }
        private void ScrollViewerChanged()
        {
            double offset = App.AppViewModel.CurrentPagePanel.PageScrollViewer.VerticalOffset;
            _scrollOffset = offset;
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_scrollOffset > 0)
            {
                App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, _scrollOffset, 1);
            }
        }

        private void RecommendVideoView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoCard card = (DefaultVideoCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
