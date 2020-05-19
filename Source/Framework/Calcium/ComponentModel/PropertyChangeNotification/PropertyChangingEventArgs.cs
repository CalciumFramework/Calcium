#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-09-06 16:54:30Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Codon.ComponentModel
{
	/// <summary>
	/// <c>PropertyChangingEventArgs</c> is implemented differently
	/// on different platforms. This interface serves to abstract
	/// the implementation.
	/// </summary>
	public interface IPropertyChangingEventArgs : IPropertyChangedEventArgs
	{
		/// <summary>
		/// Cancels the update so that no change is made
		/// and the value remains the same.
		/// </summary>
		void Cancel();
	}

	/// <summary>
	/// Provides data for the <see cref="INotifyPropertyChanging.PropertyChanging"/> event,
	/// exposed via the <see cref="PropertyChangeNotifier"/>.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	public sealed class PropertyChangingEventArgs<TProperty> : PropertyChangingEventArgs, IPropertyChangingEventArgs
	{
		/// <summary>
		/// Gets the value of the property before it was changed.
		/// </summary>
		/// <value>The old value.</value>
		public TProperty OldValue { get; private set; }

		/// <summary>
		/// Gets the new value of the property after it was changed.
		/// </summary>
		/// <value>The new value.</value>
		public TProperty NewValue { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// this <see cref="PropertyChangingEventArgs{TProperty}"/> has been cancelled.
		/// </summary>
		/// <value><c>true</c> if cancelled; otherwise, <c>false</c>.</value>
		public bool Cancelled { get; private set; }

		/// <summary>
		/// Cancels this instance so that the change will not occur.
		/// </summary>
		public void Cancel()
		{
			Cancelled = true;
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangedEventArgs{TProperty}"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property that changed.</param>
		/// <param name="oldValue">The old value before the change occurred.</param>
		/// <param name="newValue">The new value after the change occurred.</param>
		public PropertyChangingEventArgs(
			string propertyName, TProperty oldValue, TProperty newValue)
			: base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		#region IPropertyChangingEventArgs Members

		string IPropertyChangedEventArgs.PropertyName => PropertyName;

		object IPropertyChangedEventArgs.OldValue => OldValue;

		object IPropertyChangedEventArgs.NewValue => NewValue;

		void IPropertyChangingEventArgs.Cancel()
		{
			Cancel();
		}

		#endregion
	}
}