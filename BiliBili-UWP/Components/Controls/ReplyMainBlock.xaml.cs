using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class ReplyMainBlock : UserControl
    {
        public ReplyMainBlock()
        {
            this.InitializeComponent();
        }

        public event EventHandler<Reply> CommentButtonClick;

        public Reply Data
        {
            get { return (Reply)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(Reply), typeof(ReplyMainBlock), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is Reply data)
            {
                var instance = d as ReplyMainBlock;
                instance.UserAvatar.Source = new BitmapImage(new Uri(data.member.avatar + "@50w.jpg"));
                instance.UserNameBlock.Text = data.member.uname;
                instance.LevelImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Level/level_{data.member.level_info.current_level}.png"));
                instance.TimeBlock.Text = AppTool.GetReadDateString(data.ctime);
                instance.ContentBlock.EmoteSource = data.content.emote;
                instance.ContentBlock.Text = data.content.message;
                instance.LikeBlock.Text = AppTool.GetNumberAbbreviation(data.like);
                instance.LikeIcon.Foreground = data.action==1 ? UIHelper.GetThemeBrush(Models.Enums.ColorType.PrimaryColor) : UIHelper.GetThemeBrush(Models.Enums.ColorType.TipTextColor);
                if (data.rcount > 0 && instance.SubReplyVisibility==Visibility.Visible)
                {
                    instance.SubReplyContainer.Visibility = Visibility.Visible;
                    instance.SubReplyItemsControl.ItemsSource = data.replies;
                    instance.TotalReplyCount.Text = $"共{AppTool.GetNumberAbbreviation(data.rcount)}条回复";
                    instance.MoreReplyButton.Visibility = data.rcount > 3 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                    instance.SubReplyContainer.Visibility = Visibility.Collapsed;
            }
        }

        private async void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            bool isLike = !(Data.action == 1);
            bool result = await App.BiliViewModel._client.LikeReplyAsync(isLike, Data.oid.ToString(), Data.rpid.ToString(), Data.type.ToString());
            if (result)
            {
                Data.action = Data.action == 0 ? 1 : 0;
                LikeIcon.Foreground = Data.action==1 ? UIHelper.GetThemeBrush(Models.Enums.ColorType.PrimaryColor) : UIHelper.GetThemeBrush(Models.Enums.ColorType.TipTextColor);
                if (isLike)
                    Data.like += 1;
                else
                    Data.like -= 1;
                LikeBlock.Text = AppTool.GetNumberAbbreviation(Data.like);
            }
            else
                new TipPopup("操作失败").ShowError();
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            CommentButtonClick?.Invoke(this, Data);
        }

        public Visibility TopBadgeVisibility
        {
            get { return (Visibility)GetValue(TopBadgeVisibilityProperty); }
            set { SetValue(TopBadgeVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopBadgeVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopBadgeVisibilityProperty =
            DependencyProperty.Register("TopBadgeVisibility", typeof(Visibility), typeof(ReplyMainBlock), new PropertyMetadata(Visibility.Collapsed));

        private void MoreReplyButton_Click(object sender, RoutedEventArgs e)
        {
            App.AppViewModel.ShowReplyDetailPopup(Data.rpid_str, Data.oid.ToString(), Data.type.ToString());
        }

        public Visibility SubReplyVisibility
        {
            get { return (Visibility)GetValue(SubReplyVisibilityProperty); }
            set { SetValue(SubReplyVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubReplyVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubReplyVisibilityProperty =
            DependencyProperty.Register("SubReplyVisibility", typeof(Visibility), typeof(ReplyMainBlock), new PropertyMetadata(Visibility.Visible));

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), Data.member.mid);
        }

    }
}
