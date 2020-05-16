using BiliBili_UWP.Models.Enums;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class BiliImage : UserControl
    {
        public BiliImage()
        {
            this.InitializeComponent();
            VisualStateManager.GoToState(this, "Loading", true);
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }
        private bool _isRing = false;
        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(BiliImage), new PropertyMetadata(null, new PropertyChangedCallback(Source_Changed)));

        private static void Source_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as BiliImage;
                if (instance._isRing)
                    VisualStateManager.GoToState(instance, "RingLoading", false);
                else
                    VisualStateManager.GoToState(instance, "Loading", false);
                if (e.NewValue == null)
                {
                    instance.DisplayImage.Source = null;
                    VisualStateManager.GoToState(instance, "Failed", false);
                }
                else if (e.NewValue is string url)
                {
                    var img = new BitmapImage() { DecodePixelType = DecodePixelType.Logical };
                    if (instance.DecodePixelWidth > 0)
                        img.DecodePixelWidth = instance.DecodePixelWidth;
                    instance.DisplayImage.Source = img;
                    if (string.IsNullOrEmpty(url))
                        instance.DisplayImage.Source = null;
                    else if (!url.StartsWith("http") && !url.StartsWith("ms-appx"))
                        img.UriSource = new Uri("ms-appx:///Assets/img_holder_color.png");
                    else
                        img.UriSource = new Uri(url);
                }
                else if (e.NewValue is ImageSource image)
                {
                    instance.DisplayImage.Source = image;
                }
            }
        }

        public BiliImageDisplayType DisplayType
        {
            get { return (BiliImageDisplayType)GetValue(DisplayTypeProperty); }
            set { SetValue(DisplayTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayTypeProperty =
            DependencyProperty.Register("DisplayType", typeof(BiliImageDisplayType), typeof(BiliImage), new PropertyMetadata(BiliImageDisplayType.None, new PropertyChangedCallback(DisplayType_Changed)));

        private static void DisplayType_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && e.NewValue is BiliImageDisplayType type)
            {
                var instance = d as BiliImage;
                instance._isRing = false;
                if (type == BiliImageDisplayType.Rect)
                {
                    instance.HolderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/img_holder_rect.png"));
                }
                else if (type == BiliImageDisplayType.Wide)
                {
                    instance.HolderImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/img_holder.png"));
                }
                else if (type == BiliImageDisplayType.Ring)
                {
                    instance._isRing = true;
                }
            }
        }

        public int DecodePixelWidth
        {
            get { return (int)GetValue(DecodePixelWidthProperty); }
            set { SetValue(DecodePixelWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DecodePixelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DecodePixelWidthProperty =
            DependencyProperty.Register("DecodePixelWidth", typeof(int), typeof(BiliImage), new PropertyMetadata(0));

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(BiliImage), new PropertyMetadata(Stretch.UniformToFill));





        private void DisplayImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Loaded", true);
        }

        private void DisplayImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Failed", true);
        }
    }
}
