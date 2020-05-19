#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-12 12:51:29Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

using Codon.ComponentModel;
using Codon.ComponentModel.ExceptionHandlers;
using Codon.InversionOfControl;
using Codon.IO;
using Codon.Logging;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class is used to persist and dictionary
	/// of keyed setting value to isolated store.
	/// It makes use of the <see cref="IBinarySerializer"/>
	/// to serialize the dictionary.
	/// This class makes use of the <see cref="IExceptionHandler"/>
	/// implementation. If an exception is thrown during
	/// deserialization, then the exception
	/// handler instance is used to determine if the exception
	/// should be rethrown. <seealso cref="LoggingExceptionHandler"/>
	/// </summary>
	public sealed class IsolatedStorageSettings :
		IDictionary<string, object>, IDictionary,
		ICollection<KeyValuePair<string, object>>, ICollection,
		IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		static IsolatedStorageSettings applicationSettings;

		readonly System.IO.IsolatedStorage.IsolatedStorageFile container;
		readonly Dictionary<string, object> settings;

		const string localSettingsFileName = "CodonLocalSettings";

		internal IsolatedStorageSettings(IsolatedStorageFile storageFile)
		{
			container = storageFile;

			if (!storageFile.FileExists(localSettingsFileName))
			{
				settings = new Dictionary<string, object>();
				return;
			}

			using (var fileStream = storageFile.OpenFile(localSettingsFileName, FileMode.Open))
			{
				try
				{
					var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();
					settings = serializer.Deserialize<Dictionary<string, object>>((Stream)fileStream);
				}
				catch (Exception ex)
				{
					var exceptionHandler = Dependency.Resolve<IExceptionHandler>();
					if (exceptionHandler.ShouldRethrowException(ex, this))
					{
						throw;
					}

					settings = new Dictionary<string, object>();
				}
			}
		}

		/// <summary>
		/// Persist the settings to isolated storage.
		/// </summary>
		/// <exception cref="ResolutionException">
		/// If an exception occurs while retrieving the binary
		/// serializer.</exception>
		/// <exception cref="Exception">
		/// If the serialization fails.</exception>
		public void Save()
		{
			var serializer = Dependency.Resolve<IBinarySerializer, BinarySerializer>();

			using (var fileStream = container.CreateFile(localSettingsFileName))
			{
				serializer.Serialize(settings, fileStream);
			}
		}

		~IsolatedStorageSettings()
		{
			try
			{
				Save();
			}
			catch (ObjectDisposedException)
			{
				/* Ignore. */
			}
			catch (Exception ex)
			{
				var log = Dependency.Resolve<ILog>();
				log.Error("Saving the isolated storage settings" +
						" threw an exception.", ex);
			}
		}

		/// <summary>
		/// Obtains the user-scoped isolated storage settings
		/// corresponding to the calling code's application identity.
		/// </summary>
		public static IsolatedStorageSettings ApplicationSettings => 
			applicationSettings ?? 
			(applicationSettings = new IsolatedStorageSettings(
							IsolatedStorageFile.GetUserStoreForApplication()));

		public int Count => settings.Count;

		public ICollection Keys => settings.Keys;

		public ICollection Values => settings.Values;

		public object this[string key]
		{
			get => settings[key];
			set => settings[key] = value;
		}

		public void Add(string key, object value)
		{
			settings.Add(key, value);
		}

		public void Clear()
		{
			settings.Clear();
		}

		public bool Contains(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			return settings.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return settings.Remove(key);
		}

		/// <summary>
		/// Attempts to retrieve the setting
		/// with the specified unique key.
		/// </summary>
		/// <typeparam name="TSetting">
		/// The type of the setting.</typeparam>
		/// <param name="key">
		/// The unique key of the setting.</param>
		/// <param name="value">
		/// The resulting value if located.</param>
		/// <returns><c>true</c> if the setting is retrieved;
		/// <c>false</c> otherwise.</returns>
		/// <exception cref="InvalidCastException">
		/// Thrown if the <c>TSetting</c> does not
		/// correspond to the actual setting value.</exception>
		public bool TryGetValue<TSetting>(string key, out TSetting value)
		{
			object settingsValue;
			if (!settings.TryGetValue(key, out settingsValue))
			{
				value = default(TSetting);
				return false;
			}
			value = (TSetting)settingsValue;
			return true;
		}

		int ICollection<KeyValuePair<string, object>>.Count => settings.Count;

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			settings.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<string, object>>.Clear()
		{
			settings.Clear();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return settings.ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)settings).CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return settings.Remove(item.Key);
		}

		ICollection<string> IDictionary<string, object>.Keys => settings.Keys;

		ICollection<object> IDictionary<string, object>.Values => settings.Values;

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return settings.ContainsKey(key);
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			return settings.TryGetValue(key, out value);
		}

		string ExtractKey(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}
			return key as string;
		}

		void IDictionary.Add(object key, object value)
		{
			string extractedKey = ExtractKey(key);
			if (extractedKey == null)
			{
				throw new ArgumentException(nameof(key));
			}

			settings.Add(extractedKey, value);
		}

		void IDictionary.Clear()
		{
			settings.Clear();
		}

		bool IDictionary.Contains(object key)
		{
			string skey = ExtractKey(key);
			if (skey == null)
			{
				return false;
			}
			return settings.ContainsKey(skey);
		}

		object IDictionary.this[object key]
		{
			get
			{
				string extractedKey = ExtractKey(key);
				return (extractedKey == null) ? null : settings[extractedKey];
			}
			set
			{
				string extractedKey = ExtractKey(key);
				if (extractedKey == null)
				{
					throw new ArgumentException(nameof(key));
				}
				settings[extractedKey] = value;
			}
		}

		bool IDictionary.IsFixedSize => false;

		bool IDictionary.IsReadOnly => false;

		void IDictionary.Remove(object key)
		{
			string extractedKey = ExtractKey(key);
			if (extractedKey != null)
			{
				settings.Remove(extractedKey);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)settings).CopyTo(array, index);
		}

		bool ICollection.IsSynchronized => ((ICollection)settings).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection)settings).SyncRoot;

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return settings.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return settings.GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return settings.GetEnumerator();
		}
	}
}
