using System;
using System.ComponentModel;

namespace Calcium.UI.Data
{
	internal class InpcTargetBinder
	{
		public static Action BindTarget(PropertyBinding propertyBinding, object dataContext)
		{
			AssertArg.IsNotNull(propertyBinding.View, "propertyBinding.View");

			var view = propertyBinding.View as INotifyPropertyChanged;

			if (view == null)
			{
				throw new InvalidOperationException("Binding target does not implement INotifyPropertyChanged.");
			}

			string propertyName = propertyBinding.TargetProperty.Name;

			PropertyChangedEventHandler handler = (sender, args) =>
			{
				if (args.PropertyName == propertyName)
				{
					var rawValue = propertyBinding.TargetProperty.GetValue(propertyBinding.View);
					ViewValueChangedHandler.HandleTargetValueChanged(propertyBinding, rawValue, dataContext);
				}
			};

			view.PropertyChanged += handler;

			Action removeAction = () => { view.PropertyChanged -= handler; };
			return removeAction;
		}
	}
}
