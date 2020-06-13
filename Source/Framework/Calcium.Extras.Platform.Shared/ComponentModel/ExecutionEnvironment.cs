#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-11-26 11:56:06Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Provides information about the execution environment
	/// of the application.
	/// </summary>
	public partial class ExecutionEnvironment
	{
		static bool? designTime;

		/// <summary>
		/// Indicates if the app code is running in a designer.
		/// </summary>
		public static bool DesignTime
		{
			get
			{
				if (!designTime.HasValue)
				{
#if WINDOWS_UWP
					designTime = global::Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#elif __ANDROID__ || __IOS__
					designTime = false;
#elif WPF
					designTime = (bool)System.ComponentModel.DesignerProperties.IsInDesignModeProperty.GetMetadata(
						typeof(System.Windows.DependencyObject)).DefaultValue;
#else
					designTime = false;
#endif
				}

				return designTime.Value;
			}
		}

		//		public event PropertyChangedEventHandler PropertyChanged;
		//
		//		protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
		//		{
		//			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		//		}
	}
}