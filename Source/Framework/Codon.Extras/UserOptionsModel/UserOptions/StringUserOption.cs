using System;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// Represents a user settable string setting.
	/// </summary>
	public class StringUserOption : UserOptionBase<string>
	{
		public string InputScope { get; set; }

		public StringUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<string> defaultValueFunc,
			string inputScope = null)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => "String";
			InputScope = inputScope;
		}
	}
}