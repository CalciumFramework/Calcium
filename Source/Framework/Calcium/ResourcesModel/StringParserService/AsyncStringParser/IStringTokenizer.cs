#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:39:01Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

using Calcium.InversionOfControl;

namespace Calcium.ResourcesModel.Experimental
{
	[DefaultType(typeof(StringTokenizer), Singleton = true)]
	public interface IStringTokenizer
	{
		IList<TagSegment> Tokenize(string text, TagDelimiters delimiters);
	}

	public class StringTokenizer : IStringTokenizer
	{
		public IList<TagSegment> Tokenize(string text, TagDelimiters delimiters)
		{
			List<TagSegment> segments = new();
			int currentIndex = 0;

			while (currentIndex < text.Length)
			{
				int startIndex = text.IndexOf(delimiters.Start, currentIndex, StringComparison.Ordinal);
				if (startIndex == -1)
				{
					// No more tags found, exit loop
					break;
				}

				int tagStart = startIndex + delimiters.Start.Length;
				int endIndex = text.IndexOf(delimiters.End, tagStart, StringComparison.Ordinal);
				if (endIndex == -1)
				{
					// If no closing delimiter is found, stop searching
					break;
				}

				// Extract tag content without allocating new strings unnecessarily
				int tagContentLength = endIndex - tagStart;
				if (tagContentLength == 0 || string.IsNullOrWhiteSpace(text.Substring(tagStart, tagContentLength)))
				{
					// Skip empty or whitespace-only tags
					currentIndex = endIndex + delimiters.End.Length;
					continue;
				}

				// Trim the tag content manually to avoid unnecessary string allocations
				int contentStart = tagStart;
				int contentEnd = endIndex - 1;

				while (contentStart < endIndex && char.IsWhiteSpace(text[contentStart]))
				{
					contentStart++;
				}

				while (contentEnd > contentStart && char.IsWhiteSpace(text[contentEnd]))
				{
					contentEnd--;
				}

				string tagContent = text.Substring(contentStart, contentEnd - contentStart + 1);
				int colonIndex = tagContent.IndexOf(':');
				string tagName = colonIndex == -1 ? tagContent : tagContent.Substring(0, colonIndex).Trim();
				string tagArg = colonIndex == -1 ? string.Empty : tagContent.Substring(colonIndex + 1).Trim();

				TagSegment segment = new()
				{
					Index = (uint)startIndex,
					Length = (uint)(endIndex + delimiters.End.Length - startIndex),
					TagName = tagName,
					TagArg = tagArg,
					Tag = tagContent
				};

				segments.Add(segment);
				currentIndex = endIndex + delimiters.End.Length;
			}

			return segments;
		}
	}
}