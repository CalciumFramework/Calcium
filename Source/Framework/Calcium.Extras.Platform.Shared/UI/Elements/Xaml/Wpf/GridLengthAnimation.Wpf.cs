#if WPF
using System;

using System.Windows;
using System.Windows.Media.Animation;

namespace Codon.UI.Elements
{
	/* Sourced from http://www.codeproject.com/KB/WPF/GridLengthAnimation.aspx */
	public class GridLengthAnimation : AnimationTimeline
	{
		static GridLengthAnimation()
		{
			FromProperty = DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));
			ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));
		}

		public override Type TargetPropertyType => typeof(GridLength);

		protected override Freezable CreateInstanceCore()
		{
			return new GridLengthAnimation();
		}

		public static readonly DependencyProperty FromProperty;

		public GridLength From
		{
			get => (GridLength)GetValue(FromProperty);
			set => SetValue(FromProperty, value);
		}

		public static readonly DependencyProperty ToProperty;

		public GridLength To
		{
			get => (GridLength)GetValue(ToProperty);
			set => SetValue(ToProperty, value);
		}

		public override object GetCurrentValue(
			object defaultOriginValue, object defaultDestinationValue, 
			AnimationClock animationClock)
		{
			double fromVal = ((GridLength)GetValue(FromProperty)).Value;
			double toVal = ((GridLength)GetValue(ToProperty)).Value;

			if (fromVal > toVal)
			{
				return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal) + toVal, GridUnitType.Star);
			}

			return new GridLength(animationClock.CurrentProgress.Value * (toVal - fromVal) + fromVal, GridUnitType.Star);
		}
	}
}
#endif