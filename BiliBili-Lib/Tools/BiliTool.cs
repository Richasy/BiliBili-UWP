using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using MetroLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

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
        public static string _accessToken = "";
        public static string sid = "";
        public static string mid = "";
        public static ILogger _logger = LogManagerFactory.CreateLogManager().GetLogger("网络请求");
        /// <summary>
        /// 从网络获取文本
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> GetTextFromWebAsync(string url, bool total = false, string path = "data", bool needReferer = false)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            filter.AllowAutoRedirect = false;
            var client = new HttpClient(filter);
            using (client)
            using (filter)
            {
                try
                {
                    //client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.19041");
                    client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                    var uri = new Uri(url);
                    if (needReferer)
                        client.DefaultRequestHeaders.Referer = new Uri(uri.Scheme + "://" + uri.Host);
                    var response = await client.GetAsync(new Uri(url));
                    if (response.IsSuccessStatusCode)
                    {
                        string res = await response.Content.ReadAsStringAsync();
                        if (total)
                            return res;
                        var jobj = JObject.Parse(res);
                        string content = string.Empty;
                        if (jobj.SelectToken(path) != null)
                            content = jobj.SelectToken(path).ToString();
                        else
                            content = res;
                        return content;
                    }
                    else if (response.StatusCode == Windows.Web.Http.HttpStatusCode.TemporaryRedirect || response.StatusCode == Windows.Web.Http.HttpStatusCode.MovedPermanently)
                    {
                        string tempUrl = response.Headers.Location.AbsoluteUri;
                        return await GetTextFromWebAsync(tempUrl, total, path);
                    }
                    else
                    {
                        _logger.Warn($"请求数据异常(Text)：URL: {url}; Message: {await response.Content.ReadAsStringAsync()}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"请求出现异常:{url}", ex);
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
        public static async Task<T> ConvertEntityFromWebAsync<T>(string url, string path = "data", bool needReferer = false) where T : class
        {
            string response = await GetTextFromWebAsync(url, path: path, needReferer: needReferer);
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(response);
                }
                catch (Exception ex)
                {
                    _logger.Error($"数据转化失败: {nameof(T)}", ex);
                }
            }
            return null;
        }
        /// <summary>
        /// 从网络获取流
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static async Task<Stream> GetStreamFromWebAsync(string url)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            var client = new HttpClient(filter);
            using (client)
            {
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 BiliDroid/4.34.0 (bbcallen@gmail.com)");
                client.DefaultRequestHeaders.Referer = new Uri("http://www.bilibili.com/");
                var response = await client.GetAsync(new Uri(url));
                if (response.IsSuccessStatusCode)
                {
                    var buffer = await response.Content.ReadAsBufferAsync();
                    return buffer.AsStream();
                }
                else
                {
                    _logger.Warn($"请求数据(Stream)异常：URL: {url}; Message: {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
        }
        /// <summary>
        /// 向网络上传字符串
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="content">数据</param>
        /// <returns></returns>
        public static async Task<string> PostContentToWebAsync(string url, string content)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.IgnorableServerCertificateErrors.Add(ChainValidationResult.Expired);
            var client = new HttpClient(filter);
            if (url.Contains("oauth2/login") && !string.IsNullOrEmpty(sid))
            {
                filter.CookieManager.SetCookie(new HttpCookie("sid", "bilibili.com", "/") { Value = sid });
            }
            using (client)
            {
                client.DefaultRequestHeaders.Referer = new Uri("http://www.bilibili.com/");
                var response = await client.PostAsync(new Uri(url), new HttpStringContent(content, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    _logger.Warn($"上传数据异常：URL: {url}; Message: {await response.Content.ReadAsStringAsync()}");
                }
                return "";
            }
        }
        /// <summary>
        /// 获取B站网页的Cookie
        /// </summary>
        /// <returns></returns>
        public static string GetCookies()
        {
            HttpBaseProtocolFilter hb = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = hb.CookieManager.GetCookies(new Uri("http://bilibili.com/"));
            string cookie = "";
            foreach (HttpCookie item in cookieCollection)
            {
                cookie += item.Name + "=" + item.Value + ";";
            }
            return cookie;
        }
        /// <summary>
        /// 清除Cookie
        /// </summary>
        public static void ClearCookies()
        {
            List<HttpCookie> listCookies = new List<HttpCookie>();
            listCookies.Add(new HttpCookie("sid", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("DedeUserID", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("DedeUserID__ckMd5", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("SESSDATA", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("LIVE_LOGIN_DATA", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("LIVE_LOGIN_DATA__ckMd5", ".bilibili.com", "/"));
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            foreach (HttpCookie cookie in listCookies)
            {
                filter.CookieManager.DeleteCookie(cookie);
            }
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
            {
                apiKeyInfo = AndroidKey;
            }
            string result;
            if (url.StartsWith("http"))
                url.Substring(url.IndexOf("?", 4) + 1);
            List<string> list = url.Split('&').ToList();
            list.Sort();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str1 in list)
            {
                stringBuilder.Append((stringBuilder.Length > 0 ? "&" : string.Empty));
                stringBuilder.Append(str1);
            }
            stringBuilder.Append(apiKeyInfo.Secret);
            result = MD5Tool.GetMd5String(stringBuilder.ToString()).ToLower();
            return result;
        }
        /// <summary>
        /// 请求地址拼接
        /// </summary>
        /// <param name="_baseUrl">请求url</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="hasAccessKey">是否包含令牌</param>
        /// <returns></returns>
        public static string UrlContact(string _baseUrl, Dictionary<string, string> parameters = null, bool hasAccessKey = false, bool useWeb = false,
            bool useiPhone = false)
        {
            if (parameters == null)
                parameters = new Dictionary<string, string>();

            parameters.Add("build", BuildNumber);
            if (useiPhone)
            {
                parameters.Add("appkey", IosKey.Appkey);
                parameters.Add("mobi_app", "iphone");
                parameters.Add("platform", "ios");
                parameters.Add("ts", AppTool.GetNowSeconds().ToString());
            }
            else if (!useWeb)
            {
                parameters.Add("appkey", AndroidKey.Appkey);
                parameters.Add("mobi_app", "android");
                parameters.Add("platform", "android");
                parameters.Add("ts", AppTool.GetNowSeconds().ToString());
            }
            else
            {
                parameters.Add("appkey", WebVideoKey.Appkey);
                parameters.Add("ts", AppTool.GetNowMilliSeconds().ToString());
            }
            if (hasAccessKey && !string.IsNullOrEmpty(_accessToken))
                parameters.Add("access_key", _accessToken);
            string param = string.Empty;
            foreach (var item in parameters)
            {
                param += $"{item.Key}={item.Value}&";
            }
            param = param.TrimEnd('&');
            string sign = useWeb ? GetSign(param, WebVideoKey) : GetSign(param);
            param += $"&sign={sign}";
            return !string.IsNullOrEmpty(_baseUrl) ? _baseUrl + $"?{param}" : param;
        }

        /// <summary>
        /// 分析URL指向的结果
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static UriResult GetResultFromUri(string url)
        {
            var uri = new Uri(url);
            string path = uri.AbsolutePath;
            if (path.Contains("/ep", StringComparison.OrdinalIgnoreCase))
            {
                var regex_ep = new Regex(@"ep(\d+)");
                if (regex_ep.IsMatch(url))
                {
                    string epId = regex_ep.Match(path).Value.Replace("ep", "");
                    return new UriResult(Enums.UriType.Bangumi, epId);
                }
            }
            else if (path.Contains("/cv", StringComparison.OrdinalIgnoreCase))
            {
                var regex_cv = new Regex(@"cv(\d+)");
                if (regex_cv.IsMatch(url))
                {
                    string cId = regex_cv.Match(path).Value.Replace("cv", "");
                    return new UriResult(Enums.UriType.Document, cId);
                }
            }
            else if (path.Contains("video/", StringComparison.OrdinalIgnoreCase))
            {
                var regex_av = new Regex(@"av(\d+)");
                var regex_bv = new Regex(@"BV(.*)");

                if (regex_av.IsMatch(path))
                {
                    string aid = regex_av.Match(path).Value.Replace("av", "");
                    return new UriResult(Enums.UriType.VideoA, aid);
                }
                else if (regex_bv.IsMatch(path))
                {
                    string bid = regex_bv.Match(path).Value;
                    return new UriResult(Enums.UriType.VideoB, bid);
                }
            }
            return new UriResult(Enums.UriType.Web, url);
        }
    }
}
