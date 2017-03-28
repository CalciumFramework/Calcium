#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-10 18:27:36Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Codon.Reflection
{
	/// <summary>
	/// Default implementation of the 
	/// <see cref="IReflectionCache"/> interface.
	/// See the interface for API documentation.
	/// </summary>
	sealed class ReflectionCache : IReflectionCache
	{
		readonly ReaderWriterLockSlim getterLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, object> getterDictionary
			= new Dictionary<PropertyInfo, object>();

		readonly ReaderWriterLockSlim getterGenericLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, Func<object, object>> getterGenericDictionary
			= new Dictionary<PropertyInfo, Func<object, object>>();

		readonly ReaderWriterLockSlim setterLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, Action<object, object>> setterDictionary
			= new Dictionary<PropertyInfo, Action<object, object>>();

		readonly ReaderWriterLockSlim setterGenericLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, object> setterGenericDictionary
			= new Dictionary<PropertyInfo, object>();

		readonly ReaderWriterLockSlim voidMethodLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, Action<object, object[]>> voidMethodDictionary
			= new Dictionary<MethodInfo, Action<object, object[]>>();

		readonly ReaderWriterLockSlim nonVoidMethodLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, Func<object, object[], object>> nonVoidMethodDictionary
			= new Dictionary<MethodInfo, Func<object, object[], object>>();

		readonly ReaderWriterLockSlim nonVoidMethodGenericLockSlim
			= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, object> nonVoidMethodGenericDictionary
			= new Dictionary<MethodInfo, object>();

		public void Clear()
		{
			var dictionaries = new Dictionary<ReaderWriterLockSlim, IDictionary>
				{
					{getterLockSlim, getterDictionary},
					{getterGenericLockSlim, getterGenericDictionary},
					{setterLockSlim, setterDictionary},
					{setterGenericLockSlim, setterGenericDictionary},
					{voidMethodLockSlim, voidMethodDictionary},
					{nonVoidMethodLockSlim, nonVoidMethodDictionary},
					{nonVoidMethodGenericLockSlim, nonVoidMethodGenericDictionary},
				};

			foreach (var pair in dictionaries)
			{
				var lockSlim = pair.Key;
				var dictionary = pair.Value;

				lockSlim.EnterWriteLock();
				try
				{
					dictionary.Clear();
				}
				finally
				{
					lockSlim.ExitWriteLock();
				}
			}

			dictionaries.Clear();
		}

		/* The Get* methods in this class are deliberately more verbose 
		 * than they need to be. We could remove the duplication within 
		 * each code block, however the code needs to be fast, 
		 * and the overhead of creating an Action for each call
		 * would not be worth it. */

		public Func<object, object> GetPropertyGetter(
			PropertyInfo propertyInfo)
		{
			Func<object, object> getter;

			var lockSlim = getterLockSlim;
			var dictionary = getterGenericDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(propertyInfo, out getter))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(propertyInfo, out getter))
						{
							getter = ReflectionCompiler.CreatePropertyGetter(propertyInfo);
							dictionary[propertyInfo] = getter;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			return getter;
		}

		public Func<object, TProperty> GetPropertyGetter<TProperty>(
			PropertyInfo propertyInfo)
		{
			Func<object, TProperty> result;
			object getter;

			var lockSlim = getterGenericLockSlim;
			var dictionary = getterDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(propertyInfo, out getter))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(propertyInfo, out getter))
						{
							result = ReflectionCompiler.CreatePropertyGetter<TProperty>(propertyInfo);
							dictionary[propertyInfo] = result;
							return result;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			result = (Func<object, TProperty>)getter;
			return result;
		}

		public Action<object, object[]> GetVoidMethodInvoker(
			MethodInfo methodInfo)
		{
			Action<object, object[]> action;

			var lockSlim = voidMethodLockSlim;
			var dictionary = voidMethodDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(methodInfo, out action))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(methodInfo, out action))
						{
							action = ReflectionCompiler.CreateMethodAction(methodInfo);
							dictionary[methodInfo] = action;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			return action;
		}

		public Func<object, object[], object> GetMethodInvoker(
			MethodInfo methodInfo)
		{
			Func<object, object[], object> func;

			var lockSlim = nonVoidMethodLockSlim;
			var dictionary = nonVoidMethodDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(methodInfo, out func))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(methodInfo, out func))
						{
							func = ReflectionCompiler.CreateMethodFunc(methodInfo);
							dictionary[methodInfo] = func;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			return func;
		}

		public Func<object, object[], TReturn> GetMethodInvoker<TReturn>(
			MethodInfo methodInfo)
		{
			object func;

			var lockSlim = nonVoidMethodGenericLockSlim;
			var dictionary = nonVoidMethodGenericDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(methodInfo, out func))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(methodInfo, out func))
						{
							var result = ReflectionCompiler.CreateMethodFunc<TReturn>(methodInfo);
							dictionary[methodInfo] = result;
							return result;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			return (Func<object, object[], TReturn>)func;
		}

		public Action<object, object> GetPropertySetter(
			PropertyInfo propertyInfo)
		{
			Action<object, object> setter;

			var lockSlim = setterLockSlim;
			var dictionary = setterDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(propertyInfo, out setter))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(propertyInfo, out setter))
						{
							setter = ReflectionCompiler.CreatePropertySetter(propertyInfo);
							dictionary[propertyInfo] = setter;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}
			
			return setter;
		}

		public Action<object, TProperty> GetPropertySetter<TProperty>(
			PropertyInfo propertyInfo)
		{
			object setter;

			var lockSlim = setterGenericLockSlim;
			var dictionary = setterGenericDictionary;

			lockSlim.EnterUpgradeableReadLock();
			try
			{
				if (!dictionary.TryGetValue(propertyInfo, out setter))
				{
					lockSlim.EnterWriteLock();
					try
					{
						if (!dictionary.TryGetValue(propertyInfo, out setter))
						{
							var result = ReflectionCompiler.CreatePropertySetter<TProperty>(propertyInfo);
							dictionary[propertyInfo] = result;
							return result;
						}
					}
					finally
					{
						lockSlim.ExitWriteLock();
					}
				}
			}
			finally
			{
				lockSlim.ExitUpgradeableReadLock();
			}

			return (Action<object, TProperty>)setter;
		}

		//public Action<object> GetEventAction(EventInfo eventInfo, Action action)
		//{
		//	var handler = ReflectionCompiler.CreateEventHandler(eventInfo, action);
		//	Action<object> result = target => { eventInfo.AddEventHandler(target, handler);}
		//	return result;
		//}

		static readonly Dictionary<string, PropertyInfo> propertyCache
							= new Dictionary<string, PropertyInfo>();
		
		static readonly ReaderWriterLockSlim propertyCacheLockSlim 
			= new ReaderWriterLockSlim();
		
		internal static PropertyInfo GetProperty(Type type, string propertyName)
		{
			AssertArg.IsNotNull(type, nameof(type));
			AssertArg.IsNotNull(propertyName, nameof(propertyName));
		
			string key = GetFieldKey(type, propertyName);
		
			propertyCacheLockSlim.EnterUpgradeableReadLock();
			try
			{
				PropertyInfo result;
		
				if (propertyCache.TryGetValue(key, out result))
				{
					return result;
				}
		
				try
				{
					propertyCacheLockSlim.EnterWriteLock();
		
					result = type.GetTypeInfo().GetDeclaredProperty(propertyName);
		
					propertyCache[key] = result;
		
					return result;
				}
				finally
				{
					propertyCacheLockSlim.ExitWriteLock();
				}
						
			}
			finally
			{
				propertyCacheLockSlim.ExitUpgradeableReadLock();
			}
		}

		static string GetFieldKey(Type type, string memberName)
		{
			return type.FullName + memberName;
		}
	}
}
