using BiliBili_Lib.Models.BiliBili.Feedback;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI.Others;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class MessageReplyBlock : UserControl
    {
        public MessageReplyBlock()
        {
            this.InitializeComponent();
        }

        public FeedReplyDetail Data
        {
            get { return (FeedReplyDetail)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(FeedReplyDetail), typeof(MessageReplyBlock), new PropertyMetadata(null, new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue is FeedReplyDetail data)
            {
                var instance = d as MessageReplyBlock;
                instance.UserAvatar.ProfilePicture = new BitmapImage(new Uri(data.user.avatar + "@40w.jpg")) { DecodePixelWidth = 40 };
                instance.ShowUserBlock.Text = data.user.nickname;
                instance.TimeBlock.Text = AppTool.GetReadDateString(data.reply_time);
                instance.MultipleUserBlock.Text = data.is_multi == 1 ? "等人" : "";
                instance.TypeBlock.Text = $"对我的{data.item.business}发表了{data.counts}条评论";
                instance.DetailBlock.Text = data.item.source_content;
                instance.TitleBlock.Text = string.IsNullOrEmpty(data.item.title) ? data.item.desc : data.item.title;
            }
        }

        private void Container_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(e.OriginalSource is Grid && Data!=null)
            {
                var uri = Data.item.native_uri;
                if (Data.is_multi == 1)
                {
                    HandleReply();
                }
                else
                {
                    App.AppViewModel.ShowReplyDetailPopup(Data.item.source_id.ToString(), Data.item.subject_id.ToString(), Data.item.business_id.ToString());
                }
            }
        }

        private void Title_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Data.item.type == "reply")
                HandleReply();
            else
                App.AppViewModel.HandleUri(Data.item.uri, TitleBlock.Text);
        }

        private void HandleReply()
        {
            var param = new Dictionary<string, string>();
            param.Add("type", Data.item.business_id.ToString());
            param.Add("oid", Data.item.subject_id.ToString());
            param.Add("mode", "2");
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.ReplyPage), param);
        }

        private void UserAvatar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), Data.user.mid);
        }
    }
}
