using NSDanmaku.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;

namespace NSDanmaku.Helper
{
    public class DanmakuParse
    {
        public async Task<List<NSDanmaku.Model.DanmakuModel>> ParseBiliBili(long cid)
        {
            WebHelper webHelper = new WebHelper();
            string danmuStr = await webHelper.GetResults(new Uri(string.Format("https://api.bilibili.com/x/v1/dm/list.so?oid={0}", cid)));
            return ParseBiliBiliXml(danmuStr);
        }

        public async Task<string> GetBiliBili(long cid)
        {

            WebHelper webHelper = new WebHelper();
            string danmuStr = await webHelper.GetResults(new Uri(string.Format("https://api.bilibili.com/x/v1/dm/list.so?oid={0}", cid)));
            return danmuStr;
        }


        public List<NSDanmaku.Model.DanmakuModel> ParseBiliBili(string xml)
        {
            return ParseBiliBiliXml(xml);
        }
        public async Task<List<NSDanmaku.Model.DanmakuModel>> ParseBiliBili(Uri url)
        {
            WebHelper webHelper = new WebHelper();
            string danmuStr = await webHelper.GetResults(url);
            return ParseBiliBiliXml(danmuStr);
        }
        public async Task< List<NSDanmaku.Model.DanmakuModel>> ParseBiliBili(Windows.Storage.StorageFile file)
        {
            string danmuStr = await FileIO.ReadTextAsync(file);
            return  ParseBiliBiliXml(danmuStr);
        }
        private List<NSDanmaku.Model.DanmakuModel> ParseBiliBiliXml(string xmlStr)
        {
            List<DanmakuModel> ls = new List<DanmakuModel>();
          
            XmlDocument xdoc = new XmlDocument();
            //处理下特殊字符
            xmlStr = Regex.Replace(xmlStr, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
            xdoc.LoadXml(xmlStr);
            XmlElement el = xdoc.DocumentElement;
            XmlNodeList xml = el.ChildNodes;
            foreach (XmlNode item in xml)
            {
                if (item.Attributes["p"] != null)
                {
                    try
                    {
                        string heheda = item.Attributes["p"].Value;
                        string[] haha = heheda.Split(',');
                        var location = DanmakuLocation.Roll;
                        switch (haha[1])
                        {
                            case "7":
                                location = DanmakuLocation.Position;
                                break;
                            case "4":
                                location = DanmakuLocation.Bottom;
                                break;
                            case "5":
                                location = DanmakuLocation.Top;
                                break;
                            default:
                                location = DanmakuLocation.Roll;
                                break;
                        }
                        ls.Add(new DanmakuModel
                        {
                            time = double.Parse(haha[0]),
                            time_s = Convert.ToInt32(double.Parse(haha[0])),
                            location = location,
                            size = double.Parse(haha[2]),
                            color = haha[3].ToColor(),
                            sendTime = haha[4],
                            pool = haha[5],
                            sendID = haha[6],
                            rowID = haha[7],
                            text = item.InnerText,
                            source = item.OuterXml,
                            fromSite = DanmakuSite.Bilibili
                        });
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
            }
            return ls;

        }
    }
}
