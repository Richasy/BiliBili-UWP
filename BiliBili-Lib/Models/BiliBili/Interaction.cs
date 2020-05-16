using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiliBili_Lib.Models.BiliBili
{

    public class InteractionVideo
    {
        public string title { get; set; }
        public int edge_id { get; set; }
        public List<StoryItem> story_list { get; set; }
        public Edges edges { get; set; }
        public Preload preload { get; set; }
        public int is_leaf { get; set; }
        public List<HiddenVariable> hidden_vars { get; set; }
    }

    public class Edges
    {
        public List<Question> questions { get; set; }
    }

    public class Question
    {
        public int id { get; set; }
        public int type { get; set; }
        public int start_time_r { get; set; }
        public int duration { get; set; }
        public int pause_video { get; set; }
        public string title { get; set; }
        public List<Choice> choices { get; set; }
    }

    public class Choice
    {
        public int id { get; set; }
        public string platform_action { get; set; }
        public string native_action { get; set; }
        public string condition { get; set; }
        public int cid { get; set; }
        public string option { get; set; }
        public int is_default { get; set; }
    }

    public class Preload
    {
        public List<PreloadVideo> video { get; set; }
    }

    public class PreloadVideo
    {
        public int aid { get; set; }
        public int cid { get; set; }
    }

    public class StoryItem
    {
        public int node_id { get; set; }
        public int edge_id { get; set; }
        public string title { get; set; }
        public int cid { get; set; }
        public int start_pos { get; set; }
        public string cover { get; set; }
        public int is_current { get; set; }
        public int cursor { get; set; }
    }

    public class HiddenVariable
    {
        public int value { get; set; }
        public string id { get; set; }
        public string id_v2 { get; set; }
        public int type { get; set; }
        public int is_show { get; set; }
        public string name { get; set; }
        public int skip_overwrite { get; set; }
    }

}
