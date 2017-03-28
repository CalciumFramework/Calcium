#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:09Z</CreationDate>
</File>
*/
#endregion

using System.Threading.Tasks;
using Codon.Concurrency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.ComponentModel.PropertyChangeNotification
{
	[TestClass]
	public class PropertyChangeNotifierTests
	{
		[TestMethod]
		public void ShouldRaisePropertyChangedUsingPropertyName()
		{
			var m1 = new MockObject1();

			string propertyName = null;

			var notifier = new PropertyChangeNotifier(m1)
			{
				SynchronizationContext = new SynchronizationContextForTests()
			};
			notifier.PropertyChanged += (sender, args) =>
			{
				propertyName = args.PropertyName;
			};

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);
			
			Assert.AreEqual(nameof(MockObject1.Text1), propertyName);
			Assert.AreEqual(p1, m1.Text1);
		}

		//[TestMethod]
		//public async Task ShouldRaisePropertyChangedUsingLambda()
		//{
		//	var m1 = new MockObject1();
		//
		//	string propertyName = null;
		//
		//	var notifier = new PropertyChangeNotifier(m1);
		//	notifier.SynchronizationContext = new SynchronizationContextForTests();
		//	notifier.PropertyChanged += (sender, args) =>
		//	{
		//		propertyName = args.PropertyName;
		//	};
		//
		//	const string p1 = "p1";
		//	notifier.Set(() => m1.Text1, ref m1.text1Field, p1);
		//
		//	Assert.AreEqual(nameof(MockObject1.Text1), propertyName);
		//	Assert.AreEqual(p1, m1.Text1);
		//}

		[TestMethod]
		public void ShouldRaisePropertyChangingUsingPropertyName()
		{
			var m1 = new MockObject1();

			string propertyName = null;

			var notifier = new PropertyChangeNotifier(m1)
			{
				SynchronizationContext = new SynchronizationContextForTests()
			};
			notifier.PropertyChanging += (sender, args) =>
			{
				propertyName = args.PropertyName;
			};

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);

			Assert.AreEqual(nameof(MockObject1.Text1), propertyName);
			Assert.AreEqual(p1, m1.Text1);	
		}

		[TestMethod]
		public void ShouldCancelPropertyChanged()
		{
			var m1 = new MockObject1();
			const string initialValue = "InitialValue";
			m1.Text1 = initialValue;

			string propertyName = null;

			var notifier = new PropertyChangeNotifier(m1)
			{
				SynchronizationContext = new SynchronizationContextForTests()
			};

			notifier.PropertyChanging += (sender, args) =>
			{
				propertyName = args.PropertyName;
				var cancelArgs = (PropertyChangingEventArgs<string>)args;
				cancelArgs.Cancel();
			};

			bool changedEventRaised = false;

			notifier.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == nameof(MockObject1.Text1))
				{
					changedEventRaised = true;
				}
			};

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);

			Assert.AreEqual(nameof(MockObject1.Text1), propertyName);
			Assert.AreEqual(initialValue, m1.Text1);
			Assert.IsFalse(changedEventRaised, 
				"Changed event should not be raised when change was cancelled.");
		}

		class MockObject1
		{
			public string text1Field;

			public string Text1
			{
				get => text1Field;
				set => text1Field = value;
			}
		}
	}
}
