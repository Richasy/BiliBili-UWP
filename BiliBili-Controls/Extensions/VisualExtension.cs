using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace BiliBili_Controls.Extensions
{
    public static class VisualExtensions
    {
        public static string GetScale(DependencyObject obj)
        {
            return (string)obj.GetValue(ScaleProperty);
        }

        public static void SetScale(DependencyObject obj, string value)
        {
            obj.SetValue(ScaleProperty, value);
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.RegisterAttached("Scale", typeof(string), typeof(VisualExtensions), new PropertyMetadata("1,1,1", (s, a) =>
            {
                if (!string.Equals(a.NewValue, a.OldValue))
                {
                    if (s is UIElement element)
                    {
                        var newValue = a.NewValue as string;
                        var arr = newValue.Replace(" ", string.Empty).Replace("f", string.Empty).Split(',');
                        if (arr.Length == 3)
                        {
                            var v3 = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                            var visual = ElementCompositionPreview.GetElementVisual(element);
                            visual.Scale = v3;
                        }
                        else
                        {
                            throw new ArgumentException("数据不是Vector3");
                        }
                    }
                }
            }));

        public static bool GetIsBindCenterPoint(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBindCenterPointProperty);
        }

        public static void SetIsBindCenterPoint(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBindCenterPointProperty, value);
        }

        public static readonly DependencyProperty IsBindCenterPointProperty =
            DependencyProperty.RegisterAttached("IsBindCenterPoint", typeof(bool), typeof(VisualExtensions), new PropertyMetadata(false, (s, a) =>
            {
                if (a.NewValue != a.OldValue)
                {
                    if (s is UIElement ele)
                    {
                        if (a.NewValue is true)
                        {
                            ElementCompositionPreview.GetElementVisual(ele).BindCenterPoint();
                        }
                        else
                        {
                            ElementCompositionPreview.GetElementVisual(ele).StopAnimation("CenterPoint");
                        }
                    }
                }
            }));

        public static string GetTransations(DependencyObject obj)
        {
            return (string)obj.GetValue(TransationsProperty);
        }

        public static void SetTransations(DependencyObject obj, string value)
        {
            obj.SetValue(TransationsProperty, value);
        }

        public static readonly DependencyProperty TransationsProperty =
            DependencyProperty.RegisterAttached("Transations", typeof(string), typeof(VisualExtensions), new PropertyMetadata(null, (s, a) =>
            {
                if (!string.Equals(a.NewValue, a.OldValue))
                {
                    if (s is UIElement ele)
                    {
                        if (a.NewValue is string str)
                        {
                            var arr = str.Replace(" ", string.Empty).Split(',');
                            if (arr.Length > 0)
                            {
                                var visual = ElementCompositionPreview.GetElementVisual(ele);
                                visual.ImplicitAnimations = visual.Compositor.CreateImplicitAnimationCollection();
                                foreach (var item in arr)
                                {
                                    visual.ImplicitAnimations.CreateImplicitAnimation(item);
                                }
                                return;
                            }
                        }
                        ElementCompositionPreview.GetElementVisual(ele).ImplicitAnimations = null;
                    }
                }
            }));

    }
}
