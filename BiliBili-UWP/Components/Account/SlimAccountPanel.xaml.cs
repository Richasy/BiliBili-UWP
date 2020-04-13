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
    public sealed partial class SlimAccountPanel : UserControl
    {
        public SlimAccountPanel()
        {
            this.InitializeComponent();
            App.BiliViewModel.IsLoginChanged -= LoginStatusChanged;
            App.BiliViewModel.IsLoginChanged += LoginStatusChanged;
        }

        private void LoginStatusChanged(object sender, bool e)
        {
            CheckElementStatus();
            if (e)
            {
                var me = App.BiliViewModel._client.Account.Me;
                if (me != null)
                {
                    UserAvatar.ProfilePicture = new BitmapImage(new Uri(me.face)) { DecodePixelWidth = 55 };
                    UserAvatarNarrrow.ProfilePicture = new BitmapImage(new Uri(me.face)) { DecodePixelWidth = 55 };
                    UserNameBlock.Text = me.name;
                    LevelBlock.Level = me.level;
                    DynamicBlock.Text = AppTool.GetNumberAbbreviation(me.dynamic);
                    FollowBlock.Text = AppTool.GetNumberAbbreviation(me.following);
                    FanBlock.Text = AppTool.GetNumberAbbreviation(me.follower);
                    if (me.pendant!=null && !string.IsNullOrEmpty(me.pendant.image))
                    {
                        PendantImage.Visibility = Visibility.Visible;
                        PendantImage.Source = new BitmapImage(new Uri(me.pendant.image));
                    }
                    else
                    {
                        PendantImage.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                UserAvatarNarrrow.ProfilePicture = null;
            }
        }

        public bool IsOnlyAvatar
        {
            get { return (bool)GetValue(IsOnlyAvatarProperty); }
            set { SetValue(IsOnlyAvatarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOnlyAvatar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlyAvatarProperty =
            DependencyProperty.Register("IsOnlyAvatar", typeof(bool), typeof(SlimAccountPanel), new PropertyMetadata(false,new PropertyChangedCallback(IsOnlyAvatar_Changed)));

        private static void IsOnlyAvatar_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var instance = d as SlimAccountPanel;
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
    }
}
