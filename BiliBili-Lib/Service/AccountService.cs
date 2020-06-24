using BiliBili_Lib.Enums;
using BiliBili_Lib.Models;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.BiliBili.Account;
using BiliBili_Lib.Models.BiliBili.Favorites;
using BiliBili_Lib.Models.BiliBili.Feedback;
using BiliBili_Lib.Models.BiliBili.Video;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
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

namespace BiliBili_Lib.Service
{
    public class AccountService
    {
        public string _accessToken;
        private string _refreshToken;
        private string _userId;
        public int _expiry;
        public Me Me;
        public AccountService(TokenPackage p)
        {
            InitToken(p);
        }
        public event EventHandler<TokenPackage> TokenChanged;
        private void InitToken(TokenPackage p)
        {
            BiliTool._accessToken = _accessToken = p.AccessToken;
            _refreshToken = p.RefreshToken;
            _expiry = Convert.ToInt32(p.Expiry);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        public async Task<BitmapImage> GetCaptchaAsync()
        {
            var stream = await BiliTool.GetStreamFromWebAsync($"{Api.PASSPORT_CAPTCHA}?ts=${AppTool.GetNowSeconds()}");
            if (stream != null)
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                return bitmap;
            }
            return new BitmapImage(new Uri("ms-appx:///Assets/captcha_refresh.png"));
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="captcha">验证码</param>
        /// <returns></returns>
        public async Task<LoginCallback> LoginAsync(string userName, string password, string captcha = "")
        {
            string param = $"appkey={BiliTool.AndroidKey.Appkey}&build={BiliTool.BuildNumber}&mobi_app=android&password={Uri.EscapeDataString(await EncryptedPasswordAsync(password))}&platform=android&ts={AppTool.GetNowSeconds()}&username={Uri.EscapeDataString(userName)}";
            if (!string.IsNullOrEmpty(captcha))
            {
                param += "&captcha=" + captcha;
            }
            param += "&sign=" + BiliTool.GetSign(param);
            string response = await BiliTool.PostContentToWebAsync(Api.PASSPORT_LOGIN, param);
            var result = new LoginCallback();
            result.Status = LoginResultType.Error;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var jobj = JObject.Parse(response);
                    int code = Convert.ToInt32(jobj["code"]);
                    if (code == 0)
                    {
                        var data = JsonConvert.DeserializeObject<LoginResult>(jobj["data"].ToString());
                        var package = new TokenPackage(data.token_info.access_token, data.token_info.refresh_token, AppTool.DateToTimeStamp(DateTime.Now.AddSeconds(data.token_info.expires_in)));
                        InitToken(package);
                        TokenChanged?.Invoke(this, package);
                        result.Status = LoginResultType.Success;
                        await SSO();
                    }
                    else if (code == -2100)
                    {
                        result.Status = LoginResultType.NeedValidate;
                        result.Url = jobj["url"].ToString();
                    }
                    else if (code == -105)
                        result.Status = LoginResultType.NeedCaptcha;
                    else if (code == -449)
                        result.Status = LoginResultType.Busy;
                    else
                        result.Status = LoginResultType.Fail;
                }
                catch (Exception)
                {
                    return new LoginCallback { Status = LoginResultType.Fail, Url = "" };
                }
            }
            return result;
        }
        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private async Task<string> EncryptedPasswordAsync(string password)
        {
            string base64String;
            try
            {
                string param = BiliTool.UrlContact("").TrimStart('?');
                string content = await BiliTool.PostContentToWebAsync(Api.PASSPORT_KEY_ENCRYPT, param);
                JObject jobj = JObject.Parse(content);
                string str = jobj["data"]["hash"].ToString();
                string str1 = jobj["data"]["key"].ToString();
                string str2 = string.Concat(str, password);
                string str3 = Regex.Match(str1, "BEGIN PUBLIC KEY-----(?<key>[\\s\\S]+)-----END PUBLIC KEY").Groups["key"].Value.Trim();
                byte[] numArray = Convert.FromBase64String(str3);
                AsymmetricKeyAlgorithmProvider asymmetricKeyAlgorithmProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1);
                CryptographicKey cryptographicKey = asymmetricKeyAlgorithmProvider.ImportPublicKey(WindowsRuntimeBufferExtensions.AsBuffer(numArray), 0);
                IBuffer buffer = CryptographicEngine.Encrypt(cryptographicKey, WindowsRuntimeBufferExtensions.AsBuffer(Encoding.UTF8.GetBytes(str2)), null);
                base64String = Convert.ToBase64String(WindowsRuntimeBufferExtensions.ToArray(buffer));
            }
            catch (Exception)
            {
                base64String = password;
            }
            return base64String;
        }
        /// <summary>
        /// 登陆成功后设置令牌状态
        /// </summary>
        /// <param name="accessToken">令牌</param>
        /// <param name="mid">用户ID</param>
        /// <returns></returns>
        public async Task<bool> SetLoginStatusAsync(string accessToken, string refreshToken = "", int expiry = 0)
        {
            try
            {
                string refe = string.IsNullOrEmpty(refreshToken) ? accessToken : refreshToken;
                var package = new TokenPackage(accessToken, refe, AppTool.DateToTimeStamp(DateTime.Now.AddSeconds(expiry == 0 ? 7200 : expiry)));
                InitToken(package);
                TokenChanged?.Invoke(this, package);
                await SSO();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var param = $"access_token={_accessToken}&refresh_token={_refreshToken}&appkey={BiliTool.AndroidKey.Appkey}&ts={AppTool.GetNowSeconds()}";
                param += "&sign=" + BiliTool.GetSign(param);
                var content = await BiliTool.PostContentToWebAsync(Api.PASSPORT_REFRESH_TOKEN, param);
                var obj = JObject.Parse(content);
                if (Convert.ToInt32(obj["code"]) == 0)
                {
                    var data = JsonConvert.DeserializeObject<TokenInfo>(obj["data"].ToString());
                    var package = new TokenPackage(data.access_token, data.refresh_token, AppTool.DateToTimeStamp(DateTime.Now.AddSeconds(data.expires_in)));
                    InitToken(package);
                    TokenChanged?.Invoke(this, package);
                    await SSO();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 检查令牌状态
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckTokenStatusAsync()
        {
            try
            {
                var param = new Dictionary<string, string>();
                param.Add("access_token", _accessToken);
                var url = BiliTool.UrlContact(Api.PASSPORT_CHECK_TOKEN, param);
                var content = await BiliTool.GetTextFromWebAsync(url, true);
                var obj = JObject.Parse(content);
                if (Convert.ToInt32(obj["code"]) == 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
        /// <summary>
        /// Cookie转化处理
        /// </summary>
        /// <returns></returns>
        public async Task SSO()
        {
            try
            {
                var url = BiliTool.UrlContact(Api.PASSPORT_SSO, hasAccessKey: true);
                await BiliTool.GetTextFromWebAsync(url, true);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取我的个人信息
        /// </summary>
        /// <returns></returns>
        public async Task<Me> GetMeAsync()
        {
            var url = BiliTool.UrlContact(Api.ACCOUNT_MINE, hasAccessKey: true);
            var data = await BiliTool.ConvertEntityFromWebAsync<Me>(url);
            Me = data;
            BiliTool.mid = _userId = data.mid.ToString();
            return data;
        }
        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public async Task<bool> FollowUser(int uid)
        {
            var param = new Dictionary<string, string>();
            param.Add("uid", Me.mid.ToString());
            param.Add("follow", uid.ToString());
            var data = BiliTool.UrlContact("", param, true);
            string response = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_FOLLOW_USER, data);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }
        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public async Task<bool> UnfollowUser(int uid)
        {
            var param = new Dictionary<string, string>();
            param.Add("uid", Me.mid.ToString());
            param.Add("follow", uid.ToString());
            var data = BiliTool.UrlContact("", param, true);
            string response = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_UNFOLLOW_USER, data);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }

        /// <summary>
        /// 获取观看的历史记录
        /// </summary>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public async Task<List<VideoDetail>> GetVideoHistoryAsync(int page = 1)
        {
            string url = Api.ACCOUNT_HISTORY + $"?pn={page}&ps=40";
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoDetail>>(url);
            return data;
        }

        /// <summary>
        /// 清空观看历史记录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearHistoryAsync()
        {
            string url = BiliTool.UrlContact(Api.ACCOUNT_HISTORY_CLEAR, null, true);
            string content = await BiliTool.PostContentToWebAsync(url, "");
            return content != null;
        }
        /// <summary>
        /// 移出稍后再看
        /// </summary>
        /// <param name="aids">视频ID组</param>
        /// <returns></returns>
        public async Task<bool> DeleteHistoryAsync(params int[] aids)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", string.Join(',', aids));
            var data = BiliTool.UrlContact("", param, true);
            string content = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_HISTORY_DEL, data);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }

        /// <summary>
        /// 获取我收藏的播单(最多20个)
        /// </summary>
        /// <returns>Item1: 我创建的；Item2: 我收藏的</returns>
        public async Task<Tuple<List<FavoriteItem>, List<FavoriteItem>>> GetMyMainlyFavoritesAsync()
        {
            var param = new Dictionary<string, string>();
            param.Add("up_mid", _userId);
            string url = BiliTool.UrlContact(Api.ACCOUNT_MEDIALIST, param, true);
            string response = await BiliTool.GetTextFromWebAsync(url);
            try
            {
                var jarr = JArray.Parse(response);
                bool hasMy = jarr[0]["mediaListResponse"] != null;
                bool hasOther = jarr[1]["mediaListResponse"] != null;
                var myList = new List<FavoriteItem>();
                var otherList = new List<FavoriteItem>();
                if (hasMy)
                {
                    myList = JsonConvert.DeserializeObject<List<FavoriteItem>>(jarr[0].SelectToken("mediaListResponse.list").ToString());
                }
                if (hasOther)
                {
                    otherList = JsonConvert.DeserializeObject<List<FavoriteItem>>(jarr[1].SelectToken("mediaListResponse.list").ToString());
                }
                return new Tuple<List<FavoriteItem>, List<FavoriteItem>>(myList, otherList);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取收藏夹
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="pn">页码</param>
        /// <returns></returns>
        public async Task<List<FavoriteItem>> GetFavoritesAsync(int uid,int pn=1)
        {
            var param = new Dictionary<string, string>();
            param.Add("up_mid", uid.ToString());
            param.Add("ps", "20");
            param.Add("pn", pn.ToString());
            param.Add("type", "2");
            string url = BiliTool.UrlContact(Api.ACCOUNT_FAVORITE_LIST, param, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<FavoriteItem>>(url, "data.list");
        }
        /// <summary>
        /// 获取收集列表
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="pn">页码</param>
        /// <returns></returns>
        public async Task<List<FavoriteItem>> GetCollectListAsync(int uid,int pn=1)
        {
            var param = new Dictionary<string, string>();
            param.Add("up_mid", uid.ToString());
            param.Add("ps", "20");
            param.Add("pn", pn.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_COLLECT_LIST, param, true);
            return await BiliTool.ConvertEntityFromWebAsync<List<FavoriteItem>>(url, "data.list");
        }

        /// <summary>
        /// 获取我正在追的动漫
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<int, List<FavoriteAnime>>> GetMyFavoriteAnimeAsync(int page = 1)
        {
            var param = new Dictionary<string, string>();
            param.Add("ps", "20");
            param.Add("pn", page.ToString());
            param.Add("status", "2");
            string url = BiliTool.UrlContact(Api.ACCOUNT_FAVOROTE_ANIME, param, true);
            var respons = await BiliTool.GetTextFromWebAsync(url,false,"result");
            if (!string.IsNullOrEmpty(respons))
            {
                var jobj = JObject.Parse(respons);
                var list = JsonConvert.DeserializeObject<List<FavoriteAnime>>(jobj["follow_list"]?.ToString()??"[]");
                if (list == null)
                    list = new List<FavoriteAnime>();
                var total = Convert.ToInt32(jobj["total"].ToString());
                return new Tuple<int, List<FavoriteAnime>>(total, list);
            }
            return null;
        }

        /// <summary>
        /// 获取我正在追的影视剧
        /// </summary>
        /// <returns></returns>
        public async Task<Tuple<int, List<FavoriteAnime>>> GetMyFavoriteCinemaAsync(int page=1)
        {
            var param = new Dictionary<string, string>();
            param.Add("ps", "20");
            param.Add("pn", page.ToString());
            param.Add("status", "2");
            string url = BiliTool.UrlContact(Api.ACCOUNT_FAVOROTE_CINEMA, param, true);
            var respons = await BiliTool.GetTextFromWebAsync(url, false, "result");
            if (!string.IsNullOrEmpty(respons))
            {
                var jobj = JObject.Parse(respons);
                var list = JsonConvert.DeserializeObject<List<FavoriteAnime>>(jobj["follow_list"]?.ToString() ?? "[]");
                if (list == null)
                    list = new List<FavoriteAnime>();
                var total = Convert.ToInt32(jobj["total"].ToString());
                return new Tuple<int, List<FavoriteAnime>>(total, list);
            }
            return null;
        }

        /// <summary>
        /// 获取收藏夹的视频索引
        /// </summary>
        /// <param name="id">收藏夹Id</param>
        /// <returns></returns>
        public async Task<List<FavoriteId>> GetFavoriteIdsAsync(int id)
        {
            var param = new Dictionary<string, string>();
            param.Add("pn", "1");
            param.Add("media_id", id.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_FAVORITE_IDS, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<List<FavoriteId>>(url);
            return response;
        }

        /// <summary>
        /// 根据传入的id列表获取视频信息
        /// </summary>
        /// <param name="ids">Id列表</param>
        /// <returns></returns>
        public async Task<List<FavoriteVideo>> GetFavoriteVideosAsync(IEnumerable<FavoriteId> ids)
        {
            var items = ids.Select(p => p.id + ":" + p.type);
            var param = new Dictionary<string, string>();
            param.Add("resources", string.Join(",", items));
            string url = BiliTool.UrlContact(Api.ACCOUNT_FAVORITE_INFO, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<List<FavoriteVideo>>(url);
            return response;
        }

        /// <summary>
        /// 获取稍后观看列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public async Task<List<VideoDetail>> GetViewLaterAsync(int page = 1)
        {
            string url = Api.ACCOUNT_VIEWLATER + $"?pn={page}&ps=40";
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoDetail>>(url, "data.list");
            return data;
        }

        /// <summary>
        /// 清空稍后观看列表
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearViewLaterAsync()
        {
            string url = BiliTool.UrlContact(Api.ACCOUNT_VIEWLATER_CLEAR, null, true);
            string content = await BiliTool.PostContentToWebAsync(url, "");
            return content != null;
        }

        /// <summary>
        /// 添加稍后再看
        /// </summary>
        /// <param name="aid">视频ID</param>
        /// <returns></returns>
        public async Task<bool> AddViewLaterAsync(int aid)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", aid.ToString());
            var data = BiliTool.UrlContact("", param, true);
            string content = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_VIEWLATER_ADD, data);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }
        /// <summary>
        /// 移出稍后再看
        /// </summary>
        /// <param name="aids">视频ID组</param>
        /// <returns></returns>
        public async Task<bool> DeleteViewLaterAsync(params int[] aids)
        {
            var param = new Dictionary<string, string>();
            param.Add("aid", string.Join(',', aids));
            var data = BiliTool.UrlContact("", param, true);
            string content = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_VIEWLATER_DEL, data);
            if (!string.IsNullOrEmpty(content))
            {
                var jobj = JObject.Parse(content);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }

        /// <summary>
        /// 获取用户空间信息（经过删减）
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <returns></returns>
        public async Task<UserResponse> GetUserSpaceAsync(int uid)
        {
            var param = new Dictionary<string, string>();
            param.Add("vmid", uid.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_USER_SPACE, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<UserResponse>(url);
            return data;
        }

        /// <summary>
        /// 获取用户的投稿
        /// </summary>
        /// <param name="uid">用户ID</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        public async Task<ArchiveResponse> GetUserArchiveAsync(int uid, int page)
        {
            var param = new Dictionary<string, string>();
            param.Add("vmid", uid.ToString());
            param.Add("pn", page.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_USER_ARCHIVE, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<ArchiveResponse>(url);
            return data;
        }

        /// <summary>
        /// 获取表情包
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmojiReplyContainer>> GetUserEmojiAsync()
        {
            var param = new Dictionary<string, string>();
            param.Add("business", "reply");
            string url = BiliTool.UrlContact(Api.ACCOUNT_EMOJI_PANEL, param, true);
            var items = await BiliTool.ConvertEntityFromWebAsync<List<EmojiReplyContainer>>(url, "data.packages");
            return items;
        }
        /// <summary>
        /// 获取账户的未读消息
        /// </summary>
        /// <returns></returns>
        public async Task<MyMessage> GetMyUnreadMessageAsync()
        {
            string url = BiliTool.UrlContact(Api.ACCOUNT_UNREAD, null, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<MyMessage>(url);
            return data;
        }
        /// <summary>
        /// 获取我的粉丝
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="reversion">刷新标识</param>
        /// <returns></returns>
        public async Task<FanResponse> GetMyFansAsync(int page, long reversion = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("pn", page.ToString());
            param.Add("vmid", BiliTool.mid);
            string url = BiliTool.UrlContact(Api.ACCOUNT_RELATION_FANS, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<FanResponse>(url);
            return response;
        }
        /// <summary>
        /// 获取我的关注的分组
        /// </summary>
        /// <returns></returns>
        public async Task<List<FollowTag>> GetMyFollowTagsAsync()
        {
            var param = new Dictionary<string, string>();
            param.Add("vmid", BiliTool.mid);
            string url = BiliTool.UrlContact(Api.ACCOUNT_RELATION_FOLLOW_TAGS, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<FollowTag>>(url);
            return data;
        }
        /// <summary>
        /// 获取我关注的用户（按分组）
        /// </summary>
        /// <param name="tagId">分组ID</param>
        /// <param name="pn">页码</param>
        /// <returns></returns>
        public async Task<List<RelationUser>> GetMyFollowUserAsync(int tagId, int pn)
        {
            var param = new Dictionary<string, string>();
            param.Add("mid", BiliTool.mid);
            param.Add("ps", "50");
            param.Add("pn", pn.ToString());
            param.Add("tagid", tagId.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_RELATION_FOLLOW_DETAIL, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<RelationUser>>(url);
            return data;
        }
        /// <summary>
        /// 删除收藏夹内视频
        /// </summary>
        /// <param name="aid">视频ID</param>
        /// <param name="videoType">视频类型</param>
        /// <param name="listId">收藏夹ID</param>
        /// <returns></returns>
        public async Task<bool> RemoveFavoriteVideoAsync(int aid, int videoType, int listId)
        {
            var param = new Dictionary<string, string>();
            param.Add("media_id", listId.ToString());
            param.Add("resources", $"{aid}:{videoType}");
            var req = BiliTool.UrlContact("", param, true);
            var data = await BiliTool.PostContentToWebAsync(Api.ACCOUNT_FAVORITE_VIDEO_DELETE, req);
            if (!string.IsNullOrEmpty(data))
            {
                var jobj = JObject.Parse(data);
                return jobj["code"].ToString() == "0";
            }
            return false;
        }
        /// <summary>
        /// 获取回复我的列表
        /// </summary>
        /// <param name="replyTime">偏移值（上次请求的底部时间戳）</param>
        /// <returns></returns>
        public async Task<FeedReplyResponse> GetReplyMeListAsync(int replyTime = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("reply_time", replyTime.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_FEEDBACK_REPLY, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<FeedReplyResponse>(url);
            return response;
        }
        /// <summary>
        /// 获取At我的列表
        /// </summary>
        /// <param name="id">上次请求的Id</param>
        /// <param name="atTime">上次请求的底部时间戳</param>
        /// <returns></returns>
        public async Task<FeedAtResponse> GetAtMeListAsync(long id, int atTime = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            param.Add("at_time", atTime.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_FEEDBACK_AT, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<FeedAtResponse>(url);
            return response;
        }
        /// <summary>
        /// 获取点赞的列表
        /// </summary>
        /// <param name="id">上次请求的Id</param>
        /// <param name="likeTime">上次请求的底部时间戳</param>
        /// <returns></returns>
        public async Task<FeedLikeResponse> GetLikeMeListAsync(long id, int likeTime = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("id", id.ToString());
            param.Add("at_time", likeTime.ToString());
            string url = BiliTool.UrlContact(Api.ACCOUNT_FEEDBACK_LIKE, param, true);
            var response = await BiliTool.ConvertEntityFromWebAsync<FeedLikeResponse>(url);
            return response;
        }
    }
}
