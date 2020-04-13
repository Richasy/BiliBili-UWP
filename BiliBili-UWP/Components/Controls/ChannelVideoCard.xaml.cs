using BiliBili_Lib.Models.BiliBili;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class ChannelVideoCard : UserControl
    {
        public ChannelVideoCard()
        {
            this.InitializeComponent();
        }
        public VideoChannel Data
        {
            get { return (VideoChannel)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(VideoChannel), typeof(ChannelVideoCard), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is VideoChannel data)
            {
                var instance = d as ChannelVideoCard;
                instance.VideoCover.Source = new BitmapImage(new Uri(data.cover));
                instance.TitleBlock.Text = data.title;
                ToolTipService.SetToolTip(instance.TitleBlock, data.title);
                instance.PlayCountBlock.Text = data.cover_left_text_1;
                instance.DanmuCountBlock.Text = data.cover_left_text_2;
                instance.DurationBlock.Text = data.cover_left_text_3;
            }
        }
    }
}
