#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-04 11:52:49Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using Calcium.ComponentModel.Experimental;
using Calcium.InversionOfControl;

namespace Calcium.ResourcesModel.Experimental
{
	[DefaultType(typeof(AsyncSetterRegistry), Singleton = true)]
	public interface IAsyncSetterRegistry : IRegistry<string, IAsyncTagValueSetter>
	{
	}

	public class AsyncSetterRegistry : Registry<string, IAsyncTagValueSetter>, IAsyncSetterRegistry
	{
	}

	public interface IAsyncSetterRegistry<TContext> : IRegistry<string, IAsyncTagValueSetter<TContext>>
	{
	}

	public class AsyncSetterRegistry<TContext> : Registry<string, IAsyncTagValueSetter<TContext>>, 
												 IAsyncSetterRegistry<TContext>
	{
	}
}