using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace BiliBili_Lib.Tools
{
    public class NotificationTool
    {
        public static async void SendDynamicToast(List<NotificationModel> items)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (index >= 2)
                {
                    var overflow = GetOverflowToast("更多动态请在应用内查看");
                    await Task.Delay(500);
                    ShowToast(overflow, group: "Dynamic");
                    break;
                }
                var content = new ToastContent
                {
                    Launch = item.Args,
                    Visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {

                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text=item.Title,
                                    HintMaxLines=2,
                                    HintStyle=AdaptiveTextStyle.Header
                                },
                                new AdaptiveText()
                                {
                                    Text = item.Description,
                                    HintMaxLines=2,
                                    HintStyle=AdaptiveTextStyle.Default
                                },
                            },
                            AppLogoOverride = new ToastGenericAppLogo()
                            {
                                Source = item.Logo,
                                HintCrop = ToastGenericAppLogoCrop.Circle
                            },
                            Attribution = new ToastGenericAttributionText()
                            {
                                Text = "动态"
                            }
                        },
                    }
                };
                if (!string.IsNullOrEmpty(item.HeroImage))
                {
                    content.Visual.BindingGeneric.HeroImage = new ToastGenericHeroImage()
                    {
                        Source = item.HeroImage,
                        AlternateText = item.Title
                    };
                }
                index++;
                ShowToast(content, group: "Dynamic");
            }
        }
        public static void SendScreenShotToast(string imageName,string folderPath)
        {
            var content = new ToastContent
            {
                Launch = $"action=screenshot&image={imageName}",
                ActivationType = ToastActivationType.Background,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        HeroImage=new ToastGenericHeroImage()
                        {
                            Source = $"{folderPath}\\{imageName}",
                            AlternateText="截图"
                        },
                        Children =
                            {
                                new AdaptiveText()
                                {
                                    Text="保存到图库的截图",
                                    HintMaxLines=2,
                                    HintStyle=AdaptiveTextStyle.Header
                                },
                                new AdaptiveText()
                                {
                                    Text = "哔哩已将您的截图保存到图库中，点击通知可通过默认应用打开图片",
                                    HintMaxLines=2,
                                    HintStyle=AdaptiveTextStyle.Default
                                },
                            },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = "ms-appx:///Assets/logo_small.png",
                        },
                        Attribution = new ToastGenericAttributionText()
                        {
                            Text = "截图"
                        }
                    },
                }
            };
            ShowToast(content, group: "ScreenShot");
        }
        public static ToastContent GetOverflowToast(string title)
        {
            var content = new ToastContent
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text=title,
                                HintMaxLines=2,
                                HintStyle=AdaptiveTextStyle.Header
                            },
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = $"ms-appx:///Assets/logo_small.png"
                        }
                    },
                }
            };
            return content;
        }
        public static void ShowToast(ToastContent content, string tag = null, string group = "")
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var notification = new ToastNotification(content.GetXml());
            if (!string.IsNullOrEmpty(group))
                notification.Group = group;
            if (!string.IsNullOrEmpty(tag))
                notification.Tag = tag;
            notifier.Show(notification);
        }
        public static void HideToast(string tag)
        {
            ToastNotificationManager.History.Remove(tag);
        }
    }
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string HeroImage { get; set; }
        public string Args { get; set; }
        public string Logo { get; set; }
        public NotificationModel()
        {

        }
        public NotificationModel(string t, string d, string img, string arg, string l = "")
        {
            Title = t;
            Description = d;
            HeroImage = img;
            Args = arg;
            if (string.IsNullOrEmpty(l))
                Logo = "ms-appx:///Assets/logo_small.png";
            else
                Logo = l;
        }
    }
}
