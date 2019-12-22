#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-15 12:24:24Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

using Codon.InversionOfControl;

namespace Codon
{
	/// <summary>
	/// This class is used to retrieve object instances, using type associations,
	/// and to create associations between types and object instances.
	/// </summary>
	public static class Dependency
	{
		static IContainer container;

		internal static IContainer Container
		{
			get
			{
				if (container == null)
				{
					container = new FrameworkContainer();
					container.InitializeContainer();
					container.Register<IContainer>(container);
					Initialized = true;
				}

				return container;
			}
			set => container = value;
		}

		/// <summary>
		/// Creates a type association between one type TTo, to another type TFrom;
		/// so that when the TFrom type is requested using e.g., <c>Resolve</c>, 
		/// an instance of the TTo type is returned. 
		/// </summary>
		/// <typeparam name="TFrom">The type forming the whole or partial key 
		/// for resolving the TTo type.</typeparam>
		/// <typeparam name="TTo">The associated type.</typeparam>
		/// <param name="key">The key. Can be <c>null</c>.</param>
		/// <param name="singleton">if set to <c>true</c> 
		/// only one instance will be created of the TTo type.</param>
		public static void Register<TFrom, TTo>(bool singleton = false, string key = null) 
			where TTo : TFrom
		{
			Container.Register<TFrom, TTo>(singleton, key);
		}
		
		/// <summary>
		/// Associate the <c>TFrom</c> type with the specified 
		/// singleton instance.
		/// </summary>
		/// <typeparam name="TFrom">When <c>Resolve</c> is called
		/// using the specified type, the instance is returned.</typeparam>
		/// <param name="instance">The instance to be registered as a singleton.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		public static void Register<TFrom>(TFrom instance, string key = null)
		{
			Container.Register<TFrom>(instance, key);
		}

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
		public static void Register(Type fromType, Type toType, bool singleton = false, string key = null)
		{
			Container.Register(fromType, toType, singleton, key);
		}

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
		public static void Register(
			Type fromType, object instance, string key = null)
		{
			Container.Register(fromType, instance, key);
		}

		/// <summary>
		/// Associate the generic type T. 
		/// When <c>Resolve</c> is called an instance
		/// of <c>T</c> is created. If <c>singleton</c>
		/// is <c>true</c>, the instance is retained and returned
		/// upon subsequent calls to <c>Resolve</c>.
		/// </summary>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>T</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		public static void Register<T>(string key, bool singleton = false)
		{
			Container.Register<T, T>(singleton, key);
		}

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
		public static void Register<TFrom>(
			Func<TFrom> getInstanceFunc, 
			bool singleton = false, 
			string key = null)
		{
			Container.Register<TFrom>(getInstanceFunc, singleton, key);
		}

		/// <summary>
		/// Associate the <c>fromType</c> with a func
		/// to return an instance of that type. 
		/// When <c>Resolve</c> is called, getInstanceFunc is used
		/// to resolve an object that derives from <c>fromType</c>. 
		/// If <c>singleton</c> is <c>true</c>, the instance is retained 
		/// and returned upon subsequent calls to <c>Resolve</c>.
		/// </summary>
		/// <param name="fromType">
		/// The type to associate with the specified func.
		/// When an object of type <c>fromType</c> is requested by a caller,
		/// the <c>getInstanceFunc</c> is used top resolve
		/// an instance of that type.</param>
		/// <param name="getInstanceFunc">Retrieves an instance
		/// of type <c>fromType</c>.</param>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>fromType</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		public static void Register(
			Type fromType, 
			Func<object> getInstanceFunc, 
			bool singleton = false, 
			string key = null)
		{
			Container.Register(fromType, getInstanceFunc, singleton, key);
		}

		/// <summary>
		/// Determines if there is a type registration from the 
		/// specified from type <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The registered from type.</typeparam>
		/// <returns><c>true</c> if a type registration exists;
		/// <c>false</c> otherwise.</returns>
		public static bool IsRegistered<T>()
		{
			Type fromType = typeof(T);
			return Container.IsRegistered(fromType);
		}

		/// <summary>
		/// Determines if there is a type registration from the 
		/// specified from type <c>fromType</c>.
		/// </summary>
		/// <param name="fromType">
		/// The registered from type mapping.</param>
		/// <returns><c>true</c> if a type registration exists;
		/// <c>false</c> otherwise.</returns>
		public static bool IsRegistered(Type fromType)
		{
			return Container.IsRegistered(fromType);
		}

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
		public static T Resolve<T>(string key = null)
		{
			return Container.Resolve<T>(key);
		}

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
		public static object ResolveWithType(Type type, string key = null)
		{
			AssertArg.IsNotNull(type, nameof(type));
			if (key != null)
			{
				return Container.Resolve(type, key);
			}

			return Container.Resolve(type);
		}

		/// <summary>
		/// Resolves an object instance deriving from the specified
		/// from type <c>TFrom</c>.
		/// </summary>
		/// <typeparam name="TFrom">The registered from type mapping.
		/// </typeparam>
		/// <typeparam name="TDefaultImplementation">
		/// If no type mapping is associated with the <c>TFrom</c> type,
		/// than an instance of <c>TDefaultImplementation</c> is returned.
		/// </typeparam>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>fromType</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.
		/// Can be <c>null</c>.</returns>
		/// <exception cref="ResolutionException">
		/// Is raised if the type is unable to be located,
		/// or an exception is raised during resolution.</exception>
		public static TFrom Resolve<TFrom, TDefaultImplementation>(
			bool singleton = true, 
			string key = null)
			where TDefaultImplementation : TFrom
		{
			TFrom instance;
			if (Container.IsRegistered(typeof(TFrom), key))
			{
				instance = string.IsNullOrEmpty(key) 
					? Container.Resolve<TFrom>() 
					: Container.Resolve<TFrom>(key);
			}
			else
			{
				instance = Container.Resolve<TDefaultImplementation>();
				if (singleton)
				{
					Register<TFrom>(instance, key);
				}
				else
				{
					Register<TFrom, TDefaultImplementation>(false, key);
				}
			}

			return instance;
		}

		/// <summary>
		/// Resolves an object instance deriving from the specified
		/// from type <c>TFrom</c>.
		/// </summary>
		/// <typeparam name="TFrom">The registered from type mapping.
		/// </typeparam>
		/// <param name="defaultImplementation">
		/// If no type mapping is associated with the <c>TFrom</c> type,
		/// than the <c>TDefaultImplementation</c> instance is returned.
		/// </param>
		/// <param name="singleton">If <c>true</c> once an instance
		/// of <c>fromType</c> is created, it is retained, and returned
		/// upon subsequent calls to <c>Resolve</c>.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.
		/// Can be <c>null</c>.</returns>
		/// <exception cref="ResolutionException">
		/// Is raised if the type is unable to be located,
		/// or an exception is raised during resolution.</exception>
		public static TFrom ResolveOrRegister<TFrom>(
			TFrom defaultImplementation, 
			bool singleton = true, 
			string key = null)
		{
			Type fromType = typeof(TFrom);
			TFrom instance;

			if (Container.IsRegistered(fromType, key))
			{
				instance = string.IsNullOrEmpty(key) 
					? Container.Resolve<TFrom>() 
					: Container.Resolve<TFrom>(key);
			}
			else
			{
				instance = defaultImplementation;
				if (singleton)
				{
					Register<TFrom>(defaultImplementation, key);
				}
				else
				{
					if (!object.Equals(defaultImplementation, default(TFrom)))
					{
						Register(fromType, instance.GetType());
					}
				}
			}

			return instance;
		}

		/// <summary>
		/// Attempts to resolves an object instance deriving from the specified
		/// from type <c>T</c>.
		/// </summary>
		/// <typeparam name="T">The registered from type mapping.
		/// </typeparam>
		/// <param name="result">The resulting object of type <c>T</c>.
		/// </param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns>An instance of <c>T</c>.
		/// Can be <c>null</c>.</returns>
		public static bool TryResolve<T>(out T result, string key = null)
			where T : class
		{
//			try
//			{
//				/* When a developer has built the Codon on his or her machine, installed it, 
//                 * and is building applications using Codon, Visual Studio is over eager 
//                 * in breaking on exceptions that occur and are handled within the Codon assemblies. 
//                 * This can be rectified by turning of "Enable Just my code" 
//                 * in Visual Studio options under Debugging. Checking to see if a type 
//                 * is registered before resolving it, avoids an exception from being raised. */
//				result = Container.IsRegistered(typeof(T), key) 
//							? Resolve<T>(key) : null;
//			}
//			catch (Exception) /* Unable to be more specific because 
//							   * we don't know the container implementation. */
//			{
//				result = null;
//			}
//
//			return result != null;
			return Container.TryResolve<T>(out result, key);
		}

		/// <summary>
		/// Attempts to resolves an object instance deriving 
		/// from the specified <c>type</c>.
		/// </summary>
		/// <param name="type">The registered from type mapping.
		/// </param>
		/// <param name="result">The resulting object.</param>
		/// <param name="key">Multiple instance can be associated
		/// with a type. The key is used to differentiate them.</param>
		/// <returns><c>true</c> if a registered type exists for the 
		/// specified type; <c>false</c> otherwise.</returns>
		/// <exception cref="ResolutionException">
		/// Is raised if the type is unable to be located,
		/// or an exception is raised during resolution.</exception>
		public static bool TryResolve(
			Type type, 
			out object result, 
			string key = null)
		{
			try
			{
				result = Container.IsRegistered(type, key) 
							? ResolveWithType(type, key) : null;
			}
			catch (Exception) /* Unable to be more specific because 
							   * we don't know the container implementation. */
			{
				result = null;
			}

			return result != null;
		}

		/// <summary>
		/// Resolves all types that have a type registration
		/// for the specified <c>TFrom</c> type.
		/// </summary>
		/// <typeparam name="TFrom">The from type mapping.</typeparam>
		/// <returns>All objects that are registered with the 
		/// specified 'from type' mapping.</returns>
		public static IEnumerable<TFrom> ResolveAll<TFrom>() where TFrom : class
		{
			return Container.ResolveAll<TFrom>();
		}

		/// <summary>
		/// This property indicates whether or not an
		/// <see cref="IContainer"/> has been associated
		/// with the <c>Dependency</c> class.
		/// </summary>
		public static bool Initialized { get; internal set; }
	}
}
