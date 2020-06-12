using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Weakly;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RegionPage : Page
    {
        private List<RegionData> RegionDetailList = new List<RegionData>();
        public ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        private VideoService _regionService = App.BiliViewModel._client.Video;
        private bool _isInit = false;
        private bool _isRequesting = false;
        public class RegionData
        {
            public int Tid { get; set; }
            public ObservableCollection<RegionVideo> Collection { get; set; }
            public ObservableCollection<RegionBanner> Banner { get; set; }
            public int Ctime { get; set; }
            public RegionData() { }
            public RegionData(int tid, int ctime = 0)
            {
                Tid = tid;
                Collection = new ObservableCollection<RegionVideo>();
                Banner = new ObservableCollection<RegionBanner>();
                Ctime = ctime;
            }
        }
        public RegionPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            App.BiliViewModel.RegionCollection.Where(p => p.name != "国创" && p.name != "番剧").ForEach(p => RegionCollection.Add(p));
            foreach (var item in RegionCollection)
            {
                RegionDetailList.Add(new RegionData(item.tid));
            }
        }

        private async Task LoadMoreVideo(int tid)
        {
            if (_isRequesting)
                return;
            _isRequesting = true;
            var source = RegionDetailList.Where(p => p.Tid == tid).FirstOrDefault();
            var videos = await _regionService.GetRegionSquareAsync(tid, source.Ctime);
            source.Ctime = videos.Item2;
            if (videos.Item1 != null && source.Banner.Count == 0)
            {
                videos.Item1.ForEach(p => source.Banner.Add(p));
            }
            if (videos.Item3 != null && videos.Item3.Count > 0)
            {
                videos.Item3.ForEach(p => source.Collection.Add(p));
            }
            _isRequesting = false;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            RegionListView.SelectedIndex = 0;
            await SwitchRegion(RegionDetailList.First().Tid);
            _isInit = true;
            base.OnNavigatedTo(e);
        }

        private async Task SwitchRegion(int tid)
        {
            var source = RegionDetailList.Where(p => p.Tid == tid).FirstOrDefault();
            if (source != null)
            {
                VideoGridView.ItemsSource = source.Collection;
                BannerListView.ItemsSource = source.Banner;
                if (source.Collection.Count == 0)
                {
                    LoadingBar.Visibility = Visibility.Visible;
                    await LoadMoreVideo(tid);
                }
                NoDataContainer.Visibility = source.Collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                LoadingBar.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void VideoGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionVideo;
            App.AppViewModel.PlayVideo(Convert.ToInt32(item.param));
        }

        private async void RegionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionContainer;
            await SwitchRegion(item.tid);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadMoreVideo((RegionListView.SelectedItem as RegionContainer).tid);
            }
        }

        private void SubRegionButton_Click(object sender, RoutedEventArgs e)
        {
            var region = RegionListView.SelectedItem as RegionContainer;
            if (region != null)
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.SubRegionPage), region);
        }

        private async void BannerListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var banner = e.ClickedItem as RegionBanner;
            await Launcher.LaunchUriAsync(new Uri(banner.uri));
        }
    }
}
