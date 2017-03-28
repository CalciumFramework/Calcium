using System;
using System.Collections.Generic;
using Codon.MissingTypes.System.Windows.Data;

namespace Codon.UI.Data
{
	public class BindingApplicator
	{
		readonly InternalBindingApplicator internalApplicator = new InternalBindingApplicator();
		public Action ApplyBinding(
			BindingExpression bindingExpression, object target, object dataContext, IValueConverter converter = null)
		{
			List<Action> unbindActions = new List<Action>();
#if __ANDROID__
			bindingExpression.View = (Android.Views.View)target;
#else
			bindingExpression.View = target;
#endif
			internalApplicator.ApplyBinding(bindingExpression, dataContext, converter, unbindActions);

			Action unbindAction = () =>
			{
				foreach (Action action in unbindActions)
				{
					action();
				}
			};

			return unbindAction;
		}
	}
}