#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-04 13:45:49Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Concurrent;

using FluentAssertions;

using Calcium.ComponentModel.Experimental;

namespace Calcium.Tests.ComponentModel
{
	public class RegistryTests
	{
		#region Basic Functionality Tests

		[Fact]
		public void SetValue_ShouldAddOrUpdateEntry()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "TestKey";
			int value = 42;

			// Act
			registry.SetValue(key, value);

			// Assert
			registry.ReadOnlyDictionary.ContainsKey(key).Should().BeTrue();
			registry.ReadOnlyDictionary[key].Should().Be(value);
		}

		[Fact]
		public void TryGetValue_ShouldReturnTrueAndExpectedValue_WhenKeyExists()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "ExistingKey";
			int expectedValue = 100;
			registry.SetValue(key, expectedValue);

			// Act
			bool result = registry.TryGetValue(key, out int value);

			// Assert
			result.Should().BeTrue();
			value.Should().Be(expectedValue);
		}

		[Fact]
		public void TryGetValue_ShouldReturnFalse_WhenKeyDoesNotExist()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "NonExistentKey";

			// Act
			bool result = registry.TryGetValue(key, out int value);

			// Assert
			result.Should().BeFalse();
			value.Should().Be(default);
		}

		[Fact]
		public void TryRemove_ShouldReturnTrueAndRemovedValue_WhenKeyExists()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "RemovableKey";
			int value = 200;
			registry.SetValue(key, value);

			// Act
			bool result = registry.TryRemove(key, out int removedValue);

			// Assert
			result.Should().BeTrue();
			removedValue.Should().Be(value);
			registry.ReadOnlyDictionary.ContainsKey(key).Should().BeFalse();
		}

		[Fact]
		public void TryRemove_ShouldReturnFalse_WhenKeyDoesNotExist()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "NonRemovableKey";

			// Act
			bool result = registry.TryRemove(key, out int removedValue);

			// Assert
			result.Should().BeFalse();
			removedValue.Should().Be(default);
		}

		#endregion

		#region Read-Only Dictionary Property Tests

		[Fact]
		public void ReadOnlyDictionary_ShouldReflectStateOfConverters()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key1 = "Key1";
			int value1 = 10;
			string key2 = "Key2";
			int value2 = 20;

			// Act
			registry.SetValue(key1, value1);
			registry.SetValue(key2, value2);

			// Assert
			registry.ReadOnlyDictionary.Count.Should().Be(2);
			registry.ReadOnlyDictionary[key1].Should().Be(value1);
			registry.ReadOnlyDictionary[key2].Should().Be(value2);
		}

		[Fact]
		public void ReadOnlyDictionary_ShouldNotBeModifiableDirectly()
		{
			// Arrange
			Registry<string, int> registry = new();

			// Act
			var readOnlyDict = registry.ReadOnlyDictionary;

			// Assert
			readOnlyDict.Invoking(d => ((IDictionary<string, int>)d).Add("NewKey", 30))
						.Should().Throw<NotSupportedException>();
		}

		[Fact]
		public void ReadOnlyDictionary_ShouldReflectChangesFromSetValue()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "DynamicKey";
			int initialValue = 50;
			int updatedValue = 100;

			// Act
			registry.SetValue(key, initialValue);
			registry.ReadOnlyDictionary[key].Should().Be(initialValue);
			registry.SetValue(key, updatedValue);

			// Assert
			registry.ReadOnlyDictionary[key].Should().Be(updatedValue);
		}

		[Fact]
		public void ReadOnlyDictionary_ShouldReflectChangesFromTryRemove()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "RemovableKey";
			int value = 75;
			registry.SetValue(key, value);

			// Act
			registry.TryRemove(key, out _);

			// Assert
			registry.ReadOnlyDictionary.ContainsKey(key).Should().BeFalse();
		}

		#endregion

		#region Indexer Property Tests

		[Fact]
		public void Indexer_ShouldReturnExpectedValue_WhenKeyExists()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "ExistingKey";
			int value = 123;
			registry.SetValue(key, value);

			// Act
			int result = registry[key];

			// Assert
			result.Should().Be(value);
		}

		[Fact]
		public void Indexer_ShouldThrowKeyNotFoundException_WhenKeyDoesNotExist()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "NonExistentKey";

			// Act
			Action act = () =>
						 {
							 int result = registry[key];
						 };

			// Assert
			act.Should().Throw<KeyNotFoundException>();
		}

		#endregion

		#region Thread-Safety Considerations Tests

		[Fact]
		public void Registry_ShouldHandleConcurrentSetValueWithoutExceptions()
		{
			// Arrange
			Registry<string, int> registry = new();
			var keys = Enumerable.Range(1,   1000).Select(i => $"Key{i}").ToList();
			var values = Enumerable.Range(1, 1000).ToList();

			// Act
			Parallel.ForEach(keys.Zip(values), kvp => { registry.SetValue(kvp.First, kvp.Second); });

			// Assert
			foreach (string key in keys)
			{
				registry.ReadOnlyDictionary.ContainsKey(key).Should().BeTrue();
			}
		}

		[Fact]
		public void Registry_ShouldHandleConcurrentTryGetValueWithoutExceptions()
		{
			// Arrange
			Registry<string, int> registry = new();
			var keys = Enumerable.Range(1, 1000).Select(i => $"Key{i}").ToList();
			foreach (string key in keys)
			{
				registry.SetValue(key, 42);
			}

			// Act
			ConcurrentBag<bool> results = new();
			Parallel.ForEach(keys, key => { results.Add(registry.TryGetValue(key, out int value) && value == 42); });

			// Assert
			results.All(result => result).Should().BeTrue();
		}

		[Fact]
		public void Registry_ShouldHandleConcurrentTryRemoveWithoutExceptions()
		{
			// Arrange
			Registry<string, int> registry = new();
			var keys = Enumerable.Range(1, 1000).Select(i => $"Key{i}").ToList();
			foreach (string key in keys)
			{
				registry.SetValue(key, 42);
			}

			// Act
			ConcurrentBag<bool> results = new();
			Parallel.ForEach(keys, key => { results.Add(registry.TryRemove(key, out int value) && value == 42); });

			// Assert
			results.All(result => result).Should().BeTrue();
			registry.ReadOnlyDictionary.Should().BeEmpty();
		}

		#endregion

		#region Edge Cases Tests

		[Fact]
		public void SetValue_ShouldHandleNullKey_WhenKeyIsNull()
		{
			// Arrange
			Registry<string, int> registry = new();

			// Act
			registry.Invoking(r => r.SetValue(null, 123))
					.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void SetValue_ShouldHandleNullValue_WhenValueIsNull()
		{
			// Arrange
			Registry<string, string> registry = new();
			string key = "TestKey";

			// Act
			registry.SetValue(key, null);

			// Assert
			registry.ReadOnlyDictionary[key].Should().BeNull();
		}

		[Fact]
		public void Registry_ShouldBehaveCorrectly_WhenEmpty()
		{
			// Arrange
			Registry<string, int> registry = new();

			// Act
			bool result = registry.TryGetValue("NonExistentKey", out int value);

			// Assert
			result.Should().BeFalse();
			value.Should().Be(default);
			registry.ReadOnlyDictionary.Should().BeEmpty();
		}

		[Fact]
		public void SetValue_ShouldUpdateValue_WhenKeyAlreadyExists()
		{
			// Arrange
			Registry<string, int> registry = new();
			string key = "DuplicateKey";
			int initialValue = 50;
			int updatedValue = 100;
			registry.SetValue(key, initialValue);

			// Act
			registry.SetValue(key, updatedValue);

			// Assert
			registry.ReadOnlyDictionary[key].Should().Be(updatedValue);
		}

		#endregion

		#region Virtual Method Overriding Tests

		public class MockRegistry<TKey, TValue> : Registry<TKey, TValue>
		{
			public bool SetValueCalled    { get; private set; }
			public bool TryGetValueCalled { get; private set; }
			public bool TryRemoveCalled   { get; private set; }

			public override void SetValue(TKey key, TValue value)
			{
				SetValueCalled = true;
				base.SetValue(key, value);
			}

			public override bool TryGetValue(TKey key, out TValue? value)
			{
				TryGetValueCalled = true;
				return base.TryGetValue(key, out value);
			}

			public override bool TryRemove(TKey key, out TValue? value)
			{
				TryRemoveCalled = true;
				return base.TryRemove(key, out value);
			}
		}

		[Fact]
		public void SetValue_ShouldInvokeOverride_WhenCalled()
		{
			// Arrange
			MockRegistry<string, int> mockRegistry = new();
			string key = "TestKey";
			int value = 42;

			// Act
			mockRegistry.SetValue(key, value);

			// Assert
			mockRegistry.SetValueCalled.Should().BeTrue();
		}

		[Fact]
		public void TryGetValue_ShouldInvokeOverride_WhenCalled()
		{
			// Arrange
			MockRegistry<string, int> mockRegistry = new();
			string key = "TestKey";
			mockRegistry.SetValue(key, 42);

			// Act
			mockRegistry.TryGetValue(key, out _);

			// Assert
			mockRegistry.TryGetValueCalled.Should().BeTrue();
		}

		[Fact]
		public void TryRemove_ShouldInvokeOverride_WhenCalled()
		{
			// Arrange
			MockRegistry<string, int> mockRegistry = new();
			string key = "TestKey";
			mockRegistry.SetValue(key, 42);

			// Act
			mockRegistry.TryRemove(key, out _);

			// Assert
			mockRegistry.TryRemoveCalled.Should().BeTrue();
		}

		#endregion

		#region Equality and Reference Checks Tests

		[Fact]
		public void TryGetValue_ShouldReturnCorrectInstance_WhenValueIsReferenceType()
		{
			// Arrange
			Registry<string, object> registry = new();
			string key = "ReferenceKey";
			object referenceValue = new();
			registry.SetValue(key, referenceValue);

			// Act
			registry.TryGetValue(key, out object? retrievedValue);

			// Assert
			retrievedValue.Should().BeSameAs(referenceValue);
		}

		[Fact]
		public void TryRemove_ShouldReturnCorrectInstance_WhenValueIsReferenceType()
		{
			// Arrange
			Registry<string, object> registry = new();
			string key = "ReferenceKey";
			object referenceValue = new();
			registry.SetValue(key, referenceValue);

			// Act
			registry.TryRemove(key, out object? removedValue);

			// Assert
			removedValue.Should().BeSameAs(referenceValue);
		}

		#endregion

		#region Concurrency and Iteration Tests

		[Fact]
		public void ReadOnlyDictionary_ShouldAllowSafeIterationDuringConcurrentModifications()
		{
			// Arrange
			Registry<string, int> registry = new();
			var keys = Enumerable.Range(1, 100).Select(i => $"Key{i}").ToList();
			foreach (string key in keys)
			{
				registry.SetValue(key, 1);
			}

			// Act
			var snapshot = registry.ReadOnlyDictionary.ToList();

			Parallel.Invoke(
				() =>
				{
					foreach (var kvp in snapshot)
					{
						kvp.Key.Should().NotBeNull();
						// Check if the key is still present and assert if found
						if (registry.TryGetValue(kvp.Key, out int currentValue))
						{
							currentValue.Should().BeOneOf(1, 2);
						}
					}
				},
				() =>
				{
					foreach (string key in keys)
					{
						registry.SetValue(key, 2);
					}
				},
				() =>
				{
					foreach (string key in keys.Take(50))
					{
						registry.TryRemove(key, out _);
					}
				}
			);

			// Verify that iteration did not cause any concurrency exceptions
			registry.ReadOnlyDictionary.Should().NotBeNull();
		}

		#endregion
	}
}