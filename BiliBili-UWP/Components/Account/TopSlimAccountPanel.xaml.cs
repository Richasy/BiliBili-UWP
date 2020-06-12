using BiliBili_Controls.Extensions;
using BiliBili_Lib.Tools;
using Microsoft.Toolkit.Uwp.UI.Controls;
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

namespace BiliBili_UWP.Components.Account
{
    public sealed partial class TopSlimAccountPanel : UserControl
    {
        public TopSlimAccountPanel()
        {
            this.InitializeComponent();
            App.BiliViewModel.IsLoginChanged -= LoginStatusChanged;
            App.BiliViewModel.IsLoginChanged += LoginStatusChanged;
            App.BiliViewModel.MyInfoChanged -= MyInfoChanged;
            App.BiliViewModel.MyInfoChanged += MyInfoChanged;
        }
        private void MyInfoChanged(object sender, EventArgs e)
        {
            MyInfoInit(true);
        }

        private void LoginStatusChanged(object sender, bool e)
        {
            CheckElementStatus();
            if (e)
            {
                MyInfoInit();
            }
        }

        private void MyInfoInit(bool isSlimRefresh = false)
        {
            var me = App.BiliViewModel._client.Account.Me;
            if (me != null)
            {
                if (!isSlimRefresh)
                {
                    UserAvatar.ProfilePicture = new BitmapImage(new Uri(me.face)) { DecodePixelWidth = 55 };
                    if (me.pendant != null && !string.IsNullOrEmpty(me.pendant.image))
                    {
                        PendantImage.Visibility = Visibility.Visible;
                        PendantImage.Source = new BitmapImage(new Uri(me.pendant.image));
                        UserNameBlock.Margin = new Thickness(0);
                    }
                    else
                    {
                        PendantImage.Visibility = Visibility.Collapsed;
                        UserNameBlock.Margin = new Thickness(10, 0, 0, 0);
                    }
                }
                UserNameBlock.Text = me.name;
                LevelBlock.Level = me.level;
            }
        }
        private void CheckElementStatus()
        {
            if (App.BiliViewModel.IsLogin)
            {
                DetailContainer.Visibility = Visibility.Visible;
                LoginButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DetailContainer.Visibility = Visibility.Collapsed;
                LoginButton.Visibility = Visibility.Visible;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.BiliViewModel.ShowLoginPopup();
        }
        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginButton.IsLoading = true;
            await App.BiliViewModel.AutoLoginAsync();
            LoginButton.IsLoading = false;
        }
        private void DetailContainer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AccountFlyout.ShowAt(DetailContainer);
        }

        public void SetMessageUnread(int unread)
        {
            MessageUnreadBlock.Text = unread.ToString();
            MessageUnreadContainer.Visibility = UnreadMessageSign.Visibility = unread > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void MenuListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FrameworkElement;
            var parent = VisualTreeExtension.GetParentObject<ListViewItem>(item, "");
            string tag = parent.Tag.ToString();
            switch (tag)
            {
                case "Message":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.MessagePage));
                    break;
                case "VideoDynamic":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.DynamicPage));
                    break;
                case "MyAnime":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteAnimePage), "anime");
                    break;
                case "MyMovie":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteAnimePage), "cinema");
                    break;
                case "MyFavorite":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteContainerPage), "Favorite");
                    break;
                case "MyCollect":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FavoriteContainerPage), "Collect");
                    break;
                case "ViewLater":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.ViewLaterPage));
                    break;
                case "History":
                    App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Video.HistoryPage));
                    break;
                default:
                    break;
            }
            AccountFlyout.Hide();
        }
    }
}
