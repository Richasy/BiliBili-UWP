using BiliBili_Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class TokenInfo
    {
        public long mid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }
    public class Cookies
    {
        public string name { get; set; }
        public string value { get; set; }
        public int http_only { get; set; }
        public int expires { get; set; }
    }

    public class CookieInfo
    {
        public List<Cookies> cookies { get; set; }
        public List<string> domains { get; set; }
    }
    public class LoginResult
    {
        public int status { get; set; }
        public TokenInfo token_info { get; set; }
        public CookieInfo cookie_info { get; set; }
        public List<string> sso { get; set; }
        public string url { get; set; }
        public long mid { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }

    }
    public class LoginCallback
    {
        public LoginResultType Status { get; set; }
        public string Url { get; set; }
    }
}
