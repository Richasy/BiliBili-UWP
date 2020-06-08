using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Layout
{
    public sealed partial class PagePanel : UserControl, INotifyPropertyChanged
    {
        public bool IsNavigationFromCode = false;
        private List<Tuple<Type, object>> MainFrameHistoryList = new List<Tuple<Type, object>>();
        public Action ScrollToBottom = null;
        public Action ScrollChanged = null;
        public PagePanel()
        {
            this.InitializeComponent();
            App.AppViewModel.CurrentPagePanel = this;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackrequested;
        }
        public new Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); OnPropertyChanged(); }
        }
        public bool IsDefault
        {
            get { return (bool)GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
        }
        public bool IsSubPageOpen
        {
            get { return (bool)GetValue(IsSubPageOpenProperty); }
            set { SetValue(IsSubPageOpenProperty, value); }
        }
        public static new readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(PagePanel), new PropertyMetadata(new Thickness(50, 35, 20, 0)));
        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(PagePanel), new PropertyMetadata(false, new PropertyChangedCallback(IsDefaultChanged)));
        public static readonly DependencyProperty IsSubPageOpenProperty =
            DependencyProperty.Register("IsSubPageOpen", typeof(bool), typeof(PagePanel), new PropertyMetadata(false));

        public bool IsStretch
        {
            get { return (bool)GetValue(IsStretchProperty); }
            set { SetValue(IsStretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsStretchProperty =
            DependencyProperty.Register("IsStretch", typeof(bool), typeof(PagePanel), new PropertyMetadata(false,new PropertyChangedCallback(IsStretch_Changed)));

        private static void IsStretch_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        private static void IsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is bool isDefault)
            {
                var instance = d as PagePanel;
                instance.HolderContainer.Visibility = isDefault ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void OnBackrequested(object sender, BackRequestedEventArgs e)
        {
            // 判断应用当前是否有页面可以回退，没有则继续冒泡
            bool result = AutoJudgeBack();
            if (result)
                e.Handled = true;
        }
        public void NavigateToPage(AppMenuItemType type, object parameter = null, bool isBack = false)
        {
            if(PageSplitView.DisplayMode==SplitViewDisplayMode.CompactOverlay)
                IsSubPageOpen = false;
            var last = MainFrameHistoryList.LastOrDefault();
            var page = GetPageTypeFromMenuType(type);
            bool isRepeat = false;
            if (last != null && last.Item1 == page && last.Item2 == parameter)
                isRepeat = true;
            if (page != null)
            {
                App.AppViewModel.CurrentPageType = page;
                NavigationTransitionInfo transitionInfo = null;
                if (type == AppMenuItemType.VideoPlayer || !App.AppViewModel.IsEnableAnimation)
                {
                    transitionInfo = new SuppressNavigationTransitionInfo();
                }  
                else
                {
                    if (isBack)
                        transitionInfo = new EntranceNavigationTransitionInfo();
                    else
                        transitionInfo = new DrillInNavigationTransitionInfo();
                }
                PageFrame.Navigate(page, parameter, transitionInfo);
                if (!isBack)
                {
                    if (page.Equals(typeof(Pages.Main.VideoPage)) || page.Equals(typeof(Pages.Main.BangumiPage)))
                        MainFrameHistoryList.RemoveAll(p => p.Item1 == page);
                    if (!isRepeat)
                    {
                        MainFrameHistoryList.Add(new Tuple<Type, object>(page, parameter));
                    }
                    if (MainFrameHistoryList.Count > 1)
                    {
                        BackButton.Visibility = Visibility.Visible;
                    }
                }
                IsDefault = false;
            }
            else
                IsDefault = true;
            PageFrame.Focus(FocusState.Programmatic);
        }
        public void NavigateToRegion(Region region)
        {
            PageSplitView.IsPaneOpen = false;
            IsDefault = false;
            if (region.name == "番剧" || region.name == "国创")
            {
                NavigateToPage(AppMenuItemType.Anime, region.name == "番剧");
            }
            else
            {
                NavigateToPage(AppMenuItemType.Region, region);
            }
        }
        public void ClearCache()
        {
            var cacheSize = PageFrame.CacheSize;
            PageFrame.CacheSize = 0;
            PageFrame.CacheSize = cacheSize;
        }
        private Type GetPageTypeFromMenuType(AppMenuItemType type)
        {
            Type page = null;
            switch (type)
            {
                case AppMenuItemType.Home:
                    page = typeof(Pages.Main.HomePage);
                    break;
                case AppMenuItemType.Live:
                    page = typeof(Pages.Main.LivePage);
                    break;
                case AppMenuItemType.Rank:
                    page = typeof(Pages.Main.RankPage);
                    break;
                case AppMenuItemType.Anime:
                    page = typeof(Pages.Main.AnimePage);
                    break;
                case AppMenuItemType.Dynamic:
                    page = typeof(Pages.Main.DynamicPage);
                    break;
                case AppMenuItemType.MyHistory:
                    page = typeof(Pages.Main.HistoryPage);
                    break;
                case AppMenuItemType.MyFavorite:
                    page = typeof(Pages.Main.FavoritePage);
                    break;
                case AppMenuItemType.MyDownload:
                    page = typeof(Pages.Main.DownloadPage);
                    break;
                case AppMenuItemType.MyMessage:
                    page = typeof(Pages.Main.MessagePage);
                    break;
                case AppMenuItemType.ViewLater:
                    page = typeof(Pages.Main.ViewLaterPage);
                    break;
                case AppMenuItemType.Settings:
                    page = typeof(Pages_Share.Main.SettingPage);
                    break;
                case AppMenuItemType.Help:
                    page = typeof(Pages_Share.Main.HelpPage);
                    break;
                case AppMenuItemType.VideoPlayer:
                    page = typeof(Pages.Main.VideoPage);
                    break;
                case AppMenuItemType.BangumiPlayer:
                    page = typeof(Pages.Main.BangumiPage);
                    break;
                case AppMenuItemType.MiniPlayer:
                    page = typeof(Pages.Main.MiniPlayerPage);
                    break;
                case AppMenuItemType.Region:
                    page = typeof(Pages.Main.RegionPage);
                    break;
                default:
                    break;
            }
            return page;
        }
        private AppMenuItemType GetMenuTypeFromPageType(Type type)
        {
            AppMenuItemType result = AppMenuItemType.Line;
            if (type.Equals(typeof(Pages.Main.HomePage)))
                result = AppMenuItemType.Home;
            else if (type.Equals(typeof(Pages.Main.AnimePage)))
                result = AppMenuItemType.Anime;
            else if (type.Equals(typeof(Pages.Main.DynamicPage)))
                result = AppMenuItemType.Dynamic;
            else if (type.Equals(typeof(Pages.Main.RegionPage)))
                result = AppMenuItemType.Region;
            else if (type.Equals(typeof(Pages.Main.VideoPage)))
                result = AppMenuItemType.VideoPlayer;
            else if (type.Equals(typeof(Pages.Main.BangumiPage)))
                result = AppMenuItemType.BangumiPlayer;
            else if (type.Equals(typeof(Pages.Main.HistoryPage)))
                result = AppMenuItemType.MyHistory;
            else if (type.Equals(typeof(Pages.Main.FavoritePage)))
                result = AppMenuItemType.MyFavorite;
            else if (type.Equals(typeof(Pages.Main.ViewLaterPage)))
                result = AppMenuItemType.ViewLater;
            else if (type.Equals(typeof(Pages.Main.LivePage)))
                result = AppMenuItemType.Live;
            else if (type.Equals(typeof(Pages.Main.RankPage)))
                result = AppMenuItemType.Rank;
            else if (type.Equals(typeof(Pages_Share.Main.SettingPage)))
                result = AppMenuItemType.Settings;
            else if (type.Equals(typeof(Pages.Main.DownloadPage)))
                result = AppMenuItemType.MyDownload;
            else if (type.Equals(typeof(Pages.Main.MessagePage)))
                result = AppMenuItemType.MyMessage;
            else if (type.Equals(typeof(Pages_Share.Main.HelpPage)))
                result = AppMenuItemType.Help;
            return result;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainPageBack();
        }

        public void MainPageBack()
        {
            if (MainFrameHistoryList.Count <= 1)
                return;
            int c = MainFrameHistoryList.Count - 2;
            var last = MainFrameHistoryList[c];
            var previousType = last.Item1;
            var menu = GetMenuTypeFromPageType(previousType);
            if (App.AppViewModel.SelectedSideMenuItem == null || menu != App.AppViewModel.SelectedSideMenuItem.Type)
            {
                App.AppViewModel.CurrentSidePanel.SetSelectedItem(menu);
            }
            MainFrameHistoryList.RemoveAt(MainFrameHistoryList.Count - 1);
            if (MainFrameHistoryList.Count <= 1)
            {
                BackButton.Visibility = Visibility.Collapsed;
            }
            try
            {
                NavigateToPage(menu, last.Item2, true);
            }
            catch (Exception)
            {

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDefault)
            {
                var main = PageFrame.Content as IRefreshPage;
                if (main != null)
                {
                    RefreshButton.IsEnabled = false;
                    LoadingBar.Visibility = Visibility.Visible;
                    await main.Refresh();
                    LoadingBar.Visibility = Visibility.Collapsed;
                    RefreshButton.IsEnabled = true;
                }
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = e.NewSize.Width;
            double OpenWidth = 400 + ((App.AppViewModel.BasicFontSize - 14) * App.AppViewModel.BasicFontSize);
            double breakpoint = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PagePanelDisplayBreakpoint, "1500"));
            PageSplitView.DisplayMode = width < breakpoint ? SplitViewDisplayMode.CompactOverlay : SplitViewDisplayMode.CompactInline;
            PageSplitView.OpenPaneLength = width < OpenWidth+200 ? width : OpenWidth;
        }

        private void PageSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            MaskGrid.Visibility = Visibility.Collapsed;
            SubPageControl.FrameVisibility = Visibility.Collapsed;
            OpenPaneButton.Visibility = Visibility.Visible;
        }

        private void PageSplitView_PaneOpening(SplitView sender, object args)
        {
            if (PageSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                MaskGrid.Visibility = Visibility.Visible;
            SubPageControl.FrameVisibility = Visibility.Visible;
            OpenPaneButton.Visibility = Visibility.Collapsed;
        }

        private void PageFrame_Navigated(object sender, NavigationEventArgs e)
        {
            App.AppViewModel.CurrentPageType = e.SourcePageType;
        }

        private void OpenPaneButton_Click(object sender, RoutedEventArgs e)
        {
            IsSubPageOpen = true;
        }

        private void PageScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            ScrollChanged?.Invoke();
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                ScrollToBottom?.Invoke();
            }
        }

        private void PageFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            new TipPopup(e.Exception.Message).ShowError();
            e.Handled = true;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == Windows.UI.Input.PointerUpdateKind.XButton1Released)
            {
                e.Handled = true;
                AutoJudgeBack();
            }
            base.OnPointerReleased(e);
        }

        /// <summary>
        /// 判断当前是主页回退还是副页回退
        /// </summary>
        private bool AutoJudgeBack()
        {
            if (App.AppViewModel.CurrentVideoPlayer != null)
            {
                if (App.AppViewModel.CurrentVideoPlayer.ExitCurrentStatus())
                    return true;
            }
            bool result = false;
            if (PageSplitView.IsPaneOpen
                    && PageSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
            {
                result = true;
                SubPageControl.SubPageBack();
            }
            else if (MainFrameHistoryList.Count > 1)
            {
                result = true;
                MainPageBack();
            }
            return result;
        }

        public void RemoveMainHistory(AppMenuItemType type)
        {
            var page = GetPageTypeFromMenuType(type);
            MainFrameHistoryList.RemoveAll(p => p.Item1 == page);
        }

        public void CheckSubReplyPage()
        {
            if(SubPageControl.CheckSubReplyPage())
            {
                IsSubPageOpen = false;
                return;
            }
            if (PageSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay && IsSubPageOpen)
                IsSubPageOpen = false;
        }

        private void SubPageControl_CloseButtonClick(object sender, EventArgs e)
        {
            PageSplitView.IsPaneOpen = false;
        }
    }
}
