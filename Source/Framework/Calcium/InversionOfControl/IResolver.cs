#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-03-27 15:10:49Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System;

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// Represents the retrieval aspect of an IoC container.
	/// <seealso cref="IContainer"/>
	/// </summary>
	public interface IResolver
	{
		/// <summary>
		/// Resolves an object instance deriving from the specified
		/// from type <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The registered from type mapping.
		/// </typeparam>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.</returns>
		/// <exception cref="ResolutionException">
		/// Is raised if the type is unable to be located,
		/// or an exception is raised during resolution.</exception>
		T Resolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string key = null);

		/// <summary>
		/// Resolves an object instance deriving from the specified
		/// from type <c>T</c>.
		/// </summary>
		/// <param name="type">The registered from type mapping.
		/// </param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.
		/// Can be <c>null</c>.</returns>
		/// <exception cref="ResolutionException">
		/// Is raised if the type is unable to be located,
		/// or an exception is raised during resolution.</exception>
		object Resolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string key = null);

		/// <summary>
		/// Resolves all types that have a type registration
		/// for the specified <c>TFrom</c> type.
		/// </summary>
		/// <typeparam name="TFrom">The from type mapping.</typeparam>
		/// <returns>All objects that are registered with the 
		/// specified 'from type' mapping.</returns>
		IEnumerable<TFrom> ResolveAll<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom>()
			where TFrom : class;

		/// <summary>
		/// Resolves all types that have a type registration
		/// for the specified <c>fromType</c> type.
		/// </summary>
		/// <returns>All objects that are registered with the 
		/// specified 'from type' mapping.</returns>
		IEnumerable<object> ResolveAll([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType);

		/// <summary>
		/// Resolves all objects that have a type registration
		/// with the specified key.
		/// </summary>
		/// <returns>All objects that are registered with the 
		/// specified key mapping.</returns>
		IEnumerable<object> ResolveAll(string key);

		/// <summary>
		/// Determines if there is a type registration from the 
		/// specified from type <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The registered from type.</typeparam>
		/// <returns><c>true</c> if a type registration exists;
		/// <c>false</c> otherwise.</returns>
		bool IsRegistered<T>(string key = null);

		/// <summary>
		/// Determines if there is a type registration from the 
		/// specified from type <c>fromType</c>.
		/// </summary>
		/// <param name="fromType">
		/// The registered from type mapping.</param>
		/// <param name="key">The ID of the item, which
		/// can be used to retrieve the item if there are multiple
		/// instances that use the same <c>fromType</c>.</param>
		/// <returns><c>true</c> if a type registration exists;
		/// <c>false</c> otherwise.</returns>
		bool IsRegistered(Type fromType, string key = null);

		/// <summary>
		/// Attempts to resolve an object instance deriving from the specified
		/// from type <c>T</c>. Returns <c>true</c> if the instance
		/// is registered; <c>false</c> if no registration exists.
		/// This method does *not* swallow exceptions thrown during resolution.
		/// </summary>
		/// <typeparam name="T">The registered from type mapping.</typeparam>
		/// <param name="result">The resulting object of type <c>T</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.
		/// Can be <c>null</c>.</returns>
		/// <exception cref="ResolutionException">
		/// May be thrown during resolution of the instance.</exception>
		bool TryResolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
			out T result, string key = null);

		/// <summary>
		/// Attempts to resolve an object instance deriving from the specified
		/// type. Returns <c>true</c> if the instance
		/// is registered; <c>false</c> if no registration exists.
		/// This method does *not* swallow exceptions thrown during resolution.
		/// </summary>
		/// <param name="type">The type of object to resolve.</param>
		/// <param name="result">The resulting object of type <c>T</c>.
		/// </param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>Returns <c>true</c> if the instance
		/// is registered; <c>false</c> if no registration exists.
		/// This method does *not* swallow exceptions thrown during resolution.</returns>
		/// <exception cref="ResolutionException">
		/// May be thrown during resolution of the instance.</exception>
		bool TryResolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type,
						out object result,
						string key = null);
	}
}
