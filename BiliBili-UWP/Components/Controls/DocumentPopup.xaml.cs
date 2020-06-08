using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.Others;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Dialogs;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class DocumentPopup : UserControl, IAppPopup
    {
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        private int _articleId;
        public DocumentPopup()
        {
            this.InitializeComponent();
            UIHelper.PopupInit(this);
        }
        public async void Init(string title, int id)
        {
            _articleId = id;
            TitleBlock.Text = title;
            LoadingRing.IsActive = true;
            string content = await GetArticleContent();
            string html = await PackageHTML(content);
            DetailWebView.NavigateToString(html);
        }
        private async Task<string> GetArticleContent()
        {
            string url = $"https://www.bilibili.com/read/app/{_articleId}";
            string text = await AppTool.GetHtmlFromWeb(url);
            if (!string.IsNullOrEmpty(text))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(text);
                var node = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-holder')]").FirstOrDefault();
                if (node != null)
                {
                    return node.InnerHtml;
                }
            }
            return "";
        }
        /// <summary>
        /// 包装HTML页面
        /// </summary>
        /// <param name="content">文章主题</param>
        /// <returns></returns>
        private async Task<string> PackageHTML(string content)
        {
            content = content.Replace("=\"//", "=\"http://");
            content = content.Replace("data-src", "src");
            string html = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Components/HTML/ShowPage.html")));
            string theme = Application.Current.RequestedTheme.ToString();
            string css = await FileIO.ReadTextAsync(await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Components/HTML/{theme}.css")));
            string fontFamily = AppTool.GetLocalSetting(Settings.FontFamily, "微软雅黑");
            string fontSize = AppTool.GetLocalSetting(Settings.BasicFontSize, "14");
            css = css.Replace("$FontFamily$", fontFamily).Replace("$FontSize$", fontSize);
            string result = html.Replace("$theme$", theme.ToLower()).Replace("$style$", css).Replace("$body$", content);
            result = result.Replace("$noscroll$", "style=\"-ms-overflow-style: none;\"");
            result = result.Replace("$return$", "");
            return result;
        }
        public void ShowPopup()
        {
            UIHelper.PopupShow(this, () =>
            {
                DisplayContainer.Height = Window.Current.Bounds.Height * 0.8;
            });
            DisplayContainer.Height = Window.Current.Bounds.Height * 0.8;
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            DetailWebView.NavigateToString("");
            PopupOut.Begin();
            PopupOut.Completed -= PopupOut_Completed;
            PopupOut.Completed += PopupOut_Completed;
        }
        private void PopupOut_Completed(object sender, object e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == _popupId);
            _popup.IsOpen = false;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            HidePopup();
        }

        private async void NewWebButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri($"https://www.bilibili.com/read/cv{_articleId}"));
        }

        private void DetailWebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            LoadingRing.IsActive = false;
            DetailWebView.Focus(FocusState.Programmatic);
            if (string.IsNullOrEmpty(TitleBlock.Text))
            {
                TitleBlock.Text = DetailWebView.DocumentTitle;
            }
        }

        private async void DetailWebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<KeyValueModel>(e.Value);
                // 图片点击事件，弹出图片对话框
                if (data.Key == "ImageClick" && !string.IsNullOrEmpty(data.Value))
                {
                    var imageDialog = new ShowImageDialog(data.Value);
                    await imageDialog.ShowAsync();
                }
                // 文本选中事件
                else if (data.Key == "SelectText" && !string.IsNullOrEmpty(data.Value))
                {
                }
                else if (data.Key == "LinkClick" && !string.IsNullOrEmpty(data.Value))
                {
                    try
                    {
                        await Launcher.LaunchUriAsync(new Uri(data.Value));
                    }
                    catch (Exception)
                    {
                        new TipPopup("无法解析该链接的内容").ShowError();
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void DetailWebView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
