using System.Collections.ObjectModel;
using System.Collections.Specialized;

using FluentAssertions;
using Xunit;

namespace Calcium.Collections
{
	public class AdaptiveCollectionTests
	{
		[Fact]
		public void AdaptiveCollection_ShouldAdaptItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
		}

		#region Changes to ObservableCollection affect AdaptiveCollection
		[Fact]
		public void AdaptiveCollection_ShouldAddOuterItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			observableCollection.Add(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
			eventArgs.Should().NotBeNull();
			eventArgs.Action.Should().Be(NotifyCollectionChangedAction.Add);
			adaptiveCollection.LastOrDefault().Should().Be(eventArgs.NewItems[0]);
		}

		[Fact]
		public void AdaptiveCollection_ShouldRemoveOuterItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			void PerformRemoveTest(int removeIndex)
			{
				int length = observableCollection.Count;

				var attachedItemToRemove = adaptiveCollection[removeIndex];
				observableCollection.RemoveAt(removeIndex);

				observableCollection.Count.Should().Be(length - 1);
				adaptiveCollection.Count.Should().Be(length - 1);

				AssertEqualStrings(observableCollection, adaptiveCollection);

				eventArgs.Should().NotBeNull();
				eventArgs.Action.Should().Be(NotifyCollectionChangedAction.Remove);
				eventArgs.OldItems[0].Should().Be(attachedItemToRemove);
				//Assert.IsNotNull(eventArgs);
				//Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
				//Assert.AreEqual(eventArgs.OldItems[0], attachedItemToRemove);
			}

			PerformRemoveTest(0);
			PerformRemoveTest(observableCollection.Count - 1);
			PerformRemoveTest(observableCollection.Count / 2);
		}

		[Fact]
		public void AdaptiveCollection_ShouldResetOuter()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			adaptiveCollection.Count.Should().Be(itemCount);

			observableCollection.Clear();

			adaptiveCollection.Count.Should().Be(0);
		}

		[Fact]
		public void AdaptiveCollection_ShouldMoveOuterItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			adaptiveCollection.Count.Should().Be(observableCollection.Count);

			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			int length = observableCollection.Count;

			void PerformMoveTest(int moveFrom, int moveTo)
			{
				eventArgs.Should().BeNull();

				AttachableStringMock<int> movedItem = adaptiveCollection[moveFrom];

				observableCollection.Move(moveFrom, moveTo);
				AssertEqualStrings(observableCollection, adaptiveCollection);

				eventArgs.Should().NotBeNull();
				movedItem.Value.Should().Be(observableCollection[moveTo].ToString());

				eventArgs = null;
			}

			/* Forwards */
			for (int i = 0; i < length; i++)
			{
				PerformMoveTest(0, i);
			}

			/* Backwards */
			for (int i = length - 1; i >= 0; i--)
			{
				PerformMoveTest(i, 0);
			}

			/* From the middle */
			for (int i = 0; i < length; i++)
			{
				PerformMoveTest(length / 2, i);
			}
		}
		#endregion

		#region Changes to AdaptiveCollection affect ObservableCollection

		[Fact]
		public void AdaptiveCollection_ShouldAddInnerItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			observableCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			var newItem = new AttachableStringMock<int>();
			newItem.AttachObject(itemCount + 1);
			adaptiveCollection.Add(newItem);

			AssertEqualStrings(observableCollection, adaptiveCollection);
			eventArgs.Should().NotBeNull();
			eventArgs.Action.Should().Be(NotifyCollectionChangedAction.Add);
			eventArgs.NewItems[0].Should().Be(adaptiveCollection.LastOrDefault().GetObject());
		}

		[Fact]
		public void AdaptiveCollection_ShouldRemoveInnerItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			observableCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			void PerformRemoveTest(int removeIndex)
			{
				eventArgs.Should().BeNull();

				int length = observableCollection.Count;

				var outerToBeRemoved = observableCollection[removeIndex];
				adaptiveCollection.RemoveAt(removeIndex);

				observableCollection.Count.Should().Be(length - 1);
				adaptiveCollection.Count.Should().Be(length - 1);

				AssertEqualStrings(observableCollection, adaptiveCollection);

				eventArgs.Should().NotBeNull();
				eventArgs.Action.Should().Be(NotifyCollectionChangedAction.Remove);
				eventArgs.OldItems[0].Should().Be(outerToBeRemoved);

				eventArgs = null;
			}

			PerformRemoveTest(0);
			PerformRemoveTest(observableCollection.Count - 1);
			PerformRemoveTest(observableCollection.Count / 2);
		}

		[Fact]
		public void AdaptiveCollection_ShouldResetInner()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			observableCollection.Count.Should().Be(itemCount);
			
			adaptiveCollection.Clear();

			observableCollection.Count.Should().Be(0);
		}

		[Fact]
		public void AdaptiveCollection_ShouldMoveInnerItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			adaptiveCollection.Count.Should().Be(observableCollection.Count);
			
			NotifyCollectionChangedEventArgs eventArgs = null;
			observableCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			int length = observableCollection.Count;

			void PerformMoveTest(int moveFrom, int moveTo)
			{
				eventArgs.Should().BeNull();

				var movedItem = observableCollection[moveFrom];

				adaptiveCollection.Move(moveFrom, moveTo);
				AssertEqualStrings(observableCollection, adaptiveCollection);

				eventArgs.Should().NotBeNull();
				observableCollection[moveTo].Should().Be(movedItem);

				eventArgs = null;
			}

			/* Forwards */
			for (int i = 0; i < length; i++)
			{
				PerformMoveTest(0, i);
			}

			/* Backwards */
			for (int i = length - 1; i >= 0; i--)
			{
				PerformMoveTest(i, 0);
			}

			/* From the middle */
			for (int i = 0; i < length; i++)
			{
				PerformMoveTest(length / 2, i);
			}
		}
		#endregion

		(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) CreateCollection(int length)
		{
			var oc = new ObservableCollection<int>();
			for (int i = 0; i < length; i++)
			{
				oc.Add(i);
			}
			var target = new AdaptiveCollection<AttachableStringMock<int>, int>(oc);
			return (oc, target);
		}

		void AssertEqualStrings(ObservableCollection<int> observableCollection,
			AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection)
		{
			for (int i = 0; i < observableCollection.Count; i++)
			{
				var item = adaptiveCollection[i].Value;
				var expected = observableCollection[i];
				expected.ToString().Should().Be(item);
			}
		}

	}
}
