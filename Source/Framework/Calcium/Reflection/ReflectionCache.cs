#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calcium.Reflection
{
	/// <summary>
	/// Default implementation of the 
	/// <see cref="IReflectionCache"/> interface.
	/// See the interface for API documentation.
	/// </summary>
	[Preserve(AllMembers = true)]
	sealed class ReflectionCache : IReflectionCache
	{
		readonly Dictionary<PropertyInfo, object> getterDictionary
			= new Dictionary<PropertyInfo, object>();

		readonly Dictionary<PropertyInfo, Func<object, object>> getterGenericDictionary
			= new Dictionary<PropertyInfo, Func<object, object>>();

		readonly Dictionary<PropertyInfo, Action<object, object>> setterDictionary
			= new Dictionary<PropertyInfo, Action<object, object>>();

		readonly Dictionary<PropertyInfo, object> setterGenericDictionary
			= new Dictionary<PropertyInfo, object>();

		readonly Dictionary<MethodInfo, Action<object, object[]>> voidMethodDictionary
			= new Dictionary<MethodInfo, Action<object, object[]>>();

		readonly Dictionary<MethodInfo, Func<object, object[], object>> nonVoidMethodDictionary
			= new Dictionary<MethodInfo, Func<object, object[], object>>();

		readonly Dictionary<MethodInfo, object> nonVoidMethodGenericDictionary
			= new Dictionary<MethodInfo, object>();

		public void Clear()
		{
			var dictionaries = new List<IDictionary>
			{
				getterDictionary,
				getterGenericDictionary,
				setterDictionary,
				setterGenericDictionary,
				voidMethodDictionary,
				nonVoidMethodDictionary,
				nonVoidMethodGenericDictionary,
				propertiesWithAttributeDictionary,
				constructorDictionary
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
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode)
		{
			 var dictionary = getterGenericDictionary;
			
			 if (!dictionary.TryGetValue(propertyInfo, out var getter))
			 {
			 	switch (creationMode)
			 	{
			 		case DelegateCreationMode.SlowCreationFastPerformance:
			 			getter = ReflectionCompiler.CreatePropertyGetter(propertyInfo);
			 			break;
			 		default:
			 			getter = propertyInfo.GetValue;
			 			break;
			 	}
			 	
			 	dictionary[propertyInfo] = getter;
			 }
			
			 return getter;
		}

		public Func<TOwner, TProperty> GetPropertyGetter<TOwner, TProperty>(
			Expression<Func<TOwner, TProperty>> propertyExpression)
		{
			/* TODO: extract property name and cache item in dictionary. */
			Func<TOwner, TProperty> result = ReflectionCompiler.CreatePropertyGetter(propertyExpression);
			return result;
		}

		public Func<object, TProperty> GetPropertyGetter<TProperty>(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode)
		{
			Func<object, TProperty> result;

			var dictionary = getterDictionary;

			if (!dictionary.TryGetValue(propertyInfo, out object getter))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreatePropertyGetter<TProperty>(propertyInfo);
						break;
					default:
						var getMethod = propertyInfo.GetMethod;
						result = owner => (TProperty)getMethod.Invoke(owner, null);
						break;
				}
				
				dictionary[propertyInfo] = result;
				return result;
			}
			
			result = (Func<object, TProperty>)getter;
			return result;
		}

		public Action<object, object[]> GetVoidMethodInvoker(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode)
		{
			var dictionary = voidMethodDictionary;

			if (!dictionary.TryGetValue(methodInfo, out var result))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreateMethodAction(methodInfo);
						break;
					default:
						result = (owner, args) =>  methodInfo.Invoke(owner, args);
						break;
				}
				
				dictionary[methodInfo] = result;
			}
			
			return result;
		}

		public Func<object, object[], object> GetMethodInvoker(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode)
		{
			var dictionary = nonVoidMethodDictionary;
			
			if (!dictionary.TryGetValue(methodInfo, out var result))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreateMethodFunc(methodInfo);
						break;
					default:
						result = methodInfo.Invoke;
						break;
				}
				
				dictionary[methodInfo] = result;
			}

			return result;
		}

		public Func<object, object[], TReturn> GetMethodInvoker<TReturn>(
			MethodInfo methodInfo,
			DelegateCreationMode creationMode)
		{
			var dictionary = nonVoidMethodGenericDictionary;
			
			if (!dictionary.TryGetValue(methodInfo, out object result))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreateMethodFunc<TReturn>(methodInfo);
						break;
					default:
						result = new Func<object, object[], TReturn>(
							(owner, args) => (TReturn)methodInfo.Invoke(owner, args));
						break;
				}

				dictionary[methodInfo] = result;
			}

			return (Func<object, object[], TReturn>)result;
		}

		public Action<object, object> GetPropertySetter(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode)
		{
			var dictionary = setterDictionary;

			if (!dictionary.TryGetValue(propertyInfo, out var setter))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						setter = ReflectionCompiler.CreatePropertySetter(propertyInfo);
						break;
					default:
						var setMethod = propertyInfo.SetMethod;
						setter = (owner, newValue) => setMethod.Invoke(owner, new []{newValue});
						break;
				}
				
				dictionary[propertyInfo] = setter;
			}
			
			return setter;
		}

		public Action<object, TProperty> GetPropertySetter<TProperty>(
			PropertyInfo propertyInfo,
			DelegateCreationMode creationMode)
		{
			var dictionary = setterGenericDictionary;
			
			if (!dictionary.TryGetValue(propertyInfo, out object result))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreatePropertySetter<TProperty>(propertyInfo);
						break;
					default:
						var setMethod = propertyInfo.SetMethod;
						result = new Action<object, TProperty>(
							(owner, newValue) => setMethod.Invoke(owner, new object[] { newValue }));
						break;
				}

				dictionary[propertyInfo] = result;
			}

			return (Action<object, TProperty>)result;
		}

		readonly Dictionary<ConstructorInfo, Func<object[], object>> constructorDictionary
			= new Dictionary<ConstructorInfo, Func<object[], object>>();

		public Func<object[], object> GetConstructorFunc(
			ConstructorInfo info, DelegateCreationMode creationMode)
		{
			var dictionary = constructorDictionary;

			if (!dictionary.TryGetValue(info, out Func<object[], object> result))
			{
				switch (creationMode)
				{
					case DelegateCreationMode.SlowCreationFastPerformance:
						result = ReflectionCompiler.CreateConstructorFunc(info);
						break;
					default:
						result = info.Invoke;
						break;
				}

				dictionary[info] = result;
			}

			return result;
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

		readonly Dictionary<Tuple<Type, Type>, List<PropertyWithAttribute>> propertiesWithAttributeDictionary
			= new Dictionary<Tuple<Type, Type>, List<PropertyWithAttribute>>();

		public IEnumerable<PropertyWithAttribute> GetPropertyAttributesForClass(
			Type classType, Type attributeType, bool includeAncestorClassProperties = true)
		{
			var key = new Tuple<Type, Type>(classType, attributeType);
			if (propertiesWithAttributeDictionary.TryGetValue(key, out var result))
			{
				return result;
			}

			result = new List<PropertyWithAttribute>();

			IEnumerable<PropertyInfo> properties;
			if (includeAncestorClassProperties)
			{
				properties = classType.GetRuntimeProperties();
			}
			else
			{
				properties = classType.GetTypeInfo().DeclaredProperties;
			}

			foreach (PropertyInfo info in properties)
			{
				var attribute = info.GetCustomAttributes(attributeType).FirstOrDefault();
				if (attribute != null)
				{
					var tuple = new PropertyWithAttribute(attribute, info);
					result.Add(tuple);
				}
			}

			propertiesWithAttributeDictionary[key] = result;

			return result;
		}
	}
}
