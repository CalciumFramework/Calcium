#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-04-14 18:12:03Z</CreationDate>
</File>
*/
#endregion

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Calcium.Concurrency;

namespace Calcium.UIModel
{
	/// <summary>
	/// Exposes a read-only observable value that is projected from a property
	/// on another object implementing <see cref="INotifyPropertyChanged"/>.
	/// </summary>
	/// <typeparam name="T">The projected value type.</typeparam>
	public sealed class PropertyObservable<T> : IObservableValue<T>,
												IDisposable
	{
		readonly INotifyPropertyChanged source;
		readonly Func<T> getValue;
		readonly HashSet<string>? propertyNames;
		T valueField;
		bool disposed;

		/// <summary>
		/// Gets the current projected value.
		/// </summary>
		public T Value => valueField;

		/// <param name="source">
		/// The source object whose <see cref="INotifyPropertyChanged.PropertyChanged"/>
		/// event will be observed.
		/// </param>
		/// <param name="getValue">
		/// A delegate used to retrieve the current projected value from the source.
		/// </param>
		/// <param name="propertyName">
		/// The name of the source property that should trigger reevaluation.
		/// Pass null to reevaluate on every property change.
		/// </param>
		public PropertyObservable(INotifyPropertyChanged source,
								  Func<T> getValue,
								  string? propertyName = null)
			: this(source,
				   getValue,
				   propertyName != null ? new[] { propertyName } : null)
		{
		}

		/// <param name="source">
		/// The source object whose <see cref="INotifyPropertyChanged.PropertyChanged"/>
		/// event will be observed.
		/// </param>
		/// <param name="getValue">
		/// A delegate used to retrieve the current projected value from the source.
		/// </param>
		/// <param name="propertyNames">
		/// The names of the source properties that should trigger reevaluation.
		/// Pass null to reevaluate on every property change.
		/// </param>
		public PropertyObservable(INotifyPropertyChanged source,
								  Func<T> getValue,
								  IEnumerable<string>? propertyNames)
		{
			this.source = source ?? throw new ArgumentNullException(nameof(source));
			this.getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));

			if (propertyNames != null)
			{
				this.propertyNames = new HashSet<string>(propertyNames);
			}

			valueField = this.getValue();
			source.PropertyChanged += OnSourcePropertyChanged;
		}

		/// <summary>
		/// Occurs when the <see cref="Value"/> property changes.
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (disposed)
			{
				return;
			}

			if (!ShouldReevaluate(e.PropertyName))
			{
				return;
			}

			T newValue = getValue();

			if (EqualityComparer<T>.Default.Equals(valueField, newValue))
			{
				return;
			}

			valueField = newValue;
			OnPropertyChanged(nameof(Value));
		}

		bool ShouldReevaluate(string? propertyName)
		{
			if (propertyNames == null)
			{
				return true;
			}

			if (string.IsNullOrEmpty(propertyName))
			{
				return true;
			}

			return propertyNames.Contains(propertyName!);
		}

		void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChangedEventHandler? handler = PropertyChanged;

			if (handler == null)
			{
				return;
			}

			SynchronizationContext.Send(() =>
			{
				handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
			});
		}

		ISynchronizationContext? synchronizationContext;

		/// <summary>
		/// Use this property to override the current synchronization context.
		/// </summary>
		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContext ??= UIContext.Instance;
			set => synchronizationContext = value;
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			source.PropertyChanged -= OnSourcePropertyChanged;
			disposed = true;
		}
	}
}