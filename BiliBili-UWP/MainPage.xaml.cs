using BiliBili_UWP.Components.Widgets;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace BiliBili_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private bool _isInit = false;
        public static MainPage Current;
        public MainPage()
        {
            App.AppViewModel = new AppViewModel();
            App.BiliViewModel = new BiliViewModel();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Current = this;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(TitleContainer);
            if (e.NavigationMode == NavigationMode.Back || _isInit)
            {
                return;
            }
            App.AppViewModel.CheckAppUpdate();
            var popup = new WaitingPopup("正在初始化");
            popup.ShowPopup();
            await App.BiliViewModel.GetRegionsAsync();
            popup.HidePopup();
            _isInit = true;
        }

        private void SidePanel_PaneButtonClick(object sender, bool e)
        {
            AppSplitView.IsPaneOpen = e;
        }

        private void SidePanel_SideMenuItemClick(object sender, Models.UI.SideMenuItem e)
        {

        }

        private void SidePanel_RegionSelected(object sender, BiliBili_Lib.Models.BiliBili.Region e)
        {

        }
    }
}
