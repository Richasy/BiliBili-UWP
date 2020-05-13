using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
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
        private ObservableCollection<SystemFont> FontCollection = App.AppViewModel.FontCollection;

        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_isInit || e.NavigationMode == NavigationMode.Back)
                return;

            #region 基础设置
            bool isThemeWithSystem = AppTool.GetBoolSetting(Settings.IsThemeWithSystem);
            ThemeWithSystemSwitch.IsOn = isThemeWithSystem;
            ThemeComboBox.Visibility = isThemeWithSystem ? Visibility.Collapsed : Visibility.Visible;
            string theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            ThemeComboBox.SelectedIndex = theme == "Light" ? 0 : 1;
            double pageBreakpoint = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PagePanelDisplayBreakpoint, "1500"));
            PagePaneDisplayBreakpointBox.Value = pageBreakpoint;
            bool isEnableAnimation = AppTool.GetBoolSetting(Settings.EnableAnimation);
            EnableAnimationSwitch.IsOn = isEnableAnimation;
            #endregion

            #region 播放器设置
            bool isAutoPlay = AppTool.GetBoolSetting(Settings.IsAutoPlay);
            AutoPlaySwitch.IsOn = isAutoPlay;
            bool isAutoNextPart = AppTool.GetBoolSetting(Settings.IsAutoNextPart,false);
            AutoNextPartSwitch.IsOn = isAutoNextPart;
            bool isManualMTC = AppTool.GetBoolSetting(Settings.IsManualMediaTransportControls, false);
            ManualMTCSwitch.IsOn = isManualMTC;
            double playerSkipStep = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PlayerSkipStep, "30"));
            PlayerSkipStepBox.Value = playerSkipStep;
            bool isShowDanmakuBarInFullWindow = AppTool.GetBoolSetting(Settings.IsShowDanmakuBarInFullWindow);
            OpenDanmakuBarInFullWindowSwitch.IsOn = isShowDanmakuBarInFullWindow;
            bool isShowDanmakuBarInCinema = AppTool.GetBoolSetting(Settings.IsShowDanmakuBarInCinema);
            OpenDanmakuBarInCinemaSwitch.IsOn = isShowDanmakuBarInCinema;
            bool isShowDanmakuBarInCompact = AppTool.GetBoolSetting(Settings.IsShowDanmakuBarInCompactOverlay);
            OpenDanmakuBarInCompactSwitch.IsOn = isShowDanmakuBarInCompact;
            bool isShowDanmakuBarInSeparate = AppTool.GetBoolSetting(Settings.IsShowDanmakuBarInSeparate);
            OpenDanmakuBarInSeparateSwitch.IsOn = isShowDanmakuBarInSeparate;
            bool isShowDanmakuInCompact = AppTool.GetBoolSetting(Settings.IsShowDanmakuInCompactOverlay);
            OpenDanmakuInCompactSwitch.IsOn = isShowDanmakuInCompact;
            bool isStopInBackground = AppTool.GetBoolSetting(Settings.IsStopInBackground);
            StopPlayInBackgroundSwitch.IsOn = isStopInBackground;
            #endregion

            #region 通知设置
            bool isOpenDynamicToast = AppTool.GetBoolSetting(Settings.IsOpenNewDynamicNotification, false);
            NewDynamicToastSwitch.IsOn = isOpenDynamicToast;
            #endregion

            base.OnNavigatedTo(e);
            _isInit = true;
        }

        private void FontInit()
        {
            FontComboBox.IsEnabled = false;
            if (FontCollection != null && FontCollection.Count > 0)
            {
                string fontName = AppTool.GetLocalSetting(Settings.FontFamily, "微软雅黑");
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

        private void StopPlayInBackgroundSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsStopInBackground, StopPlayInBackgroundSwitch.IsOn.ToString());
        }

        private void OpenDanmakuBarInFullWindowSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsShowDanmakuBarInFullWindow, OpenDanmakuBarInFullWindowSwitch.IsOn.ToString());
        }

        private void OpenDanmakuBarInCinemaSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsShowDanmakuBarInCinema, OpenDanmakuBarInCinemaSwitch.IsOn.ToString());
        }

        private void OpenDanmakuBarInCompactSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsShowDanmakuBarInCompactOverlay, OpenDanmakuBarInCompactSwitch.IsOn.ToString());
        }

        private void OpenDanmakuBarInSeparateSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsShowDanmakuBarInSeparate, OpenDanmakuBarInSeparateSwitch.IsOn.ToString());
        }

        private void OpenDanmakuInCompactSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsShowDanmakuInCompactOverlay, OpenDanmakuInCompactSwitch.IsOn.ToString());
        }

        private void PagePaneDisplayBreakpointBox_ValueChanged(object sender, double e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.PagePanelDisplayBreakpoint, e.ToString());
        }

        private async void ThemeWithSystemSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            bool ison = ThemeWithSystemSwitch.IsOn;
            AppTool.WriteLocalSetting(Settings.IsThemeWithSystem, ison.ToString());
            ThemeComboBox.Visibility = ison ? Visibility.Collapsed : Visibility.Visible;
            await ShowRestartDialog();
        }

        private async void NewDynamicToastSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            if (NewDynamicToastSwitch.IsOn)
            {
                bool isSuccess = await App.AppViewModel.RegisterBackgroundTask(StaticString.NOTIFICATION_NEWDYNAMIC);
                if (isSuccess)
                    AppTool.WriteLocalSetting(Settings.IsOpenNewDynamicNotification, "True");
                else
                {
                    AppTool.WriteLocalSetting(Settings.IsOpenNewDynamicNotification, "False");
                    NewDynamicToastSwitch.IsOn = false;
                }
            }
            else
            {
                App.AppViewModel.UnRegisterBackgroundTask(StaticString.NOTIFICATION_NEWDYNAMIC);
            }
        }

        private void EnableAnimationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.EnableAnimation, EnableAnimationSwitch.IsOn.ToString());
        }

        private void AutoNextPartSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsAutoNextPart, AutoNextPartSwitch.IsOn.ToString());
        }
    }
}
