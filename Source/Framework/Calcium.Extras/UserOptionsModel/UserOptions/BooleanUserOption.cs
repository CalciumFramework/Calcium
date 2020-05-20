using System;
using System.Threading.Tasks;

namespace Calcium.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option is a settable <c>bool</c> value.
	/// </summary>
	public class BooleanUserOption : UserOptionBase<bool>
	{
		const string defaultTemplateName = "Boolean";

		public BooleanUserOption(
			Func<string> titleFunc, 
			string settingKey, 
			Func<bool> defaultValueFunc)
				: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
		}

		public BooleanUserOption(
			Func<string> titleFunc,
			Func<Task<bool>> getSettingFunc,
			Func<bool, Task<SaveOptionResult>> saveSettingFunc,
			Func<bool> defaultValueFunc)
			: base(titleFunc, defaultValueFunc, saveSettingFunc, getSettingFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
		}
	}
}
