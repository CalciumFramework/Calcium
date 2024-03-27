#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-27 15:13:08Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// Required interface for an IoC container
	/// to be used with the framework.
	/// </summary>
	public interface IContainer : IResolver
	{
		/// <summary>
		/// Creates a type association between one type <c>TTo</c>, 
		/// to another type <c>TFrom</c>; so that when the <c>TFrom</c> type 
		/// is requested using e.g., <c>Resolve</c>, 
		/// an instance of <c>TTo</c> is returned. 
		/// </summary>
		/// <typeparam name="TFrom">The type forming the whole or partial key 
		/// for resolving the <c>TTo</c> type.</typeparam>
		/// <typeparam name="TTo">The associated type.</typeparam>
		/// <param name="key">The key. Can be <c>null</c>.</param>
		/// <param name="singleton">if set to <c>true</c> 
		/// only one instance will be created of the <c>TTo</c> type.</param>
		void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom,
					  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TTo>(
			bool singleton = false, string key = null) where TTo : TFrom;

		/// <summary>
		/// Associate the <c>fromType</c> type with the specified 
		/// toType. When <c>Resolve</c> is called an instance
		/// of <c>toType</c> is created.
		/// </summary>
		/// <param name="fromType">The type used to resolve the object.</param>
		/// <param name="toType">The type to be created.</param>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>toType</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
					  [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType, 
					  bool singleton = false, 
					  string key = null);

		/// <summary>
		/// Associate the <c>TFrom</c> type with the specified 
		/// singleton instance.
		/// </summary>
		/// <typeparam name="TFrom">When <c>Resolve</c> is called
		/// using the specified type, the instance is returned.</typeparam>
		/// <param name="instance">The instance to be registered as a singleton.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom>(TFrom instance, string key = null);

		/// <summary>
		/// Associate the generic type <c>TFrom</c> with a func
		/// to return an instance of type <c>TFrom</c>. 
		/// When <c>Resolve</c> is called, getInstanceFunc is used
		/// to resolve an instance of <c>TFrom</c>. If <c>singleton</c>
		/// is <c>true</c>, the instance is retained and returned
		/// upon subsequent calls to <c>Resolve</c>.
		/// </summary>
		/// <param name="getInstanceFunc">Retrieves an instance
		/// of type <c>TFrom</c>.</param>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>TFrom</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom>(
			Func<TFrom> getInstanceFunc, bool singleton = false, string key = null);

		/// <summary>
		/// Associate the <c>fromType</c> with a func
		/// to return an instance of that type. 
		/// When <c>Resolve</c> is called, getInstanceFunc is used
		/// to resolve an object that derives from <c>fromType</c>. 
		/// If <c>singleton</c> is <c>true</c>, the instance is retained 
		/// and returned upon subsequent calls to <c>Resolve</c>.
		/// </summary>
		/// <param name="type">
		/// The type to associate with the specified func.
		/// When an object of type <c>type</c> is requested by a caller,
		/// the <c>getInstanceFunc</c> is used top resolve
		/// an instance of that type.</param>
		/// <param name="getInstanceFunc">Retrieves an instance
		/// of type <c>fromType</c>.</param>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>fromType</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, 
					  Func<object> getInstanceFunc, 
					  bool singleton = false, 
					  string key = null);

		/// <summary>
		/// Associate the <c>fromType</c> type with the specified 
		/// instance. When <c>Resolve</c> is called the instance
		/// is returned.
		/// </summary>
		/// <param name="fromType">
		/// The type used to resolve the object.</param>
		/// <param name="instance">
		/// An instance deriving from <c>fromType</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType, 
					  object instance, 
					  string key = null);
	}
}
