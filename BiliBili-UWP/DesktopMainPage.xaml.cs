using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Core;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
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
    public sealed partial class DesktopMainPage : Page, IPlayerHost
    {
        private bool _isInit = false;
        string tempArgument = string.Empty;
        public static DesktopMainPage Current;
        public DesktopMainPage()
        {
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
            bool isSideOpen = AppTool.GetBoolSetting(BiliBili_Lib.Enums.Settings.IsLastSidePanelOpen);
            AppSplitView.IsPaneOpen = isSideOpen;
            App.AppViewModel.CheckAppUpdate();
            var popup = new WaitingPopup("正在初始化");
            popup.ShowPopup();
            bool isCanRequest = await App.BiliViewModel._client.ValidateRequestAsync();
            if (isCanRequest)
            {
                try
                {
                    await App.BiliViewModel.GetRegionsAsync();
                    App.AppViewModel.FontInit();
                    Window.Current.Dispatcher.AcceleratorKeyActivated += App.AppViewModel.AccelertorKeyActivedHandle;
                    if (e.Parameter != null && e.Parameter is string argument && !string.IsNullOrEmpty(argument) && argument.Contains("action"))
                    {
                        App.AppViewModel.AppInitByActivated(argument);
                    }
                    else
                    {
                        PagePanel.NavigateToPage(Models.Enums.AppMenuItemType.Home);
                    }
                }
                catch (Exception)
                {
                    var rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(Pages.Main.NetworkErrorPage));
                }
            }
            else
            {
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Pages.Main.NetworkErrorPage));
            }
            popup.HidePopup();
            _isInit = true;
        }



        private void SidePanel_SideMenuItemClick(object sender, Models.UI.AppMenuItem e)
        {
            PagePanel.NavigateToPage(e.Type);
        }

        private void SidePanel_RegionSelected(object sender, BiliBili_Lib.Models.BiliBili.Region e)
        {
            SidePanel.SetSelectedItem(Models.Enums.AppMenuItemType.Line);
            App.AppViewModel.SelectedSideMenuItem = null;
            PagePanel.NavigateToRegion(e);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = e.NewSize.Width;
            if (width < 1000)
            {
                if (AppSplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay)
                    AppSplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
            else
            {
                if (AppSplitView.DisplayMode != SplitViewDisplayMode.CompactInline)
                    AppSplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
            }
        }

        public void InsertPlayer()
        {
            FullWindowContainer.Visibility = Visibility.Visible;
            if (FullWindowContainer.Children.Count == 0)
                FullWindowContainer.Children.Add(App.AppViewModel.CurrentVideoPlayer);
            App.AppViewModel.CurrentVideoPlayer.Focus(FocusState.Programmatic);
            App.AppViewModel.CurrentVideoPlayer.ResetDanmakuStatus();
        }
        public void RemovePlayer()
        {
            FullWindowContainer.Visibility = Visibility.Collapsed;
            FullWindowContainer.Children.Clear();
        }
    }
}
