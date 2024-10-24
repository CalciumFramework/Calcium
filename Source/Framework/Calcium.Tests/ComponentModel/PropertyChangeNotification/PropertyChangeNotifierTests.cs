#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:09Z</CreationDate>
</File>
*/

#endregion

using Calcium.Concurrency;

using FluentAssertions;

// ReSharper disable InconsistentNaming

namespace Calcium.ComponentModel.PropertyChangeNotification
{
	public class PropertyChangeNotifierTests
	{
		[Fact]
		public void ShouldRaisePropertyChangedUsingPropertyName()
		{
			MockObject1 m1 = new();

			string? propertyName = null;

			PropertyChangeNotifier notifier
				= new(m1)
				{
					SynchronizationContext = new SynchronizationContextForTests()
				};
			notifier.PropertyChanged += (_, args) => { propertyName = args.PropertyName; };

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);

			propertyName.Should().Be(nameof(MockObject1.Text1));
			m1.Text1.Should().Be(p1);
		}

		//[Fact]
		//public async Task ShouldRaisePropertyChangedUsingLambda()
		//{
		//    var m1 = new MockObject1();
		//
		//    string propertyName = null;
		//
		//    var notifier = new PropertyChangeNotifier(m1);
		//    notifier.SynchronizationContext = new SynchronizationContextForTests();
		//    notifier.PropertyChanged += (sender, args) =>
		//    {
		//        propertyName = args.PropertyName;
		//    };
		//
		//    const string p1 = "p1";
		//    notifier.Set(() => m1.Text1, ref m1.text1Field, p1);
		//
		//    propertyName.Should().Be(nameof(MockObject1.Text1));
		//    m1.Text1.Should().Be(p1);
		//}

		[Fact]
		public void ShouldRaisePropertyChangingUsingPropertyName()
		{
			MockObject1 m1 = new();

			string? propertyName = null;

			PropertyChangeNotifier notifier
				= new(m1)
				{
					SynchronizationContext = new SynchronizationContextForTests()
				};
			notifier.PropertyChanging += (_, args) => { propertyName = args.PropertyName; };

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);

			propertyName.Should().Be(nameof(MockObject1.Text1));
			m1.Text1.Should().Be(p1);
		}

		[Fact]
		public void ShouldCancelPropertyChanged()
		{
			MockObject1 m1 = new();
			const string initialValue = "InitialValue";
			m1.Text1 = initialValue;

			string? propertyName = null;

			PropertyChangeNotifier notifier
				= new(m1)
				{
					SynchronizationContext = new SynchronizationContextForTests()
				};

			notifier.PropertyChanging += (_, args) =>
										 {
											 propertyName = args.PropertyName;
											 var cancelArgs = (PropertyChangingEventArgs<string>)args;
											 cancelArgs.Cancel();
										 };

			bool changedEventRaised = false;

			notifier.PropertyChanged += (_, args) =>
										{
											if (args.PropertyName == nameof(MockObject1.Text1))
											{
												changedEventRaised = true;
											}
										};

			const string p1 = "p1";
			notifier.Set(nameof(MockObject1.Text1), ref m1.text1Field, p1);

			propertyName.Should().Be(nameof(MockObject1.Text1));
			m1.Text1.Should().Be(initialValue);
			changedEventRaised.Should().BeFalse(
				"Changed event should not be raised when the change was cancelled.");
		}

		class MockObject1
		{
			public string? text1Field;

			public string? Text1
			{
				get => text1Field;
				set => text1Field = value;
			}
		}
	}
}