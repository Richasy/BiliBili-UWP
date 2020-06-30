using System;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BiliBili_UWP.Pages_Share.Main
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        private bool _isInit = false;
        Guid guid = Guid.NewGuid();
        public HelpPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back || _isInit)
                return;
            var introduceFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Others/Introduce.txt"));
            string introduce = await FileIO.ReadTextAsync(introduceFile);
            RenderBlock.Text = introduce;
            var shortcutFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Others/Shortcut.txt"));
            string shortcut = await FileIO.ReadTextAsync(shortcutFile);
            ShortcutBlock.Text = shortcut;
            App.AppViewModel.WindowsSizeChangedNotify.Add(new Tuple<Guid, Action<Size>>(guid, (size) =>
            {
                CheckLayout(size);
            }));
            CheckLayout(new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height));
            base.OnNavigatedTo(e);
            _isInit = true;
        }

        private void CheckLayout(Size size)
        {
            if (size.Width < 1100 && Grid.GetRow(SubContainer) != 1)
            {
                Grid.SetRow(SubContainer, 1);
                Grid.SetColumn(SubContainer, 0);
                SubContainer.Margin = new Thickness(0, 20, 0, 0);
            }
            else if (size.Width >= 1100 && Grid.GetRow(SubContainer) == 1)
            {
                Grid.SetRow(SubContainer, 0);
                Grid.SetColumn(SubContainer, 1);
                SubContainer.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == guid);
            base.OnNavigatedFrom(e);
        }
        private async void ToBiliButton_Click(object sender, RoutedEventArgs e)
        {
            ToBiliButton.IsLoading = true;
            await Launcher.LaunchUriAsync(new Uri("https://space.bilibili.com/5992670"));
            ToBiliButton.IsLoading = false;
        }

        private async void RenderBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        private async void UpdateHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.richasy.cn/document/bilibili/update.html"));
        }

        private async void QAButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.richasy.cn/document/bilibili/qa.html"));
        }
    }
}
