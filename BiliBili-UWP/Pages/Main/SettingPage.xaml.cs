using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class SettingPage : Page
    {
        private bool _isInit = false;

        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_isInit || e.NavigationMode==NavigationMode.Back)
                return;

            string theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            ThemeComboBox.SelectedIndex = theme == "Light" ? 0 : 1;

            base.OnNavigatedTo(e);
            _isInit = true;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = (ThemeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            string oldTheme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            if (oldTheme != item)
            {
                AppTool.WriteLocalSetting(Settings.Theme, item);
                
            }
        }

        private async Task ShowRestartDialog()
        {
            var dialog = new ConfirmDialog("您已经修改了静态资源设置，该设置将在下次启动应用时生效") { PrimaryButtonText = "立即重启" };
            dialog.PrimaryButtonClick += async (_s, _e) =>
            {
                await CoreApplication.RequestRestartAsync("restart");
            };
            await dialog.ShowAsync();
        }
    }
}
