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
        public BiliBiliClient(string accessToken = "", string refreshToken = "", string userId = "", int expiry = 0)
        {
            BiliTool._accessToken = accessToken;
            var package = new TokenPackage(accessToken, refreshToken, userId, expiry);
            Account = new AccountService(package);
            Channel = new ChannelService();
            Topic = new TopicService();
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
            var data = await BiliTool.ConvertEntityFromWebAsync<List<VideoRecommend>>(url,"data.items");
            if (data != null && data.Count > 0)
            {
                data.RemoveAll(p => p.card_goto != "av");
                return data;
            }
            return new List<VideoRecommend>();
        }
    }
}
