#if IGNOREFORNOW && (WINDOWS_UWP || WPF)
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
using System.Reflection;
using System.Windows.Input;

using Calcium.Reflection;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Calcium.UI.Elements
{
#if WINDOWS_PHONE
	public class CommandBehavior : System.Windows.Interactivity.Behavior<UIElement>
	{
#if WINDOWS_PHONE
		const string defaultEventName = "Tap";
#else
		const string defaultEventName = "Click";
#endif

		EventHandler canExecuteChangedHandler;
		Action removeHandlerAction;
		bool attached;

		protected override void OnAttached()
		{
			base.OnAttached();

			canExecuteChangedHandler = UpdateEnabledStatePrivate;

			Deployment.Current.Dispatcher.BeginInvoke(
				delegate 
				{ 
					AttachEvent(EventName);
					var element = AssociatedObject as FrameworkElement;
					if (element != null)
					{
						element.Unloaded -= HandleAssociatedObjectUnloaded;
						element.Unloaded += HandleAssociatedObjectUnloaded;
						element.Loaded -= HandleAssociatedObjectLoaded;
						element.Loaded += HandleAssociatedObjectLoaded;
					}
					attached = true;
				});
		}

		void HandleAssociatedObjectLoaded(object sender, RoutedEventArgs e)
		{
			if (attached)
			{
				return;
			}

			AttachCommand();
			AttachEvent(EventName);
			attached = true;

			UpdateEnabledState();
		}

		void HandleAssociatedObjectUnloaded(object sender, RoutedEventArgs args)
		{
			DetachBehavior();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			DetachBehavior();
			CommandParameter = null;
		}

		void DetachBehavior()
		{
			DetachCommand();
			DetachEvent();

			attached = false;
		}
		
		public void HandleExecuteCommand(object sender, EventArgs args)
		{
			ExecuteCommand();
		}

#if WINDOWS_PHONE
		public void HandleGestureExecuteCommand(object sender, GestureEventArgs args)
		{
			ExecuteCommand();
		}
#endif

		protected virtual void ExecuteCommand()
		{
			var command = Command;
			if (command != null)
			{
				command.Execute(CommandParameter);
			}
		}

#region Command Property

		public static DependencyProperty CommandProperty
			= DependencyProperty.Register(
				"Command",
				typeof(ICommand),
				typeof(CommandBehavior),
#if NETFX_CORE
				new PropertyMetadata(null, HandleCommandChanged));
#else
				new PropertyMetadata(HandleCommandChanged));
#endif

		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
		}

		void DetachCommand()
		{
			var command = Command;
			if (command != null && canExecuteChangedHandler != null)
			{
				command.CanExecuteChanged -= canExecuteChangedHandler;
			}
		}

		void AttachCommand()
		{
			var command = Command;
			if (command != null && canExecuteChangedHandler != null)
			{
				command.CanExecuteChanged -= canExecuteChangedHandler;
				command.CanExecuteChanged += canExecuteChangedHandler;
			}
		}

		static void HandleCommandChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (CommandBehavior)d;
			var command = e.NewValue as ICommand;
			var oldCommand = e.OldValue as ICommand;

			if (oldCommand != null)
			{
				oldCommand.CanExecuteChanged -= behavior.canExecuteChangedHandler;
			}

			if (command != null)
			{
				command.CanExecuteChanged += behavior.canExecuteChangedHandler;
			}

			bool setEventName = false;
			if (command != null && string.IsNullOrEmpty(behavior.EventName))
			{
				var eventCommand = e.NewValue as IEventCommand;
				string eventName = eventCommand != null && !string.IsNullOrEmpty(eventCommand.EventName)
								? eventCommand.EventName : defaultEventName;
				behavior.EventName = eventName;
				setEventName = true;
			}

			if (!setEventName && behavior.attached)
			{
				behavior.AttachEvent(behavior.EventName);
				behavior.UpdateEnabledState();
			}
		}

		static CommandBehavior GetBehavior(DependencyObject d)
		{
			var behaviors = Interaction.GetBehaviors(d);
			foreach (var behavior in behaviors)
			{
				var cb = behavior as CommandBehavior;
				if (cb != null)
				{
					return cb;
				}
			}

			return null;
		}

#endregion

#region Command Parameter

		public static readonly DependencyProperty CommandParameterProperty
			= DependencyProperty.Register(
				"CommandParameter",
				typeof(object),
				typeof(CommandBehavior),
#if NETFX_CORE
				new PropertyMetadata(null, HandleCommandParameterChanged));
#else
				new PropertyMetadata(HandleCommandParameterChanged));
#endif
		static void HandleCommandParameterChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (CommandBehavior)d; // GetBehavior(d);

			if (behavior.attached)
			{
				behavior.AttachEvent(behavior.EventName);
				behavior.UpdateEnabledState();
			}
		}

		public object CommandParameter
		{
			get
			{
				return GetValue(CommandParameterProperty);
			}
			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}

#endregion

#region Event Name Property

		public static DependencyProperty EventNameProperty
			= DependencyProperty.Register(
				"EventName",
				typeof(string),
				typeof(CommandBehavior),
#if NETFX_CORE
				new PropertyMetadata(null, HandleEventNameChanged));
#else
				new PropertyMetadata(HandleEventNameChanged));
#endif

		public string EventName
		{
			get
			{
				return (string)GetValue(EventNameProperty);
			}
			set
			{
				SetValue(EventNameProperty, value);
			}
		}

		static void HandleEventNameChanged(
			DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var behavior = (CommandBehavior)d;
			var eventName = e.NewValue as string;

			if (behavior.attached)
			{
				behavior.AttachEvent(eventName);
				behavior.UpdateEnabledState();
			}
		}
#endregion

		void DetachEvent()
		{
			if (removeHandlerAction != null)
			{
				removeHandlerAction();
				removeHandlerAction = null;
			}
		}

		void AttachEvent(string value)
		{
			DetachEvent();

			string eventName = value;
			if (string.IsNullOrEmpty(eventName))
			{
				return;
			}
			DependencyObject element = AssociatedObject;
			if (element == null)
			{
				return;
			}
#if NETFX_CORE
			EventInfo eventInfo = element.GetType().GetRuntimeEvent(eventName);
#else
			EventInfo eventInfo = element.GetType().GetEvent(eventName);
#endif
			if (eventInfo == null)
			{
				throw new ArgumentException(string.Format(
					"Event name '{0}' not found on UIElement '{1}'",
					eventName, element));
			}

			if (EnvironmentValues.DesignTime)
			{
				return;
			}

			Delegate handler;

#if NETFX_CORE
						if (typeof(EventHandler<EventArgs>).GetTypeInfo().IsAssignableFrom(eventInfo.EventHandlerType.GetTypeInfo()))
						{
							MethodInfo methodInfo = GetType().GetTypeInfo().GetDeclaredMethod("HandleExecuteCommand");
							handler = methodInfo.CreateDelegate(
								eventInfo.EventHandlerType, this/*, "HandleExecuteCommand"*/);
							eventInfo.AddEventHandler(element, handler);
							removeHandlerAction = () => eventInfo.RemoveEventHandler(element, handler);
						}
#else
			if (typeof(EventHandler<EventArgs>).IsAssignableFrom(eventInfo.EventHandlerType))
			{
				/* This is faster for non-generic event handlers, 
				 * and relies on event covarience. */
				handler = Delegate.CreateDelegate(
					eventInfo.EventHandlerType, this, "HandleExecuteCommand");
				eventInfo.AddEventHandler(element, handler);
				removeHandlerAction = () => eventInfo.RemoveEventHandler(element, handler);
			}
#endif

#if WINDOWS_PHONE
			else if (typeof(EventHandler<GestureEventArgs>).IsAssignableFrom(eventInfo.EventHandlerType))
			{
				/* This is faster for non-generic event handlers, 
				 * and relies on event covarience. */
				handler = Delegate.CreateDelegate(
					eventInfo.EventHandlerType, this, "HandleGestureExecuteCommand");
				eventInfo.AddEventHandler(element, handler);
				removeHandlerAction = () => eventInfo.RemoveEventHandler(element, handler);
			}
#endif
			else
			{
				handler = eventInfo.CreateHandler(ExecuteCommand);
				eventInfo.AddEventHandler(element, handler);
				removeHandlerAction = () => eventInfo.RemoveEventHandler(element, handler);
			}
		}

		void UpdateEnabledStatePrivate(object sender, EventArgs e)
		{
			UpdateEnabledState();
		}

		protected virtual void UpdateEnabledState()
		{
			var command = Command;
			if (command != null)
			{
				var targetControl = AssociatedObject as Control;
				if (targetControl != null)
				{
					targetControl.IsEnabled = command.CanExecute(CommandParameter);
				}
			}
		}
	}
}
#endif
#endif
