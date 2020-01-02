using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.Collections
{
	[TestClass]
	public class AdaptiveCollectionTests
	{
		[TestMethod]
		public void AdaptiveCollection_ShouldAdaptItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
		}

		#region Changes to ObservableCollection affect AdaptiveCollection
		[TestMethod]
		public void AdaptiveCollection_ShouldAddOuterItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			observableCollection.Add(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
			Assert.IsNotNull(eventArgs);
			Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
			Assert.AreEqual(eventArgs.NewItems[0], adaptiveCollection.LastOrDefault());
		}

		[TestMethod]
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

				Assert.AreEqual(length - 1, observableCollection.Count);
				Assert.AreEqual(length - 1, adaptiveCollection.Count);

				AssertEqualStrings(observableCollection, adaptiveCollection);

				Assert.IsNotNull(eventArgs);
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
				Assert.AreEqual(eventArgs.OldItems[0], attachedItemToRemove);
			}

			PerformRemoveTest(0);
			PerformRemoveTest(observableCollection.Count - 1);
			PerformRemoveTest(observableCollection.Count / 2);
		}

		[TestMethod]
		public void AdaptiveCollection_ShouldResetOuter()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			Assert.AreEqual(itemCount, adaptiveCollection.Count);

			observableCollection.Clear();

			Assert.AreEqual(0, adaptiveCollection.Count);
		}

		[TestMethod]
		public void AdaptiveCollection_ShouldMoveOuterItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			Assert.AreEqual(observableCollection.Count, adaptiveCollection.Count);

			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			int length = observableCollection.Count;

			void PerformMoveTest(int moveFrom, int moveTo)
			{
				Assert.IsNull(eventArgs);

				AttachableStringMock<int> movedItem = adaptiveCollection[moveFrom];

				observableCollection.Move(moveFrom, moveTo);
				AssertEqualStrings(observableCollection, adaptiveCollection);

				Assert.IsNotNull(eventArgs);
				Assert.AreEqual(observableCollection[moveTo].ToString(), movedItem.Value, "Moved items not equal.");
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
		[TestMethod]
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
			Assert.IsNotNull(eventArgs);
			Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
			Assert.AreEqual(eventArgs.NewItems[0], adaptiveCollection.LastOrDefault().GetObject());
		}

		[TestMethod]
		public void AdaptiveCollection_ShouldRemoveInnerItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			NotifyCollectionChangedEventArgs eventArgs = null;
			observableCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			void PerformRemoveTest(int removeIndex)
			{
				Assert.IsNull(eventArgs);
				int length = observableCollection.Count;

				var outerToBeRemoved = observableCollection[removeIndex];
				adaptiveCollection.RemoveAt(removeIndex);

				Assert.AreEqual(length - 1, observableCollection.Count);
				Assert.AreEqual(length - 1, adaptiveCollection.Count);

				AssertEqualStrings(observableCollection, adaptiveCollection);

				Assert.IsNotNull(eventArgs);
				Assert.AreEqual(NotifyCollectionChangedAction.Remove, eventArgs.Action);
				Assert.AreEqual(eventArgs.OldItems[0], outerToBeRemoved);
				eventArgs = null;
			}

			PerformRemoveTest(0);
			PerformRemoveTest(observableCollection.Count - 1);
			PerformRemoveTest(observableCollection.Count / 2);
		}

		[TestMethod]
		public void AdaptiveCollection_ShouldResetInner()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			Assert.AreEqual(itemCount, observableCollection.Count);

			adaptiveCollection.Clear();

			Assert.AreEqual(0,observableCollection.Count);
		}

		[TestMethod]
		public void AdaptiveCollection_ShouldMoveInnerItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, AdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			Assert.AreEqual(observableCollection.Count, adaptiveCollection.Count);

			NotifyCollectionChangedEventArgs eventArgs = null;
			observableCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			int length = observableCollection.Count;

			void PerformMoveTest(int moveFrom, int moveTo)
			{
				Assert.IsNull(eventArgs);

				var movedItem = observableCollection[moveFrom];

				adaptiveCollection.Move(moveFrom, moveTo);
				AssertEqualStrings(observableCollection, adaptiveCollection);

				Assert.IsNotNull(eventArgs);
				Assert.AreEqual(observableCollection[moveTo], movedItem, "Moved items not equal.");
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
				Assert.AreEqual(expected.ToString(), item, "Items are not equal.");
			}
		}

	}
}
