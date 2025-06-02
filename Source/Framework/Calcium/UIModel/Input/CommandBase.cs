#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-05 17:34:48Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using Calcium.ComponentModel;
using Calcium.Concurrency;
using Calcium.Reflection;
// ReSharper disable ExplicitCallerInfoArgument

namespace Calcium.UIModel.Input
{
	/// <summary>
	/// The base implementation of the <see cref="ICommandBase"/>
	/// interface and is intended to be the base implementation
	/// for all commands.
	/// </summary>
	/// <typeparam name="TParameter"></typeparam>
	public abstract class CommandBase<TParameter> : ICommand,
													ICommandBase,
													INotifyPropertyChanged,
													ISuspendChangeNotification
	{
		static bool IsPrimitiveParameter<T>()
		{
			TypeInfo typeInfo = typeof(T).GetTypeInfo();
			return typeInfo.IsPrimitive || typeInfo.IsEnum;
		}

		readonly bool parameterPrimitive = IsPrimitiveParameter<TParameter>();
		bool hasDefault;

		readonly string creationFilePath;
		readonly int creationLineNumber;

		TParameter defaultParameter;

		/// <summary>
		/// This value is supplied to the <c>Execute</c>
		/// and <c>CanExecute</c> methods if no value is 
		/// specified when calling those methods.
		/// It's most useful in scenarios when binding
		/// does not support the notion of parameters. 
		/// </summary>
		public TParameter DefaultParameter
		{
			get => defaultParameter;
			set
			{
				defaultParameter = value;
				hasDefault       = true;
			}
		}

		protected CommandBase(
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
		{
			creationFilePath = filePath;
			creationLineNumber = lineNumber;
		}

		protected virtual object CoerceParameterToType(object parameter)
		{
			object coercedParameter = parameter;
			Type typeOfT = typeof(TParameter);

			if (parameter != null && !typeOfT.IsAssignableFromEx(parameter.GetType()))
			{
				var implicitTypeConverter = Dependency.Resolve<IImplicitTypeConverter, DefaultImplicitTypeConverter>();
				coercedParameter = implicitTypeConverter.ConvertToType(parameter, typeOfT);
			}

			return coercedParameter;
		}

		protected TParameter ProcessParameterNonGeneric(object parameter)
		{
			TParameter result;
			if (parameter == null)
			{
				result = hasDefault ? defaultParameter : default;
			}
			else
			{
				result = (TParameter)CoerceParameterToType(parameter);

				DefaultParameter = result;
			}

			return result;
		}

		protected TParameter ProcessParameter(TParameter parameter)
		{
			bool missing =
				EqualityComparer<TParameter>.Default.Equals(parameter, default(TParameter))
				&& !parameterPrimitive;

			if (missing)
			{
				// no parameter supplied — fall back on whatever we remembered
				return DefaultParameter;
			}

			// Here, the caller *did* supply a parameter (even if it happens
			//    to be default(T) for a primitive or enum)
			// so we update our memory
			DefaultParameter = parameter;

			return parameter;
		}

		protected AssignmentResult Set<TField>(
					ref TField oldValue,
					TField newValue,
					bool raiseOnCanExecuteChanged,
					[CallerMemberName] string memberName = null)
		{
			if (!object.Equals(oldValue, newValue))
			{
				oldValue = newValue;

				if (!ChangeNotificationSuspended)
				{
					// ReSharper disable once ExplicitCallerInfoArgument
					OnPropertyChanged(memberName);

					if (raiseOnCanExecuteChanged)
					{
						OnCanExecuteChanged(EventArgs.Empty);
					}
				}

				return AssignmentResult.Success;
			}

			return AssignmentResult.AlreadyAssigned;
		}

		protected virtual void OnCanExecuteChanged(EventArgs e)
		{
			var tempEvent = CanExecuteChanged;
			if (tempEvent != null)
			{
				SynchronizationContext.Send(() => tempEvent(this, e));
			}
		}

		ISynchronizationContext synchronizationContext;

		public ISynchronizationContext SynchronizationContext
		{
			get => synchronizationContext ?? (synchronizationContext = UIContext.Instance);
			set => synchronizationContext = value;
		}

		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged(EventArgs.Empty);
		}

		public abstract void Refresh(object commandParameter);

		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(
			[CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		public abstract bool CanExecute(object parameter = null);

		public abstract void Execute(object parameter = null);

		/// <summary>
		/// If subscribing to this event, ensure that you call 
		/// <c>CanExecute</c> or <c>CanExecuteAsync</c> method 
		/// when the event is raised;
		/// otherwise the <c>Enabled</c> property may not behave as expected.
		/// </summary>
		public event EventHandler CanExecuteChanged;

		protected bool HasCanExecuteChangedSubscribers => CanExecuteChanged != null;

		protected bool ShouldRethrowException(
			Exception ex,
			[CallerMemberName] string callerMemberName = null)
		{
			var tempHandler = ExceptionHandler;
			if (tempHandler == null
				|| tempHandler.ShouldRethrowException(
					ex, this, callerMemberName,
					creationFilePath, creationLineNumber))
			{
				return true;
			}

			return false;
		}

		IExceptionHandler settableExceptionHandler;
		bool attemptedToRetrieveHandler;
		
		/// <summary>
		/// When an exception occurs during execution or during evaluating 
		/// if the command can execute, then the exception is passed to the exception manager.
		/// If <c>null</c> the IoC container is used to locate an instance.
		/// </summary>
		public IExceptionHandler ExceptionHandler
		{
			get
			{
				if (settableExceptionHandler == null
					&& !attemptedToRetrieveHandler)
				{
					if (Dependency.Initialized)
					{
						Dependency.TryResolve(out settableExceptionHandler);
					}
					attemptedToRetrieveHandler = true;
				}
				return settableExceptionHandler;
			}
			set => settableExceptionHandler = value;
		}

		public bool ChangeNotificationSuspended { get; set; }
	}
}
