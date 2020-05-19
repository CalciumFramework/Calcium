#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-11 23:56:01Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;

using Codon.ComponentModel;
using Codon.IO;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This is a WPF specific IsolatedStorageSettings implementation.
	/// It's required because apps not deployed using click-once 
	/// are unable to use <c>IsolatedStorageFile.GetUserStoreForApplication()</c>,
	/// and should use <c>IsolatedStorageFile.GetUserStoreForAssembly()</c> instead.
	/// </summary>
	sealed class IsolatedStorageSettingsWpf : 
		IDictionary<string, object>, IDictionary,
		ICollection<KeyValuePair<string, object>>, ICollection,
		IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		static IsolatedStorageSettingsWpf applicationSettings;
		static IsolatedStorageSettingsWpf siteSettings;

		IsolatedStorageFile storageFile;
		Dictionary<string, object> settings;

		const string LocalSettings = "__LocalSettings";

		internal IsolatedStorageSettingsWpf(
			IsolatedStorageFile storageFile)
		{
			this.storageFile = storageFile;

			if (!storageFile.FileExists(LocalSettings))
			{
				settings = new Dictionary<string, object>();
				return;
			}

			var serializer = Dependency.Resolve<IBinarySerializer>();

			using (IsolatedStorageFileStream stream 
						= storageFile.OpenFile(LocalSettings, FileMode.Open))
			{
				try
				{
					settings = (Dictionary<string, object>)serializer.Deserialize(stream);
				}
				catch (Exception ex)
				{
					//if (deleteOnError)
					//{
					//	storageFile.DeleteFile(LocalSettings);
					//}
					var handler = Dependency.Resolve<IExceptionHandler>();
					if (handler.ShouldRethrowException(ex, this))
					{
						throw;
					}

					settings = new Dictionary<string, object>();
				}
			}
		}

		public void Save()
		{
			var serializer = Dependency.Resolve<IBinarySerializer>();

			using (IsolatedStorageFileStream stream 
					= storageFile.CreateFile(LocalSettings))
			{
				serializer.Serialize(settings, stream);
			}
		}

		~IsolatedStorageSettingsWpf()
		{
			try
			{
				/* Settings are automatically saved 
				 * if the application close normally. */
				Save();
			}
			catch (ObjectDisposedException ex)
			{
				try
				{
					Console.WriteLine("Exception thrown in IsolatedStorageSettingsWpf during Save." + ex);
				}
				catch (Exception)
				{
					/* WPF in .NET Standard may raise exception here. */
				}
			}
		}

		public static IsolatedStorageSettingsWpf ApplicationSettings
		{
			get
			{
				if (applicationSettings == null)
				{
					var isolatedStorageFile = GetStorageFile();
                    applicationSettings = new IsolatedStorageSettingsWpf(isolatedStorageFile);
				}
				return applicationSettings;
			}
		}

		public static IsolatedStorageSettingsWpf SiteSettings
		{
			get
			{
				if (siteSettings == null)
				{
					siteSettings = new IsolatedStorageSettingsWpf(
						(System.Threading.Thread.GetDomain().ActivationContext != null) ?
							IsolatedStorageFile.GetUserStoreForApplication() : // For WPF, apps deployed via ClickOnce will have a non-null ActivationContext
							IsolatedStorageFile.GetUserStoreForAssembly());
					// IsolatedStorageFile.GetUserStoreForSite() works only for Silverlight applications
				}
				return siteSettings;
			}
		}

		internal static IsolatedStorageFile GetStorageFile()
		{
			var result = (System.Threading.Thread.GetDomain().ActivationContext != null) ?
							IsolatedStorageFile.GetUserStoreForApplication() : //for WPF, apps deployed via ClickOnce will have a non-null ActivationContext
							IsolatedStorageFile.GetUserStoreForAssembly();

			return result;
		}

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

		public bool TryGetValue<T>(string key, out T value)
		{
			object v;
			if (!settings.TryGetValue(key, out v))
			{
				value = default(T);
				return false;
			}

			value = (T)v;

			return true;
		}

		#region Explicit interface implementations

		int ICollection<KeyValuePair<string, object>>.Count 
				=> settings.Count;

		bool ICollection<KeyValuePair<string, object>>.IsReadOnly 
				=> false;

		void ICollection<KeyValuePair<string, object>>.Add(
				KeyValuePair<string, object> item)
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
		
		ICollection<string> IDictionary<string, object>.Keys 
			=> settings.Keys;

		ICollection<object> IDictionary<string, object>.Values 
			=> settings.Values;

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
			string s = ExtractKey(key);
			if (s == null)
			{
				throw new ArgumentException(nameof(key));
			}

			settings.Add(s, value);
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
				string s = ExtractKey(key);
				return s == null ? null : settings[s];
			}
			set
			{
				string s = ExtractKey(key);
				if (s == null)
				{
					throw new ArgumentException(nameof(key));
				}
				settings[s] = value;
			}
		}

		bool IDictionary.IsFixedSize => false;

		bool IDictionary.IsReadOnly => false;

		void IDictionary.Remove(object key)
		{
			string s = ExtractKey(key);
			if (s != null)
			{
				settings.Remove(s);
			}
		}
		
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)settings).CopyTo(array, index);
		}

		bool ICollection.IsSynchronized 
			=> ((ICollection)settings).IsSynchronized;

		object ICollection.SyncRoot 
			=> ((ICollection)settings).SyncRoot;


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

		#endregion
	}
}
#endif
	  