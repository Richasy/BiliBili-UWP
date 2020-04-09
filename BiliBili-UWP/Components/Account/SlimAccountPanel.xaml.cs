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
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Account
{
    public sealed partial class SlimAccountPanel : UserControl
    {
        public SlimAccountPanel()
        {
            this.InitializeComponent();
        }

        public bool IsLogin
        {
            get { return (bool)GetValue(IsLoginProperty); }
            set { SetValue(IsLoginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLogin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoginProperty =
            DependencyProperty.Register("IsLogin", typeof(bool), typeof(SlimAccountPanel), new PropertyMetadata(false,new PropertyChangedCallback(IsLogin_Changed)));

        private static void IsLogin_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=e.OldValue)
            {
                var instance = d as SlimAccountPanel;
                instance.CheckElementStatus();
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
                if (IsLogin)
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

        }
    }
}
