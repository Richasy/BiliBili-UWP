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

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class CoverVideoCard : UserControl
    {
        public CoverVideoCard()
        {
            this.InitializeComponent();
        }
        public int VideoId
        {
            get { return (int)GetValue(VideoIdProperty); }
            set { SetValue(VideoIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VideoId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VideoIdProperty =
            DependencyProperty.Register("VideoId", typeof(int), typeof(CoverVideoCard), new PropertyMetadata(0));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DefaultVideoCard), new PropertyMetadata(""));

        public string Cover
        {
            get { return (string)GetValue(CoverProperty); }
            set { SetValue(CoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoverProperty =
            DependencyProperty.Register("Cover", typeof(string), typeof(CoverVideoCard), new PropertyMetadata(""));

        public string Duration
        {
            get { return (string)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(string), typeof(CoverVideoCard), new PropertyMetadata(""));

        public int DecodePixelWidth
        {
            get { return (int)GetValue(DecodePixelWidthProperty); }
            set { SetValue(DecodePixelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelWidthProperty =
            DependencyProperty.Register("DecodePixelWidth", typeof(int), typeof(CoverVideoCard), new PropertyMetadata(350));

        public string RightBottomText
        {
            get { return (string)GetValue(RightBottomTextProperty); }
            set { SetValue(RightBottomTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightBottomText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightBottomTextProperty =
            DependencyProperty.Register("RightBottomText", typeof(string), typeof(CoverVideoCard), new PropertyMetadata(""));

        public string RightTopText
        {
            get { return (string)GetValue(RightTopTextProperty); }
            set { SetValue(RightTopTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightTopText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightTopTextProperty =
            DependencyProperty.Register("RightTopText", typeof(string), typeof(CoverVideoCard), new PropertyMetadata(""));

        public string RightTopImage
        {
            get { return (string)GetValue(RightTopImageProperty); }
            set { SetValue(RightTopImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightTopImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightTopImageProperty =
            DependencyProperty.Register("RightTopImage", typeof(string), typeof(CoverVideoCard), new PropertyMetadata(""));
        public Visibility ExtraButtonVisbility
        {
            get { return (Visibility)GetValue(ExtraButtonVisbilityProperty); }
            set { SetValue(ExtraButtonVisbilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtraButtonVisbility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraButtonVisbilityProperty =
            DependencyProperty.Register("ExtraButtonVisbility", typeof(Visibility), typeof(CoverVideoCard), new PropertyMetadata(Visibility.Visible));

        public FlyoutBase ExtraFlyout
        {
            get { return (FlyoutBase)GetValue(ExtraFlyoutProperty); }
            set { SetValue(ExtraFlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExtraFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExtraFlyoutProperty =
            DependencyProperty.Register("ExtraFlyout", typeof(FlyoutBase), typeof(CoverVideoCard), new PropertyMetadata(null, new PropertyChangedCallback(ExtraFlyout_Changed)));

        private static void ExtraFlyout_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is FlyoutBase flyout)
            {
                var instance = d as CoverVideoCard;
                instance.ExtraButton.Flyout = flyout;
            }
        }

        private async void LaterViewButton_Click(object sender, RoutedEventArgs e)
        {
            await App.BiliViewModel.AddViewLater(sender, Convert.ToInt32(VideoId));
        }
    }
}
