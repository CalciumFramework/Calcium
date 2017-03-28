#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-19 17:01:19Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Codon.ComponentModel
{
	/// <summary>
	/// This class raises a 
	/// <see cref="INotifyPropertyChanged.PropertyChanged"/>
	/// for its <see cref="Instance"/> property
	/// when its <see cref="RaisePropertyChanged"/> method is called.
	/// It allows you to update the UI eventhough an object
	/// does not implement <seealso cref="INotifyPropertyChanged"/>.
	/// </summary>
	/// <typeparam name="T">
	/// The Instance type.</typeparam>
	public class InpcWrapper<T> : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		static T instance;

		public T Instance => instance;

		public InpcWrapper(T observableObject)
		{
			instance = observableObject;
		}

		/// <summary>
		/// Calling this method raises the <see cref="PropertyChanged"/>
		/// event for this class; indicating that the Instance object
		/// has changed. If the InpcWrapper is the source of a data-binding
		/// then the target is refreshed. Its equivalent to rebinding
		/// to the Instance object.
		/// </summary>
		public void RaisePropertyChanged()
		{
			PropertyChanged?.Invoke(
				this, new PropertyChangedEventArgs(nameof(Instance)));
		}
	}
}
