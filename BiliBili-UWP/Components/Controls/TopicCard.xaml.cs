using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
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
                instance.UserAvatar.ProfilePicture = new BitmapImage(new Uri(data.desc.user_profile.info.face)) { DecodePixelWidth = 40 };
                instance.UserNameBlock.Text = data.desc.user_profile.info.uname;
                string tip = AppTool.GetReadDateString(data.desc.timestamp);
                if (!string.IsNullOrEmpty(data.display.usr_action_txt))
                    tip += " · " + data.display.usr_action_txt;
                if (data.desc.view != 0)
                    tip += " · " + AppTool.GetNumberAbbreviation(data.desc.view) + "次查看";
                instance.TipBlock.Text = tip;
                var me = App.BiliViewModel._client.Account.Me;
                if ((me != null && me.mid == data.desc.uid) || data.display.relation.is_follow == 1)
                    instance.FollowButton.Visibility = Visibility.Collapsed;
                else
                    instance.FollowButton.Visibility = Visibility.Visible;
                instance.LikeBlock.Text = AppTool.GetNumberAbbreviation(data.desc.like);
                instance.LikeIcon.Foreground = data.desc.is_liked==1 ? UIHelper.GetThemeBrush(ColorType.PrimaryColor) : UIHelper.GetThemeBrush(ColorType.TipTextColor);
                instance.RepostBlock.Text = data.desc.repost == 0 ? "转发" : AppTool.GetNumberAbbreviation(data.desc.repost);
                instance.TagList.Visibility = Visibility.Visible;
                if (data.display.topic_info.topic_details != null && data.display.topic_info.topic_details.Count > 0)
                    instance.TagList.ItemsSource = data.display.topic_info.topic_details;
                else
                    instance.TagList.Visibility = Visibility.Collapsed;
                instance.MainDisplay.Visibility = Visibility.Visible;
                if (data.desc.type == 8)
                {
                    //视频
                    var info = JsonConvert.DeserializeObject<AV>(data.card);
                    info.dynamic = Uri.UnescapeDataString(info.dynamic);
                    instance.DescriptionBlock.Text = Regex.Replace(info.dynamic, @"#(.*?)#", "").Trim();
                    instance.CommentBlock.Text = AppTool.GetNumberAbbreviation(info.stat.reply);
                    instance.MainDisplay.Data = info;
                }
                else if (data.desc.type == 2)
                {
                    //图片
                    var temp = JObject.Parse(data.card);
                    var info = JsonConvert.DeserializeObject<ImageDynamic>(temp["item"].ToString());
                    info.description = Uri.UnescapeDataString(info.description);
                    instance.DescriptionBlock.Text = Regex.Replace(info.description, @"#(.*?)#", "").Trim();
                    instance.CommentBlock.Text = AppTool.GetNumberAbbreviation(info.reply);
                    instance.MainDisplay.Data = info;
                }
                else if (data.desc.type == 4)
                {
                    //纯文本
                    var temp = JObject.Parse(data.card);
                    var info = JsonConvert.DeserializeObject<TextDynamic>(temp["item"].ToString());
                    instance.DescriptionBlock.Text = Regex.Replace(info.content, @"#(.*?)#", "").Trim();
                    instance.CommentBlock.Text = AppTool.GetNumberAbbreviation(info.reply);
                    instance.MainDisplay.Visibility = Visibility.Collapsed;
                }
                else if (data.desc.type == 64)
                {
                    var info = JsonConvert.DeserializeObject<DocumentDynamic>(data.card);
                    instance.DescriptionBlock.Text = Regex.Replace(info.dynamic, @"#(.*?)#", "").Trim();
                    instance.CommentBlock.Text = AppTool.GetNumberAbbreviation(info.stats.reply);
                    instance.MainDisplay.Data = info;
                }
            }
        }

        private void RepostButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {

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
            App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.Channel.TagDetailPage), data.topic_id);
        }
    }
}
