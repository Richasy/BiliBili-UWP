using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace BiliBili_Controls.Extensions
{
    public class PressEffect
    {
        private static Vector3 HoverScale = new Vector3(1.08f, 1.08f, 1f);
        private static Vector3 ActionScale = new Vector3(0.94f, 0.94f, 1f);

        public static string GetTargetElementName(FrameworkElement obj)
        {
            return (string)obj.GetValue(TargetElementNameProperty);
        }

        public static void SetTargetElementName(FrameworkElement obj, string value)
        {
            obj.SetValue(TargetElementNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for TargetElementName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetElementNameProperty =
            DependencyProperty.RegisterAttached("TargetElementName", typeof(string), typeof(PressEffect), new PropertyMetadata("", (s, a) =>
            {
                if (s is FrameworkElement sender)
                {
                    if (a.NewValue != a.OldValue)
                    {
                        var isOldEmpty = string.IsNullOrEmpty(a.OldValue.ToString());
                        var isNewEmpty = string.IsNullOrEmpty(a.NewValue.ToString());

                        if (isOldEmpty && isNewEmpty) return;
                        if (isOldEmpty)
                        {
                            RemoveAnimations(sender);
                            if (sender.IsLoaded)
                            {
                                var newTarget = sender.FindName(a.NewValue.ToString()) as FrameworkElement;
                                AddAnimations(newTarget);
                            }
                            else
                            {
                                sender.Loaded += Sender_Loaded;
                                void Sender_Loaded(object _, RoutedEventArgs e)
                                {
                                    sender.Loaded -= Sender_Loaded;
                                    var newTarget = sender.FindName(a.NewValue.ToString()) as FrameworkElement;
                                    AddAnimations(newTarget);
                                }
                            }
                        }
                        else
                        {
                            AddAnimations(sender);
                            if (sender.IsLoaded)
                            {
                                var oldTarget = sender.FindName(a.OldValue.ToString()) as FrameworkElement;
                                RemoveAnimations(oldTarget);
                            }
                            else
                            {
                                sender.Loaded += Sender_Loaded;
                                void Sender_Loaded(object _, RoutedEventArgs e)
                                {
                                    sender.Loaded -= Sender_Loaded;
                                    var oldTarget = sender.FindName(a.OldValue.ToString()) as FrameworkElement;
                                    RemoveAnimations(oldTarget);
                                }
                            }
                        }
                    }
                }
            }));



        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PressEffect), new PropertyMetadata(false, (s, a) =>
            {
                if (s is FrameworkElement sender)
                {
                    if (a.NewValue != a.OldValue)
                    {
                        var targetName = GetTargetElementName(sender);

                        if (a.NewValue is true)
                        {
                            if (string.IsNullOrEmpty(targetName))
                            {
                                AddAnimations(sender);
                            }

                            sender.PointerPressed += OnPointerPressed;
                            sender.PointerReleased += OnPointerReleased;
                            sender.PointerEntered += OnPointerEntered;
                            sender.PointerExited += OnPointerExited;
                            sender.PointerCanceled += OnPointerCanceled;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(targetName))
                            {
                                RemoveAnimations(sender);
                            }

                            sender.PointerPressed -= OnPointerPressed;
                            sender.PointerReleased -= OnPointerReleased;
                            sender.PointerEntered -= OnPointerEntered;
                            sender.PointerExited -= OnPointerExited;
                            sender.PointerCanceled -= OnPointerCanceled;
                        }

                    }
                }
            }));

        private static void OnPointerCanceled(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var test = sender as FrameworkElement;
            var target = GetTarget(test);

            if (test != null && target != null)
            {
                var rect = new Rect(0, 0, test.ActualWidth, test.ActualHeight);
                var cp = e.GetCurrentPoint(test);

                if (rect.Contains(cp.Position))
                {
                    ElementCompositionPreview.GetElementVisual(target).Scale = HoverScale;
                }
                else
                {
                    ElementCompositionPreview.GetElementVisual(target).Scale = Vector3.One;
                }
            }
        }

        private static void OnPointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var test = sender as FrameworkElement;
            var target = GetTarget(test);
            if (test != null && target != null)
            {
                ElementCompositionPreview.GetElementVisual(target).Scale = Vector3.One;
            }
        }

        private static void OnPointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var test = sender as FrameworkElement;
            var target = GetTarget(test);
            if (test != null && target != null)
            {
                ElementCompositionPreview.GetElementVisual(target).Scale = HoverScale;
            }
        }

        private static void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var test = sender as FrameworkElement;
            var target = GetTarget(test);

            if (test != null && target != null)
            {
                var rect = new Rect(0, 0, test.ActualWidth, test.ActualHeight);
                var cp = e.GetCurrentPoint(test);

                if (rect.Contains(cp.Position))
                {
                    ElementCompositionPreview.GetElementVisual(target).Scale = HoverScale;
                }
                else
                {
                    ElementCompositionPreview.GetElementVisual(target).Scale = Vector3.One;
                }
            }
        }

        private static void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var test = sender as FrameworkElement;
            var target = GetTarget(test);
            if (test != null && target != null)
            {
                ElementCompositionPreview.GetElementVisual(target).Scale = ActionScale;
            }
        }

        private static FrameworkElement GetTarget(FrameworkElement sender)
        {
            if (sender == null) return null;

            var targetName = GetTargetElementName(sender);
            var target = sender;
            if (!string.IsNullOrEmpty(targetName))
            {
                target = sender.FindName(targetName) as FrameworkElement;
            }

            return target;
        }

        private static void AddAnimations(FrameworkElement element)
        {
            if (element == null) return;
            var visual = ElementCompositionPreview.GetElementVisual(element);
            var compositor = visual.Compositor;

            var centerPointBind = compositor.CreateExpressionAnimation("Vector3(this.Target.Size.X / 2, this.Target.Size.Y / 2, 0f)");
            visual.StartAnimation("CenterPoint", centerPointBind);

            var imp = compositor.CreateImplicitAnimationCollection();

            var scaleAnimation = compositor.CreateVector3KeyFrameAnimation();
            scaleAnimation.InsertExpressionKeyFrame(1f, "this.FinalValue");
            scaleAnimation.Duration = TimeSpan.FromSeconds(0.2d);
            scaleAnimation.Target = "Scale";
            imp["Scale"] = scaleAnimation;

            visual.ImplicitAnimations = imp;
        }

        private static void RemoveAnimations(FrameworkElement element)
        {
            if (element == null) return;
            var visual = ElementCompositionPreview.GetElementVisual(element);
            visual.ImplicitAnimations = null;
            visual.StopAnimation("Scale");
            visual.StopAnimation("CenterPoint");
            visual.CenterPoint = Vector3.Zero;
            visual.Scale = Vector3.One;
        }
    }
}
