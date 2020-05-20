using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calcium.UserOptionsModel
{
	public class OptionCategory : IOptionCategory, IEquatable<IOptionCategory>, INotifyPropertyChanged
	{
		readonly Func<object> titleFunc;
		public object Id { get; private set; }

		public OptionCategory(object id, Func<object> titleFunc)
		{
			Id = AssertArg.IsNotNull(id, nameof(id));
			this.titleFunc = AssertArg.IsNotNull(titleFunc, nameof(titleFunc));
		}

		public object Title => titleFunc();

		public void Refresh()
		{
			OnPropertyChanged(nameof(Title));
		}

		public override int GetHashCode()
		{
			object id = Id;
			if (id != null)
			{
				return id.GetHashCode();
			}

			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var category = obj as IOptionCategory;
			return Equals(category);
		}

		public bool Equals(IOptionCategory other)
		{
			if (other == null)
			{
				return false;
			}

			object id = Id;
			var categoryId = other.Id;

			if (categoryId.Equals(id))
			{
				return true;
			}

			return false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
