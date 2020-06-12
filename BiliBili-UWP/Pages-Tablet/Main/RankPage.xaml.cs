using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RankPage : Page
    {
        private List<Tuple<int, List<WebVideo>>> RankDetailList = new List<Tuple<int, List<WebVideo>>>();
        private ObservableCollection<WebVideo> DisplayCollection = new ObservableCollection<WebVideo>();
        private List<VideoDetail> _videoDetailList = new List<VideoDetail>();
        private ObservableCollection<RegionContainer> RegionCollection = new ObservableCollection<RegionContainer>();
        private bool _isInit = false;
        private int _selectRegion = 0;
        private TabletVideoDetailBlock _videoBlock = App.AppViewModel.CurrentVideoDetailBlock;
        public RankPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DetailContainer.Children.Add(_videoBlock);
            _videoBlock.Visibility = Visibility.Collapsed;
            VideoView.SelectedIndex = -1;
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            await Refresh();
            base.OnNavigatedTo(e);
            _isInit = true;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DetailContainer.Children.Remove(_videoBlock);
            _videoBlock.Visibility = Visibility.Collapsed;
            TabletMainPage.Current.HideBackgroundImage();
            App.AppViewModel.CurrentVideoPlayer = null;
            _videoBlock.VideoPlayer.Close();
            base.OnNavigatingFrom(e);
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
            LoadingBar.Visibility = Visibility.Visible;
            if (App.AppViewModel.CurrentVideoPlayer != null)
                App.AppViewModel.CurrentVideoPlayer.Close();
            var source = RankDetailList.Where(p => p.Item1 == _selectRegion).FirstOrDefault();
            if (source != null)
            {
                DisplayCollection.Clear();
                if (source.Item2.Count == 0)
                {
                    var videos = await App.BiliViewModel._client.Video.GetRegionRankAsync(_selectRegion);
                    if (videos != null && videos.Count > 0)
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
            HolderText.Visibility = source.Item2.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            LoadingBar.Visibility=Visibility.Collapsed;
        }
        private async void RegionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionContainer;
            _selectRegion = item.tid;
            await LoadRegionRankVideo();
        }

        private async void VideoView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = VideoView.SelectedItem as WebVideo;
            if (item == null)
            {
                _videoBlock.Visibility = Visibility.Collapsed;
                HoldContainer.Visibility = Visibility.Visible;
                return;
            }
            HoldContainer.Visibility = Visibility.Collapsed;

            _videoBlock.Visibility = Visibility.Visible;
            var cache = _videoDetailList.Where(p => p.aid == Convert.ToInt32(item.aid)).FirstOrDefault();
            if (cache == null)
            {
                await _videoBlock.Init(Convert.ToInt32(item.aid));
                cache = _videoBlock._detail;
                _videoDetailList.Add(cache);
            }
            else
                await _videoBlock.Init(cache);
        }
    }
}
