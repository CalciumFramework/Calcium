using Calcium.ComponentModel;

namespace Calcium.Collections
{
	/// <summary>
	/// Implements <see cref="IAttachObject{T}"/>
	/// and materializes the item as a <c>string</c>.
	/// <seealso cref="AdaptiveCollectionTests"/>
	/// <seealso cref="ReadOnlyAdaptiveCollectionTests"/>
	/// </summary>
	/// <typeparam name="T">The type of the inner item
	/// that is materialized as a <c>string</c>.</typeparam>
	class AttachableStringMock<T> : IAttachObject<T>
	{
		T item;

		public string Value => item?.ToString();

		/// <inheritdoc />
		public void AttachObject(T item)
		{
			this.item = item;
		}

		/// <inheritdoc />
		public T DetachObject()
		{
			var temp = item;
			item = default;
			return item;
		}

		/// <inheritdoc />
		public T GetObject()
		{
			return item;
		}
	}
}
