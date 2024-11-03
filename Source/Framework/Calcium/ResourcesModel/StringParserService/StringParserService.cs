#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-03-25 20:37:27Z</CreationDate>
</File>
*/
#endregion

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Calcium.ComponentModel;

namespace Calcium.ResourcesModel
{
	/// <summary>
	/// Default implementation of the <see cref="Services.IStringParserService"/>.
	/// This class allows the replacement of 'tags', 
	/// which are sections within strings having the format: 
	/// ${TagName[:Argument]} 
	/// where [:Argument] is optional.
	/// You can register a tag with its replacement text,
	/// or you can register an <see cref="IConverter"/>
	/// that is used to replace the text.
	/// The strings representing the start '${' and end '}' can be changed
	/// via the <see cref="TagDelimiters"/> class.
	/// </summary>
	public class StringParserService : Services.IStringParserService
	{
		readonly Dictionary<string, IConverter> converters = new();
		readonly ReaderWriterLockSlim convertersLock = new();

		public TagDelimiters DefaultDelimiters { get; set; } = TagDelimiters.Default;

		public void RegisterConverter(string tagName, Func<object, object> convertFunc)
		{
			AssertArg.IsNotNull(tagName, nameof(tagName));
			AssertArg.IsNotNull(convertFunc, nameof(convertFunc));

			RegisterConverterCore(tagName, new DelegateConverter(convertFunc));
		}

		public void RegisterConverter(string tagName, IConverter converter)
		{
			AssertArg.IsNotNull(tagName, nameof(tagName));

			RegisterConverterCore(tagName, converter);
		}

		void RegisterConverterCore(string tagName, IConverter converter)
		{
			try
			{
				convertersLock.EnterWriteLock();
				converters[tagName] = converter;
			}
			finally
			{
				convertersLock.ExitWriteLock();
			}
		}
		
		public string Parse(string text, 
							IDictionary<string, string>? tagValues = null, 
							TagDelimiters? delimiters = null)
		{
			AssertArg.IsNotNull(text, nameof(text));

			delimiters ??= DefaultDelimiters;
			string? startDelimiter = delimiters.Start;
			string? endDelimiter = delimiters.End;

			StringBuilder sb = new(text.Length);
			int i = 0;

			while (i < text.Length)
			{
				// Find the next occurrence of the start delimiter using IndexOf
				int startIndex = text.IndexOf(startDelimiter, i, StringComparison.Ordinal);
				if (startIndex == -1)
				{
					// No more start delimiters; append the rest of the text and break
					sb.Append(text, i, text.Length - i);
					break;
				}

				// Append text before the start delimiter
				sb.Append(text, i, startIndex - i);
				int tagStart = startIndex     + startDelimiter.Length;

				int endIndex = text.IndexOf(endDelimiter, tagStart, StringComparison.Ordinal);
				if (endIndex == -1)
				{
					// If no end delimiter is found, append unmodified text
					sb.Append(text, startIndex, text.Length - startIndex);
					break;
				}

				string tag = text.Substring(tagStart, endIndex - tagStart);

				string replacement = Replace(tag, tagValues) 
									 ?? text.Substring(startIndex, (endIndex + endDelimiter.Length) - startIndex);

				sb.Append(replacement);

				// Move the index past the end delimiter
				i = endIndex + endDelimiter.Length;
			}

			return sb.ToString();
		}


		bool GetConverter(string tagName, out IConverter converter)
		{
			try
			{
				convertersLock.EnterReadLock();
				return converters.TryGetValue(tagName, out converter) && converter != null;
			}
			finally
			{
				convertersLock.ExitReadLock();
			}
		}

		string GetShortDateString(DateTime dateTime)
		{
#if NETFX_CORE
			return dateTime.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
#else
			return dateTime.ToString("d");
#endif
		}

		string? Replace(string tagContent, IDictionary<string, string>? customTags)
		{
			string? result = null;

			switch (tagContent)
			{
				case "date":
					result = GetShortDateString(DateTime.Today);
					break;
				case "time":
					result = GetShortDateString(DateTime.Now);
					break;
				case "timeUtc":
					result = GetShortDateString(DateTime.UtcNow);
					break;
				case TestTag:
					result = TestTagResult;
					break;
				default:
					string tagName;
					string? tagArgs = null;
					int indexOfArg = tagContent.IndexOf(':');
					if (indexOfArg > 0)
					{
						tagName = tagContent.Substring(0, indexOfArg);
						int tagLength = tagContent.Length - (indexOfArg + 1);
						if (tagLength > 0)
						{
							tagArgs = tagContent.Substring(indexOfArg + 1, tagLength);
						}
					}
					else
					{
						tagName = tagContent;
					}

					customTags?.TryGetValue(tagName, out result);

					if (result == null)
					{
						if (GetConverter(tagName, out IConverter converter))
						{
							object conversionResult = converter.Convert(tagArgs);
							result = conversionResult?.ToString();
						}
					}
					break;
			}

			return result;
		}

		internal const string TestTag = "test_BD764E2D-920D-4252-9DCD-7862060D4049";
		internal const string TestTagResult = "True";
	}
}
