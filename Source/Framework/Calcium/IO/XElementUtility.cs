using System.Collections.Generic;
using System.Xml.Linq;

namespace Calcium.IO
{
	public static class XElementUtility
	{
		/// <summary>
		/// Retrieve a <c>IXmlConvertible</c> for each element
		/// with the specified <c>childElementName</c>.
		/// </summary>
		/// <param name="element">An element that contains zero
		/// or more child elements with the name <c>childElementName</c>.</param>
		/// <param name="childElementName">The name of the elements
		/// that are used to create new instances of the specified type.</param>
		/// <returns>A list of <c>IXmlConvertible</c> objects representing
		/// the <c>childElementName</c> elements of the specified <c>XElement</c>.</returns>
		public static IEnumerable<T> GetChildren<T>(
			XElement element, string childElementName)
			where T : IXmlConvertible, new()
		{
			AssertArg.IsNotNull(element, nameof(element));
			AssertArg.IsNotNull(childElementName, nameof(childElementName));

			var childElements = element.Elements(childElementName);
			var result = new List<T>();
			foreach (XElement childElement in childElements)
			{
				var setting = new T();
				setting.FromXElement(childElement);
				result.Add(setting);
			}

			return result;
		}

		/// <summary>
		/// Retrieve a <c>IXmlConvertible</c> for each element
		/// with the specified <c>childElementName</c>.
		/// </summary>
		/// <param name="element">An element that contains zero
		/// or more child elements with the name <c>childElementName</c>.</param>
		/// <param name="childElementName">The name of the elements
		/// that are used to create new instances of the specified type.</param>
		/// <returns>A list of <c>IXmlConvertible</c> objects representing
		/// the <c>childElementName</c> elements of the specified <c>XElement</c>.</returns>
		public static IEnumerable<T> GetConvertibleChildren<T>(
			this XElement element, string childElementName) where T : IXmlConvertible, new()
		{
			return GetChildren<T>(element, childElementName);
		}
	}
}
