#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-09-29 16:32:45Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using Calcium.Collections;
using Calcium.Concurrency;
using Calcium.Logging;
using Calcium.Reflection;

namespace Calcium.UIModel.Validation
{
	/// <summary>
	/// This class is used to validate forms,
	/// and to provide a list of validation errors
	/// that can be displayed in the UI.
	/// </summary>
	public class DataErrorNotifier : INotifyDataErrorInfo
	{
		readonly IValidateData validator;

		readonly object errorsLock = new object();
		Dictionary<string, ObservableCollection<IDataValidationError>> errorsField;

		readonly object propertyDictionaryLock = new object();
		readonly IDictionary<string, Func<object>> propertyDictionary
									= new Dictionary<string, Func<object>>();

		readonly INotifyPropertyChanged owner;

		/// <summary>
		/// Initializes a new instance 
		/// of the <see cref="DataErrorNotifier"/> class.
		/// </summary>
		/// <param name="owner">The instance for which validation 
		/// is being provided.</param>
		/// <param name="validator">The validator.</param>
		public DataErrorNotifier(
			INotifyPropertyChanged owner, IValidateData validator)
		{
			this.validator = AssertArg.IsNotNull(validator, nameof(validator));
			this.owner = AssertArg.IsNotNull(owner, nameof(owner));

			owner.PropertyChanged += HandleOwnerPropertyChanged;

			ReadValidationAttributes();
		}

		void ReadValidationAttributes()
		{
			var properties = owner.GetType().GetTypeInfo().DeclaredProperties;

			foreach (PropertyInfo propertyInfo in properties)
			{
				var attributes = propertyInfo.GetCustomAttributes(
											typeof(ValidateAttribute), true);

				if (!attributes.Any())
				{
					continue;
				}

				if (!propertyInfo.CanRead)
				{
					throw new InvalidOperationException(string.Format(
						"Property {0} must have a getter to be validated.",
						propertyInfo.Name));
				}

				/* Prevents access to internal closure warning. */
				PropertyInfo info = propertyInfo;

				AddValidationProperty(
					propertyInfo.Name, () => info.GetValue(owner, null));
			}
		}

		async void HandleOwnerPropertyChanged(
			object sender, PropertyChangedEventArgs e)
		{
			if (e?.PropertyName == null)
			{
				return;
			}

			await BeginGetPropertyErrorsFromValidator(e.PropertyName);
		}

		async Task<ValidationCompleteEventArgs> BeginGetPropertyErrorsFromValidator(string propertyName)
		{
			Func<object> propertyFunc;
			lock (propertyDictionaryLock)
			{
				if (!propertyDictionary.TryGetValue(propertyName, out propertyFunc))
				{
					/* No property registered with that name. */
					return new ValidationCompleteEventArgs(propertyName);
				}
			}

			var result = await validator.ValidateAsync(propertyName, propertyFunc());
			ProcessValidationComplete(result);

			return result;
		}

		void ProcessValidationComplete(ValidationCompleteEventArgs e)
		{
			try
			{
				if (e.Exception == null)
				{
					SetPropertyErrors(e.PropertyName, e.Errors);
				}
			}
			catch (Exception ex)
			{
				var log = Dependency.Resolve<ILog>();
				log.Debug("Unable to set property error.", ex);
			}
		}

		/// <summary>
		/// Adds the property to the list of known class properties, 
		/// which is used, for example, when performing validation 
		/// of the whole class instance.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="property">The <c>Func</c> to 
		/// retrieve the property.</param>
		public void AddValidationProperty(string name, Func<object> property)
		{
			lock (propertyDictionaryLock)
			{
				propertyDictionary[name] = property;
			}

			var errorsTemp = Errors;

			lock (errorsLock)
			{
				if (!errorsTemp.ContainsKey(name))
				{
					errorsTemp[name] = new ObservableCollection<IDataValidationError>();
				}
			}
		}

		/// <summary>
		/// Validates all registered validatable properties.
		/// </summary>
		public async Task ValidateAllAsync()
		{
			if (propertyDictionary == null)
			{
				return;
			}

			foreach (KeyValuePair<string, Func<object>> pair in propertyDictionary)
			{
				string propertyName = pair.Key;
				var validateResult = await validator.ValidateAsync(propertyName, pair.Value());
				ProcessValidationComplete(validateResult);
			}
		}

		ReadOnlyDictionary<string, ObservableCollection<IDataValidationError>> readonlyErrors;

		/// <summary>
		/// A dictionary of validation errors that is populated 
		/// when one of the validation methods is called.
		/// </summary>
		public ReadOnlyDictionary<string, ObservableCollection<IDataValidationError>> ValidationErrors
		{
			get
			{
				if (readonlyErrors == null)
				{
					lock (errorsLock)
					{
						if (readonlyErrors == null)
						{
							if (errorsField == null)
							{
								errorsField = new Dictionary<string, ObservableCollection<IDataValidationError>>();
							}

							readonlyErrors = new ReadOnlyDictionary<string, ObservableCollection<IDataValidationError>>(errorsField);
						}
					}
				}

				return readonlyErrors;
			}
		}

		/// <summary>
		/// Gets the validation errors for all properties.
		/// </summary>
		/// <value>The errors.</value>
		Dictionary<string, ObservableCollection<IDataValidationError>> Errors
		{
			get
			{
				if (errorsField == null)
				{
					lock (errorsLock)
					{
						if (errorsField == null)
						{
							errorsField = new Dictionary<string, ObservableCollection<IDataValidationError>>();
						}
					}
				}

				return errorsField;
			}
		}

		#region INotifyDataErrorInfo implementation

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		/// <summary>
		/// Gets the validation errors for a specified property 
		/// or for the entire object.
		/// </summary>
		/// <param name="propertyName">The name of the property 
		/// to retrieve validation errors for, 
		/// or null or <see cref="F:System.String.Empty"/> 
		/// to retrieve errors for the entire object.</param>
		/// <returns>
		/// The validation errors for the property or object.
		/// </returns>
		public IEnumerable GetErrors(string propertyName)
		{
			return GetDataValidationErrors(propertyName);
		}

		/// <summary>
		/// Gets a value that indicates whether the object has validation errors.
		/// </summary>
		/// <value></value>
		/// <returns><c>true</c> if the object currently has validation errors; 
		/// otherwise, <c>false</c>.</returns>
		public bool HasErrors
		{
			get
			{
				lock (errorsLock)
				{
					if (errorsField == null || Errors.Count < 1)
					{
						return false;
					}

					foreach (var pair in errorsField)
					{
						var list = pair.Value;
						if (list != null && list.Any())
						{
							return true;
						}
					}

					return false;
				}
			}
		}
		#endregion

		/// <summary>
		/// Gets the validation errors for a specified property 
		/// or for the entire object.
		/// </summary>
		/// <param name="propertyName">The name of the property 
		/// to retrieve validation errors for, 
		/// or null or <see cref="F:System.String.Empty"/> 
		/// to retrieve errors for the entire object.</param>
		/// <returns>
		/// The validation errors for the property or object.
		/// </returns>
		public IEnumerable<IDataValidationError> GetDataValidationErrors(
			string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				lock (errorsLock)
				{
					if (errorsField == null)
					{
						return new List<IDataValidationError>();
					}

					var result = from list in errorsField.Values
								 from errorInfo in list
								 select errorInfo;
					return result;
				}
			}

			lock (errorsLock)
			{
				if (errorsField == null || !errorsField.TryGetValue(propertyName,
						// ReSharper disable once CollectionNeverUpdated.Local
						out ObservableCollection<IDataValidationError> propertyErrors))
				{
					return new ObservableCollection<IDataValidationError>();
				}

				return propertyErrors.ToList();
			}
		}

		/// <summary>
		/// Raises the <see cref="ErrorsChanged"/> event.
		/// </summary>
		/// <param name="property">The property 
		/// for which the list of errors changed.</param>
		protected virtual void OnErrorsChanged(string property)
		{
			SynchronizationContext.Send(
				() => ErrorsChanged?.Invoke(
						this, new DataErrorsChangedEventArgs(property)));
		}

		/// <summary>
		/// Raises the ErrorsChanged event.
		/// </summary>
		public void RaiseErrorsChanged()
		{
			OnErrorsChanged(string.Empty);
		}

		/// <summary>
		/// Removes the property error from the properties list 
		/// of validation errors.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="errorCode">The error code.</param>
		public void RemovePropertyError(string propertyName, Guid errorCode)
		{
			bool raiseEvent = false;

			lock (errorsLock)
			{
				if (errorsField == null)
				{
					return;
				}

				ObservableCollection<IDataValidationError> list 
					= errorsField[propertyName];

				IDataValidationError dataValidationError
					= list.SingleOrDefault(e => e.Id == errorCode);

				if (dataValidationError != null)
				{
					list.Remove(dataValidationError);
					raiseEvent = true;
				}
			}

			if (raiseEvent)
			{
				OnErrorsChanged(propertyName);
			}
		}

		/// <summary>
		/// Adds a property error for the specified propertyName.
		/// This may produce a prompt in the UI to correct 
		/// the error before proceeding.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="dataValidationError">The data validation error.</param>
		public void AddPropertyError(
			string propertyName, IDataValidationError dataValidationError)
		{
			SetPropertyErrors(propertyName,
				new List<IDataValidationError> { dataValidationError });
		}

		/// <summary>
		/// Adds the specified property expression 
		/// to the list of validated properties.
		/// </summary>
		/// <param name="expression">The property expression.</param>
		public void AddValidationProperty(Expression<Func<object>> expression)
		{
			PropertyInfo propertyInfo = ReflectionCompiler.GetPropertyInfo(expression);
			string name = propertyInfo.Name;

			MethodInfo getMethodInfo = propertyInfo.GetMethod;
			Func<object> getter = (Func<object>)getMethodInfo.CreateDelegate(
													typeof(Func<object>),
													this);
			AddValidationProperty(name, getter);
		}

		/// <summary>
		/// Sets the known validation errors for a property.
		/// This may produce a prompt in the UI to correct 
		/// the error before proceeding.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="dataErrors">The data errors.</param>
		public void SetPropertyErrors(
			string propertyName, IEnumerable<IDataValidationError> dataErrors)
		{
			AssertArg.IsNotNullOrEmpty(propertyName, nameof(propertyName));

			bool raiseEvent = false;
			lock (errorsLock)
			{
				bool created = false;

				var errorsArray = dataErrors as IDataValidationError[] ?? dataErrors?.ToArray();
				int paramErrorCount = errorsArray?.Length ?? 0;

				if ((errorsField == null || errorsField.Count < 1)
					&& paramErrorCount < 1)
				{
					return;
				}

				if (errorsField == null)
				{
					errorsField = new Dictionary<string, ObservableCollection<IDataValidationError>>();
					created     = true;
				}

				bool listFound = false;
				if (created || !(listFound = errorsField.TryGetValue(propertyName, 
									out ObservableCollection<IDataValidationError> list)))
				{
					list = new ObservableCollection<IDataValidationError>();
				}

				if (paramErrorCount < 1)
				{
					if (listFound)
					{
						list?.Clear();
						raiseEvent = true;
					}
				}
				else
				{
					var tempList = new List<IDataValidationError>();

					if (errorsArray != null)
					{
						foreach (var dataError in errorsArray)
						{
							if (created || list.SingleOrDefault(
									e => e.Id == dataError.Id) == null)
							{
								tempList.Add(dataError);
								raiseEvent = true;
							}
						}
					}

					list.AddRange(tempList);
					errorsField[propertyName] = list;
				}
			}

			if (raiseEvent)
			{
				OnErrorsChanged(propertyName);
			}
		}

		ISynchronizationContext synchronizationContext;

		/// <summary>
		/// Use this property to override the current synchronization context.
		/// </summary>
		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContext ?? (synchronizationContext = UIContext.Instance);
			set => synchronizationContext = value;
		}
	}
}

