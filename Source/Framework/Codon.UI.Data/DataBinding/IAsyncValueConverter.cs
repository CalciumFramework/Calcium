using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Codon.UI.Data
{
	public interface IAsyncValueConverter
	{
		Task<object> ConvertAsync(object value, Type targetType, object parameter, CultureInfo culture);

		Task<object> ConvertBackAsync(object value, Type targetType, object parameter, CultureInfo culture);
	}
}