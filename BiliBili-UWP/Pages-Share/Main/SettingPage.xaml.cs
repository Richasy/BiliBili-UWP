using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Others;
using Microsoft.Toolkit.Uwp.Helpers;
using Richasy.Font.UWP;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
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
    public sealed partial class SettingPage : Page
    {
        private bool _isInit = false;
        private bool _tempStartupHandle = false;
        private ObservableCollection<SystemFont> FontCollection = App.AppViewModel.FontCollection;

        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_isInit || e.NavigationMode == NavigationMode.Back)
                return;

            #region 基础设置
            bool isThemeWithSystem = AppTool.GetBoolSetting(Settings.IsThemeWithSystem);
            ThemeWithSystemSwitch.IsOn = isThemeWithSystem;
            ThemeComboBox.Visibility = isThemeWithSystem ? Visibility.Collapsed : Visibility.Visible;
            string theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            ThemeComboBox.SelectedIndex = theme == "Light" ? 0 : 1;
            string displayMode = AppTool.GetLocalSetting(Settings.DisplayMode, "Desktop");
            switch (displayMode)
            {
                case "Desktop":
                    DisplayModeComboBox.SelectedIndex = 0;
                    break;
                case "Tablet":
                    DisplayModeComboBox.SelectedIndex = 1;
                    break;
                default:
                    break;
            }
            double pageBreakpoint = Convert.ToDouble(AppTool.GetLocalSetting(Settings.PagePanelDisplayBreakpoint, "1500"));
            PagePaneDisplayBreakpointBox.Value = pageBreakpoint;
            bool isEnableAnimation = AppTool.GetBoolSetting(Settings.EnableAnimation);
            EnableAnimationSwitch.IsOn = isEnableAnimation;
            StartupTask startupTask = await StartupTask.GetAsync("RichasyBiliBili");
            bool isEnableStartup = startupTask.State.ToString().Contains("Enable");
            EnableStartupSwitch.IsOn = isEnableStartup;
            double fontSize = Convert.ToDouble(AppTool.GetLocalSetting(Settings.BasicFontSize, "14"));
            FontSizeBox.Value = fontSize;
            DisableScaleInXboxSwitch.IsEnabled = SystemInformation.DeviceFamily == "Windows.Xbox";
            bool disableScale = AppTool.GetBoolSetting(Settings.DisableXboxScale);
            DisableScaleInXboxSwitch.IsOn = disableScale;
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
            bool isFirst4K = AppTool.GetBoolSetting(Settings.IsFirst4K, false);
            First4KSwitch.IsOn = isFirst4K;
            bool isFirstHEVC = AppTool.GetBoolSetting(Settings.IsUseHevc, false);
            FirstHEVCSwitch.IsOn = isFirstHEVC;
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
            string playerMode = AppTool.GetLocalSetting(Settings.PlayerMode, "Default");
            switch (playerMode)
            {
                case "Default":
                default:
                    PlayerModeComboBox.SelectedIndex = 0;
                    break;
                case "Cinema":
                    PlayerModeComboBox.SelectedIndex = 1;
                    break;
                case "Full":
                    PlayerModeComboBox.SelectedIndex = 2;
                    break;
            }
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

        private async void EnableStartupSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            if (_tempStartupHandle)
            {
                _tempStartupHandle = false;
                return;
            }
            bool isOn = EnableStartupSwitch.IsOn;
            StartupTask startupTask = await StartupTask.GetAsync("RichasyBiliBili");
            bool isHandled = false;
            if (isOn)
            {
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                        StartupTaskState newState = await startupTask.RequestEnableAsync();
                        if(newState==StartupTaskState.Enabled || newState==StartupTaskState.EnabledByPolicy)
                        {
                            isHandled = true;
                            new TipPopup("已开启应用自启动").ShowMessage();
                        }
                        else
                            new TipPopup("启动失败").ShowMessage();
                        break;
                    case StartupTaskState.DisabledByUser:
                        await new ConfirmDialog("您已在任务管理器中禁用了 哔哩 的启动项，请先启用它").ShowAsync();
                        break;
                    case StartupTaskState.DisabledByPolicy:
                        await new ConfirmDialog("受限于设备或组织策略，您无法设置启动项").ShowAsync();
                        break;
                    case StartupTaskState.Enabled:
                    case StartupTaskState.EnabledByPolicy:
                        isHandled = true;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (startupTask.State)
                {
                    case StartupTaskState.Disabled:
                    case StartupTaskState.DisabledByUser:
                    case StartupTaskState.DisabledByPolicy:
                        isHandled = true;
                        break;
                    case StartupTaskState.Enabled:
                        startupTask.Disable();
                        isHandled = true;
                        new TipPopup("已禁用应用自启").ShowMessage();
                        break;
                    case StartupTaskState.EnabledByPolicy:
                        await new ConfirmDialog("受限于设备或组织策略，您无法设置启动项").ShowAsync();
                        break;
                    default:
                        break;
                }
            }
            if (!isHandled)
            {
                _tempStartupHandle = true;
                EnableStartupSwitch.IsOn = !isOn;
            }
        }

        private void PlayerModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = PlayerModeComboBox.SelectedItem as ComboBoxItem;
            string tag = item.Tag.ToString();
            AppTool.WriteLocalSetting(Settings.PlayerMode, tag);
        }

        private void FontSizeBox_ValueChanged(object sender, double e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.BasicFontSize, e.ToString());
        }

        private async void ExportLogButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MetroLogs", CreationCollisionOption.OpenIfExists);
            await Launcher.LaunchFolderAsync(folder);
        }

        private async void DisplayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInit)
                return;
            var item = DisplayModeComboBox.SelectedItem as ComboBoxItem;
            AppTool.WriteLocalSetting(Settings.DisplayMode, item.Tag.ToString());
            await ShowRestartDialog();
        }

        private async void DisableScaleInXboxSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.DisableXboxScale, DisableScaleInXboxSwitch.IsOn.ToString());
            await ShowRestartDialog();
        }

        private void First4KSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsFirst4K, First4KSwitch.IsOn.ToString());
        }

        private void FirstHEVCSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!_isInit)
                return;
            AppTool.WriteLocalSetting(Settings.IsUseHevc, FirstHEVCSwitch.IsOn.ToString());
        }
    }
}
