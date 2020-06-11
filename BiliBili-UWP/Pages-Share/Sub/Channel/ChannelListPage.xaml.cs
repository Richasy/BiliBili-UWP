using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
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

namespace BiliBili_UWP.Pages_Share.Sub.Channel
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ChannelListPage : Page, IRefreshPage
    {
        public ObservableCollection<ChannelTab> TabCollection = new ObservableCollection<ChannelTab>();
        public List<Tuple<int, ObservableCollection<ChannelListItem>>> GroupChannelList = new List<Tuple<int, ObservableCollection<ChannelListItem>>>();
        private ChannelService _channelService = App.BiliViewModel._client.Channel;
        private int _selectedId = 100;
        private bool _isRequesting = false;
        private bool _isInit = false;
        public ChannelListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            App.BiliViewModel.IsLoginChanged += IsLoginChanged;
        }

        private async void IsLoginChanged(object sender, bool e)
        {
            await Refresh();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = StaticString.CHANNEL_TOTAL;
            if (_isInit || e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            await Refresh();
            _isInit = true;
        }
        public async Task Refresh()
        {
            Reset();
            LoadingRing.IsActive = true;
            await GetTabs();
            await RefreshSelectedChannels();
            LoadingRing.IsActive = false;
        }
        private async Task GetTabs()
        {
            var tabs = await _channelService.GetChannelTabsAsync();
            if (tabs.Count > 0)
            {
                foreach (var item in tabs)
                {
                    TabCollection.Add(item);
                    var channel = new Tuple<int, ObservableCollection<ChannelListItem>>(item.id, new ObservableCollection<ChannelListItem>());
                    if (item.id == 100)
                        ChannelListView.ItemsSource = channel.Item2;
                    GroupChannelList.Add(channel);
                }
            }
            var index = tabs.IndexOf(tabs.Where(p => p.id == 100).FirstOrDefault());
            TabListView.SelectedIndex = index;
            _selectedId = 100;
            ChannelListView.ItemTemplate = DefaultChannelItemTemplate;
        }
        private async Task RefreshSelectedChannels()
        {
            var tab = TabCollection.Where(p => p.id == _selectedId).FirstOrDefault();
            if (tab == null)
                return;
            var collection = GroupChannelList.Where(p => p.Item1 == _selectedId).FirstOrDefault().Item2;
            if (collection.Count >= tab.count)
                return;
            List<ChannelListItem> items = new List<ChannelListItem>();
            if (_selectedId == 999)
            {
                //我的订阅
                items = await _channelService.GetMySubscibeChannelsAsync(collection.Count > 0 ? collection.Count.ToString() : "");
                items.ForEach(p => p.is_atten = 1);
            }
            else
            {
                items = await _channelService.GetChannelListAsync(_selectedId, collection.Count.ToString());
            }
            if (items != null && items.Count>0)
            {
                items.ForEach(p => collection.Add(p));
            }
            LoadingRing.IsActive = false;
        }
        public void Reset()
        {
            TabListView.SelectedIndex = -1;
            ChannelListView.SelectedIndex = -1;
            ChannelListView.ItemsSource = new List<ChannelListItem>();
            TabCollection.Clear();
            GroupChannelList.Clear();
            _selectedId = 100;
            HolderBlock.Visibility = Visibility.Collapsed;
        }
        private void ChannelSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var item = args.ChosenSuggestion as ChannelListItem;
                App.AppViewModel.NavigateToSubPage(typeof(ChannelDetailPage), item.id);
                ChannelSearchBox.Text = "";
            }
        }

        private void ChannelListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ChannelListItem;
            App.AppViewModel.NavigateToSubPage(typeof(ChannelDetailPage), item.id);
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_isRequesting)
                return;
            var ele = sender as ScrollViewer;
            _isRequesting = true;

            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await RefreshSelectedChannels();
            }
            _isRequesting = false;
        }

        private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                var btn = sender as AsyncButton;
                var data = btn.DataContext as ChannelListItem;
                bool result = false;
                if (data.is_atten == 0)
                    result = await _channelService.SubscribeChannelAsync(data.id);
                else
                    result = await _channelService.UnsubscribeChannelAsync(data.id);
                if (result)
                {
                    data.is_atten = data.is_atten == 0 ? 1 : 0;
                    btn.Text = data.is_atten == 1 ? "取消订阅" : "订阅";
                    if (data.is_atten == 0 && _selectedId==999)
                    {
                        var collection = GroupChannelList.Where(p => p.Item1 == _selectedId).FirstOrDefault().Item2;
                        collection.Remove(data);
                        CheckCollectionCount();
                    }
                    App.BiliViewModel._isChannelChanged = true;
                }
                else
                    new TipPopup("操作失败，请稍后重试").ShowError();
            }
        }

        private void CheckCollectionCount()
        {
            var temp = GroupChannelList.Where(p => p.Item1 == _selectedId).FirstOrDefault();
            if (temp != null)
            {
                var collection = temp.Item2;
                HolderBlock.Visibility = collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void TabListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tab=e.ClickedItem as ChannelTab;
            if (tab.id != _selectedId)
            {
                _selectedId = tab.id;
                ChannelListView.ItemTemplate = tab.id == 999 ? MyChannelItemTemplate : DefaultChannelItemTemplate;
                ChannelListView.ItemsSource = null;
                ChannelListView.SelectedIndex = -1;
                var collection = GroupChannelList.Where(p => p.Item1 == tab.id).FirstOrDefault().Item2;
                collection.Clear();
                LoadingRing.IsActive = true;
                await RefreshSelectedChannels();
                LoadingRing.IsActive = false;
                CheckCollectionCount();
                ChannelListView.ItemsSource = collection;
            }
        }

        private async void ChannelSearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string text = ChannelSearchBox.Text;
                SearchErrorFlyout.Hide();
                if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
                {
                    SearchLoadBar.Visibility = Visibility.Visible;
                    var items = await _channelService.GetChannelSearchResult(text.Trim());
                    if (items != null && items.Count > 0)
                    {
                        ChannelSearchBox.ItemsSource = items;
                    }
                    else
                    {
                        SearchErrorFlyout.ShowAt(ChannelSearchBox);
                    }
                    SearchLoadBar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ChannelSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(ChannelSearchBox.Text))
                ChannelSearchBox.ItemsSource = null;
        }
    }
}
