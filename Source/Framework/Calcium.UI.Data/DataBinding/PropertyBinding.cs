#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System.Diagnostics;
using System.Reflection;
using Calcium.MissingTypes.System.Windows.Data;
#if !(MONODROID || __ANDROID__)
using View = System.Object;
#else
using Android.Views;
#endif

namespace Calcium.UI.Data
{
	[DebuggerDisplay("View={View}, SourceProperty={SourceProperty?.Name}, TargetProperty={TargetProperty?.Name}, TargetMethod={TargetMethod?.Name}, Converter={Converter}, ConverterParameter={ConverterParameter}")]
	public class PropertyBinding
	{
		public View View { get; set; }
		public PropertyInfo SourceProperty { get; set; }
		public PropertyInfo TargetProperty { get; set; }

		public MethodInfo TargetMethod { get; set; }

		public IValueConverter Converter { get; set; }
		public string ConverterParameter { get; set; }

#pragma warning disable 414
		internal bool PreventUpdateForSourceProperty;
#pragma warning restore 414

		internal bool PreventUpdateForTargetProperty;

		public override string ToString()
		{
			return $"View={View}, SourceProperty={SourceProperty?.Name}, TargetProperty={TargetProperty?.Name}, TargetMethod={TargetMethod?.Name}, Converter={Converter}, ConverterParameter={ConverterParameter}";
		}
	}
}
