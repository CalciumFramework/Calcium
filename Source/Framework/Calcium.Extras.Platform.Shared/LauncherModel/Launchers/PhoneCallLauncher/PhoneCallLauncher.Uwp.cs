/* This class is commented out because it requires 
 * Windows Mobile Extentions for UWP. 
 * The PhoneCall capability must also be declared in the app's manifest. */

//#if WINDOWS_UWP
//
//using System;
//using System.Threading.Tasks;
//
//namespace Calcium.LauncherModel.Launchers
//{
//    public class PhoneCallLauncher : IPhoneCallLauncher
//    {
//	    public async Task<bool> ShowAsync()
//	    {
//			PhoneCallStore store = await PhoneCallManager.RequestStoreAsync();
//			Guid lineGuid = await store.GetDefaultLineAsync();
//
//			var phoneLine = await PhoneLine.FromIdAsync(lineGuid);
//			phoneLine.Dial(PhoneNumber, DisplayName);
//		}
//
//	    public string DisplayName { get; set; }
//	    public string PhoneNumber { get; set; }
//    }
//}
//#endif
