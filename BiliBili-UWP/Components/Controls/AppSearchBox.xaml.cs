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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class AppSearchBox : UserControl
    {
        public AppSearchBox()
        {
            this.InitializeComponent();
        }

        private void BiliSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void BiliSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void BiliSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }
    }
}
