using BiliBili_Lib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BiliBili_Lib.Tools
{
    public class AppTool
    {
        /// <summary>
        /// 写入本地设置
        /// </summary>
        /// <param name="key">设置名</param>
        /// <param name="value">设置值</param>
        public static void WriteLocalSetting(Settings key, string value)
        {
            var localSetting = ApplicationData.Current.LocalSettings;
            var localcontainer = localSetting.CreateContainer("BiliBili", ApplicationDataCreateDisposition.Always);
            localcontainer.Values[key.ToString()] = value;
        }
        /// <summary>
        /// 读取本地设置
        /// </summary>
        /// <param name="key">设置名</param>
        /// <returns></returns>
        public static string GetLocalSetting(Settings key, string defaultValue)
        {
            var localSetting = ApplicationData.Current.LocalSettings;
            var localcontainer = localSetting.CreateContainer("BiliBili", ApplicationDataCreateDisposition.Always);
            bool isKeyExist = localcontainer.Values.ContainsKey(key.ToString());
            if (isKeyExist)
            {
                return localcontainer.Values[key.ToString()].ToString();
            }
            else
            {
                WriteLocalSetting(key, defaultValue);
                return defaultValue;
            }
        }
        /// <summary>
        /// 获取Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static int DateToTimeStamp(DateTime date)
        {
            TimeSpan ts = date - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            int seconds = Convert.ToInt32(ts.TotalSeconds);
            return seconds;
        }
        /// <summary>
        /// 转化Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static DateTime TimeStampToDate(int seconds)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(seconds);
            return date.ToLocalTime();
        }
        /// <summary>
        /// 转化Unix时间戳
        /// </summary>
        /// <returns></returns>
        public static DateTime TimeStampToDate(string seconds)
        {
            try
            {
                var date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToInt32(seconds));
                return date;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }

        }
        /// <summary>
        /// 获取数字的缩写
        /// </summary>
        /// <param name="number">数字</param>
        /// <returns></returns>
        public static string GetNumberAbbreviation(double number)
        {
            string result = string.Empty;
            if (number < 1000)
                result = number.ToString();
            if (number < 1000000)
                result = Math.Round(number / 1000.0, 1).ToString() + "K";
            else
                result = Math.Round(number / 1000000, 1).ToString() + "M";
            return result;
        }
    }
}
