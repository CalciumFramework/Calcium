#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Reflection;

namespace Calcium.Reflection
{
	public static class TypeExtensions
	{
		public static bool IsEnum(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsEnum;
#else
			return type.IsEnum;
#endif
		}

		public static bool IsAssignableFromEx(this Type type, Type otherType)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
#else
			return type.IsAssignableFrom(otherType.GetTypeInfo());
#endif
		}

		public static bool IsPrimitive(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsPrimitive;
#else
			return type.IsPrimitive;
#endif
		}

		public static bool IsGenericType(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsGenericType;
#else
			return type.IsGenericType;
#endif
		}

		public static bool IsValueType(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsValueType;
#else
			return type.IsValueType;
#endif
		}

		public static bool IsClass(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsClass;
#else
			return type.IsClass;
#endif
		}

		public static bool IsInterface(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().IsInterface;
#else
			return type.IsInterface;
#endif
		}

		public static Assembly GetAssembly(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if NETSTANDARD || WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().Assembly;
#else
			return type.Assembly;
#endif
		}

		public static string GetFullName(this Type type)
		{
			AssertArg.IsNotNull(type, nameof(type));

#if WINDOWS_UWP || NETFX_CORE
			return type.GetTypeInfo().FullName;
#else
			return type.FullName;
#endif
		}

#if NETFX_CORE || WINDOWS_UWP
		/// <summary>
		/// Gets the type code for various built-in types. 
		/// This takes the place of the GetTypeCode method that exists in .NET, 
		/// but does not exist in the UWP.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeCode GetTypeCode(this Type type)
		{
			Dictionary<Type, TypeCode> typeCodeLookup = new Dictionary<Type, TypeCode>
				{
					{typeof(bool), TypeCode.Boolean},
					{typeof(byte), TypeCode.Byte},
					{typeof(char), TypeCode.Char},
					{typeof(DateTime), TypeCode.DateTime},
					{typeof(decimal), TypeCode.Decimal},
					{typeof(double), TypeCode.Double},
					{typeof(short), TypeCode.Int16},
					{typeof(int), TypeCode.Int32},
					{typeof(long), TypeCode.Int64},
					{typeof(object), TypeCode.Object},
					{typeof(sbyte), TypeCode.SByte},
					{typeof(float), TypeCode.Single},
					{typeof(ushort), TypeCode.UInt16},
					{typeof(uint), TypeCode.UInt32},
					{typeof(ulong), TypeCode.UInt64},
					{typeof(string), TypeCode.String}
				};

			TypeCode result;
			if (!typeCodeLookup.TryGetValue(type, out result))
			{
				result = TypeCode.Empty;
			}

			return result;
		}
#endif
	}
}
