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
    }
}
