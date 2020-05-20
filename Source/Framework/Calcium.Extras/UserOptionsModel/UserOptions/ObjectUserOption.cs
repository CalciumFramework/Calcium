using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calcium.UserOptionsModel.UserOptions
{
	/// <summary>
	/// This class represents a option that normally requires
	/// a custom template. 
	/// </summary>
	/// <typeparam name="TSetting">
	/// The type of setting. <seealso cref="Services.ISettingsService"/>
	/// </typeparam>
	public class ObjectUserOption<TSetting> : UserOptionBase<TSetting>
	{
		public ObjectUserOption(
			Func<string> titleFunc,
			string settingKey,
			Func<TSetting> defaultValueFunc)
			: base(titleFunc, settingKey, defaultValueFunc)
		{
			TemplateNameFunc = () => typeof(TSetting).Name;
		}

		public void SetDirty()
		{
			var writer = ReaderWriter as ICompositeObjectWriter;
			writer?.SetDirty();
		}
	}
}
