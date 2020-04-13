using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public PagePanel()
        {
            this.InitializeComponent();
            App.AppViewModel.CurrentPagePanel = this;
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
        public bool IsStretch
        {
            get { return (bool)GetValue(IsStretchProperty); }
            set { SetValue(IsStretchProperty, value); }
        }
        public string SubPageTitle
        {
            get { return (string)GetValue(SubPageTitleProperty); }
            set { SetValue(SubPageTitleProperty, value); }
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
        public static readonly DependencyProperty IsStretchProperty =
            DependencyProperty.Register("IsStretch", typeof(bool), typeof(PagePanel), new PropertyMetadata(false, new PropertyChangedCallback(IsStretch_Changed)));
        public static readonly DependencyProperty SubPageTitleProperty =
            DependencyProperty.Register("SubPageTitle", typeof(string), typeof(PagePanel), new PropertyMetadata(""));
        public static readonly DependencyProperty IsSubPageOpenProperty =
            DependencyProperty.Register("IsSubPageOpen", typeof(bool), typeof(PagePanel), new PropertyMetadata(false));



        private static void IsStretch_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is bool isStretch)
            {
                var instance = d as PagePanel;
                var frame = instance.PageFrame;
                if (isStretch)
                {
                    instance.NoScrollContainer.Visibility = Visibility.Visible;
                    instance.PageScrollViewer.Visibility = Visibility.Collapsed;
                    instance.DisplayContainer.Children.Remove(frame);
                    instance.NoScrollContainer.Children.Add(frame);
                }
                else
                {
                    instance.NoScrollContainer.Visibility = Visibility.Collapsed;
                    instance.PageScrollViewer.Visibility = Visibility.Visible;
                    instance.NoScrollContainer.Children.Remove(frame);
                    instance.DisplayContainer.Children.Insert(0, frame);
                }
            }
        }

        private static void IsDefaultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is bool isDefault)
            {
                var instance = d as PagePanel;
                instance.HolderContainer.Visibility = isDefault ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public void NavigateToPage(SideMenuItemType type, object parameter = null)
        {
            PageSplitView.IsPaneOpen = false;
            var page = GetPageTypeFromMenuType(type);
            if (page != null)
            {
                App.AppViewModel.CurrentPageType = page;
                PageFrame.Navigate(page, parameter, new DrillInNavigationTransitionInfo());
                IsDefault = false;
            }
            else
                IsDefault = true;
        }
        public void NavigateToRegion(Region region)
        {
            PageSplitView.IsPaneOpen = false;
            IsDefault = false;
            // TODO
        }
        public void ClearCache()
        {
            var cacheSize = PageFrame.CacheSize;
            PageFrame.CacheSize = 0;
            PageFrame.CacheSize = cacheSize;
        }
        private Type GetPageTypeFromMenuType(SideMenuItemType type)
        {
            Type page = null;
            switch (type)
            {
                case SideMenuItemType.Home:
                    page = typeof(Pages.Main.HomePage);
                    break;
                case SideMenuItemType.Live:
                    break;
                case SideMenuItemType.Anime:
                    break;
                case SideMenuItemType.Dynamic:
                    break;
                case SideMenuItemType.MyFollow:
                    break;
                case SideMenuItemType.MyFavorite:
                    break;
                case SideMenuItemType.MyDownload:
                    break;
                case SideMenuItemType.ViewLater:
                    break;
                case SideMenuItemType.Settings:
                    break;
                case SideMenuItemType.Help:
                    break;
                default:
                    break;
            }
            return page;
        }
        private SideMenuItemType GetMenuTypeFromPageType(Type type)
        {
            SideMenuItemType result = SideMenuItemType.Line;
            if (type.Equals(typeof(Pages.Main.HomePage)))
                result = SideMenuItemType.Home;
            return result;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageFrame.CanGoBack)
            {
                PageSplitView.IsPaneOpen = false;
                var previousType = PageFrame.BackStack.Last().SourcePageType;
                var menu = GetMenuTypeFromPageType(previousType);
                if (menu != App.AppViewModel.SelectedSideMenuItem.Type)
                {
                    App.AppViewModel.CurrentSidePanel.SetSelectedItem(menu);
                }
                PageFrame.GoBack();
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

        private void ClosePaneButton_Click(object sender, RoutedEventArgs e)
        {
            PageSplitView.IsPaneOpen = false;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var width = e.NewSize.Width;
            PageSplitView.DisplayMode = width < 1000 ? SplitViewDisplayMode.CompactOverlay : SplitViewDisplayMode.CompactInline;
            PageSplitView.OpenPaneLength = width < 600 ? width : 400;
        }

        private void PageSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args)
        {
            MaskGrid.Visibility = Visibility.Collapsed;
            SubPageFrame.Visibility = Visibility.Collapsed;
        }

        private void PageSplitView_PaneOpening(SplitView sender, object args)
        {
            if (PageSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay)
                MaskGrid.Visibility = Visibility.Visible;
            SubPageFrame.Visibility = Visibility.Visible;
        }

        public void NavigateToSubPage(Type page, object parameter = null)
        {
            PageSplitView.IsPaneOpen = true;
            SubPageFrame.Navigate(page, parameter, new DrillInNavigationTransitionInfo());
        }

        private void PageFrame_Navigated(object sender, NavigationEventArgs e)
        {
            BackButton.Visibility = PageFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            App.AppViewModel.CurrentPageType = e.SourcePageType;
        }

        private async void SubRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var sub = SubPageFrame.Content as IRefreshPage;
            if (sub != null)
            {
                SubRefreshButton.IsEnabled = false;
                LoadingBar.Visibility = Visibility.Visible;
                await sub.Refresh();
                LoadingBar.Visibility = Visibility.Collapsed;
                SubRefreshButton.IsEnabled = true;
            }
        }

        private void SubBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (SubPageFrame.CanGoBack)
            {
                SubPageFrame.GoBack();
            }
        }

        private void SubPageFrame_Navigated(object sender, NavigationEventArgs e)
        {
            SubBackButton.Visibility = SubPageFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
