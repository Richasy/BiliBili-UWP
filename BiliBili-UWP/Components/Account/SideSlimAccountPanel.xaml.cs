using BiliBili_Lib.Tools;
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
    public sealed partial class SideSlimAccountPanel : UserControl
    {
        public SideSlimAccountPanel()
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
            else
            {
                UserAvatarNarrrow.ProfilePicture = null;
            }
        }

        private void MyInfoInit(bool isSlimRefresh=false)
        {
            var me = App.BiliViewModel._client.Account.Me;
            if (me != null)
            {
                if (!isSlimRefresh)
                {
                    UserAvatar.ProfilePicture = new BitmapImage(new Uri(me.face)) { DecodePixelWidth = 55 };
                    UserAvatarNarrrow.ProfilePicture = new BitmapImage(new Uri(me.face)) { DecodePixelWidth = 55 };
                    if (me.pendant != null && !string.IsNullOrEmpty(me.pendant.image))
                    {
                        PendantImage.Visibility = Visibility.Visible;
                        PendantImage.Source = new BitmapImage(new Uri(me.pendant.image));
                        UserNameBlock.Margin = new Thickness(0);
                    }
                    else
                    {
                        PendantImage.Visibility = Visibility.Collapsed;
                        UserNameBlock.Margin = new Thickness(0, 20, 0, 0);
                    }
                }
                UserNameBlock.Text = me.name;
                LevelBlock.Level = me.level;
                DynamicBlock.Text = AppTool.GetNumberAbbreviation(me.dynamic);
                FollowBlock.Text = AppTool.GetNumberAbbreviation(me.following);
                FanBlock.Text = AppTool.GetNumberAbbreviation(me.follower);
            }
        }

        public bool IsOnlyAvatar
        {
            get { return (bool)GetValue(IsOnlyAvatarProperty); }
            set { SetValue(IsOnlyAvatarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOnlyAvatar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlyAvatarProperty =
            DependencyProperty.Register("IsOnlyAvatar", typeof(bool), typeof(SideSlimAccountPanel), new PropertyMetadata(false, new PropertyChangedCallback(IsOnlyAvatar_Changed)));

        private static void IsOnlyAvatar_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as SideSlimAccountPanel;
                instance.CheckElementStatus();
            }
        }

        private void CheckElementStatus()
        {
            if (IsOnlyAvatar)
            {
                DetailContainer.Visibility = Visibility.Collapsed;
                LoginButton.Visibility = Visibility.Collapsed;
                UserAvatarNarrrow.Visibility = Visibility.Visible;
            }
            else
            {
                UserAvatarNarrrow.Visibility = Visibility.Collapsed;
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
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.BiliViewModel.ShowLoginPopup();
        }

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToMe();
        }

        private void ToMe()
        {
            var me = App.BiliViewModel._client.Account.Me;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), me.mid);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoginButton.IsLoading = true;
            await App.BiliViewModel.AutoLoginAsync();
            LoginButton.IsLoading = false;
        }

        private void UserAvatarNarrrow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.BiliViewModel.IsLogin)
            {
                ToMe();
            }
            else
            {
                if (!LoginButton.IsLoading)
                    App.BiliViewModel.ShowLoginPopup();
            }
        }

        private void Dynamic_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var me = App.BiliViewModel._client.Account.Me;
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), new Tuple<int, bool>(me.mid, true));
        }

        private void Fans_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FansPage));
        }

        private void Follow_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.FollowPage));
        }
    }
}
