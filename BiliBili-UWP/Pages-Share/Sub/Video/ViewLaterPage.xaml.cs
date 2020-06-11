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
    public sealed partial class ViewLaterPage : Page,IRefreshPage
    {
        private bool _isInit = false;
        public BiliViewModel biliVM = App.BiliViewModel;
        public AccountService _account = App.BiliViewModel._client.Account;
        private ObservableCollection<VideoDetail> ViewLaterCollection = new ObservableCollection<VideoDetail>();
        private bool _isViewLaterRequesting = false;
        private int _page = 1;
        public ViewLaterPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            biliVM.IsLoginChanged += IsLoginChanged;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            App.AppViewModel.CurrentSubPageControl.SubPageTitle = "稍后再看";
            if (_isInit || e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            await Refresh();
            _isInit = true;
        }
        public async Task Refresh()
        {
            ViewLaterCollection.Clear();
            _page = 1;
            if (biliVM.IsLogin)
            {
                await LoadViewLater();
            }
        }
        private async Task LoadViewLater(bool isIncrease = false)
        {
            if (_isViewLaterRequesting)
                return;
            _isViewLaterRequesting = true;
            VideoLoadingBar.Visibility = Visibility.Visible;
            if (isIncrease)
                _page += 1;
            var data = await _account.GetViewLaterAsync(_page);
            if (data != null && data.Count > 0)
                data.ForEach(p => ViewLaterCollection.Add(p));
            HolderText.Visibility = ViewLaterCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            VideoLoadingBar.Visibility = Visibility.Collapsed;
            _isViewLaterRequesting = false;
        }
        private async void IsLoginChanged(object sender, bool e)
        {
            await Refresh();
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (!biliVM.CheckAccoutStatus())
                return;
            if (ViewLaterCollection.Count == 0)
                return;
            var dialog = new ConfirmDialog("您确认清空稍后观看记录吗？");
            dialog.PrimaryButtonClick += async (_s, _e) =>
            {
                _e.Cancel = true;
                dialog.IsPrimaryButtonEnabled = false;
                dialog.PrimaryButtonText = "清空中...";
                bool reuslt = await _account.ClearViewLaterAsync();
                if (reuslt)
                {
                    new TipPopup("清空成功").ShowMessage();
                    ViewLaterCollection.Clear();
                    HolderText.Visibility = Visibility.Visible;
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

        private void ViewLaterVideoView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as VideoDetail;
            if (item.bangumi != null)
                App.AppViewModel.PlayBangumi(item.bangumi.ep_id, null, true);
            else
            {
                var videos = ViewLaterCollection.Where(p => p.bangumi == null).ToList();
                App.AppViewModel.PlayVideoList(item.aid, null, videos);
            }
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var aid = Convert.ToInt32((sender as AppBarButton).Tag);
            bool result = await _account.DeleteViewLaterAsync(Convert.ToInt32(aid));
            if (result)
            {
                ViewLaterCollection.Remove(ViewLaterCollection.Where(p => p.aid == aid).FirstOrDefault());
                HolderText.Visibility = ViewLaterCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
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
                await LoadViewLater(true);
            }
        }
    }
}
