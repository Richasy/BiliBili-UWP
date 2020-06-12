using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Anime;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RecommendPage : Page
    {
        private IncreaseCollection<VideoRecommend> VideoCollection;
        private bool _isInit = false;
        private VideoService _videoService = App.BiliViewModel._client.Video;
        private List<VideoDetail> _videoDetailList = new List<VideoDetail>();
        private TabletVideoDetailBlock _videoBlock = App.AppViewModel.CurrentVideoDetailBlock;
        private TabletBangumiDetailBlock _bangumiBlock = App.AppViewModel.CurrentBangumiDetailBlock;
        public RecommendPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            VideoCollection = new IncreaseCollection<VideoRecommend>(async(col)=>
            {
                int idx = 0;
                if (col.Count > 0)
                    idx = col.Last().idx;
                var data = await App.BiliViewModel._client.GetRecommendVideoAsync(idx);
                col.HasMoreItems = true;
                return data;
            });
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DetailContainer.Children.Add(_videoBlock);
            DetailContainer.Children.Add(_bangumiBlock);
            _bangumiBlock.Visibility = Visibility.Collapsed;
            _videoBlock.Visibility = Visibility.Collapsed;
            VideoView.SelectedIndex = -1;
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            await RefreshVideo();
            _isInit = true;
            base.OnNavigatedTo(e);
        }
        private async Task InitBangumi(VideoRecommend item)
        {
            var sp = item.uri.Split("#");
            if (sp.Length > 1)
                await _bangumiBlock.Init(Convert.ToInt32(sp.Last()), true);
            else
                await _bangumiBlock.Init(Convert.ToInt32(item.param), true);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DetailContainer.Children.Remove(_videoBlock);
            DetailContainer.Children.Remove(_bangumiBlock);
            _videoBlock.Visibility = Visibility.Collapsed;
            _bangumiBlock.Visibility = Visibility.Collapsed;
            TabletMainPage.Current.HideBackgroundImage();
            App.AppViewModel.CurrentVideoPlayer = null;
            _videoBlock.VideoPlayer.Close();
            _bangumiBlock.VideoPlayer.Close();
            base.OnNavigatingFrom(e);
        }
        private async Task RefreshVideo()
        {
            if(App.AppViewModel.CurrentVideoPlayer != null)
            {
                App.AppViewModel.CurrentVideoPlayer.Close();
            }
            _videoBlock.Visibility = Visibility.Collapsed;
            _bangumiBlock.Visibility = Visibility.Collapsed;
            HoldContainer.Visibility = Visibility.Visible;
            VideoCollection.Clear();
            await LoadMoreRecommendVideo();
        }
        private async Task LoadMoreRecommendVideo()
        {
            var tip = new WaitingPopup("正在加载数据");
            tip.ShowPopup();
            var data = await App.BiliViewModel._client.GetRecommendVideoAsync();
            if (data != null)
            {
                data.ForEach(p => VideoCollection.Add(p));
            }
            CheckRecommendStatus();
            tip.HidePopup();
            VideoCollection.HasMoreItems = true;
        }
        private void CheckRecommendStatus()
        {
            HolderText.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        private void VideoContainer_NeedRemoveVideo(object sender, BiliBili_Lib.Models.BiliBili.VideoRecommend e)
        {
            VideoCollection.Remove(e);
            HolderText.Visibility = VideoCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void VideoView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = VideoView.SelectedItem as VideoRecommend;
            if(item==null)
            {
                if (App.AppViewModel.CurrentVideoPlayer != null)
                    App.AppViewModel.CurrentVideoPlayer.Close();
                _videoBlock.Visibility = Visibility.Collapsed;
                _bangumiBlock.Visibility = Visibility.Collapsed;
                HoldContainer.Visibility = Visibility.Visible;
                return;
            }
            
            HoldContainer.Visibility = Visibility.Collapsed;
            if (item.card_goto == "bangumi")
            {
                _bangumiBlock.Visibility = Visibility.Visible;
                await InitBangumi(item);
            }
            else
            {
                _videoBlock.Visibility = Visibility.Visible;
                try
                {
                    var cache = _videoDetailList.Where(p => p.aid == item.args.aid).FirstOrDefault();
                    if (cache == null)
                    {
                        await _videoBlock.Init(item.args.aid);
                        cache = _videoBlock._detail;
                        _videoDetailList.Add(cache);
                    }
                    else
                        await _videoBlock.Init(cache);
                }
                catch (InvalidDataException)
                {
                    await _bangumiBlock.Init(Convert.ToInt32(item.param), true);
                }
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshButton.IsLoading = true;
            await RefreshVideo();
            RefreshButton.IsLoading = false;
        }
    }
}
