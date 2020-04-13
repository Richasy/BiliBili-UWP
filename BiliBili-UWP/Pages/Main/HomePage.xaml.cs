using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.Core;
using BiliBili_UWP.Models.UI.Interface;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            App.AppViewModel.CurrentPagePanel.IsStretch = true;
            if (_isInit || e.NavigationMode == NavigationMode.Back)
                return;
            await Refresh();
            _isInit = true;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.AppViewModel.CurrentPagePanel.IsStretch = false;
            base.OnNavigatingFrom(e);
        }
        public async Task Refresh()
        {
            RecommendCollection.Clear();
            await channelVM.GetChannelSquareAsync();
            int i = 0;
            while (i < 4)
            {
                await LoadMoreRecommendVideo();
                i++;
            }
        }

        private async Task LoadMoreRecommendVideo()
        {
            int idx = 0;
            VideLoadingBar.Visibility = Visibility.Visible;
            if (RecommendCollection.Count > 0)
                idx = RecommendCollection.Last().idx;
            var data = await channelVM._client.GetRecommendVideoAsync(idx);
            data.ForEach(p => RecommendCollection.Add(p));
            CheckRecommendStatus();
            VideLoadingBar.Visibility = Visibility.Collapsed;
        }

        private void CheckRecommendStatus()
        {
            HolderText.Visibility = RecommendCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            RecommendVideoView.Visibility = RecommendCollection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_isRecommendRequesting)
                return;
            _isRecommendRequesting = true;
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadMoreRecommendVideo();
            }
            _isRecommendRequesting = false;
        }

        private void RecommendVideoView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoRecommend;
            App.AppViewModel.PlayVideo(item.args.aid);
        }
    }
}
