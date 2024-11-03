#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Calcium.ResourcesModel.Experimental
{
	public interface IAsyncTagValueSetter
	{
		Task SetValuesAsync(ISet<TagSegment> argSet, CancellationToken token);
	}

	public interface ITagsProcessor
	{
		/// <summary>
		/// Sets the <see cref="TagSegment.TagValue"/> property
		/// of each <see cref="TagSegment"/> in the specified dictionary.
		/// </summary>
		/// <param name="tagArgs">
		/// A dictionary whose key is the 'tag name' and the value
		/// is the set of 'tag args' for each occurrence of the tag in the input text.
		/// Consider the following input text:
		/// ```
		/// Today's date is ${Date} and 
		/// the secret email password value is ${Secret:EmailPassword}
		/// ```</param>
		/// <param name="token">Used to cancel the asynchronous operation.</param>
		Task SetTagValuesAsync(IDictionary<string, ISet<TagSegment>> tagArgs, CancellationToken token);
	}

	public class TagsProcessor : ITagsProcessor
	{
		readonly IDictionary<string, IAsyncTagValueSetter> converters;

		public TagsProcessor(IDictionary<string, IAsyncTagValueSetter> converters)
		{
			this.converters = converters ?? throw new ArgumentNullException(nameof(converters));
		}

		public async Task SetTagValuesAsync(IDictionary<string, ISet<TagSegment>> tagArgs, CancellationToken token)
		{
			foreach (var pair in tagArgs)
			{
				if (!converters.TryGetValue(pair.Key, out var converter))
				{
					continue;
				}

				await converter.SetValuesAsync(pair.Value, token);
			}
		}
	}
	
	public class TagSegment
	{
		public uint Index { get; set; }
		public uint Length { get; set; }
		public string TagName { get; set; }
		public string TagArg { get; set; }

		public string Tag { get; set; }

		public object? TagValue { get; set; }
	}
}