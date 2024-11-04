#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:40:45Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Calcium.ComponentModel;
using Calcium.InversionOfControl;
using Calcium.Services;

namespace Calcium.ResourcesModel.Experimental
{
	[DefaultType(typeof(AsyncStringParser), Singleton = true)]
	public interface IAsyncStringParser
	{
		/// <summary>
		/// Asynchronously parses a string and replaces tags within it
		/// based on the provided processors and values.
		/// </summary>
		/// <param name="text">
		/// The input text containing tags to be parsed. Cannot be null.
		/// </param>
		/// <param name="tagsProcessor">
		/// An optional processor to dynamically resolve tag values.
		/// </param>
		/// <param name="tagValues">
		/// An optional dictionary of tag names and their corresponding replacement values.
		/// </param>
		/// <param name="delimiters">
		/// An optional set of tag delimiters to use for identifying tags in the input text.
		/// </param>
		/// <param name="token">
		/// A cancellation token to observe while waiting for the task to complete.
		/// </param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the asynchronous operation.
		/// The task result contains the parsed string with tags replaced.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="text"/> is null.
		/// </exception>
		public Task<string> ParseAsync(string text, 
									   ITagsProcessor? tagsProcessor = null, 
									   IDictionary<string, string>? tagValues = null, 
									   TagDelimiters? delimiters = null, 
									   CancellationToken token = default);
	}

	/// <summary>
	/// Default implementation of <see cref="IAsyncStringParser"/>.
	/// Adds async parsing support to <see cref="IStringParserService"/>.
	/// </summary>
	public class AsyncStringParser : IAsyncStringParser, IStringParserService
	{
		readonly IStringTokenizer tokenizer;
		readonly IConverterRegistry converterRegistry;
		readonly Dictionary<string, IConverter> builtInConverters = new();

		public TagDelimiters DefaultDelimiters { get; set; } = TagDelimiters.Default;

		public AsyncStringParser() : this(new ConverterRegistry(), new StringTokenizer())
		{
		}

		public AsyncStringParser(IConverterRegistry converterRegistry, IStringTokenizer tokenizer)
		{
			this.converterRegistry = converterRegistry ?? throw new ArgumentNullException(nameof(converterRegistry));
			this.tokenizer         = tokenizer         ?? throw new ArgumentNullException(nameof(tokenizer));

			InitializeBuiltInConverters();
			BuiltInConvertersEnabled = true;
		}

		/// <inheritdoc />
		public async Task<string> ParseAsync(string text, 
											 ITagsProcessor? tagsProcessor = null, 
											 IDictionary<string, string>? overriddenValues = null, 
											 TagDelimiters? delimiters = null, 
											 CancellationToken cancellationToken = default)
		{
			if (text == null)
			{
				throw new ArgumentNullException(nameof(text));
			}

			cancellationToken.ThrowIfCancellationRequested();

			IList<TagSegment> segments = tokenizer.Tokenize(text, delimiters ?? DefaultDelimiters);

			if (segments.Count == 0)
			{
				// No segments to process; return the original text early
				return text;
			}

			IList<TagSegment> filteredSegments = ProcessOverrides(overriddenValues, segments);

			Dictionary<string, ISet<TagSegment>> tagNameToSegments = ToTagNameSegments(filteredSegments);

			if (tagsProcessor != null)
			{
				await tagsProcessor.SetTagValuesAsync(tagNameToSegments, cancellationToken);
			}

			ProcessSegmentSets(tagNameToSegments);

			return BuildFinalString(segments, text);
		}

		IList<TagSegment> ProcessOverrides(IDictionary<string, string>? overriddenValues, 
										   IList<TagSegment> segments)
		{
			IList<TagSegment> filteredSegments;

			if (overriddenValues != null)
			{
				filteredSegments = new List<TagSegment>();

				foreach (TagSegment segment in segments)
				{
					if (overriddenValues.TryGetValue(segment.Tag, out string? value) && value != null)
					{
						segment.TagValue = value;
						continue;
					}

					filteredSegments.Add(segment);
				}
			}
			else
			{
				filteredSegments = segments;
			}

			return filteredSegments;
		}

		void ProcessSegmentSets(Dictionary<string, ISet<TagSegment>> tagNameToSegments)
		{
			foreach (var pair in tagNameToSegments)
			{
				string tagName = pair.Key;

				if (converterRegistry.TryGetValue(tagName, out var converter) && converter != null)
				{
					foreach (TagSegment tagSegment in pair.Value)
					{
						if (tagSegment.TagValue == null)
						{
							tagSegment.TagValue = converter.Convert(tagSegment.TagArg);
						}
					}
				}
			}
		}

		Dictionary<string, ISet<TagSegment>> ToTagNameSegments(IEnumerable<TagSegment> segments)
		{
			return segments.GroupBy(x => x.TagName).ToDictionary(
				x => x.Key,
				x => (ISet<TagSegment>)new HashSet<TagSegment>(x));
		}

		string BuildFinalString(IEnumerable<TagSegment> segments, string originalText)
		{
			StringBuilder sb = new();

			int lastIndex = 0;

			foreach (TagSegment tagSegment in segments)
			{
				// Append the text before the current segment
				sb.Append(originalText, lastIndex, (int)tagSegment.Index - lastIndex);

				// Append the tag value if it exists, otherwise the original tag
				sb.Append(tagSegment.TagValue?.ToString() 
						  ?? originalText.Substring((int)tagSegment.Index, (int)tagSegment.Length));

				lastIndex = (int)(tagSegment.Index + tagSegment.Length);
			}

			// Append any remaining text after the last tag
			sb.Append(originalText, lastIndex, originalText.Length - lastIndex);

			return sb.ToString();
		}

		#region Implementation of IStringParserService

		/// <inheritdoc />
		public string Parse(string text, 
							IDictionary<string, string>? overriddenValues = null,
							TagDelimiters? delimiters = null)
		{
			IList<TagSegment> segments = tokenizer.Tokenize(text, delimiters ?? DefaultDelimiters);

			IEnumerable<TagSegment> filteredSegments = ProcessOverrides(overriddenValues, segments);

			Dictionary<string, ISet<TagSegment>> tagNameToSegments = ToTagNameSegments(filteredSegments);

			ProcessSegmentSets(tagNameToSegments);
			
			return BuildFinalString(segments, text);
		}

		/// <inheritdoc />
		public void RegisterConverter(string tagName, IConverter converter)
		{
			converterRegistry.SetValue(tagName, converter);
		}

		/// <inheritdoc />
		public void RegisterConverter(string tagName, Func<object, object> convertFunc)
		{
			converterRegistry.SetValue(tagName, new DelegateConverter(convertFunc));
		}

		#endregion

		#region Built-in Converters

		void InitializeBuiltInConverters()
		{
			builtInConverters["date"]    = new DelegateConverter(_ => DateTime.Today.ToString("d"));
			builtInConverters["time"]    = new DelegateConverter(_ => DateTime.Now.ToString("HH:mm:ss"));
			builtInConverters["timeUtc"] = new DelegateConverter(_ => DateTime.UtcNow.ToString("HH:mm:ss"));
		}

		bool builtInConvertersEnabled;

		public bool BuiltInConvertersEnabled
		{
			get => builtInConvertersEnabled;
			set
			{
				if (builtInConvertersEnabled == value)
				{
					return;
				}

				builtInConvertersEnabled = value;

				if (builtInConvertersEnabled)
				{
					EnableBuiltInConverters();
				}
				else
				{
					DisableBuiltInConverters();
				}
			}
		}

		void EnableBuiltInConverters()
		{
			foreach (var entry in builtInConverters)
			{
				converterRegistry.SetValue(entry.Key, entry.Value);
			}
		}

		void DisableBuiltInConverters()
		{
			foreach (var key in builtInConverters.Keys)
			{
				converterRegistry.TryRemove(key, out _);
			}
		}

		#endregion
	}
}