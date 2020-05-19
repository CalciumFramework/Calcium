#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-02-20 00:00:00Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

using Codon.Navigation;
using Codon.Networking;
using Codon.UIModel.Validation;

namespace Codon.UIModel
{
	/// <summary>
	/// A feature rich view-model base class that supports
	/// property state preservation, input validation,
	/// and navigation awareness.
	/// </summary>
	public abstract class PageViewModelBase : StatefulViewModelBase,
		INotifyDataErrorInfo, 
		IValidateData, 
		INavigationAware
	{
		DataErrorNotifier errorNotifier;

		/// <summary>
		/// Gets the error notifier that is used for input validation.
		/// <see cref="DataErrorNotifier"/>
		/// </summary>
		protected DataErrorNotifier DataErrorNotifier 
			=> errorNotifier ?? (errorNotifier = new DataErrorNotifier(this, this));
		
		#region Navigation 

		protected event EventHandler<NavigatedArgs> NavigatedTo;

		void OnNavigatedTo(NavigatedArgs e)
		{
			NavigatedTo?.Invoke(this, e);
		}

		IDictionary<string, string> queryParameters;

		protected IDictionary<string, string> NavigationQueryParameters 
			=> queryParameters ?? (queryParameters = new Dictionary<string, string>());

		void INavigationAware.HandleNavigatedTo(NavigatedArgs e)
		{
#if !(NETFX_CORE || WINDOWS_UWP)
			if (e.Uri != null)
			{
				string decodedUrl = WebUtility.UrlDecode(e.Uri.ToString());

				queryParameters = WebUtilityExtended.ParseQueryString(decodedUrl);
			}
#endif
			OnNavigatedTo(e);
		}

		protected event EventHandler<NavigatingFromEventArgs> NavigatingFrom;

		void OnNavigatingFrom(NavigatingFromEventArgs e)
		{
			NavigatingFrom?.Invoke(this, e);
		}

		void INavigationAware.HandleNavigatingFrom(NavigatingArgs e)
		{
			var args = new NavigatingFromEventArgs(e.NavigationType, e.Uri);

			OnNavigatingFrom(args);
			if (args.Cancel)
			{
				e.Cancel = true;
			}
		}

		#endregion
		
		#region Input Validation

		#region INotifyDataErrorInfo Implementation

		event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
		{
			add => DataErrorNotifier.ErrorsChanged += value;
			remove => DataErrorNotifier.ErrorsChanged -= value;
		}

		IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
		{
			return DataErrorNotifier?.GetErrors(propertyName);
		}

		bool INotifyDataErrorInfo.HasErrors => DataErrorNotifier?.HasErrors ?? false;

		#endregion

		/// <summary>
		/// Gets the property errors. This should be overriden in a subclass
		/// to provide property validation.
		/// </summary>
		/// <param name="propertyName">Name of the property to validate.</param>
		/// <param name="value">The proposed value of a property.</param>
		/// <returns>A list of validation errors; 
		/// an empty list if no errors are found.</returns>
		public virtual Task<ValidationCompleteEventArgs> ValidateAsync(
			string propertyName, object value)
		{
			return Task.FromResult(new ValidationCompleteEventArgs(
										propertyName, new List<DataValidationError>()));
		}

		#endregion
	}
}
