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

        public event EventHandler ImageTapped;
        public event EventHandler DocumentTapped;

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
                        repost.origin_user.info = new SlimUserInfo();
                        var anime = repost.render_origin as AnimeDynamic;
                        repost.origin_user.info.face = anime.season.cover;
                        repost.origin_user.info.uname = anime.season.title;
                        repost.render_origin_content = anime.new_desc;
                    }
                    else if (repost.item.orig_type == 4303)
                    {
                        repost.origin_user.info = new SlimUserInfo();
                        var da = repost.render_origin as CourseDynamic;
                        repost.origin_user.info.face = da.up_info.avatar;
                        repost.origin_user.info.uname = da.up_info.name;
                        repost.origin_user.info.uid = da.up_id;
                    }
                    else if (repost.item.orig_type == 4101)
                    {
                        repost.origin_user.info = new SlimUserInfo();
                        var da = repost.render_origin as SeriesDynamic;
                        repost.origin_user.info.face = da.season.square_cover;
                        repost.origin_user.info.uname = da.season.title;
                        repost.origin_user.info.uid = da.season.season_id;
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
                else if (data is SeriesDynamic)
                {
                    instance._cardType = "series";
                    instance.MainContentControl.ContentTemplate = instance.SeriesTemplate;
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
                DocumentTapped?.Invoke(this, EventArgs.Empty);
            }
            else if (_cardType == "series")
            {
                var item = Data as SeriesDynamic;
                App.AppViewModel.PlayBangumi(item.episode_id, sender, true);
            }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ImageTapped?.Invoke(this, EventArgs.Empty);
        }

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            var data = Data as RepostDynamic;
            if (data.item.orig_type != 4101)
                App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.Account.DetailPage), data.origin_user.info.uid);
            else
                App.AppViewModel.PlayBangumi(data.origin_user.info.uid);
        }
        private void MainDisplay_ImageTapped(object sender, EventArgs e)
        {
            var MainDisplay = sender as DynamicContentBlock;
            var img = MainDisplay.Data as ImageDynamic;
            var rep = Data as RepostDynamic;
            App.AppViewModel.ShowDynamicDetailPopup(rep.origin_user.info, img.description, img, img.id.ToString());
        }

        private void MainDisplay_DocumentTapped(object sender, EventArgs e)
        {
            var MainDisplay = sender as DynamicContentBlock;
            var doc = MainDisplay.Data as DocumentDynamic;
            var rep = Data as RepostDynamic;
            string content = string.IsNullOrEmpty(rep.render_origin_content) ? doc.title : rep.render_origin_content;
            App.AppViewModel.ShowDynamicDetailPopup(rep.origin_user.info, content, doc, doc.id.ToString());
        }
    }
}
