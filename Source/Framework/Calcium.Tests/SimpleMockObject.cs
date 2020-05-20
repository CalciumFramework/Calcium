#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:23:48Z</CreationDate>
</File>
*/
#endregion

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calcium
{
	class SimpleMockObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		string text1;

		public string Text1
		{
			get => text1;
			set
			{
				if (text1 != value)
				{
					text1 = value;
					OnPropertyChanged();
				}
			}
		}
	}
}
