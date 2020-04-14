using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
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
        public BiliBiliClient(string accessToken = "", string refreshToken = "", string userId = "", int expiry = 0)
        {
            BiliTool._accessToken = accessToken;
            var package = new TokenPackage(accessToken, refreshToken, userId, expiry);
            Account = new AccountService(package);
            Channel = new ChannelService();
            Topic = new TopicService();
            Video = new VideoService();
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
            param.Add("pull", (idx == 0).ToString().ToLower());
            string url = BiliTool.UrlContact(Api.VIDEO_RECOMMEND, param, true);
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoRecommend>>(url, "data.items");
            if (data != null && data.Count > 0)
            {
                data.RemoveAll(p => p.card_goto != "av");
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
        public async Task<SearchResult> GetComplexSearchResult(string keyword, string order = "default", int pn = 1,string region="",string duration="")
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
        public async Task<T> SearchTypeItems<T>(string keyword,int type, string order, int pn = 1,Dictionary<string,string> param=null) where T: class
        {
            if(param==null)
                param = new Dictionary<string, string>();
            param.Add("highlight", "0");
            param.Add("keyword", Uri.EscapeDataString(keyword));
            param.Add("pn", pn.ToString());
            param.Add("ps", "20");
            param.Add("order", order);
            param.Add("type", type.ToString());
            string url = BiliTool.UrlContact(Api.APP_SEARCH_TYPE, param, true);
            var result = await BiliTool.ConvertEntityFromWebAsync<T>(url,"data.items");
            return result;
        }
    }
}
