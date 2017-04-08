using System;
using System.Globalization;
using Codon.ComponentModel;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Data
{
	/// <summary>
	/// This class is used for testing the <see cref="BindingApplicator"/>.
	/// </summary>
	public class ViewModelTestClass : ObservableBase
	{
		bool bool1;

		public bool Bool1
		{
			get => bool1;
			set => Set(ref bool1, value);
		}

		NestedClass nestedClass1;

		public NestedClass NestedClass1
		{
			get => nestedClass1;
			set => Set(ref nestedClass1, value);
		}

		public class NestedClass : ObservableBase
		{
			bool bool2;

			public bool Bool2
			{
				get => bool2;
				set => Set(ref bool2, value);
			}
		}
	}

	public class ViewTestClass
	{
		public bool Bool1 { get; set; }

		public DummyVisibility Visibility { get; set; }
	}

	public enum DummyVisibility
	{
		Invisible,
		Visible
	}

	public class DummyBooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return DummyVisibility.Visible;
			}

			bool boolValue = (bool)value;
			return boolValue ? DummyVisibility.Visible : DummyVisibility.Invisible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}