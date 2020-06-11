using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Models.Core;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page, IRefreshPage
    {
        private bool _isInit = false;
        public BiliViewModel channelVM = App.BiliViewModel;
        public ObservableCollection<VideoRecommend> RecommendCollection = App.BiliViewModel.RecommendVideoCollection;
        private bool _isRecommendRequesting = false;
        private double _scrollOffset = 0;

        public HomePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            channelVM.IsLoginChanged += IsLoginChanged;
        }

        private async void IsLoginChanged(object sender, bool e)
        {
            await Refresh();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = ScrollViewerBottomHandle;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = ScrollViewerChanged;
            RecommendVideoView.EnableAnimation = App.AppViewModel.IsEnableAnimation;
            RecommendVideoView.DesiredWidth = 220 + ((App.AppViewModel.BasicFontSize - 14) * 5);
            if (_isInit || e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            await Refresh();
            _isInit = true;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = null;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = null;
            base.OnNavigatingFrom(e);
        }
        public async Task Refresh()
        {
            if (PageContainer.Visibility == Visibility.Collapsed)
                PageContainer.Visibility = Visibility.Visible;
            await channelVM.GetChannelSquareAsync();
            ScanPanel.Visibility = App.BiliViewModel.IsLogin ? Visibility.Visible : Visibility.Collapsed;
            await RefreshVideo();
        }

        private async Task LoadMoreRecommendVideo()
        {
            int idx = 0;
            VideoLoadingBar.Visibility = Visibility.Visible;
            if (RecommendCollection.Count > 0)
                idx = RecommendCollection.Last().idx;
            var data = await channelVM._client.GetRecommendVideoAsync(idx);
            data.ForEach(p => RecommendCollection.Add(p));
            CheckRecommendStatus();
            VideoLoadingBar.Visibility = Visibility.Collapsed;
        }

        private void CheckRecommendStatus()
        {
            HolderText.Visibility = RecommendCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ScrollViewerBottomHandle()
        {
            if (_isRecommendRequesting)
                return;
            _isRecommendRequesting = true;
            await LoadMoreRecommendVideo();
            _isRecommendRequesting = false;
        }

        private void RecommendVideoView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoRecommend;
            var container = RecommendVideoView.ContainerFromItem(item);
            if (item.card_goto == "av")
                App.AppViewModel.PlayVideo(item.args.aid, container, StaticString.SIGN_RECOMMEND);
            else if (item.card_goto == "bangumi")
            {
                var sp = item.uri.Split("#");
                if (sp.Length > 1)
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(sp.Last()), container, true);
                else
                    App.AppViewModel.PlayBangumi(Convert.ToInt32(item.param), container, true);
            }
        }

        private async Task RefreshVideo()
        {
            RecommendCollection.Clear();
            _scrollOffset = 0;
            int i = 0;
            while (i < 2)
            {
                await LoadMoreRecommendVideo();
                i++;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshVideo();
        }
        private void ScrollViewerChanged()
        {
            double offset = App.AppViewModel.CurrentPagePanel.PageScrollViewer.VerticalOffset;
            _scrollOffset = offset;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageContainer.Visibility = Visibility.Visible;
            await Task.Delay(100);
            if (_scrollOffset > 0)
            {
                App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, _scrollOffset, 1);
            }
        }

        private void VideoContainer_NeedRemoveVideo(object sender, VideoRecommend e)
        {
            RecommendCollection.Remove(e);
        }

        private void RecommendVideoView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            RecommendVideoCard card = (RecommendVideoCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
