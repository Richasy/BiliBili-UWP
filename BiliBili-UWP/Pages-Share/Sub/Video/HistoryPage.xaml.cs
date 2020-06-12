using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Share.Sub.Video
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HistoryPage : Page,IRefreshPage
    {
        private bool _isInit = false;
        public BiliViewModel biliVM = App.BiliViewModel;
        public AccountService _account = App.BiliViewModel._client.Account;
        private ObservableCollection<VideoDetail> HistoryCollection = new ObservableCollection<VideoDetail>();
        private bool _isHistoryRequesting = false;
        private int _page = 1;
        public HistoryPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            biliVM.IsLoginChanged += IsLoginChanged;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "历史记录";
            if (_isInit || e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            await Refresh();
            _isInit = true;
        }
        public async Task Refresh()
        {
            HistoryCollection.Clear();
            _page = 1;
            if (biliVM.IsLogin)
            {
                await LoadHistory();
            }
        }
        private async Task LoadHistory(bool isIncrease = false)
        {
            if (_isHistoryRequesting)
                return;
            _isHistoryRequesting = true;
            VideoLoadingBar.Visibility = Visibility.Visible;
            if (isIncrease)
                _page += 1;
            var data = await _account.GetVideoHistoryAsync(_page);
            if (data != null && data.Count > 0)
                data.ForEach(p => { if (!HistoryCollection.Contains(p)) { HistoryCollection.Add(p); } });
            HolderText.Visibility = HistoryCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            VideoLoadingBar.Visibility = Visibility.Collapsed;
            _isHistoryRequesting = false;
        }
        private async void ScrollViewerBottomHandle()
        {
            await LoadHistory(true);
        }
        private async void IsLoginChanged(object sender, bool e)
        {
            await Refresh();
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (!biliVM.CheckAccoutStatus())
                return;
            var dialog = new ConfirmDialog("您确认清空观看的历史记录吗？");
            dialog.PrimaryButtonClick += async (_s, _e) =>
            {
                _e.Cancel = true;
                dialog.IsPrimaryButtonEnabled = false;
                dialog.PrimaryButtonText = "清空中...";
                bool reuslt = await _account.ClearHistoryAsync();
                if (reuslt)
                {
                    new TipPopup("清空成功").ShowMessage();
                    HistoryCollection.Clear();
                    dialog.Hide();
                }
                else
                {
                    new TipPopup("清空失败").ShowError();
                }
                dialog.PrimaryButtonText = "确认";
                dialog.IsPrimaryButtonEnabled = true;
            };
            await dialog.ShowAsync();
        }

        private void HistoryVideoView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoDetail;
            if (item.bangumi != null)
                App.AppViewModel.PlayBangumi(item.bangumi.ep_id, null, true);
            else
                App.AppViewModel.PlayVideo(item.aid, null, "main.my-history.0.0");
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var aid = (sender as AppBarButton).Tag.ToString();
            bool result = await _account.DeleteHistoryAsync(Convert.ToInt32(aid));
            if (result)
            {
                HistoryCollection.Remove(HistoryCollection.Where(p => p.aid == Convert.ToInt32(aid)).FirstOrDefault());
                HolderText.Visibility = HistoryCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                new TipPopup("移出失败，请稍后重试").ShowError();
            }
        }
        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadHistory(true);
            }
        }
    }
}
