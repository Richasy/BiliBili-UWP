using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Service;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Account;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI.Others;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace BiliBili_UWP.Models.Core
{
    public partial class BiliViewModel
    {
        private bool _isLogin;
        public bool IsLogin
        {
            get => _isLogin;
            set
            {
                if (_isLogin != value)
                {
                    Set(ref _isLogin, value);
                    IsLoginChanged?.Invoke(this, value);
                }
            }
        }
        
        public event EventHandler<bool> IsLoginChanged;
        public LoginPopup LoginPopup;
        public void ShowLoginPopup()
        {
            if (LoginPopup == null)
                LoginPopup = new LoginPopup();
            LoginPopup.ShowPopup();
        }
        /// <summary>
        /// 获取我的个人信息
        /// </summary>
        /// <returns></returns>
        public async Task GetMeAsync()
        {
            var data = await _client.Account.GetMeAsync();
            if (data != null)
            {
                if (!IsLogin)
                    IsLogin = true;
            }
        }
        
        public void ClearAccountInformation()
        {
            IsLogin = false;
            AppTool.WriteLocalSetting(Settings.AccessToken, "");
            AppTool.WriteLocalSetting(Settings.RefreshToken, "");
            AppTool.WriteLocalSetting(Settings.TokenExpiry, "0");
            AppTool.WriteLocalSetting(Settings.UserId, "");
            _client.Account = new AccountService(new TokenPackage());
            BiliTool.ClearCookies();
        }
        
        /// <summary>
        /// 自动登录
        /// </summary>
        /// <returns></returns>
        public async Task AutoLoginAsync()
        {
            if (!string.IsNullOrEmpty(_client.Account._accessToken))
            {
                bool isValid = false;
                bool isExpiry = AppTool.DateToTimeStamp(DateTime.Now) >= _client.Account._expiry;
                if (isExpiry)
                    isValid = await _client.Account.RefreshTokenAsync();
                else
                    isValid = await _client.Account.CheckTokenStatusAsync();
                if (isValid)
                    await GetMeAsync();
                else
                    ClearAccountInformation();
            }
            else
            {
                await _client.Account.SSO();
                IsLogin = false;
            }  
        }
        /// <summary>
        /// 检查用户是否登录
        /// </summary>
        /// <returns></returns>
        public bool CheckAccoutStatus()
        {
            var me = _client.Account.Me;
            if (me == null)
            {
                new TipPopup("你需要登录才能执行此操作").ShowError();
                return false;
            }
            return true;
        }
    }
}
