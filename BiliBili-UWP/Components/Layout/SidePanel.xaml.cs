using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Layout
{
    public sealed partial class SidePanel : UserControl
    {
        private ObservableCollection<AppMenuItem> MenuItemCollection = new ObservableCollection<AppMenuItem>();
        private ObservableCollection<RegionContainer> RegionCollection = App.BiliViewModel.RegionCollection;
        public SidePanel()
        {
            this.InitializeComponent();
            App.AppViewModel.CurrentSidePanel = this;
            var list = AppMenuItem.GetSideMenuItems(App.BiliViewModel.IsLogin);
            list.ForEach(p => MenuItemCollection.Add(p));
            App.AppViewModel.SelectedSideMenuItem = list.Where(p=>p.IsSelected).FirstOrDefault();
            App.BiliViewModel.IsLoginChanged -= IsLoginChanged;
            App.BiliViewModel.IsLoginChanged += IsLoginChanged;
        }

        
        public event EventHandler<Region> RegionSelected;
        public event EventHandler<AppMenuItem> SideMenuItemClick;
        public bool IsWide
        {
            get { return (bool)GetValue(IsWideProperty); }
            set { SetValue(IsWideProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWideProperty =
            DependencyProperty.Register("IsWide", typeof(bool), typeof(SidePanel), new PropertyMetadata(true,new PropertyChangedCallback(IsWide_Changed)));

        private static void IsWide_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is bool iswide)
            {
                var instance = d as SidePanel;
                if (iswide)
                    VisualStateManager.GoToState(instance, "WideState", true);
                else
                    VisualStateManager.GoToState(instance, "NarrowState", true);
            }
        }

        private void IsLoginChanged(object sender, bool e)
        {
            SideMenuListView.SelectedIndex = -1;
            var selectItem = MenuItemCollection.Where(p => p.IsSelected).FirstOrDefault();
            var type = selectItem == null ? AppMenuItemType.Line : selectItem.Type;
            var list = AppMenuItem.GetSideMenuItems(e, type);
            MenuItemCollection.Clear();
            list.ForEach(p => MenuItemCollection.Add(p));
            var select = MenuItemCollection.Where(p => p.IsSelected).FirstOrDefault();
            SideMenuListView.SelectedItem = App.AppViewModel.SelectedSideMenuItem = select;
        }

        private void SideMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AppMenuItem;
            if (item.Type != AppMenuItemType.Line && item != App.AppViewModel.SelectedSideMenuItem)
            {
                SetSelectedItem(item.Type);
                App.AppViewModel.SelectedSideMenuItem = item;
                SideMenuItemClick?.Invoke(this, item);
            }
        }

        private void PaneButton_Click(object sender, RoutedEventArgs e)
        {
            IsWide = !IsWide;
            AppTool.WriteLocalSetting(BiliBili_Lib.Enums.Settings.IsLastSidePanelOpen, IsWide.ToString());
        }

        public void SetSelectedItem(AppMenuItemType type)
        {
            if (type == AppMenuItemType.Line)
            {
                SideMenuListView.SelectedIndex = -1;
                SideMenuListView.SelectedItem = null;
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
            SideMenuListView.SelectedIndex = index;
        }

        private void RegionGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var region = e.ClickedItem as Region;
            RegionFlyout.Hide();
            RegionSelected?.Invoke(this, region);
        }

        private void SideMenuListView_Loaded(object sender, RoutedEventArgs e)
        {
            var index = MenuItemCollection.IndexOf(MenuItemCollection.Where(p => p.IsSelected).FirstOrDefault());
            SideMenuListView.SelectedIndex = index;
        }

        public void SetMenuItemUnread(AppMenuItemType type,int value)
        {
            var item = MenuItemCollection.Where(p => p.Type == type).FirstOrDefault();
            if (item != null)
            {
                item.Unread = value;
            }
        }
    }
}
