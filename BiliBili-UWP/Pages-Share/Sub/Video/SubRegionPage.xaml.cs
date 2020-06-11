using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Controls;
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

namespace BiliBili_UWP.Pages_Share.Sub.Video
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubRegionPage : Page, IRefreshPage
    {
        private ObservableCollection<Region> TabCollection = new ObservableCollection<Region>();
        private List<TempVideoStore> VideoList = new List<TempVideoStore>();
        private List<TempSortStore> SortTypeList = new List<TempSortStore>();
        private List<IconItem> SortTypeSelectionList = IconItem.GetSubRegionSortItems();
        private RegionContainer _regionContainer;
        private int _selectedRegion = 0;
        private bool _isRequesting = false;
        private bool _isInit = false;
        private VideoService _videoService = App.BiliViewModel._client.Video;
        public SubRegionPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is RegionContainer _con)
            {
                _regionContainer = _con;
            }
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = _regionContainer.name + "分区";
            await Refresh();
            _isInit = true;
        }
        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            var first = VideoList.First();
            var videos = await _videoService.GetSubRegionDefaultAsync(first.rid, first.ctime);
            if (videos != null)
            {
                first.ctime = videos.Item1;
                videos.Item2.ForEach(p => first.videos.Add(p));
                VideoListView.ItemsSource = first.videos;
                VideoHolderBlock.Visibility = first.videos.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            LoadingRing.IsActive = false;
        }
        public void Reset()
        {
            _isInit = false;
            TabCollection.Clear();
            VideoList.Clear();
            SortTypeList.Clear();
            VideoSortComboBox.SelectedIndex = 0;

            _selectedRegion = 0;
            _isRequesting = false;

            if (_regionContainer != null)
            {
                foreach (var item in _regionContainer.children)
                {
                    TabCollection.Add(item);
                    VideoList.Add(new TempVideoStore(item.tid));
                    SortTypeList.Add(new TempSortStore(item.tid));
                }
                _selectedRegion = VideoList.First().rid;
                TabListView.SelectedIndex = 0;
            }
            _isInit = true;
        }
        private async void VideoSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var source = SortTypeList.Where(p => p.rid == _selectedRegion).First();
            var item = VideoSortComboBox.SelectedItem as IconItem;
            if (source.sort != item.Param.ToString())
            {
                source.sort = item.Param.ToString();
                source.pn = 1;
                var video = VideoList.Where(p => p.rid == _selectedRegion).First();
                video.videos.Clear();
                video.ctime = 0;
                await RefreshVideo();
            }
        }
        private async Task RefreshVideo(bool isMore = false)
        {
            VideoHolderBlock.Visibility = Visibility.Collapsed;
            var type = SortTypeList.Where(p => p.rid == _selectedRegion).First();
            var video = VideoList.Where(p => p.rid == _selectedRegion).First();
            if (!string.IsNullOrEmpty(type.sort))
            {
                if (isMore)
                    type.pn += 1;
                var response = await _videoService.GetSubRegionSortVideoAsync(_selectedRegion, type.sort, type.pn);
                if (response != null && response.Count > 0)
                    response.ForEach(p => video.videos.Add(p));
            }
            else
            {
                var response = await _videoService.GetSubRegionDefaultAsync(_selectedRegion, video.ctime);
                if (response != null)
                {
                    video.ctime = response.Item1;
                    response.Item2.ForEach(p => video.videos.Add(p));
                }
            }
            if (video.videos.Count == 0)
                VideoHolderBlock.Visibility = Visibility.Visible;
        }
        

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                if (!_isRequesting)
                {
                    _isRequesting = true;
                    await RefreshVideo(true);
                    _isRequesting = false;
                }
            }
        }

        private void VideoListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RegionVideo;
            if(item.@goto=="av")
                App.AppViewModel.PlayVideo(Convert.ToInt32(item.param));
            else if (item.@goto == "bangumi")
            {
                var sp = item.uri.Split("#");
                if (sp.Length > 1)
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(sp.Last()), null, true);
                else
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(item.param), null);
            } 
        }

        private async void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tab = e.ClickedItem as Region;
            if (tab.tid != _selectedRegion)
            {
                _selectedRegion = tab.tid;
                var targetSort = SortTypeList.Where(p => p.rid == _selectedRegion).First();
                var targetVideo = VideoList.Where(p => p.rid == _selectedRegion).First();
                var sortIndex = SortTypeSelectionList.IndexOf(SortTypeSelectionList.Where(p => p.Param.ToString() == targetSort.sort).First());
                _isInit = false;
                VideoSortComboBox.SelectedIndex = sortIndex;
                VideoListView.ItemsSource = targetVideo.videos;
                if (targetVideo.videos.Count == 0)
                {
                    LoadingRing.IsActive = true;
                    await RefreshVideo();
                    LoadingRing.IsActive = false;
                }
                _isInit = true;
            }
        }
        public class TempVideoStore
        {
            public int rid { get; set; }
            public int ctime { get; set; }
            public ObservableCollection<RegionVideo> videos { get; set; }
            public TempVideoStore(int r)
            {
                rid = r;
                ctime = 0;
                videos = new ObservableCollection<RegionVideo>();
            }
        }
        public class TempSortStore
        {
            public int rid { get; set; }
            public string sort { get; set; }
            public int pn { get; set; }
            public TempSortStore(int r)
            {
                rid = r;
                sort = "";
                pn = 1;
            }
        }

        private void VideoListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoPanel card = (DefaultVideoPanel)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
