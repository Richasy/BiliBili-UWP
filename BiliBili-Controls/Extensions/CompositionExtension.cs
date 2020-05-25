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
			sb.Completed += (_s, _e) =>
			{
				(target as UIElement).UpdateLayout();
			};
			sb.Begin();
		}
		public static void BindSize(this Visual target, Visual source)
		{
			var exp = target.Compositor.CreateExpressionAnimation("host.Size");
			exp.SetReferenceParameter("host", source);
			target.StartAnimation("Size", exp);
		}

		public static void BindCenterPoint(this Visual target)
		{
			var exp = target.Compositor.CreateExpressionAnimation("Vector3(this.Target.Size.X / 2, this.Target.Size.Y / 2, 0f)");
			target.StartAnimation("CenterPoint", exp);
		}

		public static ImplicitAnimationCollection CreateImplicitAnimation(this ImplicitAnimationCollection source, string Target, TimeSpan? Duration = null)
		{
			KeyFrameAnimation animation = null;
			switch (Target.ToLower())
			{
				case "offset":
				case "scale":
				case "centerPoint":
				case "rotationAxis":
					animation = source.Compositor.CreateVector3KeyFrameAnimation();
					break;
				case "size":
					animation = source.Compositor.CreateVector2KeyFrameAnimation();
					break;
				case "opacity":
				case "blueRadius":
				case "rotationAngle":
				case "rotationAngleInDegrees":
					animation = source.Compositor.CreateScalarKeyFrameAnimation();
					break;
				case "color":
					animation = source.Compositor.CreateColorKeyFrameAnimation();
					break;
			}
			if (animation == null) throw new ArgumentNullException("未知的Target");
			if (!Duration.HasValue) Duration = TimeSpan.FromSeconds(0.2d);
			animation.InsertExpressionKeyFrame(1f, "this.FinalValue");
			animation.Duration = Duration.Value;
			animation.Target = Target;

			source[Target] = animation;
			return source;
		}
	}
}
