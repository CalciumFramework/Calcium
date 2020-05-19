#if WINDOWS_UWP || WPF
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

#if WINDOWS_UWP
using Windows.UI.Xaml;
#elif WPF
using System.Windows;
#endif

namespace Codon.UI.Elements
{
	public static class Validation
	{
		public static readonly DependencyProperty ErrorsProperty
			= DependencyProperty.RegisterAttached(
				"Errors", typeof(List<ValidationError>),
				typeof(Validation),
				new PropertyMetadata(new List<ValidationError>(), HandleErrorsChanged));

		static void HandleErrorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}

		public static readonly DependencyProperty HasErrorProperty
			= DependencyProperty.RegisterAttached(
				"HasError", typeof(bool),
				typeof(Validation),
				new PropertyMetadata(true, HandleHasErrorChanged));

		static void HandleHasErrorChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

		}

		public static ReadOnlyObservableCollection<ValidationError> GetErrors(
			DependencyObject element)
		{
			if (element == null)
			{
				goto returnEmpty;
			}

			var errors = (IEnumerable<ValidationError>)element.GetValue(ErrorsProperty);
			if (errors == null)
			{
				goto returnEmpty;
			}

			var errorsCollection = new ObservableCollection<ValidationError>(errors);

			return new ReadOnlyObservableCollection<ValidationError>(errorsCollection);

			returnEmpty:
			return new ReadOnlyObservableCollection<ValidationError>(
						new ObservableCollection<ValidationError>());
		}

		public static bool GetHasError(DependencyObject element)
		{
			var errors = (IEnumerable<ValidationError>)element.GetValue(ErrorsProperty);
			return errors.Any();
		}
	}
}

#endif