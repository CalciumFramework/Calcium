#if __ANDROID__
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-26 16:44:33Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.App;
using Android.Content;

using Calcium.Concurrency;
using Double = Java.Lang.Double;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class is an <see cref="Calcium.SettingsModel.ISettingsStore"/> 
	/// implementation for Android for persistent local settings.
	/// </summary>
	public class AndroidLocalSettingsStore : ISettingsStore
	{
		const string FileName = "Settings";

		ISharedPreferences GetPreferences()
		{
			return Application.Context.GetSharedPreferences(FileName, FileCreationMode.Private);
		}

		public bool TryGetValue(string key, Type settingType, out object value)
		{
			ISharedPreferences preferences = GetPreferences();
			IDictionary<string, object> all = preferences.All;
			var hasValue = all.TryGetValue(key, out value);

			if (hasValue && settingType == typeof(double) && value is long)
			{
				value = Double.LongBitsToDouble((long)value);
			}

			return hasValue;
		}

		public bool Contains(string key)
		{
			ISharedPreferences preferences = GetPreferences();
			return preferences.Contains(key);
		}

		public bool Remove(string key)
		{
			ISharedPreferences preferences = GetPreferences();
			ISharedPreferencesEditor editor = preferences.Edit();
			editor.Remove(key);
			return editor.Commit();
		}

		public Task ClearAsync()
		{
			ISharedPreferences preferences = GetPreferences();
			ISharedPreferencesEditor editor = preferences.Edit();
			editor.Clear();
			editor.Commit();

			return Task.CompletedTask;
		}

		public Task SaveAsync()
		{
			/* Nothing to do. */

			return Task.CompletedTask;
		}

		public object this[string key]
		{
			get
			{
				ISharedPreferences preferences = GetPreferences();
				IDictionary<string, object> all = preferences.All;
				if (!all.TryGetValue(key, out object result))
				{
					return null;
				}
				return result;
			}
			set
			{
				ISharedPreferences preferences = GetPreferences();
				ISharedPreferencesEditor editor = preferences.Edit();
				StoreSetting(editor, key, value);
				editor.Commit();
			}
		}

		public SettingsStoreStatus Status => SettingsStoreStatus.Ready;

		void StoreSetting(ISharedPreferencesEditor editor, string key, object value)
		{
			if (value == null)
			{
				editor.Remove(key);
			}
			else if (value is int)
			{
				editor.PutInt(key, (int)value);
			}
			else if (value is long)
			{
				editor.PutLong(key, (long)value);
			}
			else if (value is float)
			{
				editor.PutFloat(key, (float)value);
			}
			else if (value is bool)
			{
				editor.PutBoolean(key, (bool)value);
			}
			else if (value is byte[])
			{
				string base64 = SerializationContants.Base64EncodingPrefix + Convert.ToBase64String((byte[])value);
				editor.PutString(key, base64);
			}
			else if (value is double)
			{
				long longValue = Double.DoubleToLongBits((double)value);
				editor.PutLong(key, longValue);
			}
			else
			{
				editor.PutString(key, value.ToString());
			}
		}
	}
}
#endif
