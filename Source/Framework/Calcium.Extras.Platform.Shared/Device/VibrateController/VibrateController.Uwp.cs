/* 

This file is not included because it requires an assembly reference.
Right-click References in your Solution, Add Reference->Extensions->Windows Mobile Extensions for the UWP

Copy the class below and register it with the dependency container, like so:
Dependency.Register<IVibrateController, VibrateController>(true);
 */

//#if WINDOWS_UWP
//#region File and License Information
//*
//<File>
//	<License>
//		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
//		This file is part of Codon (http://codonfx.com), 
//		which is released under the MIT License.
//		See file /Documentation/License.txt for details.
//	</License>
//	<CreationDate>2013-03-21 15:38:54Z</CreationDate>
//</File>
//*/
//#endregion
//
//using System;
//using Windows.Phone.Devices.Notification;
//
//namespace Codon.Device
//{
//	public class VibrateController : IVibrateController
//	{
//		public void Start(TimeSpan duration)
//		{
//			var v = VibrationDevice.GetDefault();
//			v.Vibrate(TimeSpan.FromMilliseconds(500));
//		}
//
//		public void Stop()
//		{
//			
//		}
//	}
//}
//#endif