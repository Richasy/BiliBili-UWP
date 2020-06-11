using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Models.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace BiliBili_UWP.Pages_Tablet.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelPage : Page
    {
        public BiliViewModel channelVM = App.BiliViewModel;
        private bool _isInit = false;
        public ChannelPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            channelVM.MyChannelChanged += ChannelChanged;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            LoadingBar.Visibility = Visibility.Visible;
            SubscribeContainer.Visibility = Visibility.Collapsed;
            ScanContainer.Visibility = Visibility.Collapsed;
            await channelVM.GetChannelSquareAsync();
            LoadingBar.Visibility = Visibility.Collapsed;
            SubscribeContainer.Visibility = Visibility.Visible;
            ScanContainer.Visibility = Visibility.Visible;
            base.OnNavigatedTo(e);
        }
        private void ChannelChanged(object sender, bool e)
        {
            SubscribeHolderText.Visibility = channelVM.MySubscribeChannelCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            ScanHolderText.Visibility = channelVM.MyScanedChannelCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
        private void SearchChannelButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.ChannelListPage));
        }

        private void ScanGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ChannelView;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.ChannelDetailPage), item.id);
        }

        private void ChannelListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ChannelBase;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.ChannelDetailPage), item.id);
        }
    }
}
