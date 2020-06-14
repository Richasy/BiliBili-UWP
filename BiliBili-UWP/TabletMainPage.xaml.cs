using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Controls;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using BiliBili_UWP.Pages_Tablet.Main;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabletMainPage : Page, IPlayerHost
    {
        public static TabletMainPage Current;
        private bool _isInit = false;
        string tempArgument = string.Empty;
        private List<Tuple<Type, object>> MainFrameHistoryList = new List<Tuple<Type, object>>();
        public TabletMainPage()
        {
            this.InitializeComponent();
            Current = this;
            if (App.AppViewModel.CurrentVideoDetailBlock == null)
                App.AppViewModel.CurrentVideoDetailBlock = new TabletVideoDetailBlock();
            if (App.AppViewModel.CurrentBangumiDetailBlock == null)
                App.AppViewModel.CurrentBangumiDetailBlock = new TabletBangumiDetailBlock();
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackrequested;
        }

        private void OnBackrequested(object sender, BackRequestedEventArgs e)
        {
            bool result = AutoJudgeBack();
            if (result)
                e.Handled = true;
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
                        NavigateToPage(AppMenuItemType.Recommend);
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
            base.OnNavigatedTo(e);
        }
        public bool IsSubPageOpen
        {
            get { return (bool)GetValue(IsSubPageOpenProperty); }
            set { SetValue(IsSubPageOpenProperty, value); }
        }
        public bool IsDefault
        {
            get { return (bool)GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
        }
        public static readonly DependencyProperty IsSubPageOpenProperty =
           DependencyProperty.Register("IsSubPageOpen", typeof(bool), typeof(TabletMainPage), new PropertyMetadata(false));
        public static readonly DependencyProperty IsDefaultProperty =
            DependencyProperty.Register("IsDefault", typeof(bool), typeof(TabletMainPage), new PropertyMetadata(false, new PropertyChangedCallback(IsDefaultChanged)));

        private static void IsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is bool isDefault)
            {
                var instance = d as TabletMainPage;
                instance.HolderContainer.Visibility = isDefault ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void BottomPanel_SettingButtonClick(object sender, EventArgs e)
        {
            TopPanel.SetSelectedItem(AppMenuItemType.Line);
            App.AppViewModel.SelectedSideMenuItem = null;
            NavigateToPage(AppMenuItemType.Settings);
        }

        private void BottomPanel_HelpButtonClick(object sender, EventArgs e)
        {
            TopPanel.SetSelectedItem(AppMenuItemType.Line);
            App.AppViewModel.SelectedSideMenuItem = null;
            NavigateToPage(AppMenuItemType.Help);
        }

        public void SetBackgroundImage(string imageUrl)
        {
            MaskGrid.Background = UIHelper.GetThemeBrush(ColorType.MaskAcrylicBackground);
            BackgroundImage.Visibility = Visibility.Visible;
            BackgroundImage.Source = imageUrl;
        }
        public void HideBackgroundImage()
        {
            MaskGrid.Background = UIHelper.GetThemeBrush(ColorType.SideBackground);
            BackgroundImage.Visibility = Visibility.Collapsed;
        }

        private void TopPanel_TopMenuItemClick(object sender, Models.UI.AppMenuItem e)
        {
            NavigateToPage(e.Type);
        }

        private Type GetPageFromType(AppMenuItemType type)
        {
            Type page = null;
            switch (type)
            {
                case AppMenuItemType.Rank:
                    page = typeof(RankPage);
                    break;
                case AppMenuItemType.Anime:
                    page = typeof(AnimePage);
                    break;
                case AppMenuItemType.Region:
                    page = typeof(RegionPage);
                    break;
                case AppMenuItemType.Recommend:
                    page = typeof(RecommendPage);
                    break;
                case AppMenuItemType.Channel:
                    page = typeof(ChannelPage);
                    break;
                case AppMenuItemType.VideoPlayer:
                    page = typeof(PlayerPage);
                    break;
                case AppMenuItemType.Settings:
                    page = typeof(Pages_Share.Main.SettingPage);
                    break;
                case AppMenuItemType.Help:
                    page = typeof(Pages_Share.Main.HelpPage);
                    break;
                default:
                    break;
            }
            return page;
        }

        private AppMenuItemType GetTypeFromPage(Type type)
        {
            AppMenuItemType result = AppMenuItemType.Line;
            if (type.Equals(typeof(RecommendPage)))
                result = AppMenuItemType.Recommend;
            else if (type.Equals(typeof(RankPage)))
                result = AppMenuItemType.Rank;
            else if (type.Equals(typeof(AnimePage)))
                result = AppMenuItemType.Anime;
            else if (type.Equals(typeof(RegionPage)))
                result = AppMenuItemType.Region;
            else if (type.Equals(typeof(ChannelPage)))
                result = AppMenuItemType.Channel;
            else if (type.Equals(typeof(PlayerPage)))
                result = AppMenuItemType.VideoPlayer;
            else if (type.Equals(typeof(Pages_Share.Main.SettingPage)))
                result = AppMenuItemType.Settings;
            else if (type.Equals(typeof(Pages_Share.Main.HelpPage)))
                result = AppMenuItemType.Help;
            return result;
        }

        public void RemoveMainHistory(AppMenuItemType type)
        {
            var page = GetPageFromType(type);
            MainFrameHistoryList.RemoveAll(p => p.Item1 == page);
        }

        public void NavigateToPage(AppMenuItemType pageType, object parameter = null, bool isBack = false)
        {
            if (AppSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                IsSubPageOpen = false;
            var last = MainFrameHistoryList.LastOrDefault();
            var page = GetPageFromType(pageType);
            bool isRepeat = false;
            if (last != null && last.Item1 == page && last.Item2 == parameter)
                isRepeat = true;
            if (page != null)
            {
                App.AppViewModel.CurrentPageType = page;
                NavigationTransitionInfo transitionInfo = null;
                if (isBack)
                    transitionInfo = new EntranceNavigationTransitionInfo();
                else
                    transitionInfo = new DrillInNavigationTransitionInfo();
                MainFrame.Navigate(page, parameter, transitionInfo);
                if (!isBack)
                {
                    if (page.Equals(typeof(PlayerPage)))
                        MainFrameHistoryList.RemoveAll(p => p.Item1 == page);
                    if (!isRepeat)
                    {
                        MainFrameHistoryList.Add(new Tuple<Type, object>(page, parameter));
                    }
                    if (MainFrameHistoryList.Count > 1)
                    {
                        TopPanel.BackButtonVisibility = Visibility.Visible;
                    }
                }
                IsDefault = false;
            }
            else
                IsDefault = true;
            MainFrame.Focus(FocusState.Programmatic);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = e.NewSize.Width;
            double OpenWidth = 400 + ((App.AppViewModel.BasicFontSize - 14) * App.AppViewModel.BasicFontSize);
            double breakpoint = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PagePanelDisplayBreakpoint, "1500"));
            AppSplitView.DisplayMode = width < breakpoint ? SplitViewDisplayMode.CompactOverlay : SplitViewDisplayMode.CompactInline;
            AppSplitView.OpenPaneLength = width < OpenWidth + 200 ? width : OpenWidth;
        }

        private void PageSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            SubPageControl.FrameVisibility = Visibility.Collapsed;
            BottomPanel.OpenSideButtonVisibility = Visibility.Visible;
        }

        private void PageSplitView_PaneOpening(SplitView sender, object args)
        {
            SubPageControl.FrameVisibility = Visibility.Visible;
            BottomPanel.OpenSideButtonVisibility = Visibility.Collapsed;
        }

        private void SubPageControl_CloseButtonClick(object sender, EventArgs e)
        {
            AppSplitView.IsPaneOpen = false;
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
            if (AppSplitView.IsPaneOpen
                    && AppSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
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
        public void MainPageBack()
        {
            if (MainFrameHistoryList.Count <= 1)
                return;
            int c = MainFrameHistoryList.Count - 2;
            var last = MainFrameHistoryList[c];
            var previousType = last.Item1;
            var menu = GetTypeFromPage(previousType);
            if (App.AppViewModel.SelectedSideMenuItem == null || menu != App.AppViewModel.SelectedSideMenuItem.Type)
            {
                TopPanel.SetSelectedItem(menu);
            }
            MainFrameHistoryList.RemoveAt(MainFrameHistoryList.Count - 1);
            if (MainFrameHistoryList.Count <= 1)
            {
                TopPanel.BackButtonVisibility = Visibility.Collapsed;
            }
            NavigateToPage(menu, last.Item2, true);
        }

        private void TopPanel_BackButtonClick(object sender, EventArgs e)
        {
            MainPageBack();
        }

        public void InsertPlayer()
        {
            VideoContainer.Visibility = Visibility.Visible;
            if (VideoContainer.Children.Count == 0)
                VideoContainer.Children.Add(App.AppViewModel.CurrentVideoPlayer);
            App.AppViewModel.CurrentVideoPlayer.Focus(FocusState.Programmatic);
            App.AppViewModel.CurrentVideoPlayer.ResetDanmakuStatus();
        }
        public void RemovePlayer()
        {
            VideoContainer.Visibility = Visibility.Collapsed;
            VideoContainer.Children.Clear();
        }

        private void BottomPanel_OpenSideButtonClick(object sender, EventArgs e)
        {
            IsSubPageOpen = true;
        }
    }
}
