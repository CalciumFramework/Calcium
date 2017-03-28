#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Data
{
	internal class ViewValueChangedHandler
	{
		internal static void HandleViewValueChanged(
			PropertyBinding propertyBinding,
			object dataContext)
		{
			try
			{
				propertyBinding.PreventUpdateForTargetProperty = true;

				var newValue = propertyBinding.TargetProperty.GetValue(propertyBinding.View);

				UpdateSourceProperty(propertyBinding.SourceProperty, dataContext, newValue,
					propertyBinding.Converter, propertyBinding.ConverterParameter);
			}
			catch (Exception)
			{
				/* TODO: log exception */
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
			finally
			{
				propertyBinding.PreventUpdateForTargetProperty = false;
			}
		}

		internal static void HandleViewValueChanged<TView, TArgs, TNewValue>(
			PropertyBinding propertyBinding,
			Func<TView, TArgs, TNewValue> newValueFunc,
			object dataContext,
			TArgs args)
#if __ANDROID__ || MONODROID
			where TView : Android.Views.View
#endif
		{
			try
			{
				propertyBinding.PreventUpdateForTargetProperty = true;
				var rawValue = newValueFunc((TView)propertyBinding.View, args);

				UpdateSourceProperty(propertyBinding.SourceProperty,
					dataContext,
					rawValue,
					propertyBinding.Converter,
					propertyBinding.ConverterParameter);
			}
			catch (Exception)
			{
				/* TODO: log exception */
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
			finally
			{
				propertyBinding.PreventUpdateForTargetProperty = false;
			}
		}

		internal static void HandleTargetValueChanged(
			PropertyBinding propertyBinding,
			object newValue,
			object dataContext)
		{
			try
			{
				propertyBinding.PreventUpdateForTargetProperty = true;

				UpdateSourceProperty(propertyBinding.SourceProperty,
					dataContext,
					newValue,
					propertyBinding.Converter,
					propertyBinding.ConverterParameter);
			}
			catch (Exception)
			{
				/* TODO: log exception */
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
			finally
			{
				propertyBinding.PreventUpdateForTargetProperty = false;
			}
		}

		internal static void UpdateSourceProperty<T>(
			PropertyInfo sourceProperty,
			object dataContext,
			T value,
			IValueConverter valueConverter,
			string converterParameter)
		{
			object newValue;

			if (valueConverter != null)
			{
				newValue = valueConverter.ConvertBack(value,
					sourceProperty.PropertyType,
					converterParameter,
					CultureInfo.CurrentCulture);
			}
			else
			{
				newValue = value;
			}

			sourceProperty.SetValue(dataContext, newValue);
		}
	}
}