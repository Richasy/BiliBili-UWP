using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BiliBili_Lib.Tools
{
    public class IOTool
    {
        /// <summary>
        /// 打开本地文件
        /// </summary>
        /// <param name="types">后缀名列表(如.jpg,.mp3等)</param>
        /// <returns>单个文件</returns>
        public async static Task<StorageFile> OpenLocalFileAsync(params string[] types)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            Regex typeReg = new Regex(@"^\.[a-zA-Z0-9]+$");
            foreach (var type in types)
            {
                if (type == "*" || typeReg.IsMatch(type))
                    picker.FileTypeFilter.Add(type);
                else
                    throw new InvalidCastException("文件后缀名不正确");
            }
            var file = await picker.PickSingleFileAsync();
            return file;
        }
        /// <summary>
        /// 获取保存的文件
        /// </summary>
        /// <param name="type">文件后缀名</param>
        /// <param name="name">文件名</param>
        /// <param name="adviceFileName">建议文件名</param>
        /// <returns></returns>
        public async static Task<StorageFile> GetSaveFileAsync(string type, string name, string adviceFileName)
        {
            var save = new FileSavePicker();
            save.DefaultFileExtension = type;
            save.SuggestedFileName = name;
            save.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            save.FileTypeChoices.Add(adviceFileName, new List<string>() { type });
            var file = await save.PickSaveFileAsync();
            return file;
        }
        /// <summary>
        /// 获取本地存储的数据并进行转化
        /// </summary>
        /// <typeparam name="T">转化类型</typeparam>
        /// <param name="path">文件名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public async static Task<T> GetLocalDataAsync<T>(string fileName, string defaultValue = "[]")
        {
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appdata:///local/"+fileName));
                string content = await FileIO.ReadTextAsync(file);
                if (string.IsNullOrEmpty(content))
                    content = defaultValue;
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception)
            {
                return JsonConvert.DeserializeObject<T>(defaultValue);
            }
        }

        /// <summary>
        /// 将数据存储到本地
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public async static Task SetLocalDataAsync(string fileName, string content)
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, content);
        }
    }
}
