using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// A command that includes various auxiliary properties
	/// such as <c>Text</c> and <c>Visibility</c>, 
	/// and that uses delegates to perform its action 
	/// and determine its enabled state, both asynchronously. 
	/// </summary>
	public class AsyncUICommand : AsyncUICommand<object>
	{
		public AsyncUICommand(
			Func<object, Task> executeAsyncFunc,
			Func<object, Task<bool>> canExecuteAsyncFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			: base(executeAsyncFunc, canExecuteAsyncFunc, filePath, lineNumber)
		{
		}
	}

	/// <summary>
	/// A command that includes various auxiliary properties
	/// such as <c>Text</c> and <c>Visibility</c>, 
	/// and that uses delegates to perform its action 
	/// and determine its enabled state, both asynchronously. 
	/// </summary>
	/// <typeparam name="TParameter">The parameter type,
	/// which may be used when executing the command,
	/// evaluating if the command can execute,
	/// and evaluting the various properties.</typeparam>
	public class AsyncUICommand<TParameter> : AsyncActionCommand<TParameter>,
		IUICommand
	{
		/// <summary>
		/// Creates an asynchronous command. 
		/// </summary>
		/// <param name="executeAsyncFunc">
		/// The action to invoke when the command is executed.</param>
		/// <param name="canExecuteAsyncFunc">
		/// A func that determines if the command 
		/// may be performed. Can be <c>null</c>.</param>
		/// <param name="filePath">
		/// The path to the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		/// <param name="lineNumber">
		/// The line number of the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public AsyncUICommand(
			Func<TParameter, Task> executeAsyncFunc, 
			Func<TParameter, Task<bool>> canExecuteAsyncFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) 
			: base(executeAsyncFunc, canExecuteAsyncFunc, filePath, lineNumber)
		{
		}

		public override bool CanExecute(TParameter parameter = default(TParameter))
		{
			bool result = base.CanExecute(parameter);

			RefreshUIProperties(parameter);

			return result;
		}

		public override async Task<bool> CanExecuteAsync(TParameter parameter)
		{
			bool result = await base.CanExecuteAsync(parameter);

			await RefreshUIPropertiesAsync(parameter);

			return result;
		}

		async void RefreshUIProperties(TParameter parameter)
		{
			try
			{
				await RefreshUIPropertiesAsync(parameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}
			}
		}

		protected virtual async Task RefreshUIPropertiesAsync(TParameter parameter)
		{
			parameter = ProcessParameter(parameter);

			await RefreshVisibilityAsync(parameter);
			await RefreshTextAsync(parameter);
			await RefreshIconUrlAsync(parameter);
			await RefreshIsCheckedAsync(parameter);
			await RefreshIconCharacterAsync(parameter);
			await RefreshIconFontAsync(parameter);
		}

		public override async Task RefreshAsync(object commandParameter = null)
		{
			TParameter parameter = ProcessParameterNonGeneric(commandParameter);
			await base.RefreshAsync(parameter);
			await RefreshUIPropertiesAsync(parameter);
		}

		#region UI Properties

		public string Id { get; set; }

		#region Text

		string text;

		/// <summary>
		/// The title of this command that may be displayed to the user.
		/// </summary>
		public string Text
		{
			get => text ?? textFunc?.Invoke(DefaultParameter).Result;
			set => Set(ref text, value, false);
		}

		Func<TParameter, Task<string>> textFunc;

		/// <summary>
		/// A func to retrieve the text of the command.
		/// </summary>
		public Func<TParameter, Task<string>> TextFunc
		{
			get => textFunc;
			set => Set(ref textFunc, value, true);
		}

		async Task RefreshTextAsync(TParameter parameter = default(TParameter))
		{
			parameter = ProcessParameter(parameter);

			if (textFunc != null)
			{
				Text = await textFunc(parameter);
			}
		}
		#endregion

		#region IconUrl
		Func<TParameter, Task<string>> iconUrlFunc;

		/// <summary>
		/// A func to retrieve the icon URL of the command.
		/// </summary>
		public Func<TParameter, Task<string>> IconUrlFunc
		{
			get => iconUrlFunc;
			set => Set(ref iconUrlFunc, value, true);
		}

		string iconUrl;

		public string IconUrl
		{
			get => iconUrl;
			set => Set(ref iconUrl, value, false);
		}

		async Task RefreshIconUrlAsync(TParameter parameter)
		{
			if (iconUrlFunc != null)
			{
				IconUrl = await iconUrlFunc(parameter);
			}
		}
		#endregion

		#region Icon Font

		Func<TParameter, Task<string>> iconFontFunc;

		/// <summary>
		/// A func to retrieve the icon URL of the command.
		/// </summary>
		public Func<TParameter, Task<string>> IconFontFunc
		{
			get => iconFontFunc;
			set => Set(ref iconFontFunc, value, true);
		}

		string iconFont;

		public string IconFont
		{
			get => iconFont;
			set => Set(ref iconFont, value, false);
		}

		async Task RefreshIconFontAsync(TParameter parameter)
		{
			if (iconFontFunc != null)
			{
				IconFont = await iconFontFunc(parameter);
			}
		}

		#endregion

		#region Visible

		Func<TParameter, Task<bool>> isVisibleFunc;

		public Func<TParameter, Task<bool>> IsVisibleFunc
		{
			get => isVisibleFunc;
			set => Set(ref isVisibleFunc, value, true);
		}

		bool visible = true;

		public bool Visible
		{
			get => visible;
			set => Set(ref visible, value, false);
		}

		async Task RefreshVisibilityAsync(TParameter parameter = default(TParameter))
		{
			parameter = ProcessParameter(parameter);

			if (isVisibleFunc != null)
			{
				Visible = await isVisibleFunc(parameter);
			}
		}
		#endregion

		#region IsChecked
		Func<TParameter, Task<bool>> isCheckedFunc;

		public Func<TParameter, Task<bool>> IsCheckedFunc
		{
			get => isCheckedFunc;
			set => Set(ref isCheckedFunc, value, true);
		}

		async Task RefreshIsCheckedAsync(TParameter parameter)
		{
			if (isCheckedFunc != null)
			{
				IsChecked = await isCheckedFunc(parameter);
			}
		}

		bool isChecked;

		public bool IsChecked
		{
			get => isChecked;
			set => Set(ref isChecked, value, false);
		}

		#endregion

		#region IconCharacter
		Func<TParameter, Task<string>> iconCharacterFunc;

		public Func<TParameter, Task<string>> IconCharacterFunc
		{
			get => iconCharacterFunc;
			set => Set(ref iconCharacterFunc, value, true);
		}

		async Task RefreshIconCharacterAsync(TParameter parameter)
		{
			if (iconCharacterFunc != null)
			{
				IconCharacter = await iconCharacterFunc(parameter);
			}
		}

		string iconCharacter;

		public string IconCharacter
		{
			get => iconCharacter;
			set => Set(ref iconCharacter, value, false);
		}
		#endregion

		#endregion
	}
}