using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calcium.Collections
{
	[TestClass]
	public class ReadOnlyAdaptiveCollectionTests
	{
		[TestMethod]
		public void ReadOnlyAdaptiveCollection_ShouldAdaptItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
		}

		[TestMethod]
		public void ReadOnlyAdaptiveCollection_ShouldAddItem()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);
			
			NotifyCollectionChangedEventArgs eventArgs = null;
			adaptiveCollection.CollectionChanged += (sender, e) => { eventArgs = e; };

			observableCollection.Add(itemCount);

			AssertEqualStrings(observableCollection, adaptiveCollection);
			Assert.IsNotNull(eventArgs);
			Assert.AreEqual(NotifyCollectionChangedAction.Add, eventArgs.Action);
			Assert.AreEqual(eventArgs.NewItems[0], adaptiveCollection.LastOrDefault());
		}

		[TestMethod]
		public void ReadOnlyAdaptiveCollection_ShouldRemoveItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

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
		public void ReadOnlyAdaptiveCollection_ShouldReset()
		{
			const int itemCount = 3;
			(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

			Assert.AreEqual(observableCollection.Count, adaptiveCollection.Count);

			observableCollection.Clear();

			Assert.AreEqual(observableCollection.Count, adaptiveCollection.Count);
		}

		[TestMethod]
		public void ReadOnlyAdaptiveCollection_ShouldMoveItem()
		{
			const int itemCount = 10;
			(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) = CreateCollection(itemCount);

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
				PerformMoveTest(0,i);
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

		(ObservableCollection<int> observableCollection, ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection) CreateCollection(int length)
		{
			var oc = new ObservableCollection<int>();
			for (int i = 0; i < length; i++)
			{
				oc.Add(i);
			}
			var roc = new ReadOnlyObservableCollection<int>(oc);
			var target = new ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int>(roc);
			return (oc, target);
		}

		void AssertEqualStrings(ObservableCollection<int> observableCollection,
			ReadOnlyAdaptiveCollection<AttachableStringMock<int>, int> adaptiveCollection)
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
