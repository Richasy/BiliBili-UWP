using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Service
{
    public class BiliBiliClient
    {
        public AccountService Account;
        public ChannelService Channel;
        public TopicService Topic;
        public VideoService Video;
        public AnimeService Anime;
        public BiliBiliClient(string accessToken = "", string refreshToken = "", int expiry = 0)
        {
            BiliTool._accessToken = accessToken;
            var package = new TokenPackage(accessToken, refreshToken, expiry);
            Account = new AccountService(package);
            Channel = new ChannelService();
            Topic = new TopicService();
            Video = new VideoService();
            Anime = new AnimeService();
        }

        /// <summary>
        /// 获取分区索引
        /// </summary>
        /// <returns></returns>
        public async Task<List<RegionContainer>> GetRegionIndexAsync()
        {
            string url = BiliTool.UrlContact(Api.REGION_INDEX);
            var result = await BiliTool.ConvertEntityFromWebAsync<List<RegionContainer>>(url);
            return result;
        }

        /// <summary>
        /// 获取首页推荐视频
        /// </summary>
        /// <param name="idx">上一次刷新最后一个视频的标识</param>
        /// <returns></returns>
        public async Task<List<VideoRecommend>> GetRecommendVideoAsync(int idx = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("idx", idx.ToString());
            param.Add("flush", "0");
            param.Add("column", "4");
            param.Add("device", "pad");
            param.Add("pull", (idx == 0).ToString().ToLower());
            string url = BiliTool.UrlContact(Api.VIDEO_RECOMMEND, param, true,useiPhone:true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoRecommend>>(url, "data.items");
            if (data != null && data.Count > 0)
            {
                data.RemoveAll(p => p.card_goto != "av" && p.card_goto != "bangumi");
                return data;
            }
            return new List<VideoRecommend>();
        }
        /// <summary>
        /// 获取热搜列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<HotSearch>> GetHotSearchAsync()
        {
            var param = new Dictionary<string, string>();
            param.Add("from", "0");
            param.Add("show", "0");
            param.Add("limit", "20");
            string url = BiliTool.UrlContact(Api.APP_SEARCH_HOT, param);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<HotSearch>>(url, "data[0].data.list");
            return data;
        }
        /// <summary>
        /// 综合搜索
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="order">排序方式（default,view,new,danmaku）</param>
        /// <param name="pn">页码</param>
        /// <returns></returns>
        public async Task<SearchResult> GetComplexSearchResult(string keyword, string order = "default", int pn = 1, string region = "", string duration = "")
        {
            var param = new Dictionary<string, string>();
            param.Add("highlight", "0");
            param.Add("is_org_query", "0");
            param.Add("keyword", Uri.EscapeDataString(keyword));
            param.Add("pn", pn.ToString());
            param.Add("ps", "20");
            param.Add("recommend", "0");
            param.Add("rid", region);
            param.Add("order", order);
            param.Add("duration", duration);
            string url = BiliTool.UrlContact(Api.APP_SEARCH_COMPLEX, param, true);
            var result = await BiliTool.ConvertEntityFromWebAsync<SearchResult>(url);
            return result;
        }
        /// <summary>
        /// 搜索指定区块的内容
        /// </summary>
        /// <typeparam name="T">内容类型</typeparam>
        /// <param name="keyword">关键词</param>
        /// <param name="type">区块ID</param>
        /// <param name="order">排序方式</param>
        /// <param name="pn">页码</param>
        /// <param name="param">可选参数</param>
        /// <returns></returns>
        public async Task<T> SearchTypeItems<T>(string keyword, int type, string order, int pn = 1, Dictionary<string, string> param = null) where T : class
        {
            if (param == null)
                param = new Dictionary<string, string>();
            param.Add("highlight", "0");
            param.Add("keyword", Uri.EscapeDataString(keyword));
            param.Add("pn", pn.ToString());
            param.Add("ps", "20");
            param.Add("order", order);
            param.Add("type", type.ToString());
            string url = BiliTool.UrlContact(Api.APP_SEARCH_TYPE, param, true);
            var result = await BiliTool.ConvertEntityFromWebAsync<T>(url, "data.items");
            return result;
        }
        public async Task<List<Owner>> SearchUserAsync(string keyword,bool isFromDynamic=true)
        {
            var param = new Dictionary<string, string>();
            param.Add("highlight", "0");
            if(isFromDynamic)
                param.Add("from_source", "dynamic_uname");
            param.Add("keyword", Uri.EscapeDataString(keyword));
            param.Add("order", "totalrank");
            param.Add("order_sort", "0");
            param.Add("pn", "0");
            param.Add("ps", "10");
            param.Add("user_type", "0");
            string url = BiliTool.UrlContact(Api.APP_SEARCH_USER, param, true);
            var result = await BiliTool.ConvertEntityFromWebAsync<List<Owner>>(url, "data.items");
            return result;
        }
        /// <summary>
        /// 获取搜索建议
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public async Task<List<SearchSuggestion>> GetSearchSuggestionAsync(string keyword)
        {
            var param = new Dictionary<string, string>();
            param.Add("suggest_type", "accurate");
            param.Add("sub_type", "tag");
            param.Add("main_ver", "v1");
            param.Add("term", keyword);
            string url = BiliTool.UrlContact(Api.APP_SEARCH_SUGGEST, param);
            var result = await BiliTool.ConvertEntityFromWebAsync<List<SearchSuggestion>>(url, "result.tag");
            return result;
        }
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="oid">源ID</param>
        /// <param name="next">下一页偏移值</param>
        /// <param name="mode">排序方式：3-按热度，2-按时间</param>
        /// <returns>Item1：下一次偏移值，Item2：评论总数，Item3：评论列表，Item4: 是否到了结尾，Item5: 置顶回复</returns>
        public async Task<Tuple<int, int, List<Reply>, bool, Reply>> GetReplyAsync(string oid, int next, int mode, string type = "1")
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", oid);
            param.Add("next", next.ToString());
            param.Add("prev", "0");
            param.Add("type", type);
            param.Add("mode", mode.ToString());
            param.Add("ps", "30");
            param.Add("plat", "3");
            var url = BiliTool.UrlContact(Api.REPLY_LIST, param, true);
            string response = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var jobj = JObject.Parse(response);
                    int ne = Convert.ToInt32(jobj["cursor"]["next"].ToString());
                    int all = Convert.ToInt32(jobj["cursor"]["all_count"].ToString());
                    bool isEnd = Convert.ToBoolean(jobj["cursor"]["is_end"].ToString());
                    Reply top = null;
                    if (jobj["top"]["upper"].Type != JTokenType.Null)
                    {
                        top = JsonConvert.DeserializeObject<Reply>(jobj["top"]["upper"].ToString());
                    }
                    var replies = JsonConvert.DeserializeObject<List<Reply>>(jobj["replies"].ToString());
                    return new Tuple<int, int, List<Reply>, bool, Reply>(ne, all, replies, isEnd, top);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
                return null;
        }
        /// <summary>
        /// 获取评论详情
        /// </summary>
        /// <param name="replyId">评论ID</param>
        /// <param name="oid">源ID</param>
        /// <param name="next">偏移值</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public async Task<ReplyDetailResponse> GetReplyDetailAsync(string replyId, string oid, int next, string type = "1")
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", oid);
            param.Add("root", replyId);
            param.Add("next", next.ToString());
            param.Add("prev", "0");
            param.Add("type", type);
            param.Add("ps", "30");
            param.Add("plat", "3");
            var url = BiliTool.UrlContact(Api.REPLY_DETAIL, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<ReplyDetailResponse>(url);
            return data;
        }
        /// <summary>
        /// 点赞/取消点赞评论
        /// </summary>
        /// <param name="isLike">是否点赞</param>
        /// <param name="oid">源ID</param>
        /// <param name="rpid">评论ID</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public async Task<bool> LikeReplyAsync(bool isLike, string oid, string rpid, string type = "1")
        {
            string like = isLike ? "1" : "0";
            var param = new Dictionary<string, string>();
            param.Add("oid", oid);
            param.Add("rpid", rpid);
            param.Add("type", type);
            param.Add("action", like);
            var data = BiliTool.UrlContact("", param, true);
            string response = await BiliTool.PostContentToWebAsync(Api.REPLY_LIKE, data);
            var jobj = JObject.Parse(response);
            return jobj["code"].ToString() == "0";
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="oid">源ID</param>
        /// <param name="message">信息</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public async Task<bool> AddReplyAsync(string oid, string message, string type = "1")
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", oid);
            param.Add("type", type);
            param.Add("message", Uri.EscapeDataString(message));
            param.Add("lottery", "0");
            param.Add("vote", "0");
            var data = BiliTool.UrlContact("", param, true);
            string response = await BiliTool.PostContentToWebAsync(Api.REPLY_ADD, data);
            var jobj = JObject.Parse(response);
            if (jobj.ContainsKey("data"))
            {
                return jobj["data"]["success_action"].ToString() == "0";
            }
            return false;
        }
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="oid">源ID</param>
        /// <param name="message">信息</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public async Task<Reply> AddReplyAsync(string oid, string message, string parentId, string rootId, string type = "1")
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", oid);
            param.Add("parent", parentId);
            param.Add("root", rootId);
            param.Add("type", type);
            param.Add("message", Uri.EscapeDataString(message));
            param.Add("lottery", "0");
            param.Add("vote", "0");
            var data = BiliTool.UrlContact("", param, true);
            try
            {
                string response = await BiliTool.PostContentToWebAsync(Api.REPLY_ADD, data);
                var jobj = JObject.Parse(response);
                if (jobj.ContainsKey("data"))
                {
                    return JsonConvert.DeserializeObject<Reply>(jobj["data"]["reply"].ToString());
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 获取Emoji表情列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<EmojiContainer>> GetTotalEmojiAsync()
        {
            string response = await BiliTool.GetTextFromWebAsync(Api.APP_EMOJI);
            var list = new List<EmojiContainer>();
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                if (jobj.ContainsKey("vip"))
                {
                    var vips = JsonConvert.DeserializeObject<List<EmojiContainer>>(jobj["vip"].ToString());
                    list = list.Concat(vips).ToList();
                }
                if (jobj.ContainsKey("free"))
                {
                    var frees = JsonConvert.DeserializeObject<List<EmojiContainer>>(jobj["free"].ToString());
                    list = list.Concat(frees).ToList();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取关注者的未读消息数
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetFollowerUnreadCountAsync()
        {
            string url = BiliTool.UrlContact(Api.APP_FOLLOWER_UNREAD, null, true);
            string response = await BiliTool.GetTextFromWebAsync(url);
            if (!string.IsNullOrEmpty(response))
            {
                var jobj = JObject.Parse(response);
                return Convert.ToInt32(jobj["count"].ToString());
            }
            return -1;
        }

        /// <summary>
        /// 测试请求，验证网络是否可访问
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ValidateRequestAsync()
        {
            string url = Api.OTHER_ZONE;
            string response = await BiliTool.GetTextFromWebAsync(url, true);
            try
            {
                if (!string.IsNullOrEmpty(response))
                {
                    var jobj = JObject.Parse(response);
                    if (jobj["code"].ToString() == "0")
                        return true;
                }
            }
            catch (Exception ex)
            {
                BiliTool._logger.Error($"测试请求失败", ex);
                return false;
            }
            return false;
        }
    }
}
