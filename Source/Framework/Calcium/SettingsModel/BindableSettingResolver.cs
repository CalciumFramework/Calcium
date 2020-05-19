#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2011-11-23 17:29:27Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;

using Codon.ComponentModel;
using Codon.Messaging;
using Codon.Services;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class allows you to bind indirectly 
	/// to the <see cref="ISettingsService"/> implementation.
	/// This class raises a property changed event when
	/// a setting changes.
	/// </summary>
	public class BindableSettingResolver : ObservableBase, 
		IMessageSubscriber<SettingChangeEventArgs>
	{
		ISettingsService settingsService;

		/// <summary>
		/// Use an indexer to resolve a property value.
		/// </summary>
		/// <param name="key">
		/// The property key.</param>
		/// <returns>
		/// The value of the property with the specified key.
		/// </returns>
		public object this[string key]
		{
			get
			{
				if (settingsService == null)
				{
					settingsService = Dependency.Resolve<ISettingsService>();
					var messageBus = Dependency.Resolve<IMessenger>();
					messageBus.Subscribe(this);
				}

				return settingsService.GetSetting<object>(key, null);
			}
		}

		public Task ReceiveMessageAsync(SettingChangeEventArgs message)
		{
			OnPropertyChanged(string.Empty);

			return Task.CompletedTask;
		}
	}
}
