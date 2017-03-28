#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading;

#if __ANDROID__ || MONODROID
using Android.Widget;
#endif

using Codon.Reflection;

namespace Codon.UI.Data
{
	public class ViewBinderRegistry
	{
		readonly ReaderWriterLockSlim dictionaryLock = new ReaderWriterLockSlim();

		public bool RemoveViewBinder(Type viewType, string propertyName)
		{
			string key = MakeDictionaryKey(viewType, propertyName);

			dictionaryLock.EnterWriteLock();
			try
			{
				return binderDictionary.Remove(key);
			}
			finally
			{
				dictionaryLock.ExitWriteLock();
			}
		}

		public bool TryGetViewBinder(Type viewType, string propertyName, out IViewBinder viewBinder)
		{
			string key = MakeDictionaryKey(viewType, propertyName);

			try
			{
				dictionaryLock.EnterUpgradeableReadLock();

				if (binderDictionary.TryGetValue(key, out viewBinder))
				{
					return true;
				}

				if (implicityBinderDictionary.TryGetValue(key, out viewBinder))
				{
					return true;
				}

				foreach (var pair in binderDictionary)
				{
					string name = pair.Key;
					if (!name.EndsWith(propertyName))
					{
						continue;
					}

					var binder = pair.Value;
					var type = binder.ViewType;
					if (type != null && type.IsAssignableFromEx(viewType))
					{
						string newItemKey = MakeDictionaryKey(viewType, propertyName);
						
						try
						{
							dictionaryLock.EnterWriteLock();

							implicityBinderDictionary[newItemKey] = binder;
						}
						finally
						{
							dictionaryLock.ExitWriteLock();
						}
						
						viewBinder = binder;

						return true;
					}
				}
			}
			finally
			{
				dictionaryLock.ExitUpgradeableReadLock();
			}

			return false;
		}

		static string MakeDictionaryKey(Type viewType, string propertyName)
		{
			return viewType.AssemblyQualifiedName + "." + propertyName;
		}

		public void SetViewBinder<TView>(string propertyName, IViewBinder viewBinder)
		{
			string key = typeof(TView).AssemblyQualifiedName + "." + propertyName;

			dictionaryLock.EnterWriteLock();
			try
			{
				binderDictionary[key] = viewBinder;
			}
			finally
			{
				dictionaryLock.ExitWriteLock();
			}
		}

		public void SetViewBinder(Type viewType, string propertyName, IViewBinder viewBinder)
		{
			string key = MakeDictionaryKey(viewType, propertyName);

			dictionaryLock.EnterWriteLock();
			try
			{
				binderDictionary[key] = viewBinder;
			}
			finally
			{
				dictionaryLock.ExitWriteLock();
			}
		}

		readonly Dictionary<string, IViewBinder> implicityBinderDictionary 
			= new Dictionary<string, IViewBinder>();

		readonly Dictionary<string, IViewBinder> binderDictionary
			= new Dictionary<string, IViewBinder>
			{
#if __ANDROID__ || MONODROID
				{MakeDictionaryKey(typeof(CalendarView), nameof(CalendarView.Date)), new ViewEventBinder<CalendarView, CalendarView.DateChangeEventArgs, DateTime>(
					(view, h) => view.DateChange += h, (view, h) => view.DateChange -= h, (view, args) => new DateTime(args.Year, args.Month, args.DayOfMonth))},
				{MakeDictionaryKey(typeof(CheckBox), nameof(CheckBox.Checked)), new ViewEventBinder<CheckBox, CheckBox.CheckedChangeEventArgs, bool>(
					(view, h) => view.CheckedChange += h, (view, h) => view.CheckedChange -= h, (view, args) => args.IsChecked)},
				{MakeDictionaryKey(typeof(RadioButton), nameof(RadioButton.Checked)), new ViewEventBinder<RadioButton, RadioButton.CheckedChangeEventArgs, bool>(
					(view, h) => view.CheckedChange += h, (view, h) => view.CheckedChange -= h, (view, args) => args.IsChecked)},
				{MakeDictionaryKey(typeof(RatingBar), nameof(RatingBar.Rating)), new ViewEventBinder<RatingBar, RatingBar.RatingBarChangeEventArgs, float>(
					(view, h) => view.RatingBarChange += h, (view, h) => view.RatingBarChange -= h, (view, args) => args.Rating)},
				{MakeDictionaryKey(typeof(SearchView), nameof(SearchView.Query)), new ViewEventBinder<SearchView, SearchView.QueryTextChangeEventArgs, string>(
					(view, h) => view.QueryTextChange += h, (view, h) => view.QueryTextChange -= h, (view, args) => args.NewText)},
				{MakeDictionaryKey(typeof(Switch), nameof(Switch.Checked)), new ViewEventBinder<Switch, Switch.CheckedChangeEventArgs, bool>(
					(view, h) => view.CheckedChange += h, (view, h) => view.CheckedChange -= h, (view, args) => args.IsChecked)},
				{MakeDictionaryKey(typeof(TimePicker), nameof(TimePicker.Minute)), new ViewEventBinder<TimePicker, TimePicker.TimeChangedEventArgs, int>(
					(view, h) => view.TimeChanged += h, (view, h) => view.TimeChanged -= h, (view, args) => args.Minute)},
				{MakeDictionaryKey(typeof(TimePicker), nameof(TimePicker.Hour)), new ViewEventBinder<TimePicker, TimePicker.TimeChangedEventArgs, int>(
					(view, h) => view.TimeChanged += h, (view, h) => view.TimeChanged -= h, (view, args) => args.HourOfDay)},
				{MakeDictionaryKey(typeof(EditText), nameof(EditText.Text)), new ViewEventBinder<EditText, Android.Text.TextChangedEventArgs, string>(
					(view, h) => view.TextChanged += h, (view, h) => view.TextChanged -= h, (view, args) => args.Text.ToString())},
				{MakeDictionaryKey(typeof(ToggleButton), nameof(ToggleButton.Text)), new ViewEventBinder<ToggleButton, CompoundButton.CheckedChangeEventArgs, bool>(
					(view, h) => view.CheckedChange += h, (view, h) => view.CheckedChange -= h, (view, args) => args.IsChecked)},
				{MakeDictionaryKey(typeof(SeekBar), nameof(SeekBar.Progress)), new ViewEventBinder<SeekBar, SeekBar.ProgressChangedEventArgs, int>(
					(view, h) => view.ProgressChanged += h, (view, h) => view.ProgressChanged -= h, (view, args) => args.Progress)},
#endif
			};
	}
}