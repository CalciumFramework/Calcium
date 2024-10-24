#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:15Z</CreationDate>
</File>
*/

#endregion

using Calcium.Concurrency;

using FluentAssertions;

namespace Calcium.ComponentModel
{
	public class ObservableBaseTests
	{
		[Fact]
		public void ShouldRaisePropertyChangedUsingPropertyName()
		{
			SimpleObservable m1 = new();

			string? propertyName = null;

			m1.PropertyChanged += (_, args) => { propertyName = args.PropertyName; };

			const string p1 = "p1";
			m1.Text1 = p1;

			propertyName.Should().Be(nameof(SimpleObservable.Text1));
			m1.Text1.Should().Be(p1);
		}

		class SimpleObservable : ObservableBase
		{
			public SimpleObservable()
			{
				PropertyChangeNotifier.SynchronizationContext = new SynchronizationContextForTests();
			}

			string? text1;

			public string? Text1
			{
				get => text1;
				set => Set(ref text1, value);
			}
		}
	}
}