using BiliBili_Lib.Models.BiliBili;
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
    public sealed partial class SidePanel : UserControl
    {
        private ObservableCollection<SideMenuItem> MenuItemCollection = new ObservableCollection<SideMenuItem>();
        private ObservableCollection<Region> RegionCollection = App.BiliViewModel.RegionCollection;
        private bool _isPaneOpen=true;
        public SidePanel()
        {
            this.InitializeComponent();
            var list = SideMenuItem.GetSideMenuItems(true);
            list.ForEach(p => MenuItemCollection.Add(p));
        }

        public event EventHandler<bool> PaneButtonClick;
        public event EventHandler<Region> RegionSelected;
        public event EventHandler<SideMenuItem> SideMenuItemClick;


        private void SideMenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SideMenuItem;
            if(item.Type!=SideMenuItemType.Line && item != App.AppViewModel.SelectedSideMenuItem)
            {
                SetSelectedItem(item.Type);
                App.AppViewModel.SelectedSideMenuItem = item;
                SideMenuItemClick?.Invoke(this, item);
            }
        }

        private void PaneButton_Click(object sender, RoutedEventArgs e)
        {
            _isPaneOpen = !_isPaneOpen;
            if (_isPaneOpen)
                VisualStateManager.GoToState(this, "WideState", true);
            else
                VisualStateManager.GoToState(this, "NarrowState", true);
            PaneButtonClick?.Invoke(this, _isPaneOpen);
        }

        public void SetSelectedItem(SideMenuItemType type)
        {
            foreach (var item in MenuItemCollection)
            {
                item.IsSelected = item.Type == type;
            }
            var index = MenuItemCollection.IndexOf(MenuItemCollection.Where(p => p.IsSelected).FirstOrDefault());
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
    }
}
