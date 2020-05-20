#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 13:39:57Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Net;

namespace Calcium.Networking
{
	/// <summary>
	/// This class provides URL encoding and decoding capabilities,
	/// and is able to process a query string 
	/// into an <c>IDictionary{string, string}</c>.
	/// </summary>
	public static class WebUtilityExtended
	{
		/// <summary>
		/// Parses the specified query string, and populates
		/// a dictionary with its key value pairs.
		/// </summary>
		/// <param name="queryString">The URL query string
		/// containing key value pairs.</param>
		/// <returns>A dictionary containing the key value 
		/// pairs from the specified query string.</returns>
		public static IDictionary<string, string> ParseQueryString(
			string queryString)
		{
			var parameterDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			int startIndex = queryString.IndexOf('?');
			string queryPart = startIndex > -1 
				? queryString.Substring(startIndex + 1) 
				: queryString;

			string[] strings = queryPart.Split('&');

			foreach (string queryItem in strings)
			{
				int equalsIndex = queryItem.IndexOf('=');
				if (equalsIndex >= 0)
				{
					parameterDictionary[queryItem.Substring(0, equalsIndex)] = queryItem.Substring(equalsIndex + 1);
				}
				else
				{
					parameterDictionary[queryItem] = null;
				}
			}

			return parameterDictionary;
		}

		/* Some .NET frameworks use HttpUtility, while others use WebUtility. 
		 * These methods provide a unified API. These will, perhaps be deprecated
		 * when WebUtility is available across the board. */

		public static string HtmlEncode(string text) 
			=> WebUtility.HtmlEncode(text);

		public static string HtmlDecode(string text)
			=> WebUtility.HtmlDecode(text);

		public static string UrlEncode(string text)
			=> WebUtility.UrlEncode(text);

		public static string UrlDecode(string text)
			=> WebUtility.UrlDecode(text);
	}
}
