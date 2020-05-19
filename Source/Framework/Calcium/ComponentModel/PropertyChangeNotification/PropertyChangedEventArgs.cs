#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2009-08-01 13:01:34Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;

namespace Calcium.ComponentModel
{
	/// <summary>
	/// <c>PropertyChangedEventArgs</c> is implemented differently
	/// on different platforms. This interface serves to abstract
	/// the implementation.
	/// </summary>
	public interface IPropertyChangedEventArgs
	{
		/// <summary>
		/// The name of the property that changed.
		/// </summary>
		string PropertyName { get; }

		/// <summary>
		/// The value before the change.
		/// </summary>
		object OldValue { get; }

		/// <summary>
		/// The value after the change occurred.
		/// </summary>
		object NewValue { get; }
	}

	/// <summary>
	/// Provides data for the 
	/// <see cref="INotifyPropertyChanged.PropertyChanged"/> event,
	/// exposed via the <see cref="PropertyChangeNotifier"/>.
	/// </summary>
	/// <typeparam name="TProperty">The type of the property.</typeparam>
	public sealed class PropertyChangedEventArgs<TProperty> 
		: PropertyChangedEventArgs, IPropertyChangedEventArgs
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

		string IPropertyChangedEventArgs.PropertyName => PropertyName;

		object IPropertyChangedEventArgs.OldValue => OldValue;

		object IPropertyChangedEventArgs.NewValue => NewValue;

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="PropertyChangedEventArgs{TProperty}"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property that changed.</param>
		/// <param name="oldValue">The old value before the change occurred.</param>
		/// <param name="newValue">The new value after the change occurred.</param>
		public PropertyChangedEventArgs(
			string propertyName, TProperty oldValue, TProperty newValue) 
			: base(propertyName)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
