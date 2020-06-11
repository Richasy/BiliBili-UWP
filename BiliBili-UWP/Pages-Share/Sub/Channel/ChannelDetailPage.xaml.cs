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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Share.Sub.Channel
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ChannelDetailPage : Page, IRefreshPage
    {
        private bool _isInit = false;
        private int _channelId = 0;
        private ObservableCollection<Tag> TagCollection = new ObservableCollection<Tag>();
        private ObservableCollection<VideoChannel> VideoCollection = new ObservableCollection<VideoChannel>();
        private ObservableCollection<Topic> TopicCollection = new ObservableCollection<Topic>();
        private List<IconItem> VideoSortTypeList = IconItem.GetChannelVideoSortItems();
        private ChannelDetail _detail = null;
        private string _sortType = "hot";
        private string _videoOffset = "";
        private string _topicOffset = "";
        private bool _isVideoRequesting = false;
        private bool _isTopicRequesting = false;
        private VideoBase _header = null;
        public ChannelDetailPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        public async Task Refresh()
        {
            ResetPage();
            var detail = await App.BiliViewModel._client.Channel.GetChannelDetailInfoAsync(_channelId);
            _detail = detail;
            if (_detail != null)
            {
                TagListView.Visibility = Visibility.Visible;
                if (detail.tags != null && detail.tags.Count > 0)
                    detail.tags.ForEach(p => TagCollection.Add(p));
                else
                    TagListView.Visibility = Visibility.Collapsed;
                TitleBlock.Text = detail.title;
                SubscribeButton.Text = detail.is_atten == 1 ? "取消订阅" : "订阅";
                InfoBlock.Text = $"{detail.label_1} | {detail.label_2} | {detail.label_3}";
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

        private async Task RefreshVideo()
        {
            var data = await App.BiliViewModel._client.Channel.GetChannelVideosAsync(_channelId, _sortType, _videoOffset);
            if (data != null)
            {
                _videoOffset = data.Item1;
                if (data.Item2 != null)
                {
                    _header = data.Item2;
                    HeaderVideoContainer.Visibility = Visibility.Visible;
                    HeaderVideoImage.Source = new BitmapImage(new Uri(_header.GetResolutionCover("400")));
                    HeaderVideoTitle.Text = _header.title;
                    HeaderVideoPlayCount.Text = _header.cover_left_text_1;
                }
                if (data.Item3 != null && data.Item3.Count > 0)
                {
                    data.Item3.ForEach(p => VideoCollection.Add(p));
                }
            }
        }

        private async Task RefreshTopic()
        {
            var data = await App.BiliViewModel._client.Topic.GetTopicAsync(_channelId, _detail.title, _topicOffset);
            if (data != null)
            {
                _topicOffset = data.Item1;
                if (data.Item2 != null && data.Item2.Count > 0)
                {
                    data.Item2.ForEach(p => TopicCollection.Add(p));
                }
            }
        }

        private void ResetPage()
        {
            _isInit = false;

            HolderContainer.Visibility = Visibility.Visible;
            TagCollection.Clear();
            TagListView.Visibility = Visibility.Collapsed;
            _detail = null;
            TitleBlock.Text = "--";
            SubscribeButton.Text = "--";
            InfoBlock.Text = "--";
            _sortType = "hot";
            _videoOffset = "";
            _topicOffset = "";
            VideoSortComboBox.SelectedIndex = 0;
            VideoCollection.Clear();
            TopicCollection.Clear();

            HeaderVideoContainer.Visibility = Visibility.Collapsed;
            _isVideoRequesting = false;

            _isInit = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = StaticString.CHANNEL_DETAIL;
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (e.Parameter != null && e.Parameter is int channelId)
            {
                _channelId = channelId;
                VideoButton.IsChecked = true;
                TopicButton.IsChecked = false;
                VideoContainer.Visibility = Visibility.Visible;
                TopicContainer.Visibility = Visibility.Collapsed;
                await Refresh();
            }
            _isInit = true;
        }

        private async void TagListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Tag;
            _channelId = item.id;
            await Refresh();
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

        private async void VideoSortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = VideoSortComboBox.SelectedItem as IconItem;
            if (item.Param.ToString() != _sortType)
            {
                _sortType = item.Param.ToString();
                VideoCollection.Clear();
                HeaderVideoContainer.Visibility = Visibility.Collapsed;
                _videoOffset = "";
                await RefreshVideo();
            }
        }

        private void VideoGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var data = e.ClickedItem as VideoChannel;
            if (data.@goto == "av")
                App.AppViewModel.PlayVideo(Convert.ToInt32(data.param),null, StaticString.SIGN_CHANNEL);
            else if (data.@goto == "bangumi")
                App.AppViewModel.PlayBangumi(Convert.ToInt32(data.param), null, true);
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

        private void RankButton_Click(object sender, RoutedEventArgs e)
        {
            string theme = App.Current.RequestedTheme == ApplicationTheme.Dark ? "282828" : "FFFFFF";
            string url = $"https://www.bilibili.com/h5/channel/rank?id={_channelId}&theme=%23{theme}&navhide=1";
            App.AppViewModel.ShowWebPopup(_detail.title + " 排行榜", url);
        }

        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_detail != null)
            {
                SubscribeButton.IsLoading = true;
                bool result = false;
                if (_detail.is_atten == 1)
                {
                    result = await App.BiliViewModel._client.Channel.UnsubscribeChannelAsync(_channelId);
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
                    result = await App.BiliViewModel._client.Channel.SubscribeChannelAsync(_channelId);
                    if (!result)
                        new TipPopup("订阅频道失败").ShowError();
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

        private void HeaderVideoContainer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_header != null)
            {
                App.AppViewModel.PlayVideo(Convert.ToInt32(_header.param));
            }
        }

        private async void LaterViewButton_Click(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as VideoChannel;
            await App.BiliViewModel.AddViewLater(sender, Convert.ToInt32(data.param));
        }

        private void VideoGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoCard card = (DefaultVideoCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }

        private void TopicListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            TopicCard card = (TopicCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
