using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Models.Core;
using BiliBili_UWP.Models.UI;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class ScanChannelPanel : UserControl
    {
        public BiliViewModel channelVM = App.BiliViewModel;
        public ScanChannelPanel()
        {
            this.InitializeComponent();
            channelVM.MyChannelChanged += ChannelChanged;
        }

        private void ChannelChanged(object sender, bool e)
        {
            ScanPanel.HolderVisibility = channelVM.MyScanedChannelCollection.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ChannelListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ChannelView;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.ChannelDetailPage), item.id);
        }
    }
}
