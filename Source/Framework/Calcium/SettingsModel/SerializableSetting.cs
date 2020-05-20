#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2015-04-25 18:20:18Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

using Calcium.IO;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class allows you to convert a setting value 
	/// to an XML representation, which is useful for exporting
	/// or importing settings. 
	/// When converting to XML, the setting value is converted 
	/// to a <c>byte[]</c> and then to a Base 64 string representation.
	/// The Base 64 string is placed into an 
	/// XML element.
	/// <seealso cref="StorageLocation"/>
	/// </summary>
	public class SerializableSetting : IXmlConvertible
	{
		const int formatVersion = 1;
		const string formatVersionElementName = "ClassVersion";
		const string elementValueName = "Value";
		const string elementStorageLocationName = "StorageLocation";
		const string elementOptionName = "Setting";
		const string elementNameName = "Name";

		public SerializableSetting()
		{
			/* Intentionally left blank. */
		}

		public SerializableSetting(
			string name, 
			object settingValue, 
			StorageLocation? storageLocation)
		{
			Name = AssertArg.IsNotNull(name, nameof(name));
			SettingValue = settingValue;
			StorageLocation = storageLocation;
		}

		public SerializableSetting(XElement element)
		{
			AssertArg.IsNotNull(element, nameof(element));

			FromXElementCore(element);
		}

		void IXmlConvertible.FromXElement(XElement element)
		{
			AssertArg.IsNotNull(element, nameof(element));

			FromXElementCore(element);
		}

		void FromXElementCore(XElement element)
		{
			var rootElement = element.Element(elementOptionName) ?? element;

			var version = (int)rootElement.Element(formatVersionElementName);

			Name = (string)rootElement.Element(elementNameName);

			var storageLocationElement = rootElement.Element(elementStorageLocationName);

			if (storageLocationElement != null)
			{
				string storageLocationString = storageLocationElement.Value;
				if (!string.IsNullOrWhiteSpace(storageLocationString))
				{
					StorageLocation parsedValue;
					try
					{
						parsedValue = (StorageLocation)Enum.Parse(typeof(StorageLocation), storageLocationString);
					}
					catch (Exception ex)
					{
						throw new FormatException("StorageLocation element has invalid value of " + storageLocationString, ex);
					}

					StorageLocation = parsedValue;
				}
			}

			var settingValueElement = rootElement.Element(elementValueName);

			if (settingValueElement != null)
			{
				try
				{
					var base64String = settingValueElement.Value;
					byte[] bytes = Convert.FromBase64String(base64String);
					var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
					object originalObject = serializer.Deserialize<object>(bytes);
					SettingValue = originalObject;
				}
				catch (Exception ex)
				{
					throw new FormatException(elementValueName + " was unable to be parsed from base64 string.", ex);
				}
			}

			if (version > 1)
			{

			}
		}

		public XElement ToXElement()
		{
			XElement element = new XElement(elementOptionName,
				new XElement(formatVersionElementName, formatVersion),
				new XElement(elementNameName, Name)
				);

			if (StorageLocation.HasValue)
			{
				element.Add(new XElement(elementStorageLocationName, StorageLocation));
			}

			var objectToSerialize = SettingValue;
			if (objectToSerialize != null)
			{
				var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
				byte[] bytes = serializer.Serialize(objectToSerialize);
				string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
				XElement valueElement = new XElement(elementValueName, base64String);
				element.Add(valueElement);
			}

			return element;
		}

		/// <summary>
		/// Retrieve a <c>SerializableSetting</c> for each 'Setting' element
		/// of the specified <c>XElement</c>.
		/// </summary>
		/// <param name="element">An element that contains zero
		/// or more 'Setting' child elements.</param>
		/// <returns>A list of <c>SerializableSetting</c> objects representing
		/// the 'Setting' elements of the specified <c>XElement</c>.</returns>
		public static IEnumerable<SerializableSetting> GetChildrenFromElement(XElement element)
		{
			return element.GetConvertibleChildren<SerializableSetting>(elementOptionName);
		}

		/// <summary>
		/// Gets or sets the unique name of the setting.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets where the setting resides.
		/// </summary>
		public StorageLocation? StorageLocation { get; set; }

		/// <summary>
		/// Gets or sets the setting value.
		/// </summary>
		public object SettingValue { get; set; }
	}
}
