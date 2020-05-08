using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

        public string _cardType = "";

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(DynamicContentBlock), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var data = e.NewValue;
                var instance = d as DynamicContentBlock;
                instance.MainContentControl.Content = data;
                if (data is VideoDynamic)
                {
                    instance._cardType = "video";
                    instance.MainContentControl.ContentTemplate = instance.VideoTemplate;
                }
                else if (data is ImageDynamic)
                {
                    instance._cardType = "image";
                    instance.MainContentControl.ContentTemplate = instance.ImageTemplate;
                }
                else if (data is DocumentDynamic)
                {
                    instance._cardType = "document";
                    instance.MainContentControl.ContentTemplate = instance.DocumentTemplate;
                }
                else if (data is RepostDynamic repost)
                {
                    instance._cardType = "repost";
                    repost.render_origin = App.BiliViewModel.DynamicContentConvert(repost.item.orig_type, repost.origin);
                    if (repost.item.orig_type == 512)
                    {
                        repost.origin_user.info = new RepostDynamic.OriginUserInfo();
                        var anime = repost.render_origin as AnimeDynamic;
                        repost.origin_user.info.face = anime.season.cover;
                        repost.origin_user.info.uname = anime.season.title;
                        repost.render_origin_content = anime.new_desc;
                    }
                    else if (repost.item.orig_type == 4303)
                    {
                        repost.origin_user.info = new RepostDynamic.OriginUserInfo();
                        var da = repost.render_origin as CourseDynamic;
                        repost.origin_user.info.face = da.up_info.avatar;
                        repost.origin_user.info.uname = da.up_info.name;
                    }
                    else if (repost.item.orig_type == 4)
                        repost.render_origin_content = (repost.render_origin as TextDynamic).content;
                    else if (repost.item.orig_type == 2)
                        repost.render_origin_content = Regex.Replace((repost.render_origin as ImageDynamic).description, @"#(.*?)#", "").Trim();
                    instance.MainContentControl.ContentTemplate = instance.RepostTemplate;
                }
                else if (data is AnimeDynamic)
                {
                    instance._cardType = "anime";
                    instance.MainContentControl.ContentTemplate = instance.AnimeTemplate;
                }
                else if (data is TextDynamic)
                {
                    instance._cardType = "text";
                    instance.MainContentControl.ContentTemplate = instance.TextTemplate;
                }
                else if (data is ShortVideoDynamic)
                {
                    instance._cardType = "shortVideo";
                    instance.MainContentControl.ContentTemplate = instance.ShortVideoTemplate;
                }
                else if (data is WebDynamic)
                {
                    instance._cardType = "web";
                    instance.MainContentControl.ContentTemplate = instance.WebTemplate;
                }
                else if (data is CourseDynamic)
                {
                    instance._cardType = "course";
                    instance.MainContentControl.ContentTemplate = instance.CourseTemplate;
                }
                else if (data is MusicDynamic)
                {
                    instance._cardType = "music";
                    instance.MainContentControl.ContentTemplate = instance.MusicTemplate;
                }
                else if (data is LiveDynamic)
                {
                    instance._cardType = "live";
                    instance.MainContentControl.ContentTemplate = instance.LiveTemplate;
                }
            }
        }

        private void MainContentControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_cardType == "video")
            {
                var data = Data as VideoDynamic;
                if (string.IsNullOrEmpty(data.redirect_url))
                    App.AppViewModel.PlayVideo(data.aid, sender, StaticString.SIGN_DYNAMIC);
                else
                {
                    var result = BiliTool.GetResultFromUri(data.redirect_url);
                    if (result.Type == BiliBili_Lib.Enums.UriType.Bangumi)
                    {
                        App.AppViewModel.PlayBangumi(Convert.ToInt32(result.Param), sender, true);
                    }
                }
            }
            else if (_cardType == "web")
            {
                var item = Data as WebDynamic;
                App.AppViewModel.ShowWebPopup(item.sketch.title, item.sketch.target_url);
            }
            else if (_cardType == "anime")
            {
                var item = Data as AnimeDynamic;
                App.AppViewModel.PlayBangumi(item.episode_id, sender, true);
            }
            else if (_cardType == "document")
            {
                var item = Data as DocumentDynamic;
                App.AppViewModel.ShowDoucmentPopup(item.title, item.id);
            }
        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var images = (Data as ImageDynamic).pictures.Select(p => p.img_src).ToList();
            var dialog = new ShowImageDialog(images);
            await dialog.ShowAsync();
        }

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            var data = Data as RepostDynamic;
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.Account.DetailPage), data.origin_user.info.uid);
        }
    }
}
