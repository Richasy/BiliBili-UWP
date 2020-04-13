using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using BiliBili_UWP.Models.UI;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class WaitingPopup : UserControl,IAppPopup
    {
        public Popup _popup { get; set; }
        public Guid _popupId { get; set; }
        public WaitingPopup()
        {
            this.InitializeComponent();
            UIHelper.PopupInit(this);
        }
        public WaitingPopup(string content) : this()
        {
            HoldTipBlock.Text = content;
        }
        public void ShowPopup()
        {
            UIHelper.PopupShow(this);
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            PopupOut.Begin();
            PopupOut.Completed += PopupOut_Completed;
        }

        private void PopupOut_Completed(object sender, object e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == _popupId);
            _popup.IsOpen = false;
        }
    }
}
