using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
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
    public sealed partial class TopicCard : UserControl
    {
        //8:视频，64:专栏，2:图片，4:纯文本
        public TopicCard()
        {
            this.InitializeComponent();
        }

        public Topic Data
        {
            get { return (Topic)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Topic), typeof(TopicCard), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Topic data)
            {
                var instance = d as TopicCard;
                if (!instance.IsUsePhase)
                {
                    instance.HeaderInit(data);
                    instance.ButtonInit(data);
                    instance.TagInit(data);
                    instance.BodyInit(data);
                }
            }
        }

        public bool EnableConnectAnimation
        {
            get { return (bool)GetValue(EnableConnectAnimationProperty); }
            set { SetValue(EnableConnectAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableConnectAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableConnectAnimationProperty =
            DependencyProperty.Register("EnableConnectAnimation", typeof(bool), typeof(TopicCard), new PropertyMetadata(true));



        public bool IsUsePhase
        {
            get { return (bool)GetValue(IsUsePhaseProperty); }
            set { SetValue(IsUsePhaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUsePhase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUsePhaseProperty =
            DependencyProperty.Register("IsUsePhase", typeof(bool), typeof(TopicCard), new PropertyMetadata(true));

        private void HeaderInit(Topic data)
        {
            if (data.desc.user_profile != null)
            {
                HeaderContainer.Visibility = Visibility.Visible;
                UserAvatar.ProfilePicture = new BitmapImage(new Uri(data.desc.user_profile.info.face)) { DecodePixelWidth = 40 };
                UserNameBlock.Text = data.desc.user_profile.info.uname;
                string tip = AppTool.GetReadDateString(data.desc.timestamp);
                if (data.display != null && !string.IsNullOrEmpty(data.display.usr_action_txt))
                    tip += " · " + data.display.usr_action_txt;
                if (data.desc.view != 0)
                    tip += " · " + AppTool.GetNumberAbbreviation(data.desc.view) + "次查看";
                TipBlock.Text = tip;
            }
            else
            {
                HeaderContainer.Visibility = Visibility.Collapsed;
            }
            var me = App.BiliViewModel._client.Account.Me;
            if (data.desc.type == 512 || data.desc.type == 4101 || data.display == null)
                FollowButton.Visibility = Visibility.Collapsed;
            else if ((me != null && me.mid == data.desc.uid) || data.display.relation.is_follow == 1)
                FollowButton.Visibility = Visibility.Collapsed;
            else
                FollowButton.Visibility = Visibility.Visible;
        }

        private void ButtonInit(Topic data)
        {
            LikeBlock.Text = AppTool.GetNumberAbbreviation(data.desc.like);
            LikeIcon.Foreground = data.desc.is_liked == 1 ? UIHelper.GetThemeBrush(ColorType.PrimaryColor) : UIHelper.GetThemeBrush(ColorType.TipTextColor);
            RepostBlock.Text = data.desc.repost == 0 ? "转发" : AppTool.GetNumberAbbreviation(data.desc.repost);
        }

        private void TagInit(Topic data)
        {
            TagList.Visibility = Visibility.Visible;
            if (data.display != null && data.display.topic_info != null && data.display.topic_info.topic_details != null && data.display.topic_info.topic_details.Count > 0)
                TagList.ItemsSource = data.display.topic_info.topic_details;
            else
                TagList.Visibility = Visibility.Collapsed;
        }

        private void BodyInit(Topic data)
        {
            MainDisplay.Visibility = Visibility.Visible;
            MoreButton.Visibility = Visibility.Collapsed;
            if (data.display != null && data.display.emoji_info != null && data.display.emoji_info.emoji_details.Count > 0)
            {
                var dict = new Dictionary<string, Emote>();
                foreach (var item in data.display.emoji_info.emoji_details)
                {
                    dict.Add(item.text, item);
                }
                DescriptionBlock.EmoteSource = dict;
            }
            if (data.desc.type == 8)
            {
                //视频
                var info = JsonConvert.DeserializeObject<VideoDynamic>(data.card);
                info.dynamic = Uri.UnescapeDataString(info.dynamic);
                if (!string.IsNullOrEmpty(info.dynamic))
                    DescriptionBlock.Text = Regex.Replace(info.dynamic, @"#(.*?)#", "").Trim();
                DescriptionBlock.Visibility = string.IsNullOrEmpty(DescriptionBlock.Text) ? Visibility.Collapsed : Visibility.Visible;
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.stat.reply);
                MoreButton.Visibility = Visibility.Visible;
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 1)
            {
                //转发
                var info = JsonConvert.DeserializeObject<RepostDynamic>(data.card);
                DescriptionBlock.Visibility = Visibility.Collapsed;
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.item.reply);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 2)
            {
                //图片
                var temp = JObject.Parse(data.card);
                var info = JsonConvert.DeserializeObject<ImageDynamic>(temp["item"].ToString());
                info.description = Uri.UnescapeDataString(info.description);
                DescriptionBlock.Text = Regex.Replace(info.description, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.reply);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 4)
            {
                //纯文本
                var temp = JObject.Parse(data.card);
                var info = JsonConvert.DeserializeObject<TextDynamic>(temp["item"].ToString());
                if (!string.IsNullOrEmpty(info.content))
                    DescriptionBlock.Text = Regex.Replace(info.content, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.reply);
                MainDisplay.Visibility = Visibility.Collapsed;
            }
            else if (data.desc.type == 64)
            {
                //专栏
                var info = JsonConvert.DeserializeObject<DocumentDynamic>(data.card);
                if (!string.IsNullOrEmpty(info.dynamic))
                    DescriptionBlock.Text = Regex.Replace(info.dynamic, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.stats.reply);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 512 || data.desc.type == 4101)
            {
                //动漫
                var info = JsonConvert.DeserializeObject<AnimeDynamic>(data.card);
                HeaderContainer.Visibility = Visibility.Visible;
                UserAvatar.ProfilePicture = new BitmapImage(new Uri(info.season.square_cover)) { DecodePixelWidth = 40 };
                UserNameBlock.Text = info.season.title;
                string tip = AppTool.GetReadDateString(data.desc.timestamp);
                tip += " · " + "更新了";
                TipBlock.Text = tip;
                DescriptionBlock.Visibility = Visibility.Collapsed;
                FollowButton.Visibility = Visibility.Collapsed;
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.stat.reply);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 16)
            {
                //小视频
                var info = JsonConvert.DeserializeObject<ShortVideoDynamic>(data.card);
                if (!string.IsNullOrEmpty(info.item.description))
                    DescriptionBlock.Text = Regex.Replace(info.item.description, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.item.reply);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 2048)
            {
                //网页
                var info = JsonConvert.DeserializeObject<WebDynamic>(data.card);
                if (!string.IsNullOrEmpty(info.vest.content))
                    DescriptionBlock.Text = Regex.Replace(info.vest.content, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(data.desc.comment);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 4303)
            {
                //视频单
                var info = JsonConvert.DeserializeObject<CourseDynamic>(data.card);
                if (!string.IsNullOrEmpty(info.new_ep.title))
                    DescriptionBlock.Text = Regex.Replace(info.new_ep.title, @"#(.*?)#", "").Trim();
                CommentBlock.Text = data.desc.comment == 0 ? "" : AppTool.GetNumberAbbreviation(data.desc.comment);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 256)
            {
                //音频
                var info = JsonConvert.DeserializeObject<MusicDynamic>(data.card);
                if (!string.IsNullOrEmpty(info.intro))
                    DescriptionBlock.Text = Regex.Replace(info.intro, @"#(.*?)#", "").Trim();
                CommentBlock.Text = AppTool.GetNumberAbbreviation(info.replyCnt);
                MainDisplay.Data = info;
            }
            else if (data.desc.type == 4200)
            {
                //直播
                var info = JsonConvert.DeserializeObject<LiveDynamic>(data.card);
                DescriptionBlock.Visibility = Visibility.Collapsed;
                CommentBlock.Text = "";
                MainDisplay.Data = info;
            }
            else
            {
                string yo = "";
            }
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", Data.desc.rid);
            string type = "11";
            if (MainDisplay._cardType == "video")
                type = "1";
            else if (MainDisplay._cardType == "anime")
            {
                param["oid"] = (MainDisplay.Data as AnimeDynamic).aid.ToString();
                type = "1";
            }
            else if (MainDisplay._cardType == "document")
                type = "12";
            else if (MainDisplay._cardType == "repost" || MainDisplay._cardType == "web" || Data.desc.type == 4)
            {
                param["oid"] = Data.desc.dynamic_id_str;
                type = "17";
            }
            else if (MainDisplay._cardType == "shortVideo")
                type = "5";
            else if (MainDisplay._cardType == "music")
            {
                var music = MainDisplay.Data as MusicDynamic;
                param["oid"] = music.id.ToString();
                type = "14";
            }
            param.Add("type", type);
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.ReplyPage), param);
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                bool isLike = Data.desc.is_liked == 1;
                bool result = await App.BiliViewModel._client.Topic.SetDynamicLikeStatus(!isLike, Data.desc.dynamic_id, Data.desc.rid, App.BiliViewModel._client.Account.Me.mid.ToString());
                if (result)
                {
                    Data.desc.is_liked = isLike ? 0 : 1;
                    Data.desc.like = isLike ? Data.desc.like - 1 : Data.desc.like + 1;
                    LikeBlock.Text = AppTool.GetNumberAbbreviation(Data.desc.like);
                    LikeIcon.Foreground = Data.desc.is_liked == 1 ? UIHelper.GetThemeBrush(ColorType.PrimaryColor) : UIHelper.GetThemeBrush(ColorType.TipTextColor);
                }
                else
                    new TipPopup("点赞失败").ShowError();
            }
        }

        private async void FollowButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.BiliViewModel.CheckAccoutStatus())
            {
                FollowButton.IsLoading = true;
                int followId = Data.desc.user_profile.info.uid;
                bool result = await App.BiliViewModel._client.Account.FollowUser(followId);
                if (result)
                {
                    Data.display.relation.is_follow = 1;
                    FollowButton.Visibility = Visibility.Collapsed;
                    new TipPopup("已关注").ShowMessage();
                }
                else
                {
                    new TipPopup("关注失败，您可能已经关注该用户了").ShowError();
                }
                FollowButton.IsLoading = false;
            }
        }

        private void Tag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as TopicDetail;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Channel.TagDetailPage), data.topic_id);
        }

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), Data.desc.uid);
        }

        private void MainDisplay_ImageTapped(object sender, EventArgs e)
        {
            var img = MainDisplay.Data as ImageDynamic;
            App.AppViewModel.ShowDynamicDetailPopup(Data.desc.user_profile.info, img.description, img, Data.desc.rid_str);
        }

        private void MainDisplay_DocumentTapped(object sender, EventArgs e)
        {
            var doc = MainDisplay.Data as DocumentDynamic;
            string content = string.IsNullOrEmpty(DescriptionBlock.Text) ? doc.title : DescriptionBlock.Text;
            App.AppViewModel.ShowDynamicDetailPopup(Data.desc.user_profile.info, content, doc, Data.desc.rid_str);
        }
        public void RenderContainer(ContainerContentChangingEventArgs args)
        {
            HeaderContainer.Opacity = 0;
            DescriptionBlock.Opacity = 0;
            TagList.Opacity = 0;
            MainDisplay.Opacity = 0;
            ButtonContainer.Opacity = 0;
            if (Data == null)
                Data = args.Item as Topic;

            args.RegisterUpdateCallback(RenderHeader);
        }

        private void RenderHeader(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            HeaderContainer.Opacity = 1;
            HeaderInit(args.Item as Topic);
            args.RegisterUpdateCallback(RenderButton);
        }

        private void RenderButton(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            ButtonInit(args.Item as Topic);
            ButtonContainer.Opacity = 1;

            args.RegisterUpdateCallback(RenderTag);
        }

        private void RenderTag(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            TagInit(args.Item as Topic);
            TagList.Opacity = 1;

            args.RegisterUpdateCallback(RenderBody);
        }

        private void RenderBody(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            BodyInit(args.Item as Topic);
            DescriptionBlock.Opacity = 1;
            MainDisplay.Opacity = 1;
        }

        private void ShareDynamicButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.BiliViewModel.CheckAccoutStatus())
                return;
            string content = DescriptionBlock.Text;
            if (string.IsNullOrEmpty(content))
            {
                if (MainDisplay.Data is RepostDynamic rep)
                    content = rep.item.content;
                else if (MainDisplay.Data is AnimeDynamic ani)
                    content = ani.show_title;
                else
                    content = $"{Data.desc.user_profile.info.uname}的动态";
            }
            App.AppViewModel.ShowRepostPopup(content, Data);
        }

        private void ShareDataButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            if (!string.IsNullOrEmpty(MainDisplay.Title))
                request.Data.Properties.Title = MainDisplay.Title;
            else if (Data.desc.user_profile != null && Data.desc.user_profile.info != null)
                request.Data.Properties.Title = $"{Data.desc.user_profile.info.uname}的动态";
            request.Data.Properties.Description = DescriptionBlock.Text ?? "";
            if (!string.IsNullOrEmpty(DescriptionBlock.Text))
                request.Data.SetText(DescriptionBlock.Text);
            else if (MainDisplay.Data is RepostDynamic rep)
                request.Data.SetText(rep.item.content);
            if (!string.IsNullOrEmpty(MainDisplay.Url))
                request.Data.SetWebLink(new Uri(MainDisplay.Url));
            else
                request.Data.SetWebLink(new Uri($"https://t.bilibili.com/{Data.desc.dynamic_id_str}?tab=2"));
            if (!string.IsNullOrWhiteSpace(MainDisplay.ImageUrl))
                request.Data.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(MainDisplay.ImageUrl)));
        }

        private async void LaterViewButton_Click(object sender, RoutedEventArgs e)
        {
            var data = MainDisplay.Data as VideoDynamic;
            await App.BiliViewModel.AddViewLater(sender, data.aid);
        }
    }
}
