using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Share.Sub.Channel
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TagDetailPage : Page, IRefreshPage
    {
        private bool _isInit = false;
        private int _tagId = 0;
        private ObservableCollection<VideoRecommend> VideoCollection = new ObservableCollection<VideoRecommend>();
        private ObservableCollection<Topic> TopicCollection = new ObservableCollection<Topic>();
        private ChannelTag _detail = null;
        private string _topicOffset = "";
        private int _videoOffset = 1;
        private bool _isVideoRequesting = false;
        private bool _isTopicRequesting = false;
        public TagDetailPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = StaticString.TAG_DETAIL;
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is int tagId)
            {
                _tagId = tagId;
                VideoButton.IsChecked = false;
                TopicButton.IsChecked = true;
                VideoContainer.Visibility = Visibility.Collapsed;
                TopicContainer.Visibility = Visibility.Visible;
                await Refresh();
            }
            _isInit = true;
        }
        public async Task Refresh()
        {
            ResetPage();
            var detail = await App.BiliViewModel._client.Channel.GetTagDetail(_tagId);
            _detail = detail;
            if (_detail != null)
            {
                TitleBlock.Text = detail.name;
                SubscribeButton.Text = detail.is_atten == 1 ? "取消订阅" : "订阅";
                InfoBlock.Text = $"{AppTool.GetNumberAbbreviation(detail.atten)}人订阅";
                if (Convert.ToBoolean(VideoButton.IsChecked))
                {
                    await RefreshVideo();
                }
                else
                {
                    await RefreshTopic();
                }
                HolderContainer.Visibility = Visibility.Collapsed;
            }
        }
        private void ResetPage()
        {
            _isInit = false;

            HolderContainer.Visibility = Visibility.Visible;
            _detail = null;
            TitleBlock.Text = "--";
            SubscribeButton.Text = "--";
            InfoBlock.Text = "--";
            _topicOffset = "";
            _videoOffset = 1;
            VideoCollection.Clear();
            TopicCollection.Clear();
            _isVideoRequesting = false;

            _isInit = true;
        }
        private async Task RefreshVideo()
        {
            var data = await App.BiliViewModel._client.Channel.GetTagRecommendVideo(_tagId,_videoOffset);
            if (data != null)
            {
                _videoOffset += 1;
                data.ForEach(p => VideoCollection.Add(p));
            }
        }

        private async Task RefreshTopic()
        {
            var data = await App.BiliViewModel._client.Topic.GetTopicAsync(_tagId, _detail.name, _topicOffset);
            if (data != null)
            {
                _topicOffset = data.Item1;
                if (data.Item2 != null && data.Item2.Count > 0)
                {
                    data.Item2.ForEach(p => TopicCollection.Add(p));
                }
            }
        }
        private async void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            TopicButton.IsChecked = false;
            VideoContainer.Visibility = Visibility.Visible;
            TopicContainer.Visibility = Visibility.Collapsed;
            if (VideoCollection.Count == 0)
            {
                LoadingRing.IsActive = true;
                await RefreshVideo();
                LoadingRing.IsActive = false;
            }
        }

        private async void TopicButton_Click(object sender, RoutedEventArgs e)
        {
            VideoButton.IsChecked = false;
            VideoContainer.Visibility = Visibility.Collapsed;
            TopicContainer.Visibility = Visibility.Visible;
            if (TopicCollection.Count == 0)
            {
                LoadingRing.IsActive = true;
                await RefreshTopic();
                LoadingRing.IsActive = false;
            }
        }

        private void VideoListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as VideoRecommend;
            App.AppViewModel.PlayVideo(Convert.ToInt32(data.param),null, "traffic.new-channel-detail-featured.0.0.0.0");
        }
        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            bool isVideo = Convert.ToBoolean(VideoButton.IsChecked);
            var ele = sender as ScrollViewer;
            if (isVideo)
            {
                if (_isVideoRequesting)
                    return;
                _isVideoRequesting = true;

                if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
                {
                    await RefreshVideo();
                }
                _isVideoRequesting = false;
            }
            else
            {
                if (_isTopicRequesting)
                    return;
                _isTopicRequesting = true;

                if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
                {
                    await RefreshTopic();
                }
                _isTopicRequesting = false;
            }
        }
        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_detail != null)
            {
                SubscribeButton.IsLoading = true;
                bool result = false;
                if (_detail.is_atten == 1)
                {
                    result = await App.BiliViewModel._client.Channel.UnsubscribeChannelAsync(_tagId);
                    if (!result)
                        new TipPopup("取消订阅失败").ShowError();
                    else
                    {
                        SubscribeButton.Text = "订阅";
                        _detail.is_atten = 0;
                    }
                }
                else
                {
                    result = await App.BiliViewModel._client.Channel.SubscribeChannelAsync(_tagId);
                    if (!result)
                        new TipPopup("订阅标签失败").ShowError();
                    else
                    {
                        SubscribeButton.Text = "取消订阅";
                        _detail.is_atten = 1;
                    }
                }
                if (result)
                    App.BiliViewModel._isChannelChanged = true;
                SubscribeButton.IsLoading = false;
            }
        }
        private async void LaterViewButton_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as VideoRecommend;
            await App.BiliViewModel.AddViewLater(sender, Convert.ToInt32(data.param));
        }

        private void TopicListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            TopicCard card = (TopicCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
