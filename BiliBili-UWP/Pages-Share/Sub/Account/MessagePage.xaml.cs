using BiliBili_Lib.Models.BiliBili.Feedback;
using BiliBili_Lib.Service;
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

namespace BiliBili_UWP.Pages_Share.Sub.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessagePage : Page,IRefreshPage
    {
        public ObservableCollection<IconItem> HeaderCollection = new ObservableCollection<IconItem>();
        public ObservableCollection<FeedReplyDetail> ReplyCollection = new ObservableCollection<FeedReplyDetail>();
        public ObservableCollection<FeedAtDetail> AtCollection = new ObservableCollection<FeedAtDetail>();
        public ObservableCollection<FeedLikeDetail> LikeCollection = new ObservableCollection<FeedLikeDetail>();
        private AccountService _accService = App.BiliViewModel._client.Account;
        private FeedCursor _replyCursor;
        private FeedCursor _atCursor;
        private FeedCursor _likeCursor;
        public MessagePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "我的消息";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            await Refresh();
            base.OnNavigatedTo(e);
        }
        public void Reset()
        {
            if (HeaderCollection.Count == 0)
            {
                var headers = IconItem.GetMessageHeaderItems();
                headers.ForEach(p => HeaderCollection.Add(p));
            }
            else
            {
                foreach (var header in HeaderCollection)
                {
                    header.Param = "";
                }
            }
            ReplyCollection.Clear();
            AtCollection.Clear();
            LikeCollection.Clear();
            LoadingRing.IsActive = false;
            NoDataContainer.Visibility = Visibility.Collapsed;

            _replyCursor = null;
            _likeCursor = null;
            _atCursor = null;
        }
        public async Task Refresh()
        {
            Reset();
            await InitHeader();
            if (HeaderListView.SelectedIndex != -1)
                await SwitchHeader(HeaderCollection[HeaderListView.SelectedIndex]);
            else
            {
                HeaderListView.SelectedIndex = 0;
                HeaderListView.SelectedItem = HeaderCollection.First();
                await SwitchHeader(HeaderCollection.First(), true);
            }
        }

        public async Task InitHeader()
        {
            var unread = await _accService.GetMyUnreadMessageAsync();
            if (unread != null)
            {
                foreach (var item in HeaderCollection)
                {
                    if (item.Name == StaticString.MESSAGE_REPLYME)
                        item.Param = unread.reply > 0 ? unread.reply.ToString() : "";
                    else if (item.Name == StaticString.MESSAGE_AT)
                        item.Param = unread.at > 0 ? unread.at.ToString() : "";
                    else if (item.Name == StaticString.MESSAGE_LIKE)
                        item.Param = unread.like > 0 ? unread.like.ToString() : "";
                }
            }
        }

        public async Task InitReply(bool isRefresh = false)
        {
            FeedListView.ItemTemplate = ReplyItemTemplate;
            FeedListView.ItemsSource = ReplyCollection;
            if (!isRefresh && ((_replyCursor != null && _replyCursor.is_end) || ReplyCollection.Count > 0))
            {
                NoDataContainer.Visibility = ReplyCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                return;
            }
            if (ReplyCollection.Count == 0)
                LoadingRing.IsActive = true;
            else
                LoadingBar.Visibility = Visibility.Visible;
            int time = _replyCursor == null ? 0 : _replyCursor.time;
            var replyResponse = await _accService.GetReplyMeListAsync(time);
            if (replyResponse != null)
            {
                _replyCursor = replyResponse.cursor;
                replyResponse.items.ForEach(p => { if (!ReplyCollection.Contains(p)) ReplyCollection.Add(p); });
            }
            NoDataContainer.Visibility = ReplyCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }
        public async Task InitAt(bool isRefresh = false)
        {
            FeedListView.ItemTemplate = AtItemTemplate;
            FeedListView.ItemsSource = AtCollection;
            if (!isRefresh && ((_atCursor != null && _atCursor.is_end) || AtCollection.Count > 0))
            {
                NoDataContainer.Visibility = AtCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                return;
            }
            if (AtCollection.Count == 0)
                LoadingRing.IsActive = true;
            else
                LoadingBar.Visibility = Visibility.Visible;
            int time = _atCursor == null ? 0 : _atCursor.time;
            long id = _atCursor == null ? 0 : _atCursor.id;
            var atResponse = await _accService.GetAtMeListAsync(id, time);
            if (atResponse != null)
            {
                _atCursor = atResponse.cursor;
                atResponse.items.ForEach(p => { if (!AtCollection.Contains(p)) AtCollection.Add(p); });
            }
            NoDataContainer.Visibility = AtCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }
        public async Task InitLike(bool isRefresh = false)
        {
            FeedListView.ItemTemplate = LikeItemTemplate;
            FeedListView.ItemsSource = LikeCollection;
            if (!isRefresh && ((_likeCursor != null && _likeCursor.is_end) || LikeCollection.Count > 0))
            {
                NoDataContainer.Visibility = LikeCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                return;
            }
            if (LikeCollection.Count == 0)
                LoadingRing.IsActive = true;
            else
                LoadingBar.Visibility = Visibility.Visible;
            long id = _likeCursor == null ? 0 : _likeCursor.id;
            int time = _likeCursor == null ? 0 : _likeCursor.time;
            var likeResponse = await _accService.GetLikeMeListAsync(id, time);
            if (likeResponse != null)
            {
                if (likeResponse.latest != null && likeResponse.latest.items != null && likeResponse.latest.items.Count > 0)
                {
                    likeResponse.latest.items.ForEach(p => { p.is_latest = true; LikeCollection.Add(p); });
                }
                if (likeResponse.total != null)
                {
                    _likeCursor = likeResponse.total.cursor;
                    if (likeResponse.total.items != null && likeResponse.total.items.Count > 0)
                        likeResponse.total.items.ForEach(p => { p.is_latest = false; LikeCollection.Add(p); });
                }
            }
            NoDataContainer.Visibility = LikeCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            LoadingRing.IsActive = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }

        private async void HeaderListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as IconItem;
            await SwitchHeader(item);
        }

        private async Task SwitchHeader(IconItem item, bool isRefresh = false)
        {
            if (item.Name == StaticString.MESSAGE_REPLYME)
                await InitReply(isRefresh);
            else if (item.Name == StaticString.MESSAGE_AT)
                await InitAt(isRefresh);
            else if (item.Name == StaticString.MESSAGE_LIKE)
                await InitLike(isRefresh);
            await InitHeader();
            await App.BiliViewModel.CheckUnreadMessage();
        }
        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                var item = HeaderListView.SelectedItem as IconItem;
                if (item != null)
                {
                    await SwitchHeader(item, true);
                }
            }
        }
    }
}
