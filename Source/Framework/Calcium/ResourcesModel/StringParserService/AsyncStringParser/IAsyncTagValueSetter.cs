#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:39:18Z</CreationDate>
</File>
*/
#endregion

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

	public interface IAsyncTagValueSetter<in TContext>
	{
		Task SetValuesAsync(ISet<TagSegment> argSet, TContext context, CancellationToken token);
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
		readonly IReadOnlyDictionary<string, IAsyncTagValueSetter> converters;

		public TagsProcessor(IReadOnlyDictionary<string, IAsyncTagValueSetter> converters)
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

	public class TagsProcessor<TContext> : ITagsProcessor
	{
		readonly IReadOnlyDictionary<string, IAsyncTagValueSetter<TContext>> converters;
		readonly TContext context;

		public TagsProcessor(IReadOnlyDictionary<string, IAsyncTagValueSetter<TContext>> converters, TContext context)
		{
			this.converters = converters ?? throw new ArgumentNullException(nameof(converters));
			this.context    = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task SetTagValuesAsync(IDictionary<string, ISet<TagSegment>> tagArgs, CancellationToken token)
		{
			foreach (var pair in tagArgs)
			{
				if (!converters.TryGetValue(pair.Key, out var converter))
				{
					continue;
				}

				await converter.SetValuesAsync(pair.Value, context, token);
			}
		}
	}
}