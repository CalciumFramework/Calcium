#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 13:11:43Z</CreationDate>
</File>
*/

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Calcium.Logging;
using Calcium.Platform;
using Calcium.Reflection;

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// This <c>IContainer</c> supports, singleton and non-singleton
	/// mapping from concrete types and interface types to concrete types,
	/// object instances, lambda expressions. 
	/// It also supports dependency injection.
	/// </summary>
	public class FrameworkContainer : IContainer
	{
		readonly Dictionary<Type, ResolverDictionary> typeDictionary 
			= new Dictionary<Type, ResolverDictionary>();
		readonly Dictionary<string, Type> keyDictionary 
			= new Dictionary<string, Type>();

		static readonly string defaultKey = Guid.NewGuid().ToString();
		readonly ReaderWriterLockSlim lockSlim 
			= new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		readonly Dictionary<Type, List<string>> cycleDictionary 
			= new Dictionary<Type, List<string>>();

		readonly Dictionary<Type, ConstructorInvokeInfo> constructorDictionary
			= new Dictionary<Type, ConstructorInvokeInfo>();
		readonly Dictionary<Type, List<PropertyInfo>> injectablePropertyDictionary
			= new Dictionary<Type, List<PropertyInfo>>();
		readonly Dictionary<string, Action<object, object>> propertyActionDictionary
			= new Dictionary<string, Action<object, object>>();
		readonly Dictionary<Type, PropertyInfo[]> propertyDictionary
			= new Dictionary<Type, PropertyInfo[]>();

		readonly ReaderWriterLockSlim constructorDictionaryLockSlim
			= new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		bool cacheEnabled = true;

		public bool CacheEnabled
		{
			get => cacheEnabled;
			set
			{
				if (cacheEnabled != value)
				{
					cacheEnabled = value;
					if (!cacheEnabled)
					{
						ClearCache();
					}
				}
			}
		}

		void ClearCache()
		{
			constructorDictionary.Clear();
			injectablePropertyDictionary.Clear();
			propertyActionDictionary.Clear();
			propertyDictionary.Clear();
		}

		/// <summary>
		/// If <c>true</c>, a <see cref="INamedTypeResolver"/> is used to resolve
		/// named types that have been declared via a <see cref="DefaultTypeNameAttribute"/>.
		/// Default value is <c>false</c> (it will not be used).
		/// Enabling this may degrade performance depending on the implementation of the type resolver.
		/// <seealso cref="NamedTypeResolver"/>.
		/// </summary>
		public bool NamedTypeResolverEnabled { get; set; } = false;

		/// <summary>
		/// Prevents multiple threads from creating more 
		/// than one singleton instance.
		/// Decreases performance if <c>true</c>.
		/// Default value is <c>false</c>.
		/// </summary>
		public bool ThreadSafe { get; set; }

		/// <summary>
		/// If <c>true</c> the container resolves values for properties 
		/// that are decorated with a <see cref="InjectDependenciesAttribute"/>.
		/// There's a performance penalty for that. If you do not use
		/// <c>InjectDependenciesAttribute</c> for properties, 
		/// set this property to false to improve performance.
		/// Default value is <c>true</c>.
		/// </summary>
		public bool PropertyInjectionAttributesEnabled { get; set; } = true;

		sealed class ResolverDictionary : Dictionary<string, Resolver>
		{
		}

		sealed class Resolver
		{
			public Func<object> CreateInstanceFunc { get; set; }
			public object Instance;
			public bool Singleton;

			public object GetObject()
			{
				if (Singleton)
				{
					if (Instance != null)
					{
						return Instance;
					}

					Instance = CreateInstanceFunc();

					if (Instance != null)
					{
						CreateInstanceFunc = null;
					}

					return Instance;
				}

				return CreateInstanceFunc();
			}
		}

		public void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom,
							 [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TTo>(
			bool singleton = false, 
			string key = null) where TTo : TFrom
		{
			Type fromType = typeof(TFrom);
			Type toType = typeof(TTo);
			Register(fromType, toType, singleton, key);
		}

		public void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType,
							 [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type toType, 
							 bool singleton = false, 
							 string key = null)
		{
			key = GetKeyValueOrDefault(key);

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterWriteLock();
			}

			try
			{
				if (!typeDictionary.TryGetValue(fromType, 
						out ResolverDictionary resolverDictionary))
				{
					resolverDictionary = new ResolverDictionary();
					typeDictionary[fromType] = resolverDictionary;
				}

				Resolver resolver = new Resolver
				{
					CreateInstanceFunc = () => Instantiate(toType),
					Singleton = singleton
				};

				resolverDictionary[key] = resolver;
				keyDictionary[key] = fromType;
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		public void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TInterface>(
			TInterface instance, string key = null)
		{
			key = GetKeyValueOrDefault(key);
			Type type = typeof(TInterface);

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterWriteLock();
			}

			try
			{
				if (!typeDictionary.TryGetValue(type, 
						out ResolverDictionary value))
				{
					value = new ResolverDictionary();
				}

				value[key] = new Resolver { CreateInstanceFunc = () => instance };
				typeDictionary[type] = value;
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		public void Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
			Func<T> getInstanceFunc, 
			bool singleton = false, 
			string key = null)
		{
			AssertArg.IsNotNull(getInstanceFunc, nameof(getInstanceFunc));

			key = GetKeyValueOrDefault(key);
			Type type = typeof(T);

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterWriteLock();
			}

			try
			{
				if (!typeDictionary.TryGetValue(type, 
					out ResolverDictionary resolverDictionary))
				{
					resolverDictionary = new ResolverDictionary();
				}

				Resolver resolver = new Resolver { Singleton = singleton };

				Func<object> getObjectFunc = () =>
				{
					if (cycleDictionary.TryGetValue(type, out List<string> keys))
					{
						if (keys.Contains(key))
						{
							/* TODO: Rather than throwing an exception, 
							 * we could Post the property set operation to the UI thread. */
							throw new ResolutionException(
								$"Cycle detected for {type} with key \"{key}\"");
						}

						keys.Add(key);
					}
					else
					{
						keys = new List<string> { key };
						cycleDictionary.Add(type, keys);
					}

					T result;
					try
					{
						result = getInstanceFunc();
					}
					finally
					{
						keys.Remove(key);
						if (!keys.Any())
						{
							cycleDictionary.Remove(type);
						}
					}

					if (resolver.Singleton)
					{
						resolver.CreateInstanceFunc = null;
						resolver.Instance = result;
					}

					return result;
				};

				resolver.CreateInstanceFunc = getObjectFunc;
				
				resolverDictionary[key] = resolver;
				keyDictionary[key] = type;

				typeDictionary[type] = resolverDictionary;
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		public void Register(
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, 
			Func<object> getInstanceFunc, 
			bool singleton = false, 
			string key = null)
		{
			AssertArg.IsNotNull(type, nameof(type));
			AssertArg.IsNotNull(getInstanceFunc, nameof(getInstanceFunc));

			key = GetKeyValueOrDefault(key);

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterWriteLock();
			}

			try
			{
				if (!typeDictionary.TryGetValue(type, 
						out ResolverDictionary resolverDictionary))
				{
					resolverDictionary = new ResolverDictionary();
				}

				Resolver resolver = new Resolver { Singleton = singleton };

				Func<object> getObjectFunc = () =>
				{
					var result = getInstanceFunc();

					if (resolver.Singleton)
					{
						resolver.CreateInstanceFunc = null;
						resolver.Instance = result;
					}

					return result;
				};

				resolver.CreateInstanceFunc = getObjectFunc;
				
				resolverDictionary[key] = resolver;
				keyDictionary[key] = type;

				typeDictionary[type] = resolverDictionary;
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		public void Register([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, 
							 object instance, 
							 string key = null)
		{
			AssertArg.IsNotNull(type, nameof(type));

			key = GetKeyValueOrDefault(key);

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterWriteLock();
			}

			try
			{
				if (!typeDictionary.TryGetValue(
						type, out ResolverDictionary value))
				{
					value = new ResolverDictionary();
				}

				Resolver info = new Resolver { CreateInstanceFunc = () => instance };

				value[key] = info;

				typeDictionary[type] = value;
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		public bool IsRegistered<T>(string key = null)
		{
			return IsRegistered(typeof(T), key);
		}

		public bool IsRegistered(Type fromType, string key = null)
		{
			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterReadLock();
			}

			try
			{
				if (!string.IsNullOrEmpty(key))
				{
					/* Improve performance of this by using a lookup dictionary. */
					foreach (KeyValuePair<Type, ResolverDictionary> pair in typeDictionary)
					{
						if (!fromType.GetTypeInfo().IsAssignableFrom(pair.Key.GetTypeInfo()))
						{
							continue;
						}

						var funcDictionary = pair.Value;
						if (pair.Value == null)
						{
							continue;
						}

						if (funcDictionary.ContainsKey(key))
						{
							return true;
						}
					}
				}

				if (typeDictionary.TryGetValue(fromType, out ResolverDictionary dictionary) 
					&& dictionary != null)
				{
					if (dictionary.ContainsKey(key ?? defaultKey))
					{
						return true;
					}
				}
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitReadLock();
				}
			}

			return false;
		}

		public T Resolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string key = null)
		{
			return (T)ResolveAux(typeof(T), key);
		}

		public object Resolve([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, string key = null)
		{
			AssertArg.IsNotNull(type, nameof(type));
			return ResolveAux(type, key);
		}

		object ResolveAux(
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type, 
			string key = null, 
			Dictionary<string, object> resolvedObjects = null)
		{
			bool alreadyResolved = false;
			object result = null;
			if (resolvedObjects != null && key != null)
			{
				if (resolvedObjects.TryGetValue(key, out result))
				{
					alreadyResolved = true;
				}
			}

			if (!alreadyResolved)
			{
				result = ResolveCore(type, key);

				if (key != null && resolvedObjects != null)
				{
					resolvedObjects[key] = result;
				}

				if (result != null && PropertyInjectionAttributesEnabled)
				{
					ResolveProperties(result, resolvedObjects);
				}
			}

			resolvedObjects?.Clear();

			return result;
		}

		public IEnumerable<object> ResolveAll(string key)
		{
			AssertArg.IsNotNull(key, nameof(key));

			List<object> result = new List<object>();
			
			foreach (KeyValuePair<Type, ResolverDictionary> pair in typeDictionary)
			{
				var dictionary = pair.Value;
				if (dictionary.TryGetValue(key, out Resolver resolver))
				{
					var item = resolver.Instance ?? resolver.CreateInstanceFunc?.Invoke();
					result.Add(item);
				}
			}

			return result;
		}

		public IEnumerable<object> ResolveAll([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type fromType)
		{
			AssertArg.IsNotNull(fromType, nameof(fromType));

			var list = new List<object>();

			ResolverDictionary resolverDictionary;
			bool retrievedValue;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterReadLock();
			}

			try
			{
				retrievedValue = typeDictionary.TryGetValue(fromType, out resolverDictionary);
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitReadLock();
				}
			}

			if (!retrievedValue)
			{
				return list;
			}

			if (resolverDictionary == null)
			{
				object builtObject = BuildUp(fromType, null);
				if (builtObject != null)
				{
					list.Add(builtObject);
				}
			}
			else
			{
				foreach (Resolver resolver in resolverDictionary.Values)
				{
					var item = resolver.GetObject();
					list.Add(item);
				}
			}

			return list;
		}

		public IEnumerable<TFrom> ResolveAll<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TFrom>()
			where TFrom : class
		{
			Type fromType = typeof(TFrom);
			List<TFrom> list = new List<TFrom>();

			ResolverDictionary dictionary;
			bool retrieved;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterReadLock();
			}

			try
			{
				retrieved = typeDictionary.TryGetValue(fromType, out dictionary);
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitReadLock();
				}
			}
			
			if (!retrieved)
			{
				return list;
			}

			foreach (var resolver in dictionary.Values)
			{
				var item = (TFrom)resolver.GetObject();
				list.Add(item);
			}

			return list;
		}

		public bool TryResolve<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(out T result, string key = null)
		{
			Type type = typeof(T);

			if (IsRegistered(type, key))
			{
				result = (T)ResolveCore(type, key);
				return true;
			}

			result = default;
			return false;
		}

		public bool TryResolve(Type type, out object result, string key = null)
		{
			if (IsRegistered(type, key))
			{
				result = ResolveCore(type, key);
				return true;
			}

			result = null;
			return false;
		}

		object ResolveCore(Type type, string key, bool raiseExceptionIfNotResolved = true)
		{
			key = GetKeyValueOrDefault(key);

			if (type == null)
			{
				type = ResolveType(key);

				if (type == null)
				{
					if (raiseExceptionIfNotResolved)
					{
						throw new ResolutionException(
							"Failed to resolve type for " + key);
					}

					return null;
				}
			}

			Resolver resolver = null;

			bool newTypeRegistered = false;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterUpgradeableReadLock();
			}

			try
			{
				if (typeDictionary.TryGetValue(type, out ResolverDictionary resolvers)
					&& resolvers != null)
				{
					/* If the ResolveProperties method calls through here, 
					 * it may be searching for a registration per
					 * strongly-typed name that does not exist. 
					 * In this case, just get the default registration. */
					if (!resolvers.TryGetValue(key, out resolver))
					{
						key = GetKeyValueOrDefault(null);
						resolver = resolvers[key];
					}
				}
				else
				{
					if (type.IsInterface())
					{
						var typeInfo = type.GetTypeInfo();
						var typeNameAttribute = typeInfo.GetCustomAttribute<DefaultTypeNameAttribute>();
						bool typeNameValid = false;
						bool failedFindingByTypeName = false;

						if (typeNameAttribute != null)
						{
							var defaultType = ResolveType(typeNameAttribute.TypeName);

							if (defaultType == null)
							{
								failedFindingByTypeName = true;
							}
							else
							{
								if (defaultType.IsInterface())
								{
									throw new ResolutionException(
										$"Invalid default type mapping. Type \'{type}\' " +
										$"has a default mapping of \'{defaultType}\', which must be a concrete type not an interface.");
								}

								typeNameValid = true;

								bool singleton = typeNameAttribute.Singleton;

								Register(type, defaultType, singleton);

								newTypeRegistered = true;
							}
						}

						if (!typeNameValid)
						{
							var typeAttribute = typeInfo.GetCustomAttribute<DefaultTypeAttribute>();

							if (typeAttribute != null)
							{
								Type defaultType = typeAttribute.Type;

								if (failedFindingByTypeName)
								{
									/* DebugLog makes use of the IPlatformIdentifier.
									 * To prevent a stack overflow, we don't log. */
									if (typeof(ILog) != type && typeof(IPlatformIdentifier) != type)
									{
										if (Log.DebugEnabled)
										{
											Log.Debug("Unable to resolve type using DefaultTypeNameAttribute with value: "
													+ $"{typeNameAttribute.TypeName}. Attempting to resolve default type '{defaultType}' instead.");
										}
									}
								}

								if (defaultType.IsInterface())
								{
									throw new ResolutionException(
										$"Invalid default type mapping. Type \'{type}\' " +
										$"has a default mapping of \'{defaultType}\', which must be a concrete type not an interface.");
								}

								bool singleton = typeAttribute.Singleton;

								Register(type, defaultType, singleton);

								newTypeRegistered = true;
							}
							else
							{
								if (typeNameAttribute != null)
								{
									if (raiseExceptionIfNotResolved)
									{
										throw new ResolutionException(
											"Unable to resolve mapping for type '" + type + "' There is a default type '" + typeNameAttribute.TypeName + 
											"' that was expected to be found in a platform specific library. " +
											"You may be missing a NuGet reference to Calcium.YourPlatform or Calcium.Extras.YourPlatform" +
											"Please add a reference to the platform specified library containing an implementation for this interface, " +
											"or register another implementation with the container.");
									}
									else
									{
										return null;
									}
								}

								/* Types in assemblies that do not reference the Framework assembly 
								 * can also use a system DefaultValueAttribute to register 
								 * the default type, like so:
								 * [System.ComponentModel.DefaultValue(typeof(UndoService))]
								 * Types registered in this manner are presumed to be singletons. */
								var defaultValueAttribute = typeInfo.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();

								if (defaultValueAttribute != null)
								{
									Type defaultType = defaultValueAttribute.Value as Type;

									if (defaultType.IsInterface())
									{
										throw new ResolutionException(
											$"Invalid default value mapping. Type \'{type}\' " +
											$"has a default mapping of \'{defaultType}\', which must be a concrete type not an interface.");
									}

									Register(type, defaultType, true);

									newTypeRegistered = true;
								}
								else
								{
									if (raiseExceptionIfNotResolved)
									{
										throw new ResolutionException(
											"Interface type " + type +
											" has no registered type mapping, " +
											"nor is there a default type mapping.");
									}
									else
									{
										return null;
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitUpgradeableReadLock();
				}
			}

			if (newTypeRegistered)
			{
				return ResolveCore(type, key);
			}

			if (resolver != null)
			{
				return resolver.GetObject();
			}

			return BuildUp(type, key);
		}

		ILog logUseProperty;

		ILog Log => logUseProperty ?? (logUseProperty = Dependency.Resolve<ILog>());

		readonly Dictionary<string, Type> typeCache 
			= new Dictionary<string, Type>();

		Type ResolveType(string key)
		{
			Type result = GetTypeFromContainer(key);
			if (result != null)
			{
				return result;
			}

			if (typeCache.TryGetValue(key, out result))
			{
				return result;
			}

			/*  Not in the container? Try the Assembly. */
			result = Type.GetType(key, false);
			
			if (result == null)
			{
				/* Attempt to find the full assembly qualified type name. */
				string suffix = PlatformDetector.PlatformName;
				if (suffix != null)
				{
					result = Type.GetType(key + "." + suffix, false);
				}
			}

			if (result == null)
			{
				if (NamedTypeResolverEnabled)
				{
					var resolver = Resolve<INamedTypeResolver>();
					resolver.TryResolveType(key, out result);
				}
			}

			if (result != null)
			{
				typeCache[key] = result;
			}

			return result;
		}

		Type GetTypeFromContainer(string key)
		{
			if (!keyDictionary.TryGetValue(key, out Type result))
			{
				return null;
			}

			return result;
		}

		void ResolveProperties(object instance, Dictionary<string, object> resolvedObjects)
		{
			Type type = instance.GetType();
			List<PropertyInfo> injectableProperties;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterReadLock();
			}

			try
			{
				bool hasCachedInjectableProperties = injectablePropertyDictionary.TryGetValue(type, out injectableProperties);

				if (!hasCachedInjectableProperties)
				{
					bool hasCachedProperties = propertyDictionary.TryGetValue(type, 
														out PropertyInfo[] properties);

					if (!hasCachedProperties)
					{
#if NETSTANDARD || NETFX_CORE
						properties = type.GetTypeInfo().DeclaredProperties.ToArray();
#else
						properties = type.GetProperties();
#endif
						if (cacheEnabled)
						{
							propertyDictionary[type] = properties;
						}
					}

					injectableProperties = new List<PropertyInfo>();

					foreach (PropertyInfo propertyInfo in properties)
					{
						var dependencyAttributes = propertyInfo.GetCustomAttributes(typeof(InjectDependenciesAttribute), false);
#if NETSTANDARD || NETFX_CORE
						if (!dependencyAttributes.Any())
#else
						if (dependencyAttributes.Length < 1) /* Faster than using LINQ. */
#endif
						{
							continue;
						}

						injectableProperties.Add(propertyInfo);
					}

					if (cacheEnabled)
					{
						injectablePropertyDictionary[type] = injectableProperties;
					}
				}
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitReadLock();
				}
			}

			if (injectableProperties == null || injectableProperties.Count < 1)
			{
				return;
			}

			if (resolvedObjects == null)
			{
				resolvedObjects = new Dictionary<string, object>();
			}

			string typeName = type.FullName + ".";

			foreach (PropertyInfo propertyInfo in injectableProperties)
			{
				string fullPropertyName = typeName + propertyInfo.Name;

				object propertyValue = ResolveAux(propertyInfo.PropertyType, fullPropertyName, resolvedObjects);


				if (!propertyActionDictionary.TryGetValue(fullPropertyName, 
												out Action<object, object> setter))
				{
					var setMethod = propertyInfo.SetMethod;
					setter = (owner, newValue) => setMethod.Invoke(owner, new[] { newValue });//ReflectionCompiler.CreatePropertySetter(propertyInfo);
					if (cacheEnabled)
					{
						propertyActionDictionary[fullPropertyName] = setter;
					}
				}

				setter(instance, propertyValue);
			}
		}
		
		object BuildUp(Type type, string key)
		{
			if (type == null)
			{
				throw new ResolutionException("type should not be null. Key: " + key);
			}

			object instance = null;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				lockSlim.EnterReadLock();
			}

			try
			{
				if (typeDictionary.TryGetValue(type, 
					out ResolverDictionary resolverDictionary))
				{
					if (resolverDictionary.TryGetValue(key, out Resolver resolver))
					{
						instance = resolver.GetObject();
					}
				}
			}
			finally
			{
				if (useLock)
				{
					lockSlim.ExitReadLock();
				}
			}

			if (instance != null)
			{
				return instance;
			}

			return Instantiate(type);
		}

		// class ConstructorInvokeInfo
		// {
		// 	internal ParameterInfo[] ParameterInfos
		// 	{
		// 		get
		// 		{
		// 			constructorInfoReference.TryGetTarget(out var target);
		// 			return target?.GetParameters();
		// 		}
		// 	}
		//
		// 	//Func<object[], object> constructorFunc;
		//
		// 	//internal Func<object[], object> ConstructorFunc => 
		// 	//constructorFunc ?? (constructorFunc = Constructor.Invoke);// = ReflectionCompiler.CreateConstructorFunc(Constructor);
		//
		// 	internal Func<object[], object> ConstructorFunc {
		// 		get
		// 		{
		// 			var constructor = Constructor;
		// 			if (constructor == null)
		// 			{
		// 				return null;
		// 			}
		//
		// 			return objects => constructor.Invoke(objects);
		// 		}
		// 	}
		//
		// 	readonly WeakReference<ConstructorInfo> constructorInfoReference;
		//
		// 	internal ConstructorInfo Constructor
		// 	{
		// 		get
		// 		{
		// 			constructorInfoReference.TryGetTarget(out var target);
		// 			return target;
		// 		}
		// 	}
		//
		// 	internal ConstructorInvokeInfo(ConstructorInfo constructor)
		// 	{
		// 		constructorInfoReference = new WeakReference<ConstructorInfo>(constructor);
		// 	}
		// }

		class ConstructorInvokeInfo
		{
			internal readonly ParameterInfo[] ParameterInfos;

			Func<object[], object> constructorFunc;

			internal Func<object[], object> ConstructorFunc =>
				constructorFunc ?? (constructorFunc = Constructor.Invoke);// = ReflectionCompiler.CreateConstructorFunc(Constructor);

			internal readonly ConstructorInfo Constructor;

			internal ConstructorInvokeInfo(ConstructorInfo constructor)
			{
				Constructor = constructor;
				ParameterInfos = constructor.GetParameters();
			}
		}

		object Instantiate(ConstructorInvokeInfo info)
		{
			var constructorParameters = info.ParameterInfos;
			var length = constructorParameters.Length;
			var parametersList = new object[length];
			for (int i = 0; i < length; i++)
			{
				ParameterInfo parameterInfo = constructorParameters[i];
				object parameter;
				try
				{
					parameter = Resolve(parameterInfo.ParameterType);
				}
				catch (Exception ex)
				{
					throw new ResolutionException(
						$"Failed to resolve parameter '{parameterInfo.Name}' in constructor for type '{info.Constructor?.DeclaringType}'", ex);
				}
				
				if (parameter == null && !parameterInfo.IsOptional)
				{
					throw new ResolutionException(
						$"Failed to instantiate non-optional parameter '{parameterInfo.Name}' in constructor for type '{info.Constructor?.DeclaringType}'");
				}

				parametersList[i] = parameter;
			}

			var constructorFunc = info.ConstructorFunc;
			try
			{
				return constructorFunc(parametersList);
			}
			catch (Exception ex)
			{
				throw new ResolutionException("Failed to resolve " 
					+ info.Constructor.DeclaringType, ex);
			}
		}

		object Instantiate(Type type)
		{
			ConstructorInvokeInfo invokeInfo;

			bool useLock = ThreadSafe;
			if (useLock)
			{
				constructorDictionaryLockSlim.EnterReadLock();
			}

			try
			{
				if (constructorDictionary.TryGetValue(type, out invokeInfo))
				{
					return Instantiate(invokeInfo);
				}
			}
			finally
			{
				if (useLock)
				{
					constructorDictionaryLockSlim.ExitReadLock();
				}
			}

#if NETFX_CORE
			var constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => !x.IsStatic && x.IsPublic).ToArray();
#else
			var constructors = type.GetTypeInfo().DeclaredConstructors;
#endif

			ConstructorInfo constructor = null;

			if (constructors.Any())
			{
				ConstructorInfo bestMatch = null;
				int biggestLength = -1;

				foreach (ConstructorInfo constructorInfo in constructors)
				{
					var dependencyAttributes = constructorInfo.GetCustomAttributes(typeof(InjectDependenciesAttribute), false).ToList();
					//#if NETSTANDARD || NETFX_CORE
					//					/* Unfortunately LINQ .Count() is much slower than .Length */
					//					var attributeCount = dependencyAttributes.Count();
					//#else
					//					var attributeCount = dependencyAttributes.Length;
					//#endif
					var attributeCount = dependencyAttributes.Count;
					bool hasAttribute = attributeCount > 0;

					if (hasAttribute)
					{
						constructor = constructorInfo;
						break;
					}

					var length = constructorInfo.GetParameters().Length;

					if (length > biggestLength)
					{
						biggestLength = length;
						bestMatch = constructorInfo;
					}
				}

				if (constructor == null)
				{
					constructor = bestMatch;
				}
			}
			else
			{
				ConstructorInfo bestMatch = null;
				int biggestLength = -1;

				constructors = type.GetTypeInfo().DeclaredConstructors.Where(x => !x.IsStatic && x.IsPrivate);

				foreach (ConstructorInfo constructorInfo in constructors)
				{
					var attributes = constructorInfo.GetCustomAttributes(typeof(InjectDependenciesAttribute), false);

					if (attributes.Any())
					{
						constructor = constructorInfo;
						break;
					}

					var length = constructorInfo.GetParameters().Length;

					if (length > biggestLength)
					{
						biggestLength = length;
						bestMatch = constructorInfo;
					}
				}

				if (constructor == null)
				{
					constructor = bestMatch;
				}
			}

			if (constructor == null)
			{
				throw new ResolutionException(
					"Could not locate a constructor for " + type.FullName);
			}

			invokeInfo = new ConstructorInvokeInfo(constructor);

			if (useLock)
			{
				constructorDictionaryLockSlim.EnterWriteLock();
			}

			try
			{
				if (cacheEnabled)
				{
					constructorDictionary[type] = invokeInfo;
				}
			}
			finally
			{
				if (useLock)
				{
					constructorDictionaryLockSlim.ExitWriteLock();
				}
			}

			return Instantiate(invokeInfo);
		}

		static string GetKeyValueOrDefault(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				key = defaultKey;
			}

			return key;
		}
	}
}
