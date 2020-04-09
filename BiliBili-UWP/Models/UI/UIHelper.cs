using BiliBili_Lib.Enums;
using BiliBili_Lib.Tools;
using BiliBili_UWP.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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
        /// 获取当前控件的指定子控件
        /// </summary>
        /// <typeparam name="T">控件类型</typeparam>
        /// <param name="obj">父控件</param>
        /// <param name="name">子控件名</param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                }
                if (grandChild != null)
                {
                    return grandChild;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取子控件集合
        /// </summary>
        /// <typeparam name="T">子控件属性</typeparam>
        /// <param name="obj">父控件</param>
        /// <param name="name">子控件名字</param>
        /// <returns></returns>
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }

                childList.AddRange(GetChildObjects<T>(child, ""));
            }
            return childList;
        }
    }
}
