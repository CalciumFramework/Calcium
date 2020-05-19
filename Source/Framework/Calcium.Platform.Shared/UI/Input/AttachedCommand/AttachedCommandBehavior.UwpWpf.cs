#if WPF || WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

#if WINDOWS_UWP || NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

using Codon.Reflection;

namespace Codon.UIModel.Input
{
	/// <summary>
	/// This class works in conjunction 
	/// with <see cref="Codon.UI.Elements.AttachedCommand"/>
	/// to allow a command to be applied to a UI element.
	/// </summary>
	public class AttachedCommandBehavior
	{
		ICommand command;
		object commandParameter;
		WeakReference targetReference;
		readonly EventHandler canExecuteChangedHandler;
		Action removeHandlerAction;

		public AttachedCommandBehavior(DependencyObject targetElement)
		{
			canExecuteChangedHandler = HandleCanExecuteChangedHandler;

			Attach(targetElement);
		}

		void HandleCanExecuteChangedHandler(object o, EventArgs args)
		{
			UpdateEnabledState();
		}

		public void Detach()
		{
			if (removeHandlerAction != null)
			{
				removeHandlerAction();
				removeHandlerAction = null;
			}

			Command = null;
			EventName = null;
			CommandParameter = null;
			targetReference.Target = null;
		}

		public void Attach(DependencyObject targetElement)
		{
			if (targetReference == null)
			{
				targetReference = new WeakReference(targetElement);
			}
			else
			{
				targetReference.Target = targetElement;
			}
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
			Command?.Execute(CommandParameter);
		}

		public ICommand Command
		{
			get => command;
			set
			{
				if (command == value)
				{
					return;
				}

				if (command != null)
				{
					command.CanExecuteChanged -= canExecuteChangedHandler;
				}

				command = value;
				if (command != null)
				{
					command.CanExecuteChanged += canExecuteChangedHandler;
					UpdateEnabledState();
				}
			}
		}

		public object CommandParameter
		{
			get => commandParameter;
			set
			{
				if (commandParameter != value)
				{
					commandParameter = value;
					UpdateEnabledState();
				}
			}
		}

		string eventName;

		public string EventName
		{
			get => eventName;
			set
			{
				if (eventName != value)
				{
					eventName = value;
					if (string.IsNullOrEmpty(eventName))
					{
						return;
					}
					DependencyObject element = TargetElement;
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
						throw new ArgumentException(
							$"Event name '{eventName}' not found on FrameworkElement '{element}'");
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
						removeHandlerAction = DelegateUtility.AddHandler(element, eventInfo.Name, ExecuteCommand);
						return;
					}

					handlerCache[eventInfo.EventHandlerType] = handler;
				}
			}
		}

		static readonly Dictionary<Type, Delegate> handlerCache 
			= new Dictionary<Type, Delegate>();

		protected DependencyObject TargetElement 
			=> (DependencyObject)targetReference.Target;

		protected virtual void UpdateEnabledState()
		{
			if (TargetElement == null)
			{
				Command = null;
				CommandParameter = null;
			}
			else if (Command != null)
			{
				var targetControl = TargetElement as Control;
				if (targetControl != null)
				{
					targetControl.IsEnabled = Command.CanExecute(CommandParameter);
				}
			}
		}
	}
}
#endif