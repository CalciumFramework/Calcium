#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 18:22:01Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Codon.Collections
{
	/// <summary>
	/// <see cref="CollectionComparer"/> Tests. 
	/// </summary>
	[TestClass]
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
				{s1, o1}, {s2, o2}
			};

			return dictionary;
		}

		#region AreEqualDictionariesGeneric
		[TestMethod]
		public void AreEqualDictionariesGenericShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(
								dictionary1, dictionary2);
			Assert.IsTrue(areEqual);
		}

		[TestMethod]
		public void AreEqualDictionariesGenericShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(
								dictionary1, dictionary2);
			Assert.IsFalse(areEqual);
		}

		[TestMethod]
		public void AreEqualDictionariesGenericShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			var firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.AreEqualDictionariesGeneric(
								dictionary1, dictionary2);
			Assert.IsFalse(areEqual);
		}
		#endregion

		#region AreEqualDictionaries
		[TestMethod]
		public void AreEqualDictionariesShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.AreEqualDictionaries(
								dictionary1, dictionary2);
			Assert.IsTrue(areEqual);
		}

		[TestMethod]
		public void AreEqualDictionariesShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.AreEqualDictionaries(
								dictionary1, dictionary2);
			Assert.IsFalse(areEqual);
		}

		[TestMethod]
		public void AreEqualDictionariesShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			var firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.AreEqualDictionaries(
								dictionary1, dictionary2);
			Assert.IsFalse(areEqual);
		}
		#endregion

		#region HaveSameElements
		[TestMethod]
		public void HaveSameElementsShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			bool areEqual = CollectionComparer.HaveSameElements(
								dictionary1.Values, dictionary2.Values);
			Assert.IsTrue(areEqual);
		}

		[TestMethod]
		public void HaveSameElementsShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			bool areEqual = CollectionComparer.HaveSameElements(
								dictionary1.Values, dictionary2.Values);
			Assert.IsFalse(areEqual);
		}

		[TestMethod]
		public void HaveSameElementsShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			var firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			bool areEqual = CollectionComparer.HaveSameElements(
								dictionary1.Values, dictionary2.Values);
			Assert.IsFalse(areEqual);
		}
		#endregion

		#region AreEqualLists
		[TestMethod]
		public void AreEqualListsShouldReturnTrueIfEqual()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();

			var list1 = new List<object>(dictionary1.Values);
			var list2 = new List<object>(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			Assert.IsTrue(areEqual);
		}

		[TestMethod]
		public void AreEqualListsShouldReturnFalseIfContainsMoreElements()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			dictionary2.Add("Foo2", new object());

			var list1 = new List<object>(dictionary1.Values);
			var list2 = new List<object>(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			Assert.IsFalse(areEqual);
		}

		[TestMethod]
		public void AreEqualListsShouldReturnFalseIfValueDifferent()
		{
			var dictionary1 = CreateDummyDictionary();
			var dictionary2 = CreateDummyDictionary();
			var firstKey = dictionary1.First().Key;
			dictionary1[firstKey] = new object();

			var list1 = new List<object>(dictionary1.Values);
			var list2 = new List<object>(dictionary2.Values);

			bool areEqual = CollectionComparer.AreEqualLists(list1, list2);
			Assert.IsFalse(areEqual);
		}
		#endregion
	}
}
