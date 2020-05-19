#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-12-10 17:29:06Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Calcium.Reflection
{
	/// <summary>
	/// This class allows you to convert an enum
	/// to a list of its values. This is useful,
	/// for example, if you wish to bind an enum 
	/// to a drop down list.
	/// </summary>
	public static class EnumToListConverter
	{
		static readonly Dictionary<Type, object> cache = new Dictionary<Type, object>();

		public static void ClearCache()
		{
			cache.Clear();
		}

		public static IList<TEnum> CreateEnumValueList<TEnum>(
			params TEnum[] excludeItems)
			where TEnum : Enum
		{
			var list = CreateEnumValueList<TEnum>();
			foreach (TEnum item in excludeItems)
			{
				list.Remove(item);
			}

			return list;
		}

//		public static IList<TEnum> CreateEnumValueList<TEnum>(
//			this TEnum enumValue, params TEnum[] excludeItems)
//			where TEnum : Enum
//		{
//			var list = CreateEnumValueList<TEnum>();
//			foreach (TEnum item in excludeItems)
//			{
//				list.Remove(item);
//			}
//
//			return list;
//		}

		public static IList<object> CreateEnumValueList(
			Type enumType, params object[] excludeItems)
		{
			var list = CreateEnumValueList(enumType);
			foreach (var item in excludeItems)
			{
				list.Remove(item);
			}

			return list;
		}

		public static IList<TEnum> CreateEnumValueList<TEnum>()
			where TEnum : Enum
		{
			return CreateEnumValueList(typeof(TEnum)).Cast<TEnum>().ToList();
		}

//		public static IList<TEnum> CreateEnumValueList<TEnum>(this TEnum enumValue)
//			where TEnum : Enum
//		{
//			return CreateEnumValueList<TEnum>();
//		}

		public static IList<object> CreateEnumValueList(Type enumType)
		{
			if (cache.TryGetValue(enumType, out object cachedList))
			{
				return new List<object>((List<object>)cachedList);
			}

			List<object> result = new List<object>();

#if NETSTANDARD || NETFX_CORE
			foreach (FieldInfo fieldInfo in enumType.GetTypeInfo().DeclaredFields)
#else
			foreach (FieldInfo fieldInfo in enumType.GetFields())
#endif
			{
				if (enumType.Equals(fieldInfo.FieldType))
				{
					var item = Enum.Parse(enumType, fieldInfo.Name, true);
					result.Add(item);
				}
			}

			cache[enumType] = result;

			return result;
		}
	}
}
