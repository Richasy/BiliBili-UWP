using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HistoryPage : Page, IRefreshPage
    {
        private bool _isInit = false;
        public BiliViewModel biliVM = App.BiliViewModel;
        public AccountService _account = App.BiliViewModel._client.Account;
        private ObservableCollection<VideoDetail> HistoryCollection = new ObservableCollection<VideoDetail>();
        private bool _isHistoryRequesting = false;
        private int _page = 1;
        private double _scrollOffset = 0;
        public HistoryPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            biliVM.IsLoginChanged += IsLoginChanged;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            HistoryVideoView.EnableAnimation = App.AppViewModel.IsEnableAnimation;
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = ScrollViewerBottomHandle;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = ScrollViewerChanged;
            HistoryVideoView.DesiredWidth = 215 + ((App.AppViewModel.BasicFontSize - 14) * 5);
            if (_isInit || e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            await Refresh();
            _isInit = true;
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            PageContainer.Visibility = Visibility.Collapsed;
            App.AppViewModel.CurrentPagePanel.ScrollToBottom = null;
            App.AppViewModel.CurrentPagePanel.ScrollChanged = null;
            base.OnNavigatingFrom(e);
        }
        public async Task Refresh()
        {
            if (PageContainer.Visibility == Visibility.Collapsed)
                PageContainer.Visibility = Visibility.Visible;
            HistoryCollection.Clear();
            _page = 1;
            _scrollOffset = 0;
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
            var ele = HistoryVideoView.ContainerFromItem(item);
            if (item.bangumi != null)
                App.AppViewModel.PlayBangumi(item.bangumi.ep_id, ele, true);
            else
                App.AppViewModel.PlayVideo(item.aid, ele, "main.my-history.0.0");
        }
        private void ScrollViewerChanged()
        {
            double offset = App.AppViewModel.CurrentPagePanel.PageScrollViewer.VerticalOffset;
            _scrollOffset = offset;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PageContainer.Visibility = Visibility.Visible;
            await Task.Delay(100);
            if (_scrollOffset > 0)
            {
                App.AppViewModel.CurrentPagePanel.PageScrollViewer.ChangeView(0, _scrollOffset, 1);
            }
        }

        private async void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var aid = (sender as AppBarButton).Tag.ToString();
            bool result = await _account.DeleteHistoryAsync(Convert.ToInt32(aid));
            if (result)
            {
                HistoryCollection.Remove(HistoryCollection.Where(p=>p.aid==Convert.ToInt32(aid)).FirstOrDefault());
                HolderText.Visibility = HistoryCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                new TipPopup("移出失败，请稍后重试").ShowError();
            }
        }

        private void HistoryVideoView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            args.Handled = true;
            DefaultVideoCard card = (DefaultVideoCard)args.ItemContainer.ContentTemplateRoot;
            card.RenderContainer(args);
        }
    }
}
