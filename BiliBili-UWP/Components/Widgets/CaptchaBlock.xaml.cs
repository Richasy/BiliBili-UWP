using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class CaptchaBlock : UserControl
    {
        public CaptchaBlock()
        {
            this.InitializeComponent();
        }

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(CaptchaBlock), new PropertyMetadata(""));


        public async Task RefreshCode()
        {
            CaptchaImage.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = true;
            var image = await App.BiliViewModel._client.Account.GetCaptchaAsync();
            CaptchaImage.Source = image;
            LoadingRing.IsActive = false;
            CaptchaImage.Visibility = Visibility.Visible;
        }

        private async void CaptchaImage_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Code = "";
            await RefreshCode();
        }
    }
}
