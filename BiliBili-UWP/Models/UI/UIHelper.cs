using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.Enums;
using BiliBili_UWP.Models.UI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace BiliBili_UWP.Models.UI
{
    public class UIHelper
    {
        /// <summary>
        /// 初始化标题栏
        /// </summary>
        public static void SetTitleBarColor()
        {
            var view = ApplicationView.GetForCurrentView();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var Theme = AppTool.GetLocalSetting(Settings.Theme, "Light");
            if (Theme == "Dark")
            {
                // active
                view.TitleBar.BackgroundColor = Colors.Transparent;
                view.TitleBar.ForegroundColor = Colors.White;

                // inactive
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveForegroundColor = Colors.Gray;
                // button
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonForegroundColor = Colors.White;

                view.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 33, 42, 67);
                view.TitleBar.ButtonHoverForegroundColor = Colors.White;

                view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 255, 86, 86);
                view.TitleBar.ButtonPressedForegroundColor = Colors.White;

                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
            else
            {
                // active
                view.TitleBar.BackgroundColor = Colors.Transparent;
                view.TitleBar.ForegroundColor = Colors.Black;

                // inactive
                view.TitleBar.InactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.InactiveForegroundColor = Colors.Gray;
                // button
                view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonForegroundColor = Colors.DarkGray;

                view.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 63, 63, 63);
                view.TitleBar.ButtonHoverForegroundColor = Colors.DarkGray;

                view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 63, 63, 63);
                view.TitleBar.ButtonPressedForegroundColor = Colors.DarkGray;

                view.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            }
        }
        /// <summary>
        /// 获取预先定义的线性画笔资源
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Brush GetThemeBrush(ColorType key)
        {
            return (Brush)Application.Current.Resources[key.ToString()];
        }
        /// <summary>
        /// 获取预先定义的字体资源
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static FontFamily GetFontFamily(string key)
        {
            return (FontFamily)Application.Current.Resources[key];
        }
        /// <summary>
        /// 获取预先定义的样式
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static Style GetStyle(string key)
        {
            return (Style)Application.Current.Resources[key];
        }
        public static void PopupInit(IAppPopup popup)
        {
            popup._popup = new Popup();
            popup._popupId = Guid.NewGuid();
            popup._popup.Child = popup as UIElement;
        }
        public static void PopupShow(IAppPopup popup,Action changeAction=null)
        {
            App.AppViewModel.WindowsSizeChangedNotify.Add(new Tuple<Guid, Action<Size>>(popup._popupId, (rect) =>
            {
                popup.Width = rect.Width;
                popup.Height = rect.Height;
                changeAction?.Invoke();
            }));
            popup.Width = Window.Current.Bounds.Width;
            popup.Height = Window.Current.Bounds.Height;
            popup._popup.IsOpen = true;
        }
        /// <summary>
        /// 根据颜色计算弹幕的颜色值
        /// </summary>
        /// <param name="c">颜色</param>
        /// <returns></returns>
        public static string GetDanmakuColor(Color c)
        {
            int num = c.R * 256 * 256 + c.G * 256 + c.B * 1;
            return num.ToString();
        }
    }
}
