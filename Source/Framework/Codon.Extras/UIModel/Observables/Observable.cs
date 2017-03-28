#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-07-21 09:42:07Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Codon.UIModel
{
	/// <summary>
	/// This class is analagous to the Knockout JS style observables. 
	/// It is used to wrap a value and provides change notification 
	/// for bindings without adding plumbing to a viewmodel.
	/// <see cref="http://danielvaughan.org/post/Knockout-Style-Observables-in-XAML.aspx" />
	/// </summary>
	/// <example>
	/// class MainWindowViewModel
	///{
	///    readonly ObservableValue&lt;string&gt; firstName = new ObservableValue&lt;string&gt;("Alan");
	///
	///    public ObservableValue&lt;string&gt; FirstName
	///    {
	///        get
	///        {
	///           return firstName;
	///        }
	///    }
	/// ...
	/// }
	/// Bind to the value in XAML using:
	/// &lt;TextBox Text="{Binding Path=FirstName.Value, UpdateSourceTrigger=PropertyChanged}" /&gt;
	/// </example>
	/// <typeparam name="T">
	/// The type of field that is managed by this object.</typeparam>
	public sealed class Observable<T> : INotifyPropertyChanged
	{
		T valueField;

		/// <summary>
		/// Gets or sets the value that is wrapped by this object.
		/// </summary>
		/// <value>
		/// The value.
		/// </value>
		public T Value
		{
			get => valueField;
			set
			{
				if (!EqualityComparer<T>.Default.Equals(valueField, value))
				{
					valueField = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="Observable{T}"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
		public Observable(T value = default(T))
		{
			Value = value;
		}

		/// <summary>
		/// Occurs when the <seealso cref="Value"/> property changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName]string propertyName = null)
		{
			var temp = PropertyChanged;

			if (temp == null)
			{
				return;
			}

			UIContext.Instance.Send(() =>
			{
				temp.Invoke(this, new PropertyChangedEventArgs(propertyName));
			});			
		}
	}
}
