using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.Others
{
    public class Api
    {
        /*
         * @pn: 页码，从1开始
         * @ps: 一页条目数量
         */
        public const string _apiBase = "https://api.bilibili.com";
        public const string _appBase = "https://app.bilibili.com";
        public const string _vcBase = "https://api.vc.bilibili.com";
        public const string _liveBase = "https://api.live.bilibili.com";
        public const string _passBase = "https://passport.bilibili.com";

        #region 验证
        /// <summary>
        /// 字符串加密
        /// </summary>
        public const string PASSPORT_KEY_ENCRYPT = _passBase + "/api/oauth2/getKey";
        /// <summary>
        /// 登录
        /// </summary>
        public const string PASSPORT_LOGIN = _passBase + "/api/v3/oauth2/login";
        /// <summary>
        /// 刷新令牌信息
        /// </summary>
        public const string PASSPORT_REFRESH_TOKEN = _passBase + "/api/oauth2/refreshToken";
        /// <summary>
        /// 验证令牌是否有效
        /// </summary>
        public const string PASSPORT_CHECK_TOKEN = _passBase + "/api/oauth2/info";
        /// <summary>
        /// SSO
        /// </summary>
        public const string PASSPORT_SSO = _passBase + "/api/login/sso";
        /// <summary>
        /// 获取验证码
        /// </summary>
        public const string PASSPORT_CAPTCHA = _passBase + "/captcha";
        #endregion

        #region 分区 Region
        /// <summary>
        /// 分区索引（包含子项）
        /// </summary>
        public const string REGION_INDEX = _appBase + "/x/v2/region/index";
        /// <summary>
        /// 分区初始加载
        /// </summary>
        public const string REGION_DYNAMIC_INIT = _appBase + "/x/v2/region/dynamic";
        /// <summary>
        /// 分区初始增量加载
        /// </summary>
        public const string REGION_DYNAMIC_REFRESH = _appBase + "/x/v2/region/dynamic/list";
        /// <summary>
        /// 分区子项初始加载
        /// </summary>
        public const string REGION_DYNAMIC_CHILD_INIT = _appBase + "/x/v2/region/dynamic/child";
        /// <summary>
        /// 分区子项初始增量加载
        /// </summary>
        public const string REGION_DYNAMIC_CHILD_REFRESH = _appBase + "/x/v2/region/dynamic/child/list";
        /// <summary>
        /// 分区子项按照一定规则排序后加载，分页
        /// </summary>
        public const string REGION_DYNAMIC_CHILD_SORT = _appBase + "/x/v2/region/show/child/list";
        /// <summary>
        /// 分区排行榜
        /// </summary>
        public const string REGION_RANK = _apiBase + "/x/web-interface/ranking/region";
        #endregion

        #region 账户 Account
        /// <summary>
        /// 我的信息
        /// </summary>
        public const string ACCOUNT_MINE = _appBase + "/x/v2/account/mine";
        /// <summary>
        /// 关注列表
        /// @分组索引使用vmid
        /// @分组详情用mid,pn,ps,tagid
        /// </summary>
        public const string ACCOUNT_FOLLOWING = _apiBase + "/x/relation/tags";
        /// <summary>
        /// 粉丝列表，使用vmid
        /// </summary>
        public const string ACCOUNT_FAN = _apiBase + "/x/relation/followers";
        /// <summary>
        /// 查询用户信息，使用vmid
        /// </summary>
        public const string ACCOUNT_INFO = _appBase + "/x/v2/space";
        /// <summary>
        /// 查询用户动态，使用vmid
        /// </summary>
        public const string ACCOUNT_USER_DYNAMIC = _vcBase + "/x/v2/space";
        /// <summary>
        /// 关注用户
        /// </summary>
        public const string ACCOUNT_FOLLOW_USER = _vcBase + "/feed/v1/feed/follow";
        /// <summary>
        /// 取消关注用户
        /// </summary>
        public const string ACCOUNT_UNFOLLOW_USER = _vcBase + "/feed/v1/feed/unfollow";
        #endregion

        #region 频道 Channel
        /// <summary>
        /// 获取热门频道（通过offset进行刷新，一次+5）
        /// </summary>
        public const string CHANNEL_HOT = _appBase + "/x/v2/channel/recommend2";
        /// <summary>
        /// 获取频道页综合信息（包括订阅，热门频道，浏览的信息等）
        /// </summary>
        public const string CHANNEL_SQUARE = _appBase + "/x/v2/channel/square2";
        /// <summary>
        /// 获取频道的详细说明
        /// </summary>
        public const string CHANNEL_DETAIL = _appBase + "/x/v2/channel/detail";
        /// <summary>
        /// 获取频道下的视频列表
        /// </summary>
        public const string CHANNEL_VIDEO = _appBase + "/x/v2/channel/multiple";
        /// <summary>
        /// 取消频道订阅
        /// </summary>
        public const string CHANNEL_UNSUBSCRIBE = _appBase + "/x/channel/cancel";
        /// <summary>
        /// 添加频道订阅
        /// </summary>
        public const string CHANNEL_SUBSCRIBE = _appBase + "/x/channel/add";
        /// <summary>
        /// 获取频道列表分类
        /// </summary>
        public const string CHANNEL_TABS = _appBase + "/x/v2/channel/tab3";
        /// <summary>
        /// 获取某分类下频道列表
        /// </summary>
        public const string CHANNEL_LIST = _appBase + "/x/v2/channel/list";
        /// <summary>
        /// 获取我订阅的频道（包括标签）
        /// </summary>
        public const string CHANNEL_MYSUBSCRIBE = _appBase + "/x/v2/channel/mine";
        /// <summary>
        /// 搜索频道
        /// </summary>
        public const string CHANNEL_SEARCH = _appBase + "/x/v2/search/channel2";
        /// <summary>
        /// 标签基础信息
        /// </summary>
        public const string CHANNEL_TAG_TAB = _appBase + "/x/channel/feed/tab";
        /// <summary>
        /// 标签推荐视频
        /// </summary>
        public const string CHANNEL_TAG_RECOMMEND = _appBase + "/x/channel/feed/index";
        #endregion

        #region 应用 Application
        /// <summary>
        /// 热搜及其它搜索条目
        /// </summary>
        public const string APP_SEARCH_HOT = _appBase + "/x/v2/search/square";
        /// <summary>
        /// 综合搜索
        /// </summary>
        public const string APP_SEARCH_COMPLEX = _appBase + "/x/v2/search";
        /// <summary>
        /// 特殊类目搜索
        /// </summary>
        public const string APP_SEARCH_TYPE = _appBase + "/x/v2/search/type";
        #endregion

        #region 视频 Video
        /// <summary>
        /// 推荐视频
        /// </summary>
        public const string VIDEO_RECOMMEND = _appBase + "/x/v2/feed/index";
        /// <summary>
        /// 视频分P列表
        /// </summary>
        public const string VIDEO_PART = _apiBase + "/x/player/pagelist";
        /// <summary>
        /// 视频详细信息
        /// </summary>
        public const string VIDEO_DETAIL_INFO = _apiBase + "/x/web-interface/view";
        /// <summary>
        /// 视频简易信息
        /// </summary>
        public const string VIDEO_SLIM_INFO = _apiBase + "/x/web-interface/archive/stat";
        /// <summary>
        /// 关联视频
        /// </summary>
        public const string VIDEO_RELATED = _apiBase + "/x/web-interface/archive/related";
        /// <summary>
        /// 获取播放地址
        /// </summary>
        public const string VIDEO_PLAY = _apiBase + "/x/player/playurl";
        /// <summary>
        /// 是否已点赞（视频）
        /// </summary>
        public const string VIDEO_IS_LIKE = _apiBase + "/x/web-interface/archive/has/like";
        /// <summary>
        /// 是否已投币（视频）
        /// </summary>
        public const string VIDEO_IS_COIN = _apiBase + "/x/web-interface/archive/coins";
        /// <summary>
        /// 是否已收藏（视频）
        /// </summary>
        public const string VIDEO_IS_FAVORITE = _apiBase + "/x/v2/fav/video/favoured";

        #endregion

        #region 话题及动态 Topic Dynamic
        /// <summary>
        /// 获取动态
        /// </summary>
        public const string TOPIC_COMPLEX = _vcBase + "/topic_svr/v1/topic_svr/fetch_dynamics";
        /// <summary>
        /// 点赞动态
        /// </summary>
        public const string DYNAMIC_LIKE = _vcBase + "/dynamic_like/v1/dynamic_like/thumb";
        #endregion

        #region 动漫 Anime
        /// <summary>
        /// 番剧综合信息
        /// </summary>
        public const string ANIME_JP_SQUARE = _apiBase + "/pgc/app/v2/page/bangumi/jp";
        /// <summary>
        /// 刷新动漫区块信息
        /// </summary>
        public const string ANIME_EXCHANGE = _apiBase + "/pgc/app/v2/page/exchange";
        /// <summary>
        /// 国创综合信息
        /// </summary>
        public const string ANIME_CHN_SQUARE = _apiBase + "/pgc/app/v2/page/bangumi/chn";
        #endregion
    }
}
