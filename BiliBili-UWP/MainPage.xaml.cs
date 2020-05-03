using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Core;
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
    public sealed partial class MainPage : Page
    {
        private bool _isInit = false;
        string tempArgument = string.Empty;
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
            bool isSideOpen=AppTool.GetBoolSetting(BiliBili_Lib.Enums.Settings.IsLastSidePanelOpen);
            AppSplitView.IsPaneOpen = isSideOpen;
            App.AppViewModel.CheckAppUpdate();
            var popup = new WaitingPopup("正在初始化");
            popup.ShowPopup();
            try
            {
                await App.BiliViewModel.GetRegionsAsync();
                App.AppViewModel.FontInit();
                Window.Current.Dispatcher.AcceleratorKeyActivated += AccelertorKeyActivedHandle;
                PagePanel.NavigateToPage(Models.Enums.SideMenuItemType.Home);
            }
            catch (Exception)
            {
                var rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(Pages.Main.NetworkErrorPage));
            }
            finally
            {
                popup.HidePopup();
            }
            if (e.Parameter != null && e.Parameter is string argument)
            {
                tempArgument = argument;
            }
            _isInit = true;
        }

        private void AccelertorKeyActivedHandle(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                var esc = Window.Current.CoreWindow.GetKeyState(VirtualKey.Escape);
                var space = Window.Current.CoreWindow.GetKeyState(VirtualKey.Space);
                var f11= Window.Current.CoreWindow.GetKeyState(VirtualKey.F11);
                var left = Window.Current.CoreWindow.GetKeyState(VirtualKey.Left);
                var right = Window.Current.CoreWindow.GetKeyState(VirtualKey.Right);
                var player = App.AppViewModel.CurrentVideoPlayer;
                
                if (esc.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null)
                    {
                        if (player.MTC.IsFullWindow)
                        {
                            args.Handled = true;
                            player.MTC.IsFullWindow = false;
                        }
                        else if (player.MTC.IsCinema)
                        {
                            args.Handled = true;
                            player.MTC.IsCinema = false;
                        }
                        player.Focus(FocusState.Programmatic);
                    }

                }
                else if (space.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && (player.IsFocus || player.MTC.IsFullWindow || player.MTC.IsCinema))
                    {
                        args.Handled = true;
                        player.MTC.IsPlaying = !player.MTC.IsPlaying;
                        player.Focus(FocusState.Programmatic);
                    }
                }
                else if (f11.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if(player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.MTC.IsFullWindow = !player.MTC.IsFullWindow;
                    }
                }
                else if (left.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.SkipRewind();
                    }
                }
                else if (right.HasFlag(CoreVirtualKeyStates.Down))
                {
                    if (player != null && player.IsFocus)
                    {
                        args.Handled = true;
                        player.SkipForward();
                    }
                }
            }
        }

        private void SidePanel_SideMenuItemClick(object sender, Models.UI.SideMenuItem e)
        {
            PagePanel.NavigateToPage(e.Type);
            if (!string.IsNullOrEmpty(tempArgument))
            {
                App.AppViewModel.AppInitByActivated(tempArgument);
                tempArgument = string.Empty;
            }
        }

        private void SidePanel_RegionSelected(object sender, BiliBili_Lib.Models.BiliBili.Region e)
        {
            SidePanel.SetSelectedItem(Models.Enums.SideMenuItemType.Line);
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
