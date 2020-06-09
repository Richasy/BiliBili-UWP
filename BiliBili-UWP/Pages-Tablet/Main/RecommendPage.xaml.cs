using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
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
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            await RefreshVideo();
            _isInit = true;
            base.OnNavigatedTo(e);
        }
        private async Task RefreshVideo()
        {
            DetailContainer.Visibility = Visibility.Collapsed;
            HoldContainer.Visibility = Visibility.Visible;
            VideoCollection.Clear();
            await LoadMoreRecommendVideo();
        }
        private async Task LoadMoreRecommendVideo()
        {
            LoadingRing.IsActive = true;
            var data = await App.BiliViewModel._client.GetRecommendVideoAsync();
            data.ForEach(p => VideoCollection.Add(p));
            CheckRecommendStatus();
            LoadingRing.IsActive = false;
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
                DetailContainer.Visibility = Visibility.Collapsed;
                HoldContainer.Visibility = Visibility.Visible;
                return;
            }
            DetailContainer.Visibility = Visibility.Collapsed;
            HoldContainer.Visibility = Visibility.Collapsed;
            if (item.card_goto == "bangumi")
            {

            }
            else
            {
                var cache = _videoDetailList.Where(p => p.aid == item.args.aid).FirstOrDefault();
                if (cache == null)
                {
                    LoadingRing.IsActive = true;
                    cache = await _videoService.GetVideoDetailAsync(item.args.aid);
                    _videoDetailList.Add(cache);
                }
                
                TabletMainPage.Current.SetBackgroundImage(cache.pic);
                InitDetail(cache);
                LoadingRing.IsActive = false;
            }
            DetailContainer.Visibility = Visibility.Visible;
        }

        private void InitDetail(VideoDetail _detail)
        {
            TitleBlock.Text = _detail.title;
            UserAvatar.ProfilePicture = new BitmapImage(new Uri(_detail.owner.face + "@50w.jpg"));
            UserNameBlock.Text = _detail.owner.name;
            PublishBlock.Text = "发布: " + AppTool.GetReadDateString(_detail.pubdate);
            PlayCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.view);
            DanmakuCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.danmaku);
            LikeCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.like);
            CoinCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.coin);
            FavoriteCountBlock.Text = AppTool.GetNumberAbbreviation(_detail.stat.favorite);
            DescriptionBlock.Text = _detail.desc;
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = VideoView.SelectedItem as VideoRecommend;
            if (selectItem != null)
            {
                if (selectItem.card_goto == "bangumi")
                {

                }
                else
                {
                    var detail = _videoDetailList.Where(p => p.aid == selectItem.args.aid).FirstOrDefault();
                    if (detail != null)
                    {
                        TabletMainPage.Current.PlayVideo(detail);
                    }
                }
            }
        }
    }
}
