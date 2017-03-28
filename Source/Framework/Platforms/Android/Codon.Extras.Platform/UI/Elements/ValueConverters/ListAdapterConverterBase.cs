using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Android.Content;
using Codon.MissingTypes.System.Windows.Data;
using Codon.UI.Data;

namespace Codon.UI.Elements
{
	public abstract class ListAdapterConverterBase<TAdapter> : IValueConverter
	{
		static readonly Dictionary<string, FieldInfo> adapterDictionary
			= new Dictionary<string, FieldInfo>();

		IDisposable adapter;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return null;
			}

			Type valueType = value.GetType();
			if (valueType.IsGenericType)
			{
				Type[] typeArguments = valueType.GetGenericArguments();
				if (typeArguments.Any())
				{
					/* If there are more than one type parameter, 
					 * the it is assumed that the first is the collection type.
					 * This allows support for AdaptiveCollection. */

					Type itemType = typeArguments[0];
					Type adapterType = typeof(TAdapter);
					Type[] typeArgs = { itemType };
					Type constructedType = adapterType.MakeGenericType(typeArgs);

					string layoutName = parameter?.ToString().Trim();
					int resourceId;
					bool hasLayout = false;

					if (!string.IsNullOrEmpty(layoutName))
					{
						if (layoutName.StartsWith("@"))
						{
							int slashIndex = layoutName.IndexOf("/", StringComparison.Ordinal);
							if (slashIndex < 0)
							{
								throw new Exception("AdapterBinding is invalid. ConverterParameter is incorrect. "
								                    + layoutName);
							}

							string directory = layoutName.Substring(1, slashIndex - 1);
							int itemNameIndex = slashIndex + 1;
							string itemName = layoutName.Substring(itemNameIndex, layoutName.Length - itemNameIndex);

							Context context = ApplicationContextHolder.Context;
							string packageName = context.PackageName;
							resourceId = context.Resources.GetIdentifier(itemName.ToLowerInvariant(), directory, packageName);

							hasLayout = resourceId != 0;
						}
						else
						{
							var dotIndex = layoutName.LastIndexOf(".", StringComparison.Ordinal);
							string propertyName = layoutName.Substring(dotIndex + 1, layoutName.Length - (dotIndex + 1));
							string typeName = layoutName.Substring(0, dotIndex);

							if (!adapterDictionary.TryGetValue(layoutName, out FieldInfo fieldInfo))
							{
								Type type = Type.GetType(typeName, false, true);
								if (type == null)
								{
									throw new Exception("Unable to locate layout type code for layout " + layoutName
									                    + " Type '" + typeName + "' could not be resolved.");
								}

								fieldInfo = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);

								if (fieldInfo != null)
								{
									adapterDictionary[layoutName] = fieldInfo;
								}
							}

							if (fieldInfo == null)
							{
								throw new Exception("Unable to locate layout type code for layout "
								                    + layoutName + " FieldInfo is null.");
							}

							resourceId = (int)fieldInfo.GetValue(null);

							hasLayout = true;
						}

						if (!hasLayout)
						{
							throw new Exception("ListAdapterBinding is invalid. Unable to locate resource using "
							                    + layoutName);
						}

						adapter?.Dispose();

						var result = Activator.CreateInstance(constructedType, value, resourceId);
						adapter = result as IDisposable;

						return result;
					}
				}
			}
			else
			{
				throw new Exception("Value is not a generic collection." + parameter);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}