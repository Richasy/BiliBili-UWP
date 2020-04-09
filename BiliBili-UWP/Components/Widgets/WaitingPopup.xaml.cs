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
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace BiliBili_UWP.Components.Widgets
{
    public sealed partial class WaitingPopup : UserControl
    {
        private Popup _popup = null;
        Guid _popupId = Guid.NewGuid();
        public WaitingPopup()
        {
            this.InitializeComponent();
            _popup = new Popup();
            _popup.Child = this;
        }
        public WaitingPopup(string content) : this()
        {
            HoldTipBlock.Text = content;
        }
        public void ShowPopup()
        {
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
            App.AppViewModel.WindowsSizeChangedNotify.Add(new Tuple<Guid, Action<Size>>(_popupId, (rect) =>
            {
                this.Width = rect.Width;
                this.Height = rect.Height;
            }));
            _popup.IsOpen = true;
            PopupIn.Begin();
        }
        public void HidePopup()
        {
            PopupOut.Begin();
            PopupOut.Completed += PopupOutCompleted;
        }

        private void PopupOutCompleted(object sender, object e)
        {
            App.AppViewModel.WindowsSizeChangedNotify.RemoveAll(p => p.Item1 == _popupId);
            _popup.IsOpen = false;
        }
    }
}
