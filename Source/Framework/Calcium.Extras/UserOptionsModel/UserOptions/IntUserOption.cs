using System;
using System.Threading.Tasks;

namespace Codon.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This option is a settable integer value.
	/// </summary>
	public class IntUserOption : UserOptionBase<int>
	{
		const string defaultTemplateName = "Int";

		public IntUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<int> defaultValueFunc)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
		}

		public IntUserOption(
			Func<string> titleFunc,
			Func<Task<int>> getSettingFunc,
			Func<int, Task<SaveOptionResult>> saveSettingFunc,
			Func<int> defaultValueFunc)
			: base(titleFunc, defaultValueFunc, saveSettingFunc, getSettingFunc)
		{
			TemplateNameFunc = () => defaultTemplateName;
		}

		public int MinValue { get; set; }
		public int MaxValue { get; set; }

		public uint Step { get; set; }
	}
}