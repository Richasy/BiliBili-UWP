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
using Windows.System;
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

        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public bool EnableConnectAnimation
        {
            get { return (bool)GetValue(EnableConnectAnimationProperty); }
            set { SetValue(EnableConnectAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableConnectAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableConnectAnimationProperty =
            DependencyProperty.Register("EnableConnectAnimation", typeof(bool), typeof(TopicCard), new PropertyMetadata(true));

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
                if (data is VideoDynamic v)
                {
                    instance._cardType = "video";
                    instance.MainContentControl.ContentTemplate = instance.VideoTemplate;
                    instance.Url = $"https://www.bilibili.com/video/av{v.aid}";
                    instance.Title = v.title;
                    instance.ImageUrl = v.pic;
                }
                else if (data is ImageDynamic i)
                {
                    instance._cardType = "image";
                    double height = 250.0 / i.render_count;
                    i.pictures.ForEach(p => p.img_height = height);
                    instance.MainContentControl.ContentTemplate = instance.ImageTemplate;
                    instance.Url = string.Empty;
                    instance.Title = i.title;
                    instance.ImageUrl = i.pictures.FirstOrDefault()?.img_src;
                }
                else if (data is DocumentDynamic doc)
                {
                    instance._cardType = "document";
                    instance.MainContentControl.ContentTemplate = instance.DocumentTemplate;
                    instance.Url = $"https://www.bilibili.com/read/cv{doc.id}";
                    instance.Title = doc.title;
                    if(!string.IsNullOrEmpty(doc.banner_url))
                        instance.ImageUrl = doc.banner_url;
                }
                else if (data is RepostDynamic repost)
                {
                    instance._cardType = "repost";
                    repost.render_origin = App.BiliViewModel.DynamicContentConvert(repost.item.orig_type, repost.origin);
                    if (repost.item.orig_type == 512 || repost.item.orig_type==4101)
                    {
                        repost.origin_user.info = new SlimUserInfo();
                        var anime = repost.render_origin as AnimeDynamic;
                        repost.origin_user.info.face = anime.season.cover;
                        repost.origin_user.info.uname = anime.season.title;
                        repost.render_origin_content = anime.new_desc;
                        repost.origin_user.info.uid = anime.season.season_id;
                    }
                    else if (repost.item.orig_type == 4303)
                    {
                        repost.origin_user.info = new SlimUserInfo();
                        var da = repost.render_origin as CourseDynamic;
                        repost.origin_user.info.face = da.up_info.avatar;
                        repost.origin_user.info.uname = da.up_info.name;
                        repost.origin_user.info.uid = da.up_id;
                    }
                    else if (repost.item.orig_type == 4)
                        repost.render_origin_content = (repost.render_origin as TextDynamic).content;
                    else if (repost.item.orig_type == 2)
                        repost.render_origin_content = Regex.Replace((repost.render_origin as ImageDynamic).description, @"#(.*?)#", "").Trim();
                    instance.MainContentControl.ContentTemplate = instance.RepostTemplate;
                    instance.Url = string.Empty;
                }
                else if (data is AnimeDynamic ani)
                {
                    instance._cardType = "anime";
                    instance.MainContentControl.ContentTemplate = instance.AnimeTemplate;
                    instance.Url = ani.url;
                    instance.Title = ani.show_title;
                    instance.ImageUrl = ani.cover;
                }
                else if (data is TextDynamic)
                {
                    instance._cardType = "text";
                    instance.MainContentControl.ContentTemplate = instance.TextTemplate;
                }
                else if (data is ShortVideoDynamic s)
                {
                    instance._cardType = "shortVideo";
                    instance.MainContentControl.ContentTemplate = instance.ShortVideoTemplate;
                    instance.Title = s.item.description;
                    instance.Url = s.item.video_playurl;
                    instance.ImageUrl = s.item.cover.unclipped;
                }
                else if (data is WebDynamic w)
                {
                    instance._cardType = "web";
                    instance.MainContentControl.ContentTemplate = instance.WebTemplate;
                    instance.Title = w.sketch.title;
                    instance.Url = w.sketch.target_url;
                    instance.ImageUrl = w.sketch.cover_url;
                }
                else if (data is CourseDynamic c)
                {
                    instance._cardType = "course";
                    instance.MainContentControl.ContentTemplate = instance.CourseTemplate;
                    instance.Title = c.title;
                    instance.Url = c.url;
                    instance.ImageUrl = c.cover;
                }
                else if (data is MusicDynamic m)
                {
                    instance._cardType = "music";
                    instance.MainContentControl.ContentTemplate = instance.MusicTemplate;
                    instance.Url = string.Empty;
                    instance.Title = m.title;
                    instance.ImageUrl = m.cover;
                }
                else if (data is LiveDynamic l)
                {
                    instance._cardType = "live";
                    instance.MainContentControl.ContentTemplate = instance.LiveTemplate;
                    instance.Url = $"https://live.bilibili.com/{l.roomid}";
                    instance.Title = l.title;
                    instance.ImageUrl = l.cover;
                }
                instance.MainContentControl.Content = data;
            }
        }

        private void MainContentControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (_cardType == "video")
            {
                var data = Data as VideoDynamic;
                object ele = EnableConnectAnimation ? sender : null;
                if (string.IsNullOrEmpty(data.redirect_url))
                    App.AppViewModel.PlayVideo(data.aid, ele, StaticString.SIGN_DYNAMIC);
                else
                {
                    var result = BiliTool.GetResultFromUri(data.redirect_url);
                    if (result.Type == BiliBili_Lib.Enums.UriType.Bangumi)
                    {
                        App.AppViewModel.PlayBangumi(Convert.ToInt32(result.Param), ele, true);
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
            else if (_cardType == "live")
            {
                var item = Data as LiveDynamic;
                App.AppViewModel.ShowWebPopup(item.title, $"https://live.bilibili.com/{item.roomid}");
            }
            else if (_cardType == "music")
            {
                var item = Data as MusicDynamic;
                App.AppViewModel.ShowWebPopup(item.title, $"https://www.bilibili.com/audio/au{item.id}?type=7");
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
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), data.origin_user.info.uid);
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
