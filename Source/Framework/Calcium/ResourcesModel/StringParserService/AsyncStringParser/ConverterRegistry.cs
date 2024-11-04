#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:41:18Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using Calcium.ComponentModel;
using Calcium.ComponentModel.Experimental;
using Calcium.InversionOfControl;

namespace Calcium.ResourcesModel.Experimental
{
	[DefaultType(typeof(ConverterRegistry), Singleton = true)]
	public interface IConverterRegistry : IRegistry<string, IConverter>
	{
	}

	public class ConverterRegistry : Registry<string, IConverter>, IConverterRegistry
	{
	}
}