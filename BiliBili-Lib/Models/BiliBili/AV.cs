using BiliBili_Lib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{
    public class AV
    {
        public int aid { get; set; }
        public int attribute { get; set; }
        public int cid { get; set; }
        public string comment_jump_url { get; set; }
        public int copyright { get; set; }
        public int ctime { get; set; }
        public string desc { get; set; }
        public Dimension dimension { get; set; }
        public int duration { get; set; }
        public string dynamic { get; set; }
        public string jump_url { get; set; }
        public Owner owner { get; set; }
        public string pic { get; set; }
        public PlayerInfo player_info { get; set; }
        public int pubdate { get; set; }
        public Rights rights { get; set; }
        public string share_subtitle { get; set; }
        public Stat stat { get; set; }
        public int state { get; set; }
        public int tid { get; set; }
        public string title { get; set; }
        public string tname { get; set; }
        public int videos { get; set; }
        public string render_pic
        {
            get => pic + "@300w.jpg";
        }
        public class Dimension
        {
            public int height { get; set; }
            public int rotate { get; set; }
            public int width { get; set; }
        }

        public class Owner
        {
            public string face { get; set; }
            public int mid { get; set; }
            public string name { get; set; }
        }

        public class PlayerInfo
        {
            public int cid { get; set; }
            public Dash dash { get; set; }
            public int expire_time { get; set; }
            public FileInfo file_info { get; set; }
            public int fnval { get; set; }
            public int fnver { get; set; }
            public int quality { get; set; }
            public string[] support_description { get; set; }
            public string[] support_formats { get; set; }
            public int[] support_quality { get; set; }
            public int video_codecid { get; set; }
            public bool video_project { get; set; }
        }

        public class Dash
        {
            public List<Audio> audio { get; set; }
            public List<Video> video { get; set; }
        }

        public class Audio
        {
            public int bandwidth { get; set; }
            public string base_url { get; set; }
            public int codecid { get; set; }
            public int id { get; set; }
            public int size { get; set; }
        }

        public class Video
        {
            public int bandwidth { get; set; }
            public string base_url { get; set; }
            public int codecid { get; set; }
            public int id { get; set; }
            public int size { get; set; }
        }

        public class FileInfo
        {
            public FileInfoItem _16 { get; set; }
            public FileInfoItem _32 { get; set; }
            public FileInfoItem _64 { get; set; }
        }

        public class FileInfoItem
        {
            public Info[] infos { get; set; }
        }

        public class Info
        {
            public int filesize { get; set; }
            public int timelength { get; set; }
        }

        public class Rights
        {
            public int autoplay { get; set; }
            public int bp { get; set; }
            public int download { get; set; }
            public int elec { get; set; }
            public int hd5 { get; set; }
            public int is_cooperation { get; set; }
            public int movie { get; set; }
            public int no_background { get; set; }
            public int no_reprint { get; set; }
            public int pay { get; set; }
            public int ugc_pay { get; set; }
            public int ugc_pay_preview { get; set; }
        }

        public class Stat
        {
            public int aid { get; set; }
            public int coin { get; set; }
            public int danmaku { get; set; }
            public int dislike { get; set; }
            public int favorite { get; set; }
            public int his_rank { get; set; }
            public int like { get; set; }
            public int now_rank { get; set; }
            public int reply { get; set; }
            public int share { get; set; }
            public int view { get; set; }
            public string play_abbr
            {
                get => AppTool.GetNumberAbbreviation(view);
            }
            public string danmaku_abbr
            {
                get => AppTool.GetNumberAbbreviation(danmaku);
            }
        }
    }

    

}
