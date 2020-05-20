using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Service;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class ReplyDetailPopup : UserControl, IAppPopup
    {
        private ObservableCollection<Reply> ReplyCollection = new ObservableCollection<Reply>();
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        private BiliBiliClient _client = App.BiliViewModel._client;
        private string _rootId;
        private string _oid;
        private string _type;
        private int _next = 0;
        private bool _isEnd = false;
        private bool _isRequesting = false;
        private string _selectReplyId = "";
        private int _prev = 0;
        public ReplyDetailPopup()
        {
            this.InitializeComponent();
            UIHelper.PopupInit(this);
        }
        public async Task Init(string replyId,string oid,string type)
        {
            LoadingRing.IsActive = true;
            _rootId = replyId;
            _oid = oid;
            _type = type;
            _next = 0;
            _prev = 0;
            _isEnd = false;
            _isRequesting = false;
            _selectReplyId = replyId;
            ReplyCollection.Clear();
            ReplyTextBox.ClearText();
            HeaderBlock.Visibility = Visibility.Visible;
            NoDataContainer.Visibility = Visibility.Collapsed;
            await LoadReply();
            LoadingRing.IsActive = false;
        }
        private async Task LoadReply()
        {
            if (_isRequesting || _isEnd)
                return;
            _isRequesting = true;
            if(!LoadingRing.IsActive)
            LoadingBar.Visibility = Visibility.Visible;
            var data = await _client.GetReplyDetailAsync(_rootId, _oid, _next, _type);
            if (data != null)
            {
                if (data.root == null)
                {
                    NoDataContainer.Visibility = Visibility.Visible;
                    HeaderBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (data.root.replies != null && data.root.replies.Count > 0)
                    {
                        foreach (var newItem in data.root.replies)
                        {
                            if (ReplyCollection.Contains(newItem))
                                continue;
                            ReplyCollection.Add(newItem);
                        }
                        data.root.replies = null;
                    }
                    _prev = _next;
                    _next = data.cursor.next;
                    _isEnd = data.cursor.is_end;
                    HeaderBlock.Data = data.root;
                    HolderText.Visibility = ReplyCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            _isRequesting = false;
            LoadingBar.Visibility = Visibility.Collapsed;
        }
        public void ShowPopup()
        {
            UIHelper.PopupShow(this, () =>
            {
                PopupContainer.Height = Window.Current.Bounds.Height * 0.8;
            });
            PopupContainer.Height = Window.Current.Bounds.Height * 0.8;
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            HeaderBlock.Data = null;
            HolderText.Visibility = Visibility.Collapsed;
            ReplyCollection.Clear();
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

        private void ReplyMainBlock_CommentButtonClick(object sender, Reply e)
        {
            _selectReplyId = e.rpid_str;
            ReplyTextBox.AtUser = e.member.uname;
            ReplyTextBox.PlaceholderText = $"回复 @{e.member.uname}：";
        }

        private async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var ele = sender as ScrollViewer;
            if (ele.ExtentHeight - ele.ViewportHeight - ele.VerticalOffset < 50)
            {
                await LoadReply();
            }
        }

        private async void ReplyTextBox_SendReply(object sender, string e)
        {
            if (!string.IsNullOrEmpty(ReplyTextBox.AtUser))
                e = $"回复 @{ReplyTextBox.AtUser} :" + e;
            var result = await _client.AddReplyAsync(_oid, e, _selectReplyId, _rootId, _type);
            if (result != null)
            {
                ReplyTextBox.ClearText();
                ReplyTextBox.PlaceholderText = "输入回复";
                if (result.parent_str == _rootId)
                {
                    ReplyCollection.Add(result);
                    ScrollViewer.ChangeView(0, 9999, 1);
                }
                else
                {
                    var parent = ReplyCollection.Where(p => p.rpid_str == result.parent_str).FirstOrDefault();
                    var index = ReplyCollection.IndexOf(parent);
                    ReplyCollection.Insert(index, result);
                }
                HolderText.Visibility = ReplyCollection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
