using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        private bool _isInit = false;
        public HelpPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Others/Introduce.txt"));
            string content = await FileIO.ReadTextAsync(file);
            RenderBlock.Text = content;
            base.OnNavigatedTo(e);
            _isInit = true;
        }
        private async void ToBiliButton_Click(object sender, RoutedEventArgs e)
        {
            ToBiliButton.IsLoading = true;
            await Launcher.LaunchUriAsync(new Uri("https://space.bilibili.com/5992670"));
            ToBiliButton.IsLoading = false;
        }

        private async void SendMailButton_Click(object sender, RoutedEventArgs e)
        {
            SendMailButton.IsLoading = true;
            await Launcher.LaunchUriAsync(new Uri("mailto://thansy@foxmail.com"));
            SendMailButton.IsLoading = false;
        }
    }
}
