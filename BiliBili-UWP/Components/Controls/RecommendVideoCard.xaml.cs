using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Models.UI;
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
    public sealed partial class RecommendVideoCard : UserControl
    {
        public RecommendVideoCard()
        {
            this.InitializeComponent();
        }

        public VideoRecommend Data
        {
            get { return (VideoRecommend)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(VideoRecommend), typeof(RecommendVideoCard), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null && e.NewValue is VideoRecommend data)
            {
                var instance = d as RecommendVideoCard;
                var card = instance.VideoCard;
                card.Cover = data.cover;
                card.Title = data.title;
                card.Tip = string.IsNullOrEmpty(data.args?.up_name) ? "--" : data.args.up_name;
                card.FirstSectionContent = data.cover_left_text_1;
                card.SecondSectionContent = data.cover_left_text_2;
                card.Duration = data.cover_right_text;
                card.RightBottomText = data.rcmd_reason ?? "";
            }
        }
    }
}
