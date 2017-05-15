using System;
using System.Threading.Tasks;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// Represents a user settable string setting.
	/// </summary>
	public class StringUserOption : UserOptionBase<string>
	{
		const string defaultTemplateName = "String";

		public string InputScope { get; set; }

		public StringUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<string> defaultValueFunc,
			string inputScope = null)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
			InputScope = inputScope;
		}

		public StringUserOption(
			Func<string> titleFunc,
			Func<Task<string>> getSettingFunc,
			Func<string, Task<SaveOptionResult>> saveSettingFunc,
			Func<string> defaultValueFunc)
			: base(titleFunc, defaultValueFunc, saveSettingFunc, getSettingFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
		}
	}
}