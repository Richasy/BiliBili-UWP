using BiliBili_Lib.Tools;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace BiliBili_UWP.Pages_Share.Main
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as Grid;
            Frame rootFrame = Window.Current.Content as Frame;
            if (item.Tag.ToString() == "Desktop")
            {
                AppTool.WriteLocalSetting(BiliBili_Lib.Enums.Settings.DisplayMode, "Desktop");
                App._isTabletMode = false;
                rootFrame.Navigate(typeof(DesktopMainPage),new DrillInNavigationTransitionInfo());
            }
            else
            {
                AppTool.WriteLocalSetting(BiliBili_Lib.Enums.Settings.DisplayMode, "Tablet");
                App._isTabletMode = true;
                rootFrame.Navigate(typeof(TabletMainPage), new DrillInNavigationTransitionInfo());
            }
        }
    }
}
