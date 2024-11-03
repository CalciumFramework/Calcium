#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		/// Asynchronously parses a string and replaces tags within it based on the provided processors and values.
		/// </summary>
		/// <param name="text">The input text containing tags to be parsed. Cannot be null.</param>
		/// <param name="tagsProcessor">An optional processor to dynamically resolve tag values.</param>
		/// <param name="tagValues">An optional dictionary of tag names and their corresponding replacement values.</param>
		/// <param name="delimiters">An optional set of tag delimiters to use for identifying tags in the input text.</param>
		/// <param name="token">A cancellation token to observe while waiting for the task to complete.</param>
		/// <returns>A <see cref="Task{TResult}"/> that represents the asynchronous operation. The task result contains the parsed string with tags replaced.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is null.</exception>
		public Task<string> ParseAsync(string text, 
									   ITagsProcessor? tagsProcessor = null, 
									   IDictionary<string, string>? tagValues = null, 
									   TagDelimiters? delimiters = null, 
									   CancellationToken token = default);
	}

	public class AsyncStringParser : IAsyncStringParser, IStringParserService
	{
		readonly IStringTokenizer tokenizer;
		readonly IConverterRegistry converterRegistry;

		public TagDelimiters DefaultDelimiters { get; set; } = TagDelimiters.Default;

		public AsyncStringParser() : this(new ConverterRegistry(), new StringTokenizer())
		{
		}

		public AsyncStringParser(IConverterRegistry converterRegistry, IStringTokenizer tokenizer)
		{
			this.converterRegistry = converterRegistry ?? throw new ArgumentNullException(nameof(converterRegistry));
			this.tokenizer         = tokenizer         ?? throw new ArgumentNullException(nameof(tokenizer));
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

		IList<TagSegment> ProcessOverrides(IDictionary<string, string>? overriddenValues, IList<TagSegment> segments)
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

				if (converterRegistry.TryGetConverter(tagName, out var converter) && converter != null)
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

		#region Implementation of IStringParserService

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
				sb.Append(tagSegment.TagValue?.ToString() ?? originalText.Substring((int)tagSegment.Index, (int)tagSegment.Length));

				lastIndex = (int)(tagSegment.Index + tagSegment.Length);
			}

			// Append any remaining text after the last tag
			sb.Append(originalText, lastIndex, originalText.Length - lastIndex);

			return sb.ToString();
		}

		public string Parse(string text, TagDelimiters? delimiters = null)
		{
			return Parse(text, null, delimiters);
		}

		public void RegisterConverter(string tagName, IConverter converter)
		{
			converterRegistry.SetConverter(tagName, converter);
		}

		public void RegisterConverter(string tagName, Func<object, object> convertFunc)
		{
			converterRegistry.SetConverter(tagName, new DelegateConverter(convertFunc));
		}

		#endregion
	}

	[DefaultType(typeof(ConverterRegistry), Singleton = true)]
	public interface IConverterRegistry
	{
		void SetConverter(string tagName, IConverter converter);
		bool TryGetConverter(string tagName, out IConverter? converter);
		bool TryRemoveConverter(string tagName, out IConverter? converter);
	}

	public class ConverterRegistry : IConverterRegistry
	{
		readonly ConcurrentDictionary<string, IConverter> converters = new();

		public IConverter this[string tagName] => converters[tagName];

		readonly ReadOnlyDictionary<string, IConverter> readOnlyDictionary;

		public ConverterRegistry()
		{
			readOnlyDictionary = new(converters);
		}

		public IReadOnlyDictionary<string, IConverter> ReadOnlyDictionary => readOnlyDictionary;

		public void SetConverter(string tagName, IConverter converter)
		{
			converters[tagName] = converter;
		}

		public bool TryGetConverter(string tagName, out IConverter? converter)
		{
			return converters.TryGetValue(tagName, out converter);
		}

		public bool TryRemoveConverter(string tagName, out IConverter? converter)
		{
			return converters.TryRemove(tagName, out converter);
		}
	}
}