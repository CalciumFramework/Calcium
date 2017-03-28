#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-09 23:13:28Z</CreationDate>
</File>
*/
#endregion

using System;

namespace Codon.DialogModel
{
	/// <summary>
	/// Defines the vertical orientation or location
	/// of a toast message, as displayed by the 
	/// <seealso cref="Services.IDialogService"/> implementation.
	/// </summary>
	public enum ToastVerticalOrientation
	{
		Top,
		Center,
		Bottom,
		Stretch
	}

	/// <summary>
	/// Defines the animation of a toast message, 
	/// as displayed by the 
	/// <seealso cref="Services.IDialogService"/> implementation.
	/// </summary>
	public enum ToastAnimationType
	{
		Slide,
		SlideHorizontal,
		Swivel,
		SwivelHorizontal,
		Fade
	}

	/// <summary>
	/// Used by the <see cref="Services.IDialogService"/>
	/// implementation to define the behaviour of a toast 
	/// message.
	/// </summary>
	public class ToastParameters
	{
		public ToastParameters()
		{
			VerticalOrientation = ToastVerticalOrientation.Top;
		}

		/// <summary>
		/// The larger text area of the toast.
		/// </summary>
		public object Caption { get; set; }

		/// <summary>
		/// A smaller text area than the <c>Caption</c>
		/// containing a potentially longer string.
		/// </summary>
		public object Body { get; set; }

		/// <summary>
		/// An image to display in the toast.
		/// Note that this may not be supported on all platforms.
		/// </summary>
		public Uri ImageUri { get; set; }

		/// <summary>
		/// The duration that the toast remains visible.
		/// This property may not be fully supported on all platforms.
		/// </summary>
		public int? MillisecondsUntilHidden { get; set; }

		/// <summary>
		/// The width of the image to display in the toast.
		/// This property may not be fully supported on all platforms.
		/// </summary>
		public double? ImageWidth { get; set; }

		/// <summary>
		/// The height of the image to display in the toast.
		/// This property may not be fully supported on all platforms.
		/// </summary>
		public double? ImageHeight { get; set; }

		/// <summary>
		/// The location or layout style of the toast.
		/// </summary>
		public ToastVerticalOrientation? VerticalOrientation { get; set; }

		/// <summary>
		/// The type of animation with which to present the toast.
		/// This property may not be fully supported on all platforms.
		/// </summary>
		public ToastAnimationType? AnimationType { get; set; }
	}
}