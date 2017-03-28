using System;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option is a settable integer value.
	/// </summary>
	public class IntUserOption : UserOptionBase<int>
	{
		public IntUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<int> defaultValueFunc)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => "Int";
		}
	}
}