using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI.Others;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<SystemFont> FontCollection = new ObservableCollection<SystemFont>();

        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_isInit || e.NavigationMode == NavigationMode.Back)
                return;

            string theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            ThemeComboBox.SelectedIndex = theme == "Light" ? 0 : 1;
            bool isAutoPlay = AppTool.GetBoolSetting(Settings.IsAutoPlay);
            AutoPlaySwitch.IsOn = isAutoPlay;
            bool isManualMTC = AppTool.GetBoolSetting(Settings.IsManualMediaTransportControls,false);
            ManualMTCSwitch.IsOn = isManualMTC;
            double playerSkipStep = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PlayerSkipStep, "30"));
            PlayerSkipStepBox.Value = playerSkipStep;

            base.OnNavigatedTo(e);
            _isInit = true;
        }

        private void FontInit()
        {
            FontComboBox.IsEnabled = false;
            var fonts = SystemFont.GetFonts();
            if (fonts != null && fonts.Count > 0)
            {
                string fontName = AppTool.GetLocalSetting(Settings.FontFamily, "微软雅黑");
                fonts.ForEach(p => FontCollection.Add(p));
                var font = FontCollection.Where(p => p.Name == fontName).FirstOrDefault();
                if (font != null)
                {
                    FontComboBox.SelectedItem = font;
                }
            }
            FontComboBox.IsEnabled = true;
        }

        private async void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = (ThemeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            string oldTheme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            if (oldTheme != item)
            {
                AppTool.WriteLocalSetting(Settings.Theme, item);
                await ShowRestartDialog();
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

        private async void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = FontComboBox.SelectedItem as SystemFont;
            string oldFont = AppTool.GetLocalSetting(Settings.FontFamily, "微软雅黑");
            if (item.Name != oldFont)
            {
                AppTool.WriteLocalSetting(Settings.FontFamily, item.Name);
                await ShowRestartDialog();
            }
        }

        private void AutoPlaySwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsAutoPlay, AutoPlaySwitch.IsOn.ToString());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FontInit();
        }

        private void ManualMTCSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsManualMediaTransportControls, ManualMTCSwitch.IsOn.ToString());
        }

        private void PlayerSkipStepBox_ValueChanged(object sender, double e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.PlayerSkipStep, e.ToString());
        }
    }
}
