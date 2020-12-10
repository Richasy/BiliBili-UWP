using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.Core;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Others;
using BiliBili_UWP.Pages_Share.Main;
using MetroLog;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Resources;

namespace BiliBili_UWP
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public static AppViewModel AppViewModel;
        public static BiliViewModel BiliViewModel;
        public static ILogger _logger = LogManagerFactory.CreateLogManager().GetLogger("应用程序");
        public static Stopwatch _watch = new Stopwatch();
        public static bool _isTabletMode;
        public App()
        {
            this.InitializeComponent();

            CustomXamlResourceLoader.Current = new CustomResourceLoader();
            this.Suspending += OnSuspending;
            UnhandledException += OnUnhandleException;
            EnteredBackground += App_EnteredBackground;
            LeavingBackground += App_LeavingBackground;
            App.AppViewModel = new AppViewModel();
            App.BiliViewModel = new BiliViewModel();
            bool isThemeWithSystem = AppTool.GetBoolSetting(Settings.IsThemeWithSystem);
            _isTabletMode = AppTool.GetLocalSetting(Settings.DisplayMode, "") == "Tablet";
            if (!isThemeWithSystem)
            {
                string theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
                RequestedTheme = theme == "Light" ? ApplicationTheme.Light : ApplicationTheme.Dark;
            }
        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {
            AppViewModel.IsInBackground = false;
            AppViewModel.CheckPlayerOnBackgroundChanged();
        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            AppViewModel.IsInBackground = true;
            AppViewModel.CheckPlayerOnBackgroundChanged();
        }

        private void OnUnhandleException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            string msg = e.Message;
            new TipPopup(msg).ShowError();
            _logger.Error(msg, e.Exception);
        }
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            OnLaunchedOrActivated(e);
        }
        protected override void OnActivated(IActivatedEventArgs args)
        {
            OnLaunchedOrActivated(args);
        }
        private void OnLaunchedOrActivated(IActivatedEventArgs e)
        {
            try
            {
                Frame rootFrame = Window.Current.Content as Frame;

                // Do not repeat app initialization when the Window already has content,
                // just ensure that the window is active
                if (rootFrame == null)
                {
                    SYEngine.Core.Initialize();
                    // Create a Frame to act as the navigation context and navigate to the first page
                    rootFrame = new Frame();

                    rootFrame.NavigationFailed += OnNavigationFailed;

                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }

                BackgroundTaskHelper.Register("ToastBackgroundTask", new ToastNotificationActionTrigger());
                if (e is LaunchActivatedEventArgs && (e as LaunchActivatedEventArgs).PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        string display = AppTool.GetLocalSetting(Settings.DisplayMode, "");
                        if (string.IsNullOrEmpty(display))
                        {
                            rootFrame.Navigate(typeof(WelcomePage), (e as LaunchActivatedEventArgs).Arguments);
                        }
                        else
                        {
                            if (_isTabletMode)
                                rootFrame.Navigate(typeof(TabletMainPage), (e as LaunchActivatedEventArgs).Arguments);
                            else
                                rootFrame.Navigate(typeof(DesktopMainPage), (e as LaunchActivatedEventArgs).Arguments);
                        }
                    }
                    // Ensure the current window is active

                }
                else if (e.Kind == ActivationKind.StartupTask)
                {
                    if (rootFrame.Content == null)
                    {
                        if (_isTabletMode)
                            rootFrame.Navigate(typeof(TabletMainPage), null);
                        else
                            rootFrame.Navigate(typeof(DesktopMainPage), null);
                    }
                }
                else if (e is ToastNotificationActivatedEventArgs toastActivationArgs)
                {
                    if (rootFrame.Content == null)
                    {
                        if (_isTabletMode)
                            rootFrame.Navigate(typeof(TabletMainPage), toastActivationArgs.Argument);
                        else
                            rootFrame.Navigate(typeof(DesktopMainPage), toastActivationArgs.Argument);
                    }
                    else
                    {
                        AppViewModel.AppInitByActivated(toastActivationArgs.Argument);
                    }
                }
                else if (e is ProtocolActivatedEventArgs protocalArgs)
                {
                    string arg = protocalArgs.Uri.Query.Replace("?", "");
                    if (rootFrame.Content == null)
                    {
                        if (_isTabletMode)
                            rootFrame.Navigate(typeof(TabletMainPage), arg);
                        else
                            rootFrame.Navigate(typeof(DesktopMainPage), arg);
                    }
                    else
                    {
                        AppViewModel.AppInitByActivated(arg);
                    }
                }
                Window.Current.Activate();
                UIHelper.SetTitleBarColor();
                bool isDisableScale = AppTool.GetBoolSetting(Settings.DisableXboxScale);
                if (SystemInformation.DeviceFamily == "Windows.Xbox")
                {
                    var view = ApplicationView.GetForCurrentView();
                    view.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                    if (isDisableScale)
                        ApplicationViewScaling.TrySetDisableLayoutScaling(true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("启动出错", ex);
            }
        }
        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            if (args.TaskInstance.TriggerDetails is ToastNotificationActionTriggerDetail toastArgs)
            {
                string argument = toastArgs.Argument;
                AppViewModel.AppInitByActivated(argument);
            }

            deferral.Complete();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            _logger.Error($"页面导航失败: {e.SourcePageType.FullName}", e.Exception);
            e.Handled = true;
            new TipPopup("导航到特定页失败，错误信息已记录").ShowError();
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
