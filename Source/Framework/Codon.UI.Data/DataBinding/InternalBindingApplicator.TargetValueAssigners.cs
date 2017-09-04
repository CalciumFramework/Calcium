using System;
using System.Globalization;
using System.Reflection;

using Codon.ComponentModel;
using Codon.MissingTypes.System.Windows.Data;
using Codon.Reflection;

namespace Codon.UI.Data
{
	partial class InternalBindingApplicator
	{
		static IReflectionCache reflectionCacheUseProperty;
		static IReflectionCache ReflectionCache
			=> reflectionCacheUseProperty
			?? (reflectionCacheUseProperty = Dependency.Resolve<IReflectionCache>());

		internal static void SetTargetProperty(
			PropertyBinding propertyBinding,
			object dataContext)
		{
			SetTargetProperty(propertyBinding.SourceProperty, dataContext, propertyBinding.View,
				propertyBinding.TargetProperty, propertyBinding.Converter, propertyBinding.ConverterParameter);
		}

		#region Reflection Avoidance

		#endregion

		static void SetTargetProperty(PropertyInfo sourceProperty, object dataContext,
			object view, PropertyInfo targetProperty, IValueConverter converter, string converterParameter)
		{
			Func<object, object> getter = ReflectionCache.GetPropertyGetter(sourceProperty);

			/* Get the value of the source (the viewmodel) 
			 * property by using the converter if provided. */
			var rawValue = getter(dataContext);

			var sourcePropertyValue = converter == null
				? rawValue
				: converter.Convert(rawValue,
					targetProperty.PropertyType,
					converterParameter,
					CultureInfo.CurrentCulture);

			Type targetPropertyType = targetProperty.PropertyType;
			Type sourcePropertyType = sourceProperty.PropertyType;

			if (targetPropertyType == typeof(string)
				&& sourcePropertyType != typeof(string)
				&& sourcePropertyValue != null)
			{
				sourcePropertyValue = sourcePropertyValue.ToString();
			}
			else if (targetPropertyType != sourcePropertyType)
			{
				sourcePropertyValue = ValueCoercer.CoerceToType(sourcePropertyValue, targetPropertyType);
			}

			Action<object, object> setter = ReflectionCache.GetPropertySetter(targetProperty);
			
			try
			{
				setter(view, sourcePropertyValue);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
					"Exception raised using setter delegate to set property. "
					+ $"Source: {dataContext}.{sourceProperty?.Name}, Target: {view}.{targetProperty?.Name}", ex);
			}

//			try
//			{
//				targetProperty.SetValue(view, sourcePropertyValue);
//			}
//			catch (ArgumentException ex)
//			{
//				throw new ArgumentException(
//					"Exception raised using reflection to set property. "
//					+ $"Source: {dataContext}.{sourceProperty?.Name}, Target: {view}.{targetProperty?.Name}", ex);
//			}
		}

		static void SetTargetProperty(PropertyInfo targetProperty, object newPropertyValue, object view, IValueConverter converter, string converterParameter)
		{
			/* Get the value of the source (the viewmodel) 
			 * property by using the converter if provided. */
			var rawValue = newPropertyValue;

			var convertedValue = converter == null
				? rawValue
				: converter.Convert(rawValue,
					targetProperty.PropertyType,
					converterParameter,
					CultureInfo.CurrentCulture);

			Type targetPropertyType = targetProperty.PropertyType;
			Type newPropertyValueType = newPropertyValue.GetType();

			if (targetPropertyType == typeof(string)
				&& newPropertyValue != null && newPropertyValueType != typeof(string)
				&& convertedValue != null)
			{
				convertedValue = convertedValue.ToString();
			}
			else if (targetPropertyType != newPropertyValueType)
			{
				convertedValue = ValueCoercer.CoerceToType(convertedValue, targetPropertyType);
			}

			Action<object, object> setter = ReflectionCache.GetPropertySetter(targetProperty);

			try
			{
				setter(view, convertedValue);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
					"Exception raised using setter delegate to set property. "
					+ $"Target: {targetProperty}.{targetProperty?.Name}, Value: {convertedValue}", ex);
			}

			//			try
			//			{
			//				targetProperty.SetValue(view, convertedValue);
			//			}
			//			catch (ArgumentException ex)
			//			{
			//				throw new ArgumentException(
			//					"Exception raised using reflection to set property. "
			//					+ $"newPropertyValue: {newPropertyValue}, Target: {view}.{targetProperty?.Name}", ex);
			//			}
		}

		static void CallTargetMethod(
			MethodInfo targetMethod, PropertyInfo sourceProperty, 
			object dataContext, object view, IValueConverter converter, string converterParameter)
		{
			/* Get the value of the source (the viewmodel) 
			 * property by using the converter if provided. */
			var getter = ReflectionCache.GetPropertyGetter(sourceProperty);
			var rawValue = getter(dataContext);

			var parameters = targetMethod.GetParameters();
			var length = parameters.Length;
			if (length != 1)
			{
				throw new InvalidOperationException("Unable to set method value on method with more than one parameter. " + targetMethod);
			}

			var parameter = parameters[0];
			var parameterType = parameter.ParameterType;

			var sourcePropertyValue = converter == null
				? rawValue
				: converter.Convert(rawValue,
					parameterType,
					converterParameter,
					CultureInfo.CurrentCulture);

			var sourcePropertyType = sourceProperty.PropertyType;

			if (parameterType == typeof(string)
				&& sourcePropertyType != typeof(string)
				&& sourcePropertyValue != null)
			{
				sourcePropertyValue = sourcePropertyValue.ToString();
			}
			else if (parameterType != sourcePropertyType)
			{
				sourcePropertyValue = ValueCoercer.CoerceToType(sourcePropertyValue, parameterType);
			}

			Action<object, object[]> action = ReflectionCache.GetVoidMethodInvoker(targetMethod);
			
			try
			{
				action(view, new[] { sourcePropertyValue });
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
					"Exception raised using method delegate to call method. "
					+ $"Target: {targetMethod}.{targetMethod?.Name}, Value: {sourcePropertyValue}", ex);
			}

			//targetMethod.Invoke(view, new[] { sourcePropertyValue });
		}

		static void CallTargetMethod(
			MethodInfo targetMethod, object parameterValue, 
			object view, IValueConverter converter, string converterParameter)
		{
			var rawValue = parameterValue;

			var parameters = targetMethod.GetParameters();
			var length = parameters.Length;
			if (length != 1)
			{
				throw new InvalidOperationException("Unable to call method with more than one parameter. " + targetMethod);
			}

			var parameter = parameters[0];
			var parameterType = parameter.ParameterType;

			var sourcePropertyValue = converter == null
				? rawValue
				: converter.Convert(rawValue,
					parameterType,
					converterParameter,
					CultureInfo.CurrentCulture);

			var parameterValueType = parameterValue?.GetType();

			if (parameterType == typeof(string)
				&& parameterValue != null && parameterValueType != typeof(string)
				&& sourcePropertyValue != null)
			{
				sourcePropertyValue = sourcePropertyValue.ToString();
			}
			else if (parameterType != parameterValueType)
			{
				sourcePropertyValue = ValueCoercer.CoerceToType(sourcePropertyValue, parameterType);
			}

			//targetMethod.Invoke(view, new[] { sourcePropertyValue });

			Action<object, object[]> action = ReflectionCache.GetVoidMethodInvoker(targetMethod);
			
			try
			{
				action(view, new[] { sourcePropertyValue });
			}
			catch (Exception ex)
			{
				throw new ArgumentException(
					"CallTargetMethod: Exception raised using method delegate to call method. "
					+ $"Target: {targetMethod}.{targetMethod?.Name}, Value: {sourcePropertyValue}", ex);
			}
		}
	}

	static class ValueCoercer
	{
		static IImplicitTypeConverter implicitTypeConverter;

		internal static object CoerceToType(object originalValue, Type destinationType)
		{
			object coercedParameter = originalValue;
			Type typeOfT = destinationType;

			if (originalValue != null && !typeOfT.IsAssignableFromEx(originalValue.GetType()))
			{
				if (implicitTypeConverter == null)
				{
					implicitTypeConverter = Dependency.Resolve<IImplicitTypeConverter>();
				}
				coercedParameter = implicitTypeConverter.ConvertToType(originalValue, typeOfT);
			}

			return coercedParameter;
		}
	}
}