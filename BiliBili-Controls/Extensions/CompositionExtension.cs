using BiliBili_Controls.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;

namespace BiliBili_Controls.Extensions
{
    public static class CompositionExtensions
    {
		public static void EnableImplicitAnimation(this Visual visual, VisualPropertyType typeToAnimate,
			double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
		{
			var compositor = visual.Compositor;

			var animationCollection = compositor.CreateImplicitAnimationCollection();

			foreach (var type in GetValues<VisualPropertyType>())
			{
				if (!typeToAnimate.HasFlag(type)) continue;

				var animation = CreateAnimationByType(compositor, type, duration, delay, easing);

				if (animation != null)
				{
					animationCollection[type.ToString()] = animation;
				}
			}

			visual.ImplicitAnimations = animationCollection;
		}
		private static IEnumerable<T> GetValues<T>() => Enum.GetValues(typeof(T)).Cast<T>();
		public static Visual Visual(this UIElement element) =>
			ElementCompositionPreview.GetElementVisual(element);
		public static CubicBezierEasingFunction EaseOutCubic(this Compositor compositor)
		{
			return compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1.0f));
		}
		private static KeyFrameAnimation CreateAnimationByType(Compositor compositor, VisualPropertyType type,
			double duration = 800, double delay = 0, CompositionEasingFunction easing = null)
		{
			KeyFrameAnimation animation;

			switch (type)
			{
				case VisualPropertyType.Offset:
				case VisualPropertyType.Scale:
					animation = compositor.CreateVector3KeyFrameAnimation();
					break;
				case VisualPropertyType.Size:
					animation = compositor.CreateVector2KeyFrameAnimation();
					break;
				case VisualPropertyType.Opacity:
				case VisualPropertyType.RotationAngleInDegrees:
					animation = compositor.CreateScalarKeyFrameAnimation();
					break;
				default:
					return null;
			}

			animation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easing);
			animation.Duration = TimeSpan.FromMilliseconds(duration);
			animation.DelayTime = TimeSpan.FromMilliseconds(delay);
			animation.Target = type.ToString();

			return animation;
		}
		public static void Animate(this DependencyObject target, double? from, double to,
		  string propertyPath, int duration = 400, int startTime = 0,
		  EasingFunctionBase easing = null, Action completed = null, bool enableDependentAnimation = false)
		{
			if (easing == null)
			{
				easing = new ExponentialEase { EasingMode = EasingMode.EaseOut };
			}

			var db = new DoubleAnimation
			{
				EnableDependentAnimation = enableDependentAnimation,
				To = to,
				From = from,
				EasingFunction = easing,
				Duration = TimeSpan.FromMilliseconds(duration)
			};
			Storyboard.SetTarget(db, target);
			Storyboard.SetTargetProperty(db, propertyPath);

			var sb = new Storyboard
			{
				BeginTime = TimeSpan.FromMilliseconds(startTime)
			};

			if (completed != null)
			{
				sb.Completed += (s, e) =>
				{
					completed();
				};
			}

			sb.Children.Add(db);
			sb.Begin();
		}

	}
}
