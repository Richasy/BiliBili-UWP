using Microsoft.Toolkit.Uwp.Connectivity;
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

namespace BiliBili_UWP.Components.Layout
{
    public sealed partial class BottomPanel : UserControl
    {
        private DispatcherTimer _timer;
        public BottomPanel()
        {
            this.InitializeComponent();
            Init();
        }
        public event EventHandler OpenSideButtonClick;
        private void Init()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            if (!NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable)
            {
                NetworkStatusBlock.Text = "";
                ToolTipService.SetToolTip(NetworkStatusBlock, "无网络");
            }
            else
            {
                switch (NetworkHelper.Instance.ConnectionInformation.ConnectionType)
                {
                    case ConnectionType.Ethernet:
                        NetworkStatusBlock.Text = "";
                        ToolTipService.SetToolTip(NetworkStatusBlock, "以太网");
                        break;
                    case ConnectionType.WiFi:
                        NetworkStatusBlock.Text = "";
                        ToolTipService.SetToolTip(NetworkStatusBlock, "Wifi");
                        break;
                    case ConnectionType.Data:
                        NetworkStatusBlock.Text = "";
                        ToolTipService.SetToolTip(NetworkStatusBlock, "数据流量");
                        break;
                    case ConnectionType.Unknown:
                        NetworkStatusBlock.Text = "";
                        ToolTipService.SetToolTip(NetworkStatusBlock, "未知网络");
                        break;
                }
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            var now = DateTime.Now.ToString("HH:mm");
            TimeBlock.Text = now;
        }

        public event EventHandler SettingButtonClick;
        public event EventHandler HelpButtonClick;

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private void OpenSideButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSideButtonClick?.Invoke(this, EventArgs.Empty);
        }


        public Visibility OpenSideButtonVisibility
        {
            get { return (Visibility)GetValue(OpenSideButtonVisibilityProperty); }
            set { SetValue(OpenSideButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenSideButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenSideButtonVisibilityProperty =
            DependencyProperty.Register("OpenSideButtonVisibility", typeof(Visibility), typeof(BottomPanel), new PropertyMetadata(Visibility.Collapsed));


    }
}
