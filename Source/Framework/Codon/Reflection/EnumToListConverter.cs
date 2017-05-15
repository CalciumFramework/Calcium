#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
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
using System.Reflection;

namespace Codon.Reflection
{
	/// <summary>
	/// This class allows you to convert an enum
	/// to a list of its values. This is useful,
	/// for example, if you wish to bind an enum 
	/// to a drop down list.
	/// </summary>
	public static class EnumToListConverter
	{
		public static List<TEnum> CreateEnumValueList<TEnum>()
		{
			Type enumType = typeof(TEnum);

			List<TEnum> result = new List<TEnum>();

#if NETSTANDARD || NETFX_CORE
			foreach (FieldInfo fieldInfo in enumType.GetTypeInfo().DeclaredFields)
#else
			foreach (FieldInfo fieldInfo in enumType.GetFields())
#endif
			{
				if (enumType.Equals(fieldInfo.FieldType))
				{
					TEnum item = (TEnum)Enum.Parse(enumType, fieldInfo.Name, true);
					result.Add(item);
				}
			}

			return result;
			//			IEnumerable<FieldInfo> fieldInfos
			//				= enumType.GetFields().Where(x => enumType.Equals(x.FieldType));
			//			return fieldInfos.Select(
			//				fieldInfo => (TEnum)Enum.Parse(enumType, fieldInfo.Name, true)).ToList();
		}

		public static IEnumerable<TEnum> CreateEnumValueList<TEnum>(this Enum enumValue)
		{
			return CreateEnumValueList<TEnum>();
		}

		public static IList<object> CreateEnumValueList(Type enumType)
		{
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

			return result;
			//			IEnumerable<FieldInfo> fieldInfos
			//				= enumType.GetFields().Where(x => enumType.Equals(x.FieldType));
			//			return fieldInfos.Select(
			//				fieldInfo => (TEnum)Enum.Parse(enumType, fieldInfo.Name, true)).ToList();
		}
	}
}
