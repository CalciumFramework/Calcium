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
		//readonly ReaderWriterLockSlim getterLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, object> getterDictionary
			= new Dictionary<PropertyInfo, object>();

		//readonly ReaderWriterLockSlim getterGenericLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, Func<object, object>> getterGenericDictionary
			= new Dictionary<PropertyInfo, Func<object, object>>();

		//readonly ReaderWriterLockSlim setterLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, Action<object, object>> setterDictionary
			= new Dictionary<PropertyInfo, Action<object, object>>();

		//readonly ReaderWriterLockSlim setterGenericLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<PropertyInfo, object> setterGenericDictionary
			= new Dictionary<PropertyInfo, object>();

		//readonly ReaderWriterLockSlim voidMethodLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, Action<object, object[]>> voidMethodDictionary
			= new Dictionary<MethodInfo, Action<object, object[]>>();

		//readonly ReaderWriterLockSlim nonVoidMethodLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, Func<object, object[], object>> nonVoidMethodDictionary
			= new Dictionary<MethodInfo, Func<object, object[], object>>();

		//readonly ReaderWriterLockSlim nonVoidMethodGenericLockSlim
		//	= new ReaderWriterLockSlim();
		readonly Dictionary<MethodInfo, object> nonVoidMethodGenericDictionary
			= new Dictionary<MethodInfo, object>();

		public void Clear()
		{
			var dictionaries = new List<IDictionary>
			{
				{getterDictionary},
				{getterGenericDictionary},
				{setterDictionary},
				{setterGenericDictionary},
				{voidMethodDictionary},
				{nonVoidMethodDictionary},
				{nonVoidMethodGenericDictionary},
				assignableFromDictionary
			};

			foreach (var dictionary in dictionaries)
			{
				dictionary.Clear();
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
			var dictionary = getterGenericDictionary;

			if (!dictionary.TryGetValue(propertyInfo, out var getter))
			{
				getter = ReflectionCompiler.CreatePropertyGetter(propertyInfo);
				dictionary[propertyInfo] = getter;
			}

			return getter;
		}

		public Func<object, TProperty> GetPropertyGetter<TProperty>(
			PropertyInfo propertyInfo)
		{
			Func<object, TProperty> result;
			object getter;

			var dictionary = getterDictionary;
			
			if (!dictionary.TryGetValue(propertyInfo, out getter))
			{
				result = ReflectionCompiler.CreatePropertyGetter<TProperty>(propertyInfo);
				dictionary[propertyInfo] = result;
				return result;
			}
			
			result = (Func<object, TProperty>)getter;
			return result;
		}

		public Action<object, object[]> GetVoidMethodInvoker(
			MethodInfo methodInfo)
		{
			var dictionary = voidMethodDictionary;

			if (!dictionary.TryGetValue(methodInfo, out var action))
			{
				action = ReflectionCompiler.CreateMethodAction(methodInfo);
				dictionary[methodInfo] = action;
			}
			
			return action;
		}

		public Func<object, object[], object> GetMethodInvoker(
			MethodInfo methodInfo)
		{
			var dictionary = nonVoidMethodDictionary;
			
			if (!dictionary.TryGetValue(methodInfo, out var func))
			{
				func = ReflectionCompiler.CreateMethodFunc(methodInfo);
				dictionary[methodInfo] = func;
			}

			return func;
		}

		public Func<object, object[], TReturn> GetMethodInvoker<TReturn>(
			MethodInfo methodInfo)
		{
			var dictionary = nonVoidMethodGenericDictionary;
			
			if (!dictionary.TryGetValue(methodInfo, out object func))
			{
				var result = ReflectionCompiler.CreateMethodFunc<TReturn>(methodInfo);
				dictionary[methodInfo] = result;
				return result;
			}

			return (Func<object, object[], TReturn>)func;
		}

		public Action<object, object> GetPropertySetter(
			PropertyInfo propertyInfo)
		{
			var dictionary = setterDictionary;

			if (!dictionary.TryGetValue(propertyInfo, out var setter))
			{
				setter = ReflectionCompiler.CreatePropertySetter(propertyInfo);
				dictionary[propertyInfo] = setter;
			}
			
			return setter;
		}

		public Action<object, TProperty> GetPropertySetter<TProperty>(
			PropertyInfo propertyInfo)
		{
			var dictionary = setterGenericDictionary;
			
			if (!dictionary.TryGetValue(propertyInfo, out object setter))
			{
				var result = ReflectionCompiler.CreatePropertySetter<TProperty>(propertyInfo);
				dictionary[propertyInfo] = result;
				return result;
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
				
		internal static PropertyInfo GetProperty(Type type, string propertyName)
		{
			AssertArg.IsNotNull(type, nameof(type));
			AssertArg.IsNotNull(propertyName, nameof(propertyName));
		
			string key = GetFieldKey(type, propertyName);

			if (!propertyCache.TryGetValue(key, out PropertyInfo result))
			{
				result = type.GetTypeInfo().GetDeclaredProperty(propertyName);
				propertyCache[key] = result;
			}
		
			return result;
		}

		static string GetFieldKey(Type type, string memberName)
		{
			return type.FullName + memberName;
		}

		readonly Dictionary<Tuple<Type, Type>, bool> assignableFromDictionary
			= new Dictionary<Tuple<Type, Type>, bool>();

		public bool IsAssignableFrom(Type interfaceType, Type type2)
		{
			var tuple = new Tuple<Type, Type>(interfaceType, type2);
			if (assignableFromDictionary.TryGetValue(tuple, out bool assignable))
			{
				return assignable;
			}

			assignable = interfaceType.IsAssignableFromEx(type2);
			assignableFromDictionary[tuple] = assignable;

			return assignable;
		}
	}
}
