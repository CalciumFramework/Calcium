#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:01Z</CreationDate>
</File>
*/

#endregion

using FluentAssertions;

namespace Calcium.Collections
{
	/// <summary>
	///     <see cref="CollectionComparer" /> Tests.
	/// </summary>
	public class CollectionComparerTests
	{
		Dictionary<string, object> CreateDummyDictionary()
		{
			string s1 = "Foo";
			string s2 = "Bah";
			object o1 = 3;
			object o2 = "15C0B33E-F01E-4FD9-8416-19A3C5E7E8EA";

			var dictionary = new Dictionary<string, object>
			{
				{ s1, o1 }, { s2, o2 }
			};

			return dictionary;
		}

		#region AreEqualDictionariesGeneric

		[Fact]
		public void AreEqualDictionariesGeneric_ShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(dictionary1, dictionary2);
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void AreEqualDictionariesGeneric_ShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(dictionary1, dictionary2);
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void AreEqualDictionariesGeneric_ShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			string firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(dictionary1, dictionary2);
			areEqual.Should().BeFalse();
		}

		#endregion

		#region AreEqualDictionaries

		[Fact]
		public void AreEqualDictionaries_ShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.AreEqualDictionaries(dictionary1, dictionary2);
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void AreEqualDictionaries_ShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.AreEqualDictionaries(dictionary1, dictionary2);
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void AreEqualDictionaries_ShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			string firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.AreEqualDictionaries(dictionary1, dictionary2);
			areEqual.Should().BeFalse();
		}

		#endregion

		#region HaveSameElements

		[Fact]
		public void HaveSameElements_ShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.HaveSameElements(dictionary1.Values, dictionary2.Values);
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void HaveSameElements_ShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.HaveSameElements(dictionary1.Values, dictionary2.Values);
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void HaveSameElements_ShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			string firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.HaveSameElements(dictionary1.Values, dictionary2.Values);
			areEqual.Should().BeFalse();
		}

		#endregion

		#region AreEqualLists

		[Fact]
		public void AreEqualLists_ShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			List<object> list1 = new(dictionary1.Values);
			List<object> list2 = new(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			areEqual.Should().BeTrue();
		}

		[Fact]
		public void AreEqualLists_ShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			List<object> list1 = new(dictionary1.Values);
			List<object> list2 = new(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			areEqual.Should().BeFalse();
		}

		[Fact]
		public void AreEqualLists_ShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			string firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			List<object> list1 = new(dictionary1.Values);
			List<object> list2 = new(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			areEqual.Should().BeFalse();
		}

		#endregion
	}
}