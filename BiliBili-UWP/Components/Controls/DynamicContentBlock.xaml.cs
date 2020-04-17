using BiliBili_Lib.Models.BiliBili;
using BiliBili_UWP.Dialogs;
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
using static BiliBili_Lib.Models.BiliBili.ImageDynamic;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class DynamicContentBlock : UserControl
    {
        public DynamicContentBlock()
        {
            this.InitializeComponent();
        }

        private string _cardType = "";

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(DynamicContentBlock), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                var data = e.NewValue;
                var instance = d as DynamicContentBlock;
                instance.MainContentControl.Content = data;
                if (data is AV)
                {
                    instance._cardType = "video";
                    instance.MainContentControl.ContentTemplate = instance.VideoTemplate;
                }
                else if (data is ImageDynamic)
                {
                    instance._cardType = "image";
                    var temp = data as ImageDynamic;
                    double width = temp.pictures.Count < 3 ? 290 / temp.pictures_count : 100;
                    temp.pictures.ForEach(p => { p.render_width = Convert.ToInt32(width); });
                    instance.MainContentControl.ContentTemplate = instance.ImageTemplate;
                }
                else if (data is DocumentDynamic)
                {
                    instance._cardType = "document";
                    instance.MainContentControl.ContentTemplate = instance.DocumentTemplate;
                }
            }
        }

        private void MainContentControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_cardType == "video")
                App.AppViewModel.PlayVideo((Data as AV).aid,sender);
        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as Picture;
            var dialog = new ShowImageDialog(data.img_src);
            await dialog.ShowAsync();
        }
    }
}
