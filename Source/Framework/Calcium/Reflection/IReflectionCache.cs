#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 18:14:49Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Calcium.InversionOfControl;

namespace Calcium.Reflection
{
	/// <summary>
	/// Determines what approach should be taken when creating a delegate
	/// for property accessors and method invokers.
	/// <seealso cref="IReflectionCache"/>
	/// </summary>
	public enum DelegateCreationMode
	{
		/// <summary>
		/// The delegate takes a smaller amount of time to create,
		/// but execution of the delegate takes longer.
		/// The default implementation of <see cref="IReflectionCache"/>
		/// uses reflection for this value.
		/// </summary>
		FastCreationSlowPerformance,
		/// <summary>
		/// The delegate takes a longer amount of time to create,
		/// but execution of the delegate is faster in release builds.
		/// The default implementation of <see cref="IReflectionCache"/>
		/// uses expression trees for this value.
		/// </summary>
		SlowCreationFastPerformance
	}

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
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>An action that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.</returns>
		Action<object, object[]> GetVoidMethodInvoker(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode);

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
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		Func<object, object[], object> GetMethodInvoker(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode);

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
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>A func that can be used to
		/// call the method. The first argument is the instance
		/// on which the method exists. The <c>object[]</c>
		/// contains the arguments for the call.
		/// The last return argument is the result of the method call.
		/// If the method has a void return type, then <c>null</c>
		/// is always returned.</returns>
		Func<object, object[], TReturn> GetMethodInvoker<TReturn>(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode);

		/// <summary>
		/// Retrieves or creates an func that can be used
		/// to retrieve the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>A func that can be used to
		/// retrieve the property value. 
		/// The argument is the instance
		/// on which the property exists.</returns>
		Func<object, object> GetPropertyGetter(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode);

		/// <summary>
		/// Retrieves or creates an func that can be used
		/// to retrieve the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <typeparam name="TProperty">
		/// The property type.
		/// </typeparam>
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>A func that can be used to
		/// retrieve the property value. 
		/// The argument is the instance
		/// on which the property exists.</returns>
		Func<object, TProperty> GetPropertyGetter<TProperty>(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode);

		Func<TOwner, TProperty> GetPropertyGetter<TOwner, TProperty>(
			Expression<Func<TOwner, TProperty>> propertyExpression);

		/// <summary>
		/// Retrieve or create an action that can be used
		/// to set the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>An action that can be used to
		/// set the property value. 
		/// The first argument is the instance
		/// on which the property exists.
		/// The second argument is new property value.</returns>
		Action<object, object> GetPropertySetter(
			PropertyInfo propertyInfo, 
			DelegateCreationMode creationMode);

		/// <summary>
		/// Retrieve or create an action that can be used
		/// to set the value of a property.
		/// </summary>
		/// <param name="propertyInfo">
		/// The property info instance for the property.</param>
		/// <typeparam name="TProperty">
		/// The property type.</typeparam>
		/// <param name="creationMode">
		/// Determines how the resulting delegate is created.
		/// <see cref="DelegateCreationMode"/></param>
		/// <returns>An action that can be used to
		/// set the property value. 
		/// The first argument is the instance
		/// on which the property exists.
		/// The second argument is new property value.</returns>
		Action<object, TProperty> GetPropertySetter<TProperty>(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode);

		Func<object[], object> GetConstructorFunc(
			ConstructorInfo info,
			DelegateCreationMode creationMode);

		// /// <summary>
		// /// Determines whether if type2 is assignable from interfaceType.
		// /// This is equivalent to calling <c>interfaceType.IsAssignableFrom(type2)</c>.
		// /// </summary>
		// /// <param name="interfaceType"></param>
		// /// <param name="type2"></param>
		// /// <returns><c>true</c> if type2 is assignable from interfaceType; 
		// /// <c>false</c> otherwise.</returns>
		// bool IsAssignableFrom(Type interfaceType, Type type2);

		/// <summary>
		/// Gets properties decorated with a specified attribute.
		/// </summary>
		/// <param name="classType">The owner type containing properties 
		/// that may be decorated with the specified <c>attributeType</c>.</param>
		/// <param name="attributeType">The type of the attribute decorating the properties.</param>
		/// <param name="includeAncestorClassProperties">
		/// If <c>true</c> properties of base classes are included;
		/// otherwise not.</param>
		/// <returns>A list of the properties with the decorated attributes 
		/// of the specified type.</returns>
		IEnumerable<PropertyWithAttribute> GetPropertyAttributesForClass(
			Type classType, Type attributeType, bool includeAncestorClassProperties = true);

		/// <summary>
		/// Removes all cached values.
		/// </summary>
		void Clear();
	}
}
