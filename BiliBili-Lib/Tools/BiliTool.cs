using BiliBili_Lib.Models.Others;
using Microsoft.QueryStringDotNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using wwh = Windows.Web.Http;
using wwhf = Windows.Web.Http.Filters;

namespace BiliBili_Lib.Tools
{
    public class BiliTool
    {
        public static ApiKeyInfo AndroidKey = new ApiKeyInfo("4409e2ce8ffd12b8", "59b43e04ad6965f34319062b478f83dd");
        public static ApiKeyInfo AndroidVideoKey = new ApiKeyInfo("iVGUTjsxvpLeuDCf", "aHRmhWMLkdeMuILqORnYZocwMBpMEOdt");
        public static ApiKeyInfo WebVideoKey = new ApiKeyInfo("84956560bc028eb7", "94aba54af9065f71de72f5508f1cd42e");
        public static ApiKeyInfo VideoKey = new ApiKeyInfo("", "1c15888dc316e05a15fdd0a02ed6584f");
        public static ApiKeyInfo IosKey = new ApiKeyInfo("4ebafd7c4951b366", "8cb98205e9b2ad3669aad0fce12a4c13");
        public const string BuildNumber = "5520400";
        /// <summary>
        /// 从网络获取文本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetTextFromWebAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ServerCertificateCustomValidationCallback= delegate { return true; }
            };
            var client = new HttpClient(handler);
            using (client)
            {
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 BiliDroid/4.34.0 (bbcallen@gmail.com)");
                client.DefaultRequestHeaders.Referrer = new Uri("http://www.bilibili.com/");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string res = await response.Content.ReadAsStringAsync();
                    var jobj = JObject.Parse(res);
                    string content = string.Empty;
                    if (jobj.ContainsKey("data"))
                        content = jobj["data"].ToString();
                    else
                        content = res;
                    return content;
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取网络数据并转化为对应的类型
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static async Task<T> ConvertEntityFromWebAsync<T>(string url) where T:class
        {
            string response = await GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }
                catch (Exception)
                { }
            }
            return null;
        }
        /// <summary>
        /// 获取B站网页的Cookie
        /// </summary>
        /// <returns></returns>
        public static string GetCookies()
        {
            wwhf.HttpBaseProtocolFilter hb = new wwhf.HttpBaseProtocolFilter();
            wwh.HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            string cookie = "";
            foreach (wwh.HttpCookie item in cookieCollection)
            {
                cookie += item.Name + "=" + item.Value + ";";
            }
            return cookie;
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="apiKeyInfo">取用的Api密钥</param>
        /// <returns></returns>
        public static string GetSign(string url, ApiKeyInfo apiKeyInfo = null)
        {
            if (apiKeyInfo == null)
                apiKeyInfo = AndroidKey;
            var uri = new Uri(url);
            string query = uri.Query.Substring(1);
            var args = QueryString.Parse(query);
            string param=string.Join('&', args.OrderBy(p => p.Name).Select(p => $"{p.Name}={p.Value}"));
            param += apiKeyInfo.Secret;
            string result = MD5Tool.GetMd5String(param).ToLower();
            return result;
        }
    }
}
