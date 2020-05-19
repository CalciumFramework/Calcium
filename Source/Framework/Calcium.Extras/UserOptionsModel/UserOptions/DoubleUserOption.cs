using System;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option is a settable double value.
	/// </summary>
	public class DoubleUserOption : UserOptionBase<double>
	{
		public DoubleUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<double> defaultValueFunc)
				: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => "Double";
		}
	}
}