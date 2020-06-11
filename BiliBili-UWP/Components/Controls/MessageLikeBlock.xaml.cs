using BiliBili_Lib.Models.BiliBili.Feedback;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.UI.Others;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class MessageLikeBlock : UserControl
    {
        public MessageLikeBlock()
        {
            this.InitializeComponent();
        }

        public FeedLikeDetail Data
        {
            get { return (FeedLikeDetail)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(FeedLikeDetail), typeof(MessageLikeBlock), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue !=null && e.NewValue is FeedLikeDetail data)
            {
                var instance = d as MessageLikeBlock;
                instance.UserAvatarItemsControl.ItemsSource = data.users.Take(5);
                bool isMany = data.users.Count > 1;
                instance.LatestContainer.Visibility = data.is_latest ? Visibility.Visible : Visibility.Collapsed;
                string detail = string.Empty;
                if (isMany)
                {
                    string first = data.users.First().nickname;
                    string second = data.users[1].nickname;
                    detail = $"**{first}**、**{second}**等{data.counts}人赞了我的{data.item.business}";
                }
                else
                {
                    detail = $"**{data.users.First().nickname}**赞了我的{data.item.business}";
                }
                instance.DetailBlock.Text = detail;
                instance.TimeBlock.Text = AppTool.GetReadDateString(data.like_time);
                instance.TitleBlock.Text = string.IsNullOrEmpty(data.item.title) ? data.item.desc : data.item.title;
            }
        }

        private void Container_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            App.AppViewModel.HandleUri(Data.item.uri, TitleBlock.Text);
        }

        private void ImageEx_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            var user = (sender as FrameworkElement).DataContext as FeedUser;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), user.mid);
        }
    }
}
