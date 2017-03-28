#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2012-02-18 13:24:46Z</CreationDate>
</File>
*/
#endregion

using Codon.Messaging;
using Codon.Services;

namespace Codon.SettingsModel
{
	/// <summary>
	/// This class monitors the <see cref="ISettingsService"/>
	/// implementation for setting changes. When a change occurs,
	/// it broadcasts a message using the <see cref="IMessenger"/>.
	/// </summary>
	public class SettingsEventBroadcaster
	{
		public SettingsEventBroadcaster(ISettingsService settingsService)
		{
			AssertArg.IsNotNull(
				settingsService, nameof(settingsService));

			settingsService.SettingChanging += HandleSettingChanging;
			settingsService.SettingChanged += HandleSettingChanged;
		}

		void HandleSettingChanging(object sender, SettingChangingEventArgs e)
		{
			var messageBus = Dependency.Resolve<IMessenger, Messenger>();
			messageBus.PublishAsync(e);
		}

		void HandleSettingChanged(object sender, SettingChangeEventArgs e)
		{
			var messageBus = Dependency.Resolve<IMessenger, Messenger>();
			messageBus.PublishAsync(e);
		}
	}
}
