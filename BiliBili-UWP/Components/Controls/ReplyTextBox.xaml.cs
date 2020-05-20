using BiliBili_UWP.Components.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ReplyTextBox : UserControl
    {
        public ReplyTextBox()
        {
            this.InitializeComponent();
            App.BiliViewModel.IsLoginChanged += LoginChanged;
        }
        public string AtUser = "";
        public event EventHandler<string> SendReply;
        private async void LoginChanged(object sender, bool e)
        {
            await CheckLogin();
        }

        private async Task CheckLogin(bool isRefresh = true)
        {
            if (App.BiliViewModel.IsLogin)
            {
                ReplyBox.IsEnabled = true;
                PlaceholderText = "输入回复";
                SendButton.IsEnabled = true;
                EmojiButton.IsEnabled = true;
                if (isRefresh)
                    await EmojiPanel.Init();
            }
            else
            {
                ReplyBox.IsEnabled = false;
                PlaceholderText = "请登录后再评论";
                SendButton.IsEnabled = false;
                EmojiButton.IsEnabled = false;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ReplyBox.Text))
            {
                new TipPopup("回复不能为空").ShowError();
                return;
            }
            SendReply?.Invoke(this, ReplyBox.Text);
        }

        private void EmojiPanel_EmojiSelected(object sender, BiliBili_Lib.Models.BiliBili.Emote e)
        {
            EmojiFlyout.Hide();
            string text = e.text;
            int pos = ReplyBox.SelectionStart;
            string content = ReplyBox.Text??"";
            content = content.Insert(pos, text);
            ReplyBox.Text = content;
            ReplyBox.Select(pos + text.Length, 0);
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!EmojiPanel.IsInit)
                await EmojiPanel.Init();
            await CheckLogin(false);
            ReplyBox.Text = "";
        }
        public void ClearText()
        {
            ReplyBox.Text = "";
        }

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlaceholderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(ReplyTextBox), new PropertyMetadata(""));


    }
}
