#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-01-10 16:27:11Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Codon.ComponentModel
{
	/// <summary>
	/// If a class implements <see cref="INotifyPropertyChanged"/>
	/// then implementing this interface allows change events
	/// to be switched off.
	/// </summary>
	public interface ISuspendChangeNotification
	{
		/// <summary>
		/// Gets or sets a value indicating if change events
		/// should be raised.
		/// </summary>
		bool ChangeNotificationSuspended { get; set; }
	}
}
