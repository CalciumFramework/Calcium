using System;

namespace Calcium.UserOptionsModel.UserOptions
{
	/// <summary>
	/// Represents a user settable range value. 
	/// A range value must have a value that falls
	/// between a lower and upper boundary, and is usually displayed
	/// as a slider on an apps options page.
	/// </summary>
	public class RangeUserOption : UserOptionBase<double>
	{
		public RangeUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<double> defaultValueFunc)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => "Range";
		}

		double minimum;
		
		public double Minimum
		{
			get => minimum;
			set => minimum = value;
		}

		double maximum = 1;

		public double Maximum
		{
			get => maximum;
			set => maximum = value;
		}
	}

	/// <summary>
	/// Represents a user settable range value. 
	/// A range value must have a value that falls
	/// between a lower and upper boundary, and is usually displayed
	/// as a slider on an apps options page.
	/// </summary>
	public class RangeUserOption<T> : UserOptionBase<T>
	{
		public RangeUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<T> defaultValueFunc)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			Type type = typeof(T);
			if (type == typeof(Double))
			{
				TemplateNameFunc = () => "Range";
			}
			else
			{
				TemplateNameFunc = () => "Range" + type.Name;
			}
		}

		T minimum;

		public T Minimum
		{
			get => minimum;
			set => minimum = value;
		}

		T maximum;

		public T Maximum
		{
			get => maximum;
			set => maximum = value;
		}
	}
}
