using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
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

namespace BiliBili_UWP.Pages_Share.Sub.Video
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VideoDynamicPage : Page,IRefreshPage
    {
        private ObservableCollection<Topic> DynamicCollection = new ObservableCollection<Topic>();
        private TopicService _topicService = App.BiliViewModel._client.Topic;
        private bool _isDynamicRequesting = false;
        private string _dynamicOffset = "";
        private bool _isInit = false;
        public VideoDynamicPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "视频动态";
            if (e.NavigationMode == NavigationMode.Back)
                return;
            if (_isInit)
            {
                GetFollowerUnread();
                return;
            }
            await Refresh();
            base.OnNavigatedTo(e);
        }

        public async Task Refresh()
        {
            _isInit = false;
            LoadingRing.IsActive = true;
            DynamicCollection.Clear();
            _dynamicOffset = "";
            _isDynamicRequesting = false;
            await LoadMoreDynamic();
            LoadingRing.IsActive = false;
            _isInit = true;
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                if (!string.IsNullOrEmpty(_dynamicOffset))
                    await LoadMoreDynamic();
            }
        }
        private async Task LoadMoreDynamic()
        {
            if (!App.BiliViewModel.IsLogin)
            {
                DynamicCollection.Clear();
                DynamicHolderText.Visibility = Visibility.Visible;
            }
            if (!_isDynamicRequesting)
            {
                _isDynamicRequesting = true;
                Tuple<string, List<Topic>> data = null;
                if (string.IsNullOrEmpty(_dynamicOffset))
                {
                    string lastSeemId = AppTool.GetLocalSetting(BiliBili_Lib.Enums.Settings.LastSeemDynamicId, "0");
                    var temp = await _topicService.GetNewDynamicAsync(lastSeemId);
                    if (temp != null)
                        data = new Tuple<string, List<Topic>>(temp.history_offset, temp.cards);
                }
                else
                    data = await _topicService.GetHistoryDynamicAsync(_dynamicOffset);
                if (data != null)
                {
                    _dynamicOffset = data.Item1;
                    DynamicCollectionInit(data.Item2);
                }
                DynamicHolderText.Visibility = DynamicCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                _isDynamicRequesting = false;
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
                        DynamicCollectionInit(data.cards);
                    }
                    _isDynamicRequesting = false;
                }
            }
        }
        private void DynamicCollectionInit(List<Topic> TotalList)
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
                    if (item.desc.type == 8 || item.desc.type == 512)
                    {
                        DynamicCollection.Insert(index, item);
                    }
                }
            }
        }
        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            TopicCard card = (TopicCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
