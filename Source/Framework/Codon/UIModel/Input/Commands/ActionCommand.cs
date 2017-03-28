#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-21 15:34:42Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state. 
	/// </summary>
	public class ActionCommand : ActionCommand<object>
	{
		/// <summary>
		/// Creates a synchronous command. 
		/// </summary>
		/// <param name="executeAction">
		/// The action to invoke when the command is executed.</param>
		/// <param name="canExecuteFunc">
		/// A func that determines if the command 
		/// may be performed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">The line number of the file 
		/// that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public ActionCommand(
			Action<object> executeAction,
			Func<object, bool> canExecuteFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			: base(executeAction, canExecuteFunc, filePath, lineNumber)
		{
		}
	}

	/// <summary>
	/// A command that uses delegates to perform its action 
	/// and determine its enabled state. 
	/// </summary>
	/// <typeparam name="TParameter">The parameter type,
	/// which may be used when executing the command or
	/// evaluating if the command can execute.</typeparam>
	public class ActionCommand<TParameter>
		: CommandBase<TParameter>
	{
		readonly Action<TParameter> executeAction;
		readonly Func<TParameter, bool> canExecuteFunc;

		/// <summary>
		/// Creates a synchronous command. 
		/// </summary>
		/// <param name="executeAction">The action to invoke 
		/// when the command is executed.</param>
		/// <param name="canExecuteFunc">A func that determines if the command 
		/// may be executed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">The line number of the file 
		/// that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public ActionCommand(
			Action<TParameter> executeAction,
			Func<TParameter, bool> canExecuteFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) 
			: base(filePath, lineNumber)
		{
			this.executeAction = AssertArg.IsNotNull(
									executeAction, nameof(executeAction));
			this.canExecuteFunc = canExecuteFunc;
		}

		#region ICommand Members

		public override bool CanExecute(object parameter = null)
		{
			TParameter coercedParameter = ProcessParameterNonGeneric(parameter);

			bool result;

			try
			{
				result = CanExecute(coercedParameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}

				result = false;
			}
			
			return result;
		}

		public override void Execute(object parameter = null)
		{
			try
			{
				TParameter coercedParameter = ProcessParameterNonGeneric(parameter);
				executeAction(coercedParameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}
			}
		}

		public virtual void Execute(TParameter parameter = default(TParameter))
		{
			try
			{
				parameter = ProcessParameter(parameter);

				executeAction(parameter);
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

			bool result;

			if (canExecuteFunc == null)
			{
				result = true;
			}
			else
			{
				try
				{
					result = canExecuteFunc(parameter);
				}
				catch (Exception ex)
				{
					if (ShouldRethrowException(ex))
					{
						throw;
					}
					result = true;
				}
			}

			Enabled = result;

			return result;
		}

		bool enabled = true;

		public bool Enabled
		{
			get => enabled;
			protected internal set => Set(ref enabled, value, false);
		}

		protected virtual void RefreshCore(TParameter parameter)
		{
			bool canCurrentlyExecute = Enabled;
			bool valueAfterUpdate = CanExecute(parameter);

			if (canCurrentlyExecute != valueAfterUpdate)
			{
				OnCanExecuteChanged(EventArgs.Empty);
			}
		}

		public override void Refresh(object commandParameter = null)
		{
			TParameter parameter = ProcessParameterNonGeneric(commandParameter);
			RefreshCore(parameter);
		}
	}
}
