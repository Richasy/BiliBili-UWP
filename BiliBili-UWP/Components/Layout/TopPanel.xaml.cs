using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace BiliBili_UWP.Components.Layout
{
    public sealed partial class TopPanel : UserControl
    {
        private ObservableCollection<AppMenuItem> MenuItemCollection = new ObservableCollection<AppMenuItem>();
        public TopPanel()
        {
            this.InitializeComponent();
            App.AppViewModel.CurrentTopPanel = this;
            var menus = AppMenuItem.GetTopMenuItems();
            MenuItemCollection.Clear();
            menus.ForEach(p => MenuItemCollection.Add(p));
            TopMenuListView.SelectedItem = App.AppViewModel.SelectedSideMenuItem = menus.Where(p => p.IsSelected).FirstOrDefault();
        }

        public event EventHandler<AppMenuItem> TopMenuItemClick;
        public event EventHandler BackButtonClick;


        public Visibility BackButtonVisibility
        {
            get { return (Visibility)GetValue(BackButtonVisibilityProperty); }
            set { SetValue(BackButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register("BackButtonVisibility", typeof(Visibility), typeof(TopPanel), new PropertyMetadata(Visibility.Collapsed));



        private void TopMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AppMenuItem;
            if (item.Type != AppMenuItemType.Line && item != App.AppViewModel.SelectedSideMenuItem)
            {
                SetSelectedItem(item.Type);
                App.AppViewModel.SelectedSideMenuItem = item;
                TopMenuItemClick?.Invoke(this, item);
            }
        }

        public void SetUnreadCount(int count)
        {
            SlimAccountPanel.SetMessageUnread(count);
        }
        public void SetSelectedItem(AppMenuItemType type)
        {
            if (type == AppMenuItemType.Line)
            {
                TopMenuListView.SelectedIndex = -1;
                TopMenuListView.SelectedItem = null;
                foreach (var item in MenuItemCollection)
                {
                    item.IsSelected = false;
                }
                return;
            }
            foreach (var item in MenuItemCollection)
            {
                item.IsSelected = item.Type == type;
            }
            var selectItem = MenuItemCollection.Where(p => p.IsSelected).FirstOrDefault();
            App.AppViewModel.SelectedSideMenuItem = selectItem;
            var index = MenuItemCollection.IndexOf(selectItem);
            TopMenuListView.SelectedIndex = index;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 1100)
                VisualStateManager.GoToState(this, "Narrow", true);
            else
                VisualStateManager.GoToState(this, "Wide", true);
        }
    }
}
