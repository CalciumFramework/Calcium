#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-03-21 20:07:26Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// A command that includes various auxiliary properties
	/// such as <c>Text</c> and <c>Visibility</c>, 
	/// and that uses delegates to perform its action 
	/// and determine if its action can be performed. 
	/// </summary>
	public class UICommand : UICommand<object>
	{
		/// <summary>
		/// Creates a synchronous UI command. 
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
		/// <param name="lineNumber">
		/// The line number of the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public UICommand(
			Action<object> executeAction,
			Func<object, bool> canExecuteFunc = null,
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0)
			: base(executeAction, canExecuteFunc, filePath, lineNumber)
		{
		}
	}

	/// <summary>
	/// A command that includes various auxiliary properties
	/// such as <c>Text</c> and <c>Visibility</c>, 
	/// and that uses delegates to perform its action 
	/// and determine if its action can be performed. 
	/// </summary>
	/// <typeparam name="TParameter">The parameter type,
	/// which may be used when executing the command,
	/// evaluating if the command can execute,
	/// and evaluting the various properties.</typeparam>
	public class UICommand<TParameter> 
		: ActionCommand<TParameter>, IUICommand
	{
		/// <summary>
		/// Creates a synchronous UI command. 
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
		/// <param name="lineNumber">
		/// The line number of the file that is instantiating this class. 
		/// This should only be explicitly specified 
		/// by classes that subclass this class.</param>
		public UICommand(
			Action<TParameter> executeAction, 
			Func<TParameter, bool> canExecuteFunc = null, 
			[CallerFilePath] string filePath = null,
			[CallerLineNumber] int lineNumber = 0) 
			: base(executeAction, canExecuteFunc, filePath, lineNumber)
		{
		}

		public override bool CanExecute(
			TParameter parameter = default(TParameter))
		{
			bool result = base.CanExecute(parameter);

			RefreshUIProperties(parameter);

			return result;
		}

		protected virtual void RefreshUIProperties(TParameter parameter)
		{
			try
			{
				parameter = ProcessParameter(parameter);

				RefreshVisibility(parameter);
				RefreshText(parameter);
				RefreshIconUrl(parameter);
				RefreshIsChecked(parameter);
				RefreshIconCharacter(parameter);
				RefreshIconFont(parameter);
			}
			catch (Exception ex)
			{
				if (ShouldRethrowException(ex))
				{
					throw;
				}
			}
		}

		public Task RefreshAsync(object commandParameter = null)
		{
			Refresh(commandParameter);

			return Task.CompletedTask;
		}

		protected override void RefreshCore(TParameter commandParameter)
		{
			base.RefreshCore(commandParameter);

			RefreshUIProperties(commandParameter);
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
			get => text ?? textFunc?.Invoke(DefaultParameter);
			set => Set(ref text, value, false);
		}

		Func<TParameter, string> textFunc;

		/// <summary>
		/// A func to retrieve the text of the command.
		/// </summary>
		public Func<TParameter, string> TextFunc
		{
			get => textFunc;
			set => Set(ref textFunc, value, true);
		}

		void RefreshText(TParameter parameter = default(TParameter))
		{
			parameter = ProcessParameter(parameter);

			if (textFunc != null)
			{
				Text = textFunc(parameter);
			}
		}

		#endregion

		#region IconUrl

		Func<TParameter, string> iconUrlFunc;

		/// <summary>
		/// A func to retrieve the icon URL of the command.
		/// </summary>
		public Func<TParameter, string> IconUrlFunc
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
		
		void RefreshIconUrl(TParameter parameter)
		{
			if (iconUrlFunc != null)
			{
				IconUrl = iconUrlFunc(parameter);
			}
		}

		#endregion

		#region Icon Font

		Func<TParameter, string> iconFontFunc;

		/// <summary>
		/// A func to retrieve the icon URL of the command.
		/// </summary>
		public Func<TParameter, string> IconFontFunc
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

		void RefreshIconFont(TParameter parameter)
		{
			if (iconFontFunc != null)
			{
				IconFont = iconFontFunc(parameter);
			}
		}

		#endregion

		#region Visible
		Func<TParameter, bool> isVisibleFunc;

		public Func<TParameter, bool> IsVisibleFunc
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

		void RefreshVisibility(TParameter parameter = default(TParameter))
		{
			parameter = ProcessParameter(parameter);

			if (isVisibleFunc != null)
			{
				Visible = isVisibleFunc(parameter);
			}
		}

		#endregion

		#region IsChecked
		Func<TParameter, bool> isCheckedFunc;

		public Func<TParameter, bool> IsCheckedFunc
		{
			get => isCheckedFunc;
			set => Set(ref isCheckedFunc, value, true);
		}

		void RefreshIsChecked(TParameter parameter)
		{
			if (isCheckedFunc != null)
			{
				IsChecked = isCheckedFunc(parameter);
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
		Func<TParameter, string> iconCharacterFunc;

		public Func<TParameter, string> IconCharacterFunc
		{
			get => iconCharacterFunc;
			set => Set(ref iconCharacterFunc, value, true);
		}

		void RefreshIconCharacter(TParameter parameter)
		{
			if (iconCharacterFunc != null)
			{
				IconCharacter = iconCharacterFunc(parameter);
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