using BiliBili_Lib.Models.BiliBili;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RankPage : Page, IRefreshPage
    {
        private List<Tuple<int, List<WebVideo>>> RankDetailList = new List<Tuple<int, List<WebVideo>>>();
        private ObservableCollection<WebVideo> DisplayCollection = new ObservableCollection<WebVideo>();
        private ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        private bool _isInit = false;
        private int _selectRegion = 0;
        public RankPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            VideoGridView.EnableAnimation = App.AppViewModel.IsEnableAnimation;
            await Refresh();
            base.OnNavigatedTo(e);
            _isInit = true;
        }

        private void Reset()
        {
            RankDetailList.Clear();
            RegionCollection.Clear();
            foreach (var item in App.BiliViewModel.RegionCollection)
            {
                if (item.is_bangumi == 0)
                    RegionCollection.Add(item);
            }
            var total = new RegionContainer()
            {
                tid = 0,
                reid = 0,
                name = "全区动态",
                logo = "ms-appx:///Assets/logo_small.png",
                type = 1
            };
            var chinaAnime = new RegionContainer()
            {
                tid = 168,
                reid = 0,
                name = "国创相关",
                logo = "http://i0.hdslb.com/bfs/archive/1586ec926eac1ea876cb74d32df51394d8e72341.png",
                type = 1
            };
            RegionCollection.Insert(0, total);
            RegionCollection.Insert(1, chinaAnime);
            foreach (var item in RegionCollection)
            {
                RankDetailList.Add(new Tuple<int, List<WebVideo>>(item.tid, new List<WebVideo>()));
            }
            RegionListView.SelectedIndex = 0;
        }

        public async Task Refresh()
        {
            Reset();
            await LoadRegionRankVideo();
        }

        private async Task LoadRegionRankVideo()
        {
            LoadingRing.IsActive = true;
            NoDataContainer.Visibility = Visibility.Collapsed;
            var source = RankDetailList.Where(p => p.Item1 == _selectRegion).FirstOrDefault();
            if (source != null)
            {
                DisplayCollection.Clear();
                if (source.Item2.Count == 0)
                {
                    var videos = await App.BiliViewModel._client.Video.GetRegionRankAsync(_selectRegion);
                    if (videos!=null && videos.Count > 0)
                    {
                        for (int i = 0; i < videos.Count; i++)
                        {
                            var item = videos[i];
                            if (i < 3)
                                item.render_sign = $"ms-appx:///Assets/Rank/ic_live_rank_{i + 1}.png";
                            source.Item2.Add(item);
                        }
                    }
                }
                source.Item2.ForEach(p => DisplayCollection.Add(p));
            }
            NoDataContainer.Visibility = source.Item2.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingRing.IsActive = false;
        }
        private async void RegionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionContainer;
            _selectRegion = item.tid;
            await LoadRegionRankVideo();
        }

        private void VideoGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as WebVideo;
            var container = VideoGridView.ContainerFromItem(item);
            App.AppViewModel.PlayVideo(Convert.ToInt32(item.aid), container, "");
        }
    }
}
