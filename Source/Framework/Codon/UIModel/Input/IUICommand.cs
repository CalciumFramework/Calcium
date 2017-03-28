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

using System.Windows.Input;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// Extends <c>ICommand</c> to provide display 
	/// centric properties such as text and visibility.
	/// </summary>
	public interface IUICommand : ICommand
	{
		/// <summary>
		/// Gets the title text of the command.
		/// This may, for example, be displayed 
		/// as the text on a button.
		/// </summary>
		string Text { get; }

		/// <summary>
		/// Gets whether the UI component
		/// should be visible.
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Gets whether the UI component
		/// should be enabled.
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// Gets the location of an icon resource.
		/// </summary>
		string IconUrl { get; }

		/// <summary>
		/// Gets the icon character.
		/// This is usually a character within an icon font face.
		/// <seealso cref="IconFont"/>
		/// </summary>
		string IconCharacter { get; }

		/// <summary>
		/// Gets the resource name of the font used to display
		/// the <see cref="IconCharacter"/>.
		/// </summary>
		string IconFont { get; }

		/// <summary>
		/// When this command is associated with a checkbox
		/// this property retrieves whether the checkbox
		/// should be in its checked state.
		/// </summary>
		bool IsChecked { get; }

		/// <summary>
		/// An arbitrary identifier that you can use to identify 
		/// commands in your application during execution. 
		/// </summary>
		string Id { get; }
	}
}