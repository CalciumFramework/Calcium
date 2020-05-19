#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-08 13:33:58Z</CreationDate>
</File>
*/
#endregion

using System.Collections.Generic;

namespace Codon.UIModel
{
	/// <summary>
	/// A class that implements this interface owns 0 or more
	/// child view-models. It is the task of the composite
	/// view-model to propagate events to children.
	/// </summary>
	public interface ICompositeViewModel : IViewModel
	{
		/// <summary>
		/// The collection of child view models.
		/// </summary>
		IEnumerable<IViewModel> ChildViewModels
		{
			get;
		}

		/// <summary>
		/// Make the specified view-model the active
		/// view-model. For example, if the composite
		/// view-model represents a tabbed interface, and each
		/// child a tab, then the composite view-model
		/// would select the specified view-model to make
		/// it the visible tab.
		/// </summary>
		/// <param name="viewModel">
		/// The view-model to activate.
		/// </param>
		void ActivateViewModel(IViewModel viewModel);
	}
}