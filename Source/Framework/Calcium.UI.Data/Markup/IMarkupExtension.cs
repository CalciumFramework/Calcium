using Calcium.InversionOfControl;

namespace Calcium.UI.Data
{
	public interface IMarkupExtension
	{
		object ProvideValue(IContainer iocContainer/*, object[] parameters*/);
	}
}
