#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-21 15:34:42Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Calcium.ComponentModel;

namespace Calcium.UIModel.Input
{
	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state, both asynchronously. 
	/// </summary>
	public class AsyncActionCommand : AsyncActionCommand<object>
	{
		/// <summary>
		/// Creates an awaitable asynchronous command. 
		/// </summary>
		/// <param name="executeAsyncFunc">The func to execute when the command is performed.</param>
		/// <param name="canExecuteAsyncFunc">A func that determines if the command 
		/// may be performed. Can be <c>null</c>.</param>
		/// <param name="filePath">The path to the file that is instantiating this class. 
		/// This should only be explicitly specified  by classes that subclass this class.</param>
		/// <param name="lineNumber">The line number of the file that is instantiating this class. 
		/// This should only be explicitly specified  by classes that subclass this class.</param>
		public AsyncActionCommand(
			Func<object, Task> executeAsyncFunc,
			Func<object, Task<bool>> canExecuteAsyncFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) : base(executeAsyncFunc, canExecuteAsyncFunc, filePath, lineNumber)
		{
		}
	}

	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state, both asynchronously. 
	/// </summary>
	/// <typeparam name="TParameter">The parameter type,
	/// which may be used when executing the command
	/// or evaluating if the command can execute.</typeparam>
	public class AsyncActionCommand<TParameter>
		: CommandBase<TParameter>, IAsynchronousCommand
	{
		volatile bool ignoreNextCanExecuteCall;

		readonly Func<TParameter, Task> executeActionAsync;
		Func<TParameter, Task<bool>> canExecuteAsyncFunc;

		/// <summary>
		/// Creates an awaitable asynchronous command. 
		/// </summary>
		/// <param name="executeAsyncFunc">The func to execute when the command is performed.</param>
		/// <param name="canExecuteAsyncFunc">A func that determines if the command 
		/// may be performed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating 
		/// this class. This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">
		/// The line number of the file that is instantiating 
		/// this class. This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public AsyncActionCommand(
			Func<TParameter, Task> executeAsyncFunc,
			Func<TParameter, Task<bool>> canExecuteAsyncFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) : base(filePath, lineNumber)
		{
			executeActionAsync = AssertArg.IsNotNull(
									executeAsyncFunc, nameof(executeAsyncFunc));

			this.canExecuteAsyncFunc = canExecuteAsyncFunc;
		}

		/// <summary>
		/// The <c>func</c> that is used to determine
		/// if the command can execute.
		/// If <c>null</c>, it is assumed the command
		/// can execute.
		/// </summary>
		public Func<TParameter, Task<bool>> CanExecuteFunc
		{
			get => canExecuteAsyncFunc;
			set
			{
				if (canExecuteAsyncFunc != value)
				{
					canExecuteAsyncFunc = value;

					OnPropertyChanged();

					OnCanExecuteChanged(EventArgs.Empty);
				}
			}
		}

		#region ICommand Members

		public override async void Refresh(object commandParameter)
		{
			try
			{
				await RefreshAsync(commandParameter);
			}
			catch (Exception ex)
			{
				var handler = Dependency.Resolve<IExceptionHandler>();
				if (handler.ShouldRethrowException(ex, this))
				{
					throw;
				}
			}
			
		}

		public override bool CanExecute(object parameter = null)
		{
			TParameter coercedParameter = ProcessParameterNonGeneric(parameter);
			
			RefreshCanExecute(coercedParameter);

			var result = Enabled;
			
			return result;
		}

		async void RefreshCanExecute(TParameter parameter)
		{
			try
			{
				await RefreshCanExecuteAsync(parameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}
			}
		}

		async Task RefreshCanExecuteAsync(TParameter parameter)
		{
			/* This method is intended to be called by the UI thread 
			 * as a result of a UI update. */
			if (ignoreNextCanExecuteCall)
			{
				ignoreNextCanExecuteCall = false;
				return;
			}

			if (canExecuteAsyncFunc != null)
			{
				bool temp = Enabled;

				try
				{
					temp = await canExecuteAsyncFunc(parameter);
				}
				catch (Exception ex)
				{
					if (ShouldRethrowException(ex))
					{
						throw;
					}
				}

				if (Enabled != temp)
				{
					if (HasCanExecuteChangedSubscribers)
					{
						ignoreNextCanExecuteCall = true;
					}

					Enabled = temp;

					OnCanExecuteChanged(EventArgs.Empty);
				}
			}
		}

		public virtual async Task<bool> CanExecuteAsync(TParameter parameter)
		{
			parameter = ProcessParameter(parameter);

			await RefreshCanExecuteAsync(parameter);

			return Enabled;
		}

		public override async void Execute(object parameter = null)
		{
			TParameter coercedParameter = ProcessParameterNonGeneric(parameter);
			await ExecuteAsync(coercedParameter);
		}

		public virtual async Task ExecuteAsync(
			TParameter parameter = default(TParameter))
		{
			try
			{
				var processedParameter = ProcessParameter(parameter);

				await executeActionAsync(processedParameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}
			}
		}

		#endregion

		public virtual bool CanExecute(TParameter parameter = default(TParameter))
		{
			parameter = ProcessParameter(parameter);

#pragma warning disable 4014
			RefreshCanExecuteAsync(parameter);
#pragma warning restore 4014

			var result = Enabled;

			return result;
		}

		bool enabled = true;

		public bool Enabled
		{
			get => enabled;
			protected internal set => Set(ref enabled, value, false);
		}
		
		public virtual async Task RefreshAsync(object commandParameter = null)
		{
			TParameter parameter = ProcessParameterNonGeneric(commandParameter);
			await RefreshAsyncCore(parameter);
		}

		public virtual async Task RefreshAsync(TParameter parameter)
		{
			parameter = ProcessParameter(parameter);
			await RefreshAsyncCore(parameter);
		}

		async Task RefreshAsyncCore(TParameter parameter)
		{
			await RefreshCanExecuteAsync(parameter);
		}
	}
}
