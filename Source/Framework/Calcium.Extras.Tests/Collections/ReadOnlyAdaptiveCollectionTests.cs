using System.Collections.ObjectModel;
using System.Collections.Specialized;

using FluentAssertions;
using Xunit;

namespace Calcium.Collections
{
	public class ReadOnlyAdaptiveCollectionTests
	{
		#region Constructor Behavior

		[Fact]
		public void Constructor_ShouldPopulateCollection_WhenPassedNonEmptyReadOnlyObservableCollection()
		{
			// Arrange
			var source = new ObservableCollection<string> { "Item1", "Item2", "Item3" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(source);
			Func<string, int> getItem = item => item.Length;

			// Act
			var collection = new ReadOnlyAdaptiveCollection<int, string>(readOnlySource, getItem);

			// Assert
			collection.Should().HaveCount(source.Count)
				.And.ContainInOrder(source.Select(getItem));
		}

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenPassedNullReadOnlyObservableCollection()
		{
			// Arrange
			ReadOnlyObservableCollection<string>? nullCollection = null;
			Func<string, int> getItem = int.Parse;

			// Act
			var action = () => new ReadOnlyAdaptiveCollection<int, string>(nullCollection!, getItem);

			// Assert
			action.Should().Throw<ArgumentNullException>()
				.WithMessage("Value cannot be null. (Parameter 'monitoredCollection')");
		}

		[Fact]
		public void Constructor_ShouldThrowArgumentNullException_WhenPassedNullGetItemDelegate()
		{
			// Arrange
			var source = new ObservableCollection<string> { "Item1", "Item2", "Item3" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(source);
			Func<string, int>? nullGetItem = null;

			// Act
			var action = () => new ReadOnlyAdaptiveCollection<int, string>(readOnlySource, nullGetItem!);

			// Assert
			action.Should().Throw<ArgumentNullException>()
				.WithMessage("Value cannot be null. (Parameter 'getItem')");
		}

		#endregion

		#region Adaptation Logic

		[Fact]
		public void Constructor_ShouldCallGetItem_ForEachSourceItem()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two", "Three" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var getItemCallCount = 0;
			Func<string, string> getItem = item =>
			{
				getItemCallCount++;
				return item.ToUpperInvariant();
			};

			// Act
			_ = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, getItem);

			// Assert
			getItemCallCount.Should().Be(sourceItems.Count);
		}

		[Fact]
		public void AdaptedCollection_ShouldReflectChanges_InSourceCollection()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			Func<string, string> getItem = item => item.ToUpperInvariant();
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, getItem);

			// Act
			sourceItems.Add("Three");

			// Assert
			adaptedCollection.Should().HaveCount(3);
			adaptedCollection.Should().Contain(readOnlySource.Select(getItem));
		}

		[Fact]
		public void AdaptedCollection_ShouldReflectItemChanges_WhenSourceCollectionItemIsReplaced()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			Func<string, string> getItem = item => item.ToUpperInvariant();
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, getItem);

			// Act
			sourceItems[1] = "Three";

			// Assert
			adaptedCollection.Should().Contain(readOnlySource.Select(getItem));
		}

		#endregion

		#region Collection Changed Events

		[Fact]
		public void CollectionChanged_ShouldBeRaised_WithAddAction_WhenItemsAreAdded()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string>();
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<NotifyCollectionChangedEventArgs>();

			adaptedCollection.CollectionChanged += (_, args) => events.Add(args);

			// Act
			sourceItems.Add("New");

			// Assert
			events.Should().ContainSingle();
			events[0].Action.Should().Be(NotifyCollectionChangedAction.Add);
		}

		[Fact]
		public void CollectionChanged_ShouldBeRaised_WithRemoveAction_WhenItemsAreRemoved()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<NotifyCollectionChangedEventArgs>();

			adaptedCollection.CollectionChanged += (_, args) => events.Add(args);

			// Act
			sourceItems.Remove("One");

			// Assert
			events.Should().ContainSingle();
			events[0].Action.Should().Be(NotifyCollectionChangedAction.Remove);
		}

		[Fact]
		public void CollectionChanged_ShouldBeRaised_WithReplaceAction_WhenItemsAreReplaced()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<NotifyCollectionChangedEventArgs>();

			adaptedCollection.CollectionChanged += (_, args) => events.Add(args);

			// Act
			sourceItems[0] = "Two";

			// Assert
			events.Should().ContainSingle();
			events[0].Action.Should().Be(NotifyCollectionChangedAction.Replace);
		}

		[Fact]
		public void CollectionChanged_ShouldBeRaised_WithMoveAction_WhenItemsAreMoved()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<NotifyCollectionChangedEventArgs>();

			adaptedCollection.CollectionChanged += (_, args) => events.Add(args);

			// Act
			sourceItems.Move(0, 1);

			// Assert
			events.Should().ContainSingle();
			events[0].Action.Should().Be(NotifyCollectionChangedAction.Move);
		}

		[Fact]
		public void CollectionChanged_ShouldBeRaised_WithResetAction_WhenCollectionIsReset()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<NotifyCollectionChangedEventArgs>();

			adaptedCollection.CollectionChanged += (_, args) => events.Add(args);

			// Act
			sourceItems.Clear();

			// Assert
			events.Should().ContainSingle();
			events[0].Action.Should().Be(NotifyCollectionChangedAction.Reset);
		}

		#endregion

		#region INotifyPropertyChanged Implementation

		[Fact]
		public void PropertyChanged_ShouldBeRaised_ForCount_WhenItemsAreAdded()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string>();
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<string?>();

			adaptedCollection.PropertyChanged += (_, args) => events.Add(args.PropertyName);

			// Act
			sourceItems.Add("New");

			// Assert
			events.Should().ContainSingle(nameof(adaptedCollection.Count));
		}

		[Fact]
		public void PropertyChanged_ShouldBeRaised_ForCount_WhenItemsAreRemoved()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<string?>();

			adaptedCollection.PropertyChanged += (_, args) => events.Add(args.PropertyName);

			// Act
			sourceItems.Remove("One");

			// Assert
			events.Should().ContainSingle(nameof(adaptedCollection.Count));
		}

		[Fact]
		public void PropertyChanged_ShouldNotBeRaised_ForOtherProperties_WhenItemsAreAdded()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string>();
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<string?>();

			adaptedCollection.PropertyChanged += (_, args) => events.Add(args.PropertyName);

			// Act
			sourceItems.Add("New");

			// Assert
			events.Should().NotContain(e => e != nameof(adaptedCollection.Count));
		}

		[Fact]
		public void PropertyChanged_ShouldNotBeRaised_ForOtherProperties_WhenItemsAreRemoved()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			var events = new List<string?>();

			adaptedCollection.PropertyChanged += (_, args) => events.Add(args.PropertyName);

			// Act
			sourceItems.Remove("One");

			// Assert
			events.Should().NotContain(e => e != nameof(adaptedCollection.Count));
		}

		#endregion

		#region Enumeration and Access

		[Fact]
		public void GetEnumerator_ShouldIterateOverAllItems_InCorrectOrder()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two", "Three" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());

			// Act
			var enumeratedItems = adaptedCollection.ToList();

			// Assert
			enumeratedItems.Should().ContainInOrder(sourceItems.Select(item => item.ToUpperInvariant()));
		}

		[Fact]
		public void Indexer_ShouldRetrieveCorrectItem_ByIndex()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two", "Three" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());

			// Act & Assert
			for (int i = 0; i < sourceItems.Count; i++)
			{
				var expectedItem = sourceItems[i].ToUpperInvariant();
				adaptedCollection[i].Should().Be(expectedItem);
			}
		}

		[Fact]
		public void Indexer_ShouldThrowArgumentOutOfRangeException_ForInvalidIndex()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two", "Three" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());

			// Act
			Action action = () => { string unused = adaptedCollection[-1]; };

			// Assert
			action.Should().Throw<ArgumentOutOfRangeException>();

			// Act
			action = () => { string unused = adaptedCollection[sourceItems.Count]; };

			// Assert
			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		#endregion

		#region Disposal Pattern

		[Fact]
		public void Dispose_ShouldUnsubscribeFromCollectionChangedEvent_OfSourceCollection()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			bool eventRaised = false;

			adaptedCollection.CollectionChanged += (_, _) => eventRaised = true;

			// Act
			adaptedCollection.Dispose();
			sourceItems.Add("Three");

			// Assert
			eventRaised.Should().BeFalse();
		}

		[Fact]
		public void Enumerate_ShouldBeSafe_AfterDisposal()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string> { "One", "Two" };
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());

			// Act
			adaptedCollection.Dispose();
			Action action = () => {
				foreach (var _ in adaptedCollection)
				{
					// Enumeration is performed purely to verify that it doesn't
					// throw exceptions after disposal.
				}
			};

			// Assert
			action.Should().NotThrow();
		}

		#endregion

		#region Thread Safety

		[Fact]
		public async Task ConcurrentReads_ShouldNotCauseCollectionToBeInInconsistentState()
		{
			// Arrange
			var sourceItems = new ObservableCollection<string>();
			for (int i = 0; i < 100; i++)
			{
				sourceItems.Add($"Item{i}");
			}
			var readOnlySource = new ReadOnlyObservableCollection<string>(sourceItems);
			var adaptedCollection = new ReadOnlyAdaptiveCollection<string, string>(readOnlySource, item => item.ToUpperInvariant());
			int numberOfReads = 1000;
			var tasks = new List<Task>();

			// Act
			for (int i = 0; i < numberOfReads; i++)
			{
				tasks.Add(Task.Run(() =>
				{
					// Perform some read operations on the adaptedCollection
					_ = adaptedCollection.Count;
					_ = adaptedCollection[0];
					_ = adaptedCollection[adaptedCollection.Count - 1];
				}));
			}

			await Task.WhenAll(tasks);

			// Assert
			// No assertion is necessary here, we're just ensuring that concurrent reads do not throw exceptions
		}

		#endregion


	}
}
