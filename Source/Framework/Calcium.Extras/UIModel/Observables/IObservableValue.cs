#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-04-14 18:12:19Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Calcium.UIModel
{
	public interface IObservableValue<out T> : INotifyPropertyChanged
	{
		T Value { get; }
	}
}
