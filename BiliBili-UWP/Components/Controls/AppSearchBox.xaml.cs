using BiliBili_Lib.Enums;
using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class AppSearchBox : UserControl
    {
        private ObservableCollection<HotSearch> HotSearchCollection = new ObservableCollection<HotSearch>();
        private ObservableCollection<string> SearchHistoryCollection = new ObservableCollection<string>();
        public AppSearchBox()
        {
            this.InitializeComponent();
            Init();
        }
        public async void Init()
        {
            HotSearchCollection.Clear();
            SearchHistoryCollection.Clear();
            var hotSearch = await App.BiliViewModel._client.GetHotSearchAsync();
            if(hotSearch!=null && hotSearch.Count > 0)
            {
                hotSearch.ForEach(p => HotSearchCollection.Add(p));
            }
            string searchHistoryString = AppTool.GetLocalSetting(Settings.SearchHistory, "[]");
            var historyList = JsonConvert.DeserializeObject<List<string>>(searchHistoryString);
            historyList.ForEach(p => SearchHistoryCollection.Add(p));
        }
        private void BiliSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            BiliSearchBox.ItemsSource = HotSearchCollection;
            BiliSearchBox.ItemTemplate = HotSearchItemTemplate;
        }

        private void BiliSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                BiliSearchBox.ItemsSource = null;
                string key = string.Empty;
                if(args.ChosenSuggestion is HotSearch hot)
                {
                    key = hot.keyword;
                }
                else
                {
                    key = args.ChosenSuggestion.ToString();
                }
                App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.SearchPage), key);
            }
        }

        private void BiliSearchBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key==Windows.System.VirtualKey.Enter && !string.IsNullOrEmpty(BiliSearchBox.Text))
            {
                string key = BiliSearchBox.Text;
                if (SearchHistoryCollection.Count > 20)
                {
                    SearchHistoryCollection.RemoveAt(0);
                }
                SearchHistoryCollection.Add(key);
                string json = JsonConvert.SerializeObject(SearchHistoryCollection.ToList());
                AppTool.WriteLocalSetting(Settings.SearchHistory, json);
                App.AppViewModel.CurrentPagePanel.NavigateToSubPage(typeof(Pages.Sub.SearchPage), key);
            }
        }

        private void BiliSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(sender.Text))
            {
                BiliSearchBox.ItemsSource = HotSearchCollection;
                BiliSearchBox.ItemTemplate = HotSearchItemTemplate;
            }
            else
            {
                BiliSearchBox.ItemTemplate = HistoryItemTemplate;
                var match = SearchHistoryCollection.Where(p => p.Equals(sender.Text, StringComparison.OrdinalIgnoreCase));
                BiliSearchBox.ItemsSource = match;
            }
        }

        private void BiliSearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            BiliSearchBox.ItemsSource = null;
        }
    }
}
