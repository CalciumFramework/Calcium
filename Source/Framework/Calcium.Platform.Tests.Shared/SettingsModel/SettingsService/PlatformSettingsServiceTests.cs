using System.Threading.Tasks;

#if __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Calcium.SettingsModel
{
	[TestClass]
	public class
#if WINDOWS_UWP
		PlatformSettingsServiceUwpTests
#elif WPF
		PlatformSettingsServiceWpfTests
#elif __ANDROID__
		PlatformSettingsServiceAndroidTests
#else
		PlatformSettingsServiceUnknownTests
#endif
	{

		[TestMethod]
		public async Task SaveAndRetrieveInt()
		{
			var settingService = new PlatformSettingsService();
			await settingService.ClearSettings();

			const string setting1Key = "Setting1";
			SetSettingResult setResult = settingService.SetSetting(setting1Key, 1);

			Assert.AreEqual(SetSettingResult.Successful, setResult, "Set result should be successful");

			var retrievedValue = settingService.GetSetting(setting1Key, 0);
			Assert.AreEqual(1, retrievedValue, "Retrieved value doesn't match saved value.");
		}

		[TestMethod]
		public async Task SaveAndRetrieveObject()
		{
			var settingService = new PlatformSettingsService();
			await settingService.ClearSettings();

			const string property1Value = "Foo";
			var savableObject = new SavableObject {Property1 = property1Value };

			const string setting1Key = "SavableObject1";
			SetSettingResult setResult = settingService.SetSetting(setting1Key, savableObject);

			Assert.AreEqual(SetSettingResult.Successful, setResult, 
				"Set result should be successful");

			var retrievedValue = settingService.GetSetting<SavableObject>(setting1Key, null);
			Assert.IsNotNull(retrievedValue);

			Assert.AreEqual(property1Value, retrievedValue.Property1, 
				"Retrieved value doesn't match saved value.");
		}
	}

	public class SavableObject
	{
		public string Property1 { get; set; }
	}
}
