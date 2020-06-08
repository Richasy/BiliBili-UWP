using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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
    public sealed partial class NetworkErrorPage : Page
    {
        public NetworkErrorPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Others/NetworkError.txt"));
            string text = await FileIO.ReadTextAsync(file);
            DisplayBlock.Text = text;
            base.OnNavigatedTo(e);
        }

        private async void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            await CoreApplication.RequestRestartAsync("restart");
        }

        private async void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MetroLogs", CreationCollisionOption.OpenIfExists);
            await Launcher.LaunchFolderAsync(folder);
        }
    }
}
