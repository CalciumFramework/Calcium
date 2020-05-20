#if WPF || WINDOWS_UWP
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-21 15:34:42Z</CreationDate>
</File>
*/
#endregion

using System.Windows.Input;
using Calcium.UIModel.Input;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
#endif

namespace Calcium.UI.Elements
{
	/// <summary>
	/// This class allows you to attach a command
	/// to any UI element.
	/// </summary>
	public static class AttachedCommand
	{
		#region Command Property

		public static DependencyProperty CommandProperty
			= DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(AttachedCommand),
#if NETFX_CORE
				new PropertyMetadata(null, HandleCommandChanged));
#else
				new PropertyMetadata(HandleCommandChanged));
		#endif

		public static void SetCommand(DependencyObject obj, ICommand propertyValue)
		{
			obj.SetValue(CommandProperty, propertyValue);
		}

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}

		const string defaultEventName = "Click";

		static void HandleCommandChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AttachedCommandBehavior behavior = GetBehavior(d);
			var command = e.NewValue as ICommand;
			behavior.Command = command;
			if (command != null && string.IsNullOrEmpty(behavior.EventName))
			{
//				var eventCommand = e.NewValue as IEventCommand;
//				string eventName = eventCommand != null
//							&& !string.IsNullOrEmpty(eventCommand.EventName)
//								? eventCommand.EventName : defaultEventName;
//				behavior.EventName = eventName;
			}

			var element = d as FrameworkElement;
			if (element != null)
			{
				element.Unloaded -= HandleAssociatedObjectUnloaded;
				element.Unloaded += HandleAssociatedObjectUnloaded;
				element.Loaded -= HandleAssociatedObjectLoaded;
				element.Loaded += HandleAssociatedObjectLoaded;
			}
		}

		static void HandleAssociatedObjectUnloaded(object sender, RoutedEventArgs args)
		{
			AttachedCommandBehavior behavior = GetBehavior((DependencyObject)sender);
			Detach(behavior);
		}

		static void Detach(AttachedCommandBehavior behavior)
		{
			behavior.Detach();
		}

		static void HandleAssociatedObjectLoaded(object sender, RoutedEventArgs e)
		{
			var d = (DependencyObject)sender;
			AttachedCommandBehavior behavior = GetBehavior(d);
			Attach(d, behavior);
		}

		static void Attach(DependencyObject d, AttachedCommandBehavior behavior)
		{
			behavior.Detach();
			behavior.Attach(d);
			behavior.Command = GetCommand(d);
			string eventName = GetEvent(d);
			if (string.IsNullOrWhiteSpace(eventName))
			{
				eventName = defaultEventName;
			}
			behavior.EventName = eventName;
			behavior.CommandParameter = GetCommandParameter(d);
		}

		#endregion

		#region Command Parameter

		public static readonly DependencyProperty CommandParameterProperty
			= DependencyProperty.RegisterAttached(
				"CommandParameter",
				typeof(object),
				typeof(AttachedCommand),
#if NETFX_CORE
				new PropertyMetadata(null, HandleCommandParameterChanged));
#else
				new PropertyMetadata(HandleCommandParameterChanged));
#endif
		static void HandleCommandParameterChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AttachedCommandBehavior behavior = GetBehavior(d);
			behavior.CommandParameter = e.NewValue;
		}

		public static void SetCommandParameter(
			DependencyObject obj, object propertyValue)
		{
			obj.SetValue(CommandParameterProperty, propertyValue);
		}

		public static object GetCommandParameter(DependencyObject obj)
		{
			return obj.GetValue(CommandParameterProperty);
		}

		#endregion

		#region Event Property

		public static DependencyProperty EventProperty
			= DependencyProperty.RegisterAttached(
				"Event",
				typeof(string),
				typeof(AttachedCommand),
#if NETFX_CORE
				new PropertyMetadata(null, HandleEventChanged));
#else
				new PropertyMetadata(HandleEventChanged));
#endif

		public static void SetEvent(DependencyObject obj, string propertyValue)
		{
			obj.SetValue(EventProperty, propertyValue);
		}

		public static string GetEvent(DependencyObject obj)
		{
			return (string)obj.GetValue(EventProperty);
		}

		static void HandleEventChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			AttachedCommandBehavior behavior = GetBehavior(d);
			var eventName = e.NewValue as string;
			behavior.EventName = eventName;
		}

		#endregion

		#region CommandBehavior Property

		static readonly DependencyProperty CommandBehaviorProperty
			= DependencyProperty.RegisterAttached(
				"CommandBehavior",
				typeof(AttachedCommandBehavior),
				typeof(AttachedCommand),
				null);

		static AttachedCommandBehavior GetBehavior(DependencyObject d)
		{
			var attachment = (AttachedCommandBehavior)d.GetValue(CommandBehaviorProperty);

			if (attachment == null)
			{
				attachment = new AttachedCommandBehavior(d);
				d.SetValue(CommandBehaviorProperty, attachment);
			}

			return attachment;
		}

		#endregion
	}
}
#endif
