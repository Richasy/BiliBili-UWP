using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using NSDanmaku.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BiliBili_UWP.Models.UI.Others
{
    public sealed class DanmakuMTC : MediaTransportControls
    {
        public DanmakuMTC()
        {
            string fontFamily = AppTool.GetLocalSetting(Settings.FontFamily, "微软雅黑");
            this.DefaultStyleKey = typeof(DanmakuMTC);
            VideoTitle = "";
            ShowDanmakuBtn = Visibility.Visible;
            ShowSendDanmaku = Visibility.Visible;
            ShowCoinsBtn = Visibility.Visible;
            ShowShareBtn = Visibility.Visible;
            ShowSendDanmuBtn = false;
            ShowNextButton = false;
            ShowPreviousButton = false;
            Brightness = 0;
            ShowDanmakuSettingBtn = Visibility.Visible;
            ShowPlayListBtn = false;

            SubTitleBackground = new SolidColorBrush(Color.FromArgb(120, 0, 0, 0));
            SubTitleColor = UIHelper.GetThemeBrush(Enums.ColorType.ImportantTextColor);
            SubTitleFontFamily = new FontFamily(fontFamily);
            SubTitleFontSize = 16;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(5);
            timer2.Tick += Timer2_Tick;
        }

        private void Timer2_Tick(object sender, object e)
        {
            try
            {
                HideOrShowMTC();
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            try
            {
                (GetTemplateChild("txt_time") as TextBlock).Text = DateTime.Now.ToString("HH:mm");
            }
            catch (Exception ex)
            {
                
            }
        }

        DispatcherTimer timer;
        public DispatcherTimer timer2;
        public event EventHandler<Danmaku> DanmuLoaded;
        public event EventHandler OnMiniWindows;
        public event EventHandler ExitPlayer;
        public event EventHandler OnExitMiniWindows;
        public event EventHandler DanmakuSetting;
        public event EventHandler CoinsEvent;
        public event EventHandler ShareEvent;
        public event EventHandler<bool> OpenDanmaku;
        public event EventHandler Next;
        public event EventHandler Previous;
        public event EventHandler SelectList;
        public event EventHandler SendDanmakued;
        public event EventHandler FullWindows;
        public event EventHandler ExitFullWindows;
        public event EventHandler Captured;
        public event EventHandler<double> FastForward;
        public bool IsMiniWindow = false;
        public bool IsFullWindow = false;

        //public Danmaku danmuControls;
        protected override void OnApplyTemplate()
        {
            try
            {
                if (DanmuLoaded != null)
                {
                    DanmuLoaded(this, GetTemplateChild("danmuControls") as Danmaku);
                }
                myDanmaku = GetTemplateChild("danmuControls") as Danmaku;
                (GetTemplateChild("MiniWindowsButton") as AppBarButton).Click += MiniWindowsButton_Click;
                (GetTemplateChild("btn_Back") as AppBarButton).Click += ExitButton_Click;
                (GetTemplateChild("btn_danmaku") as AppBarButton).Click += CloseOpenDanmaku_Click;
                (GetTemplateChild("btn_danmakusetting") as AppBarButton).Click += DanmakuSetting_Click;
                (GetTemplateChild("btn_Previous") as AppBarButton).Click += btn_Previous_Click;
                (GetTemplateChild("btn_Next") as AppBarButton).Click += btn_Next_Click;
                (GetTemplateChild("ListButton") as AppBarButton).Click += btn_Playlist_Click;
                (GetTemplateChild("CoinsBtn") as AppBarButton).Click += Btn_Coins_Click;
                (GetTemplateChild("ShareBtn") as AppBarButton).Click += Btn_Share_Click;
                (GetTemplateChild("btn_send") as AppBarButton).Click += Btn_Send_Click;
                (GetTemplateChild("MyFullWindowButton") as AppBarButton).Click += Btn_Full_Click;
                (GetTemplateChild("CaptureBtn") as AppBarButton).Click += Btn_Capture_Click;

                if (!AppTool.GetBoolSetting(Settings.IsDanmakuOpen))
                {
                    (GetTemplateChild("btn_danmaku") as AppBarButton).Icon = new FontIcon()
                    {
                        FontFamily=UIHelper.GetFontFamily("Icon"),
                        Glyph= ""
                    };
                }

                (GetTemplateChild("ControlPanel_ControlPanelVisibilityStates_Border") as Border).Tapped += DanmakuMTC_Tapped;

                (GetTemplateChild("RootGrid") as Grid).ManipulationStarting += MTC_ManipulationStarting;
                (GetTemplateChild("RootGrid") as Grid).ManipulationDelta += Grid_ManipulationDelta;
                (GetTemplateChild("RootGrid") as Grid).ManipulationCompleted += Grid_ManipulationCompleted;
                try
                {
                    if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                    {
                        (GetTemplateChild("MiniWindowsButton") as AppBarButton).IsEnabled = true;
                    }
                    else
                    {
                        (GetTemplateChild("MiniWindowsButton") as AppBarButton).IsEnabled = false;
                    }
                }
                catch (Exception)
                {
                    (GetTemplateChild("MiniWindowsButton") as AppBarButton).IsEnabled = false;
                }

                timer.Start();
                base.OnApplyTemplate();
            }
            catch (Exception ex)
            {
                
            }

        }

        public void HideOrShowMTC()
        {
            var root = (GetTemplateChild("RootGrid") as Grid);
            var bar = (GetTemplateChild("ControlPanel_ControlPanelVisibilityStates_Border") as Border);
            if (bar.Visibility == Visibility.Visible)
            {
                (root.Resources["FadeOut"] as Storyboard).Begin();
                (root.Resources["FadeOut"] as Storyboard).Completed += new EventHandler<object>((sender, e) =>
                {
                    bar.Visibility = Visibility.Collapsed;
                });
                timer2.Stop();
            }
            else
            {
                bar.Visibility = Visibility.Visible;
                (root.Resources["FadeIn"] as Storyboard).Begin();
                timer2.Start();
            }
        }



        double ssValue = 0;
        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            //progress.Visibility = Visibility.Visible;
            double X = e.Delta.Translation.X;
            if (X > 0)
            {
                double dd = X / this.ActualWidth;
                double d = dd * 90;
                ssValue += d;
                //slider.Value += d;
                new TipPopup("+" + ssValue.ToString("0") + "S").ShowMessage();
            }
            else
            {
                double dd = Math.Abs(X) / this.ActualWidth;
                double d = dd * 90;
                ssValue -= d;
                //slider.Value -= d;
                new TipPopup("-" + ssValue.ToString("0") + "S").ShowMessage();
            }



        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;


            double X = e.Cumulative.Translation.X;
            if (ssValue != 0 && FastForward != null)
            {
                FastForward(sender, ssValue);

            }
        }




        private void MTC_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            e.Handled = true;
            ssValue = 0;
        }



        private void DanmakuMTC_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as Border).Visibility == Visibility.Visible)
            {
                (sender as Border).Visibility = Visibility.Collapsed;
            }
            else
            {
                (sender as Border).Visibility = Visibility.Visible;
            }
        }

        private void Btn_Capture_Click(object sender, RoutedEventArgs e)
        {
            if (Captured != null)
            {
                Captured(sender, new EventArgs());
            }
        }







        private void Btn_Full_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFullWindow)
            {
                IsFullWindow = true;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph = "",
                    FontFamily = UIHelper.GetFontFamily("Icon")
                };

                if (FullWindows != null)
                {
                    FullWindows(this, new EventArgs());
                }
            }
            else
            {
                IsFullWindow = false;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph = "",
                    FontFamily = UIHelper.GetFontFamily("Icon")
                };
                if (ExitFullWindows != null)
                {
                    ExitFullWindows(this, new EventArgs());
                }
            }
        }


        public void ToFull()
        {
            IsFullWindow = true;
            (GetTemplateChild("MyFullWindowButton") as AppBarButton).Icon = new FontIcon()
            {
                Glyph = "",
                FontFamily = UIHelper.GetFontFamily("Icon")
            };

            if (FullWindows != null)
            {
                FullWindows(this, new EventArgs());
            }

        }

        public void ExitFull()
        {
            IsFullWindow = false;
            (GetTemplateChild("MyFullWindowButton") as AppBarButton).Icon = new FontIcon()
            {
                Glyph = "",
                FontFamily = UIHelper.GetFontFamily("Icon")
            };
            if (ExitFullWindows != null)
            {
                ExitFullWindows(this, new EventArgs());
            }
        }

        public void SetSubtitle(string text)
        {
            (GetTemplateChild("text_subtitle") as TextBlock).Text = text;
        }
        public string GetSubtitle()
        {
            return (GetTemplateChild("text_subtitle") as TextBlock).Text;
        }
        public void HideSubtitle()
        {
            (GetTemplateChild("grid_subtitle") as Grid).Visibility = Visibility.Collapsed;
        }
        public void ShowSubtitle()
        {
            (GetTemplateChild("grid_subtitle") as Grid).Visibility = Visibility.Visible;
        }
        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (SendDanmakued != null)
            {
                SendDanmakued(sender, new EventArgs());
            }
        }

        private void Btn_Share_Click(object sender, RoutedEventArgs e)
        {
            if (ShareEvent != null)
            {
                ShareEvent(sender, new EventArgs());
            }
        }

        private void Btn_Coins_Click(object sender, RoutedEventArgs e)
        {
            if (CoinsEvent != null)
            {
                CoinsEvent(sender, new EventArgs());
            }
        }

        private void btn_Playlist_Click(object sender, RoutedEventArgs e)
        {
            if (SelectList != null)
            {
                SelectList(sender, new EventArgs());
            }
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (Next != null)
            {
                Next(sender, new EventArgs());
            }
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            if (Previous != null)
            {
                Previous(sender, new EventArgs());
            }
        }

        private void DanmakuSetting_Click(object sender, RoutedEventArgs e)
        {
            if (DanmakuSetting != null)
            {
                DanmakuSetting(sender, new EventArgs());
            }
        }

        private void CloseOpenDanmaku_Click(object sender, RoutedEventArgs e)
        {
            if (myDanmaku.Visibility == Visibility.Visible)
            {
                myDanmaku.ClearAll();
                myDanmaku.Visibility = Visibility.Collapsed;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph = "",
                    FontFamily = UIHelper.GetFontFamily("Icon")
                };
                if (OpenDanmaku != null)
                {
                    OpenDanmaku(this, false);
                    AppTool.WriteLocalSetting(Settings.IsDanmakuOpen,"False");
                }
            }
            else
            {
                myDanmaku.ClearAll();
                myDanmaku.Visibility = Visibility.Visible;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph= "",
                    FontFamily=UIHelper.GetFontFamily("Icon")
                };
                if (OpenDanmaku != null)
                {
                    OpenDanmaku(this, true);
                    AppTool.WriteLocalSetting(Settings.IsDanmakuOpen, "True");
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExitPlayer != null)
            {
                ExitPlayer(sender, new EventArgs());
            }
        }

        private void MiniWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!IsMiniWindow)
            {
                IsMiniWindow = true;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph = "",
                    FontFamily = UIHelper.GetFontFamily("Icon")
                };
                if (OnMiniWindows != null)
                {
                    OnMiniWindows(this, new EventArgs());
                }
            }
            else
            {
                IsMiniWindow = false;
                (sender as AppBarButton).Icon = new FontIcon()
                {
                    Glyph = "",
                    FontFamily = UIHelper.GetFontFamily("Icon")
                };
                if (OnExitMiniWindows != null)
                {
                    OnExitMiniWindows(this, new EventArgs());
                }
            }


        }



        public void AddLog(string text)
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Visible;
            tb.Text += string.Format("[{0}]{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), text);
        }
        public void ShowLog()
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Visible;
        }
        public void HideLog()
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Visibility = Visibility.Collapsed;
        }
        public void ClearLog()
        {
            var tb = GetTemplateChild("Txt_Log") as TextBlock;
            tb.Text = "";
        }

        public string VideoTitle
        {
            get { return (string)GetValue(VideoTitleProperty); }
            set
            {
                {
                    SetValue(VideoTitleProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoTitleProperty =
            DependencyProperty.Register("VideoTitle", typeof(string), typeof(MediaTransportControls), new PropertyMetadata("", null));




        public Danmaku myDanmaku
        {
            get { return (Danmaku)GetValue(DanmakuProperty); }
            set { SetValue(DanmakuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DanmakuProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DanmakuProperty =
            DependencyProperty.Register("myDanmaku", typeof(Danmaku), typeof(MediaTransportControls), new PropertyMetadata(null));





        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BrightnessProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(MediaTransportControls), new PropertyMetadata(0, OnBrightnessChanged));

        private static void OnBrightnessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = (double)e.NewValue;
            if (value < 0)
            {
                value = 0;
            }
            if (value > 1)
            {
                value = 1;
            }

        }



        public Visibility ShowDanmakuBtn
        {
            get { return (Visibility)GetValue(ShowDanmakuBtnProperty); }
            set { SetValue(ShowDanmakuBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDanmakuBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDanmakuBtnProperty =
            DependencyProperty.Register("ShowDanmakuBtn", typeof(Visibility), typeof(MediaTransportControls), new PropertyMetadata(0));





        public Visibility ShowSendDanmaku
        {
            get { return (Visibility)GetValue(ShowSendDanmakuProperty); }
            set { SetValue(ShowSendDanmakuProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSendDanmaku.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSendDanmakuProperty =
            DependencyProperty.Register("ShowSendDanmaku", typeof(Visibility), typeof(MediaTransportControls), new PropertyMetadata(Visibility.Visible));




        public UIElement QuityContent
        {
            get { return (UIElement)GetValue(QuityContentProperty); }
            set { SetValue(QuityContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuityContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuityContentProperty =
            DependencyProperty.Register("QuityContent", typeof(UIElement), typeof(MediaTransportControls), new PropertyMetadata(null));




        public FlyoutBase MoreMenuFlyout
        {
            get { return (FlyoutBase)GetValue(MoreMenuFlyoutProperty); }
            set { SetValue(MoreMenuFlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoreMenuFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoreMenuFlyoutProperty =
            DependencyProperty.Register("MoreMenuFlyout", typeof(FlyoutBase), typeof(MediaTransportControls), new PropertyMetadata(0));





        public Visibility ShowCoinsBtn
        {
            get { return (Visibility)GetValue(ShowCoinsBtnProperty); }
            set { SetValue(ShowCoinsBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowCoinsBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowCoinsBtnProperty =
            DependencyProperty.Register("ShowCoinsBtn", typeof(Visibility), typeof(MediaTransportControls), new PropertyMetadata(Visibility.Visible));




        public Visibility ShowShareBtn
        {
            get { return (Visibility)GetValue(ShowShareBtnProperty); }
            set { SetValue(ShowShareBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowShareBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowShareBtnProperty =
            DependencyProperty.Register("ShowShareBtn", typeof(Visibility), typeof(MediaTransportControls), new PropertyMetadata(Visibility.Visible));




        public Visibility ShowDanmakuSettingBtn
        {
            get { return (Visibility)GetValue(ShowDanmakuSettingBtnProperty); }
            set { SetValue(ShowDanmakuSettingBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDanmakuSettingBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDanmakuSettingBtnProperty =
            DependencyProperty.Register("ShowDanmakuSettingBtn", typeof(Visibility), typeof(MediaTransportControls), new PropertyMetadata(Visibility.Visible));



        public bool ShowNextButton
        {
            get { return (bool)GetValue(ShowNextButtonProperty); }
            set { SetValue(ShowNextButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNextButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNextButtonProperty =
            DependencyProperty.Register("ShowNextButton", typeof(bool), typeof(MediaTransportControls), new PropertyMetadata(false));


        public bool ShowPreviousButton
        {
            get { return (bool)GetValue(ShowPreviousButtonProperty); }
            set { SetValue(ShowPreviousButtonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNextButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPreviousButtonProperty =
            DependencyProperty.Register("ShowPreviousButton", typeof(bool), typeof(MediaTransportControls), new PropertyMetadata(false));




        public bool ShowPlayListBtn
        {
            get { return (bool)GetValue(ShowPlayListBtnProperty); }
            set { SetValue(ShowPlayListBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowPlayListBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowPlayListBtnProperty =
            DependencyProperty.Register("ShowPlayListBtn", typeof(bool), typeof(MediaTransportControls), new PropertyMetadata(false));




        public bool ShowSendDanmuBtn
        {
            get { return (bool)GetValue(ShowSendDanmuBtnProperty); }
            set { SetValue(ShowSendDanmuBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowSendDanmuBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSendDanmuBtnProperty =
            DependencyProperty.Register("ShowSendDanmuBtn", typeof(bool), typeof(MediaTransportControls), new PropertyMetadata(false));




        public UIElement GestureContent
        {
            get { return (UIElement)GetValue(GestureContentProperty); }
            set { SetValue(GestureContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GestureContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GestureContentProperty =
            DependencyProperty.Register("GestureContent", typeof(UIElement), typeof(MediaTransportControls), new PropertyMetadata(null));

        //CCSelectFlyout
        public FlyoutBase CCSelectFlyout
        {
            get { return (FlyoutBase)GetValue(CCSelectFlyoutProperty); }
            set { SetValue(CCSelectFlyoutProperty, value); }
        }

        public static readonly DependencyProperty CCSelectFlyoutProperty =
            DependencyProperty.Register("CCSelectFlyout", typeof(FlyoutBase), typeof(MediaTransportControls), new PropertyMetadata(0));


        /// <summary>
        /// 字幕字体大小
        /// </summary>
        public double SubTitleFontSize
        {
            get { return (int)GetValue(SubTitleFontSizeProperty); }
            set { SetValue(SubTitleFontSizeProperty, value); }
        }
        public static readonly DependencyProperty SubTitleFontSizeProperty =
            DependencyProperty.Register("SubTitleFontSize", typeof(double), typeof(MediaTransportControls), new PropertyMetadata(25.0));


        /// <summary>
        /// 字幕字体
        /// </summary>
        public FontFamily SubTitleFontFamily
        {
            get { return (FontFamily)GetValue(SubTitleFontFamilyProperty); }
            set { SetValue(SubTitleFontFamilyProperty, value); }
        }

        public static readonly DependencyProperty SubTitleFontFamilyProperty =
            DependencyProperty.Register("SubTitleFontFamily", typeof(FontFamily), typeof(MediaTransportControls), new PropertyMetadata(new FontFamily("Segoe UI")));

        /// <summary>
        /// 字幕背景颜色
        /// </summary>
        public Brush SubTitleBackground
        {
            get { return (Brush)GetValue(SubTitleBackgroundProperty); }
            set { SetValue(SubTitleBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SubTitleBackgroundProperty =
            DependencyProperty.Register("SubTitleBackground", typeof(Brush), typeof(MediaTransportControls), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(120, 0, 0, 0))));

        /// <summary>
        /// 字幕字体颜色
        /// </summary>
        public Brush SubTitleColor
        {
            get { return (Brush)GetValue(SubTitleColorProperty); }
            set { SetValue(SubTitleColorProperty, value); }
        }

        public static readonly DependencyProperty SubTitleColorProperty =
            DependencyProperty.Register("SubTitleColor", typeof(Brush), typeof(MediaTransportControls), new PropertyMetadata(new SolidColorBrush(Colors.White)));
    }
}
