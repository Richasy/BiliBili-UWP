using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Models.Core;
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

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DynamicPage : Page, IRefreshPage
    {
        public ObservableCollection<Topic> DynamicCollection = new ObservableCollection<Topic>();
        private List<Topic> TotalList = new List<Topic>();
        private bool _isInit = false;
        private bool _isDynamicRequesting = false;
        private BiliViewModel biliVM = App.BiliViewModel;
        private TopicService _topicService = App.BiliViewModel._client.Topic;
        private string offset = "";
        private double _scrollOffset = 0;
        private bool _isOnlyVideo = false;
        public DynamicPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            biliVM.IsLoginChanged += IsLoginChanged;
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
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (_isInit)
            {
                GetFollowerUnread();
                return;
            }
            await Refresh();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = null;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = null;
            App.AppViewModel.CurrentPagePanel.CheckSubReplyPage();
            base.OnNavigatingFrom(e);
        }
        private void Reset()
        {
            if (PageContainer.Visibility == Visibility.Collapsed)
                PageContainer.Visibility = Visibility.Visible;
            DynamicCollection.Clear();
            TotalList.Clear();
            LoadingRing.IsActive = false;
            offset = "";
            _scrollOffset = 0;
            _isOnlyVideo = AppTool.GetBoolSetting(BiliBili_Lib.Enums.Settings.IsDynamicOnlyVideo);
            OnlyVideoSwitch.IsOn = _isOnlyVideo;
            HolderText.Visibility = Visibility.Collapsed;
            DynamicLoadingBar.Visibility = Visibility.Collapsed;
        }
        public async Task Refresh()
        {
            _isInit = false;
            Reset();
            if (App.BiliViewModel.IsLogin)
            {
                LoadingRing.IsActive = true;
                await LoadDynamic();
                if (DynamicCollection.Count < 10)
                    await LoadDynamic();
                LoadingRing.IsActive = false;
            }
            else
            {
                HolderText.Visibility = Visibility.Visible;
            }
            _isInit = true;
        }

        private async Task LoadDynamic()
        {
            if (_isDynamicRequesting)
                return;
            _isDynamicRequesting = true;
            Tuple<string, List<Topic>> data = null;
            if (!LoadingRing.IsActive)
                DynamicLoadingBar.Visibility = Visibility.Visible;
            if (string.IsNullOrEmpty(offset))
            {
                string lastSeemId = AppTool.GetLocalSetting(BiliBili_Lib.Enums.Settings.LastSeemDynamicId, "0");
                var temp = await _topicService.GetNewDynamicAsync(lastSeemId);
                if (temp != null)
                    data = new Tuple<string, List<Topic>>(temp.history_offset, temp.cards);
            }
            else
                data = await _topicService.GetHistoryDynamicAsync(offset);
            if (data != null)
            {
                offset = data.Item1;
                data.Item2.ForEach(p => TotalList.Add(p));
                DynamicCollectionInit();
            }
            DynamicLoadingBar.Visibility = Visibility.Collapsed;
            HolderText.Visibility = DynamicCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            _isDynamicRequesting = false;
        }

        private void DynamicCollectionInit()
        {
            foreach (var item in TotalList)
            {
                if (!DynamicCollection.Contains(item))
                {
                    var temp = DynamicCollection.Where(p => p.desc.timestamp < item.desc.timestamp).FirstOrDefault();
                    int index = 0;
                    if (temp != null)
                        index = DynamicCollection.IndexOf(temp);
                    else
                        index = DynamicCollection.Count;
                    if ((_isOnlyVideo && ((item.desc.type == 8) || (item.desc.type == 512))) || !_isOnlyVideo)
                    {
                        DynamicCollection.Insert(index, item);
                    }
                }
            }
        }

        private async void ScrollViewerBottomHandle()
        {
            await LoadDynamic();
        }
        private void ScrollViewerChanged()
        {
            double offset = App.AppViewModel.CurrentPagePanel.PageScrollViewer.VerticalOffset;
            _scrollOffset = offset;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageContainer.Visibility = Visibility.Visible;
            if (_scrollOffset > 0)
            {
                await Task.Delay(50);
                App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, _scrollOffset, 1);
            }
        }
        private async void GetFollowerUnread()
        {
            if (App.BiliViewModel.IsLogin)
            {
                var count = await App.BiliViewModel._client.GetFollowerUnreadCountAsync();
                if (count > 0)
                {
                    //刷新动态
                    if (_isDynamicRequesting)
                        return;
                    _isDynamicRequesting = true;
                    string lastSeemId = AppTool.GetLocalSetting(BiliBili_Lib.Enums.Settings.LastSeemDynamicId, "0");
                    var data = await _topicService.GetNewDynamicAsync(lastSeemId);
                    if (data != null)
                    {
                        for (int i = data.cards.Count - 1; i >= 0; i--)
                        {
                            if (!TotalList.Contains(data.cards[i]))
                                TotalList.Insert(0, data.cards[i]);
                        }
                        DynamicCollectionInit();
                    }
                    _isDynamicRequesting = false;
                }
            }
        }

        private async void OnlyVideoSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            _isOnlyVideo = (sender as ToggleSwitch).IsOn;
            AppTool.WriteLocalSetting(BiliBili_Lib.Enums.Settings.IsDynamicOnlyVideo, _isOnlyVideo.ToString());
            if (_isOnlyVideo)
            {
                for (int i = DynamicCollection.Count - 1; i >= 0; i--)
                {
                    if (DynamicCollection[i].desc.type != 8 && DynamicCollection[i].desc.type != 512)
                        DynamicCollection.RemoveAt(i);
                }
                //判断过滤后的动态是否能让滚动条显示，不能则再请求一次
                if (DynamicCollection.Count < 10)
                    await LoadDynamic();
            }
            else
            {
                DynamicCollectionInit();
            }
        }
    }
}
