#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 18:14:49Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Reflection;
using Codon.InversionOfControl;

namespace Codon.Reflection
{
	/// <summary>
	/// A reflection cache is used to create and cache
	/// delegates that can retrieve or set the
	/// value of a property; or call a method.
	/// Using the cache can dramatically improve performance 
	/// in some cases.
	/// </summary>
	[DefaultType(typeof(ReflectionCache), Singleton = true)]
	public interface IReflectionCache
	{
		/// <summary>
		/// Retrieve or create an action that can be used
		/// to call a method on a object.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info for the method you wish to call.
		/// If the method you wish to call has a non-null
		/// return type, use <see cref="GetMethodInvoker"/> instead.
		/// </param>
		/// <returns>An action that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.</returns>
		Action<object, object[]> GetVoidMethodInvoker(
			MethodInfo methodInfo);

		/// <summary>
		/// Retrieve or create a func that can be used
		/// to call a method on a object. This differs from
		/// the <see cref="GetVoidMethodInvoker"/> method
		/// in that it produces a func that returns the value
		/// from the target method. This method can be used
		/// in place of <see cref="GetVoidMethodInvoker"/>,
		/// however for methods that have a void return type,
		/// <c>null</c> is returned.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info for the method you wish to call.
		/// </param>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		Func<object, object[], object> GetMethodInvoker(
			MethodInfo methodInfo);

		/// <summary>
		/// Retrieve or create a func that can be used
		/// to call a method on a object. This differs from
		/// the <see cref="GetVoidMethodInvoker"/> method
		/// in that it produces a func that returns the value
		/// from the target method. This method can be used
		/// in place of <see cref="GetVoidMethodInvoker"/>,
		/// however for methods that have a void return type,
		/// <c>null</c> is returned.
		/// </summary>
		/// <param name="methodInfo">
		/// The method info for the method you wish to call.
		/// </param>
		/// <typeparam name="TReturn">
		/// The return type of the method to invoke.</typeparam>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		Func<object, object[], TReturn> GetMethodInvoker<TReturn>(
			MethodInfo methodInfo);

		/// <summary>
		/// Retrieves or creates an func that can be used
		/// to retrieve the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <returns>A func that can be used to
		/// retrieve the property value. 
		/// The argument is the instance
		/// on which the property exists.</returns>
		Func<object, object> GetPropertyGetter(
			PropertyInfo propertyInfo);

		/// <summary>
		/// Retrieves or creates an func that can be used
		/// to retrieve the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <typeparam name="TProperty">
		/// The property type.
		/// </typeparam>
		/// <returns>A func that can be used to
		/// retrieve the property value. 
		/// The argument is the instance
		/// on which the property exists.</returns>
		Func<object, TProperty> GetPropertyGetter<TProperty>(
			PropertyInfo propertyInfo);

		/// <summary>
		/// Retrieve or create an action that can be used
		/// to set the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <returns>An action that can be used to
		/// set the property value. 
		/// The first argument is the instance
		/// on which the property exists.
		/// The second argument is new property value.</returns>
		Action<object, object> GetPropertySetter(
			PropertyInfo propertyInfo);

		/// <summary>
		/// Retrieve or create an action that can be used
		/// to set the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <typeparam name="TProperty">
		/// The property type.</typeparam>
		/// <returns>An action that can be used to
		/// set the property value. 
		/// The first argument is the instance
		/// on which the property exists.
		/// The second argument is new property value.</returns>
		Action<object, TProperty> GetPropertySetter<TProperty>(
			PropertyInfo propertyInfo);

		/// <summary>
		/// Determines whether if type2 is assignable from interfaceType.
		/// This is equivalent to calling <c>interfaceType.IsAssignableFrom(type2)</c>.
		/// </summary>
		/// <param name="interfaceType"></param>
		/// <param name="type2"></param>
		/// <returns><c>true</c> if type2 is assignable from interfaceType; 
		/// <c>false</c> otherwise.</returns>
		bool IsAssignableFrom(Type interfaceType, Type type2);

		/// <summary>
		/// Removes all cached values.
		/// </summary>
		void Clear();
	}
}