using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class Topic
    {
        public TopicDescription desc { get; set; }
        public string card { get; set; }
        public string extend_json { get; set; }
        public TopicDisplay display { get; set; }
    }

    public class TopicOrigin
    {
        public int uid { get; set; }
        public int type { get; set; }
        public long rid { get; set; }
        public int acl { get; set; }
        public int view { get; set; }
        public int repost { get; set; }
        public int like { get; set; }
        public long dynamic_id { get; set; }
        public int timestamp { get; set; }
        public int pre_dy_id { get; set; }
        public int orig_dy_id { get; set; }
        public int uid_type { get; set; }
        public int stype { get; set; }
        public int r_type { get; set; }
        public int inner_id { get; set; }
        public int status { get; set; }
        public string dynamic_id_str { get; set; }
        public string pre_dy_id_str { get; set; }
        public string orig_dy_id_str { get; set; }
        public string rid_str { get; set; }
        public string bvid { get; set; }
    }

    public class TopicDescription
    {
        public int uid { get; set; }
        public int type { get; set; }
        public string rid { get; set; }
        public int acl { get; set; }
        public int view { get; set; }
        public int repost { get; set; }
        public int like { get; set; }
        public int comment { get; set; }
        public int is_liked { get; set; }
        public string dynamic_id { get; set; }
        public int timestamp { get; set; }
        public int orig_type { get; set; }
        public UserProfile user_profile { get; set; }
        public TopicOrigin origin { get; set; }
        public int uid_type { get; set; }
        public int stype { get; set; }
        public int r_type { get; set; }
        public long inner_id { get; set; }
        public string topic_board { get; set; }
        public string topic_board_desc { get; set; }
        public int status { get; set; }
        public string dynamic_id_str { get; set; }
        public string pre_dy_id_str { get; set; }
        public string orig_dy_id_str { get; set; }
        public string rid_str { get; set; }
        public string bvid { get; set; }
    }

    public class UserProfile
    {
        public SlimUserInfo info { get; set; }
        public Pendant pendant { get; set; }
        public string rank { get; set; }
        public string sign { get; set; }
        public LevelInfo level_info { get; set; }
    }

    public class TopicDisplay
    {
        public TopicInfo topic_info { get; set; }
        public EmojiInfo emoji_info { get; set; }
        public string usr_action_txt { get; set; }
        public Relation relation { get; set; }
    }
    public class EmojiInfo
    {
        public List<Emote> emoji_details { get; set; }
    }

    public class TopicInfo
    {
        public List<TopicDetail> topic_details { get; set; }
    }

    public class TopicDetail
    {
        public int topic_id { get; set; }
        public string topic_name { get; set; }
        public int is_activity { get; set; }
        public string topic_link { get; set; }
    }

    public class Relation
    {
        public int status { get; set; }
        public int is_follow { get; set; }
        public int is_followed { get; set; }
    }

}
