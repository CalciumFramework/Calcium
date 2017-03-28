using System;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option is a settable <c>bool</c> value.
	/// </summary>
	public class BooleanUserOption : UserOptionBase<bool>
	{
		public BooleanUserOption(
			Func<string> titleFunc, 
			string settingKey, 
			Func<bool> defaultValueFunc)
				: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => "Boolean";
		}
	}
}