using BiliBili_Lib.Models.BiliBili;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Components.Widgets;
using BiliBili_UWP.Models.UI;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili_UWP.Components.Controls
{
    public sealed partial class DynamicDetailPopup : UserControl,IAppPopup
    {
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        private FlipView _imageFlipView;
        public DynamicDetailPopup()
        {
            this.InitializeComponent();
            UIHelper.PopupInit(this);
        }
        public void ShowPopup()
        {
            UIHelper.PopupShow(this,()=>
            {
                if (Window.Current.Bounds.Width < 800)
                    ReplyContainer.Visibility = Visibility.Collapsed;
                else
                    ReplyContainer.Visibility = Visibility.Visible;
            });
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            PopupOut.Completed -= PopupOut_Completed;
            PopupOut.Completed += PopupOut_Completed;
            PopupOut.Begin();
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

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(DynamicDetailPopup), new PropertyMetadata(null,new PropertyChangedCallback(Data_Changed)));

        private static void Data_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                var instance = d as DynamicDetailPopup;
                if(e.NewValue is ImageDynamic img)
                {
                    instance.MainDisplay.ContentTemplate = instance.ImageTemplate;
                    instance.MainDisplay.Content = img;
                }
                else if(e.NewValue is DocumentDynamic doc)
                {
                    instance.MainDisplay.ContentTemplate = instance.DocumentTemplate;
                    instance.MainDisplay.Content = doc;
                }
            }
        }

        public SlimUserInfo User
        {
            get { return (SlimUserInfo)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for User.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(SlimUserInfo), typeof(DynamicDetailPopup), new PropertyMetadata(null,new PropertyChangedCallback(User_Changed)));

        private static void User_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue !=null && e.NewValue is SlimUserInfo user)
            {
                var instance = d as DynamicDetailPopup;
                var bitmap = new BitmapImage();
                instance.UserAvatar.ProfilePicture = bitmap;
                bitmap.UriSource = new Uri(user.face+"@50w.jpg");
                instance.UserNameBlock.Text = user.uname;
            }
        }


        public string Dynamic
        {
            get { return (string)GetValue(DynamicProperty); }
            set { SetValue(DynamicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Dynamic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DynamicProperty =
            DependencyProperty.Register("Dynamic", typeof(string), typeof(DynamicDetailPopup), new PropertyMetadata(""));

        public void InitReply(string rid)
        {
            var param = new Dictionary<string, string>();
            param.Add("oid", rid);
            string type = "11";
            if (Data is WebDynamic web)
                type = "17";
            else if (Data is DocumentDynamic doc)
                type = "12";
            param.Add("type", type);
            ReplyFrame.Navigate(typeof(Pages_Share.Sub.ReplyPage), param);
        }

        public int ReplyId
        {
            get { return (int)GetValue(ReplyIdProperty); }
            set { SetValue(ReplyIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReplyId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReplyIdProperty =
            DependencyProperty.Register("ReplyId", typeof(int), typeof(DynamicDetailPopup), new PropertyMetadata(0,new PropertyChangedCallback(ReplyId_Changed)));

        private static void ReplyId_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=e.OldValue && e.NewValue is int id)
            {
                
                
            }
        }

        private void Account_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (User != null)
            {
                App.AppViewModel.NavigateToSubPage(typeof(Pages_Share.Sub.Account.DetailPage), User.uid);
            }
        }

        private void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            _imageFlipView = sender as FlipView;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            btn.IsEnabled = false;
            if (_imageFlipView != null)
            {
                var item = _imageFlipView.SelectedItem as ImageDynamic.Picture;
                if (item != null)
                {
                    var package = new DataPackage();
                    package.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(item.img_src)));
                    Clipboard.SetContent(package);
                    new TipPopup("复制完成").ShowMessage();
                }
            }
            btn.IsEnabled = true;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as AppBarButton;
            btn.IsEnabled = false;
            if (_imageFlipView != null)
            {
                var item = _imageFlipView.SelectedItem as ImageDynamic.Picture;
                if (item != null)
                {
                    try
                    {
                        var file = await IOTool.GetSaveFileAsync(".png", "保存的图片.png", "PNG 图片");
                        if (file != null)
                        {
                            var stream = await BiliTool.GetStreamFromWebAsync(item.img_src);
                            using (var fileStream = await file.OpenStreamForWriteAsync())
                            {
                                await stream.CopyToAsync(fileStream);
                            }
                            new TipPopup("下载完成").ShowMessage();
                        }
                    }
                    catch (Exception)
                    {
                        new TipPopup("下载图片失败").ShowMessage();
                    }
                }
            }
            btn.IsEnabled = true;
        }
    }
}
