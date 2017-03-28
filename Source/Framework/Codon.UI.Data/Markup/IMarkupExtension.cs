using Codon.InversionOfControl;

namespace Codon.UI.Data
{
	public interface IMarkupExtension
	{
		object ProvideValue(IContainer iocContainer/*, object[] parameters*/);
	}
}