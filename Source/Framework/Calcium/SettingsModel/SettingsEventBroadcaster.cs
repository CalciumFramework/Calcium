#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 13:24:46Z</CreationDate>
</File>
*/
#endregion

using Calcium.Messaging;
using Calcium.Services;

namespace Calcium.SettingsModel
{
	/// <summary>
	/// This class monitors the <see cref="ISettingsService"/>
	/// implementation for setting changes. When a change occurs,
	/// it broadcasts a message using the <see cref="IMessenger"/>.
	/// </summary>
	public class SettingsEventBroadcaster
	{
		IMessenger messenger_UseProperty;

		/// <summary>
		/// This property allows you to explicitly set 
		/// the <see cref="IMessenger"/> instance, which is used to broadcast events.
		/// By default this value is set once to the <c>IMessenger</c> 
		/// located in the IoC container.
		/// </summary>
		public IMessenger Messenger
		{
			private get => messenger_UseProperty ?? (messenger_UseProperty = Dependency.Resolve<IMessenger, Messenger>());
			set => messenger_UseProperty = value;
		}

		public SettingsEventBroadcaster(ISettingsService settingsService)
		{
			AssertArg.IsNotNull(
				settingsService, nameof(settingsService));

			settingsService.SettingChanging += HandleSettingChanging;
			settingsService.SettingChanged += HandleSettingChanged;
			settingsService.SettingRemoved += HandleSettingRemoved;
		}

		void HandleSettingChanging(object sender, SettingChangingEventArgs e)
		{
			Messenger.PublishAsync(e);
		}

		void HandleSettingChanged(object sender, SettingChangeEventArgs e)
		{
			Messenger.PublishAsync(e);
		}

		void HandleSettingRemoved(object sender, SettingRemovedEventArgs e)
		{
			Messenger.PublishAsync(e);
		}
	}
}
