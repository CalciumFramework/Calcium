using System;
using System.Collections.Generic;
using System.Reflection;

using Codon.InversionOfControl;
using Codon.Reflection;
using Codon.UI.Data;

namespace Codon.UI.Elements
{
	public class StaticExtension : MarkupExtensionBase
	{
		readonly string member;
		string alias;
		string path;

		static readonly Dictionary<string, object> cache
			= new Dictionary<string, object>();

		static readonly IMarkupTypeResolver markupTypeResolver 
			= Dependency.Resolve<IMarkupTypeResolver, MarkupTypeResolver>(true);

		public Type MemberType { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="member">A string that identifies the member to make a reference to. 
		/// This string uses the format prefix:typeName.fieldOrPropertyName. 
		/// prefix is the mapping prefix for a XAML namespace, and is only required 
		/// to reference static values that are not mapped to the default XAML namespace.</param>
		public StaticExtension(string member)
		{
			this.member = AssertArg.IsNotNull(member, nameof(member));
			SetAliasAndTypeName(member);
		}

		void SetAliasAndTypeName(string aliasAndPath)
		{
			var aliasAndPathArray = aliasAndPath.Split(':');
			if (aliasAndPathArray.Length != 2)
			{
				throw new BindingException("Namespace aliased type name is invalid. " + aliasAndPath);
			}

			alias = aliasAndPathArray[0];
			path = aliasAndPathArray[1];

			if (string.IsNullOrWhiteSpace(alias))
			{
				throw new BindingException("Namespace alias is invalid. member=" + member);
			}

			if (string.IsNullOrWhiteSpace(path))
			{
				throw new BindingException("Path is invalid. member=" + member);
			}
		}

		public override object ProvideValue(IContainer iocContainer/*, object[] parameters*/)
		{
			Type type = MemberType;
			string memberFullName = type != null ? type.FullName + "." + member : member;
			
			object cachedObject;

			if (cache.TryGetValue(memberFullName, out cachedObject) && cachedObject != null)
			{
				Type objectType = cachedObject.GetType();
				if (objectType.IsEnum())
				{
					return cachedObject;
				}

				var propertyInfo = cachedObject as PropertyInfo;
				if (propertyInfo != null)
				{
					return propertyInfo.GetValue(null, null);
				}

				var fieldInfo = cachedObject as FieldInfo;
				if (fieldInfo != null)
				{
					return fieldInfo.GetValue(null);
				}

				return cachedObject;
			}

			object result = null;
			string fieldString;
			
			if (type != null)
			{
				fieldString = member;
			}
			else
			{
				int dotIndex = member.LastIndexOf('.');
				if (dotIndex < 0)
				{
					throw new BindingException("Invalid Static member value: " + member);
				}

				/* Extract the type substring (this will include any XML prefix, e.g. "av:Button") */

				string aliasAndType = member.Substring(0, dotIndex);
				if (aliasAndType == string.Empty)
				{
					throw new BindingException("Invalid type string for x:Static member value: " + member);
				}

				type = markupTypeResolver.Resolve(aliasAndType);

				/* Extract the member name substring. */

				fieldString = member.Substring(dotIndex + 1, member.Length - dotIndex - 1);
				if (fieldString == string.Empty)
				{
					throw new BindingException("Invalid x:Static member name: " + member);
				}
			}

			if (type.IsEnum())
			{
				var enumValue = Enum.Parse(type, fieldString);
				cache[memberFullName] = enumValue;

				return enumValue;
			}

			/* For other types, use reflection. */

			bool found = false;

#if NETSTANDARD
			object fieldOrProp = type.GetRuntimeField(fieldString);
#else

			object fieldOrProp = type.GetField(fieldString, BindingFlags.Public |
							 BindingFlags.FlattenHierarchy | BindingFlags.Static);
#endif
			if (fieldOrProp == null)
			{
#if NETSTANDARD
				fieldOrProp = type.GetRuntimeProperty(fieldString);
#else
				fieldOrProp = type.GetProperty(fieldString, BindingFlags.Public |
							  BindingFlags.FlattenHierarchy | BindingFlags.Static);
#endif
				if (fieldOrProp != null)
				{
					result = ((PropertyInfo)fieldOrProp).GetValue(null, null);
					found = true;
				}
			}
			else
			{
				result = ((FieldInfo)fieldOrProp).GetValue(null);
				found = true;
			}

			if (found)
			{
				cache[memberFullName] = fieldOrProp;
				return result;
			}
			else
			{
				throw new BindingException("Invalid x:Static member name: " + member);
			}
		}

		public void ClearCache()
		{
			cache.Clear();
		}
	}
}