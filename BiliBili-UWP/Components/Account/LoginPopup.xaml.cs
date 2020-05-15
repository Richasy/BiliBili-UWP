using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Account
{
    public sealed partial class LoginPopup : UserControl,IAppPopup
    {
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        public LoginPopup()
        {
            this.InitializeComponent();
            App.BiliViewModel._webClient.ValidateLoginEvent += WebClient_ValidateLogin;
            UIHelper.PopupInit(this);
        }

        private async void WebClient_ValidateLogin(object sender, string e)
        {
            try
            {
                JObject jObject = JObject.Parse(e);
                if (jObject["access_token"] != null)
                {
                    var result = await App.BiliViewModel._client.Account.SetLoginStatusAsync(jObject["access_token"].ToString(), jObject["refresh_token"].ToString(), Convert.ToInt32(jObject["expires_in"].ToString()));
                    if (result)
                    {
                        await App.BiliViewModel.GetMeAsync();
                        HidePopup();
                        return;
                    }
                }
            }
            catch (Exception)
            {  
            }
            ShowMessageBanner("登陆失败");
        }

        public void ShowPopup()
        {
            UserNameInputBox.SetText("");
            PasswordInputBox.SetText("");
            CaptchaBlock.Visibility = Visibility.Collapsed;
            WebContainer.Visibility = Visibility.Collapsed;
            BackupWebView.NavigateToString("");
            UIHelper.PopupShow(this);
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            PopupOut.Begin();
            PopupOut.Completed -= PopupOut_Completed;
            PopupOut.Completed += PopupOut_Completed;
        }

        private void PopupOut_Completed(object sender, object e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == _popupId);
            _popup.IsOpen = false;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }
        public void SetUserName(string name)
        {
            UserNameInputBox.SetText(name);
            PasswordInputBox.SetText("");
            HideMessageBanner();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await Login();
        }

        private async Task Login()
        {
            var waiting = new WaitingPopup("正在验证数据...");
            waiting.ShowPopup();
            string userName = UserNameInputBox.Text;
            string password = PasswordInputBox.Text;
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ShowMessageBanner("用户名或密码不能为空");
            }
            else if (CaptchaBlock.Visibility == Visibility.Visible && string.IsNullOrEmpty(CaptchaBlock.Code))
            {
                ShowMessageBanner("验证码不能为空");
            }
            else
            {
                var result = await App.BiliViewModel._client.Account.LoginAsync(userName, password, CaptchaBlock.Code);
                switch (result.Status)
                {
                    case LoginResultType.Success:
                        await App.BiliViewModel.GetMeAsync();
                        HidePopup();
                        break;
                    case LoginResultType.Fail:
                        ShowMessageBanner("登录失败，请检查账号密码");
                        break;
                    case LoginResultType.Error:
                        ShowMessageBanner("2233娘无情地驳回了请求，请稍后重试");
                        break;
                    case LoginResultType.NeedCaptcha:
                        ShowMessageBanner("需要输入验证码");
                        CaptchaBlock.Visibility = Visibility.Visible;
                        await CaptchaBlock.RefreshCode();
                        await Task.Delay(1000);
                        HideMessageBanner();
                        break;
                    case LoginResultType.Busy:
                        ShowMessageBanner("服务器忙，请稍后重试");
                        break;
                    case LoginResultType.NeedValidate:
                        ShowMessageBanner("需要安全验证");
                        BackupWebView.Visibility = Visibility.Visible;
                        BackupWebView.Source = new Uri(result.Url.Replace("&ticket=1", ""));
                        await Task.Delay(1000);
                        HideMessageBanner();
                        break;
                    default:
                        break;
                }
            }
            waiting.HidePopup();
        }

        public void ShowMessageBanner(string text)
        {
            MessageBanner.Text = text;
            MessageBanner.Visibility = Visibility.Visible;
        }
        public void HideMessageBanner()
        {
            MessageBanner.Visibility = Visibility.Collapsed;
        }

        private async void BackupWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri == null)
                return;
            if (args.Uri.AbsoluteUri.Contains("access_key="))
            {
                var access = Regex.Match(args.Uri.AbsoluteUri, "access_key=(.*?)&").Groups[1].Value;
                await App.BiliViewModel._client.Account.SetLoginStatusAsync(access);
                await App.BiliViewModel.GetMeAsync();
                HidePopup();
            }
            try
            {
                BackupWebView.AddWebAllowedObject("biliapp", App.BiliViewModel._webClient);
                BackupWebView.AddWebAllowedObject("secure", App.BiliViewModel._secure);
            }
            catch (Exception)
            {
            }
        }

        private async void BackupWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.Uri == null)
                return;
            if (args.Uri.AbsoluteUri == "https://passport.bilibili.com/ajax/miniLogin/redirect")
            {
                var results = await BiliTool.GetTextFromWebAsync("https://passport.bilibili.com/login/app/third?appkey=27eb53fc9058f8c3&api=http%3A%2F%2Flink.acg.tv%2Fforum.php&sign=67ec798004373253d60114caaad89a8c",true);
                var obj = JObject.Parse(results);
                if (Convert.ToInt32(obj["code"].ToString()) == 0)
                {
                    BackupWebView.Navigate(new Uri(obj["data"]["confirm_uri"].ToString()));
                }
                else
                {
                    new TipPopup("登陆失败，请重试").ShowError();
                }
            }
        }

        private void WebCloseButton_Click(object sender, RoutedEventArgs e)
        {
            WebContainer.Visibility = Visibility.Collapsed;
        }

        private void WebButton_Click(object sender, RoutedEventArgs e)
        {
            WebContainer.Visibility = Visibility.Visible;
            BackupWebView.Source= new Uri("https://passport.bilibili.com/ajax/miniLogin/minilogin");
            BackupWebView.Width = 440;
            BackupWebView.Height = 480;
        }

        private async void PasswordInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                await Login();
            }
        }
    }
}
