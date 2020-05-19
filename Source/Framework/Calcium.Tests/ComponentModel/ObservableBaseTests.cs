#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:15Z</CreationDate>
</File>
*/
#endregion

using Calcium.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.ComponentModel
{
	[TestClass]
	public class ObservableBaseTests
	{
		[TestMethod]
		public void ShouldRaisePropertyChangedUsingPropertyName()
		{
			var m1 = new SimpleObservable();

			string propertyName = null;

			m1.PropertyChanged += (sender, args) =>
			{
				propertyName = args.PropertyName;
			};

			const string p1 = "p1";
			m1.Text1 = p1;
			
			Assert.AreEqual(nameof(SimpleObservable.Text1), propertyName);
			Assert.AreEqual(p1, m1.Text1);
		}

		class SimpleObservable : ObservableBase
		{
			public SimpleObservable()
			{
				PropertyChangeNotifier.SynchronizationContext = new SynchronizationContextForTests();
			}

			string text1;

			public string Text1
			{
				get => text1;
				set => Set(ref text1, value);
			}
		}
	}
}
