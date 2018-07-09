using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Codon.Logging;
using Codon.MissingTypes.System.Windows.Data;
using Codon.Reflection;

namespace Codon.UI.Data
{
	partial class InternalBindingApplicator
	{
#if NETSTANDARD
		static MethodInfo commandExecuteMethodInfo = typeof(ICommand).GetTypeInfo().GetDeclaredMethod(nameof(ICommand.Execute));
#else
		static MethodInfo commandExecuteMethodInfo = typeof(ICommand).GetMethod(nameof(ICommand.Execute), new[] { typeof(object) });
#endif

		void Bind(BindingExpression bindingExpression,
			object dataContext,
			object originalDataContext,
			string[] sourcePath,
			IValueConverter converter,
			PropertyInfo targetProperty,
			IList<Action> localRemoveActions,
			IList<Action> globalRemoveActions,
			int position)
		{
			int pathSplitLength = sourcePath.Length;
			
			/* If the path is empty we assign the data context to the target property 
			 * or call the target method (if applicable). */
			if (pathSplitLength < 1 || pathSplitLength == 1 && sourcePath[0] == ".")
			{
				if (targetProperty == null)
				{
					/* If the Source property of the data context 
					 * is not a property, check if it's a method. */
					var viewType = bindingExpression.View.GetType();

					MethodInfo targetMethod = null;
					Type dataContextType = dataContext.GetType();

					try
					{
#if NETSTANDARD
						targetMethod = viewType.GetRuntimeMethod(bindingExpression.Target, new[] { dataContextType });
#else
						targetMethod = viewType.GetMethod(bindingExpression.Target, new[] { dataContextType });
#endif
					}
					catch (Exception)
					{
						/* Ignore. */
					}
					
					if (targetMethod == null)
					{
#if NETSTANDARD
						targetMethod = viewType.GetRuntimeMethods().FirstOrDefault(
							x => x.Name == bindingExpression.Target && x.IsPublic);
#else
						targetMethod = viewType.GetMethod(bindingExpression.Target,
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
#endif
					}

					if (targetMethod == null)
					{
						throw new InvalidOperationException($"Target property {bindingExpression.Target} does not exist "
															+ $"on {bindingExpression.View?.GetType().Name}.");
					}

					CallTargetMethod(targetMethod, dataContext, bindingExpression.View, converter, bindingExpression.ConverterParameter);
				}
				else
				{
					SetTargetProperty(targetProperty, dataContext, bindingExpression.View, converter, bindingExpression.ConverterParameter);
				}

				return;
			}

			object currentContext = dataContext;

			int lastIndex = pathSplitLength - 1;
			PropertyBinding[] propertyBinding = new PropertyBinding[1];

			for (int i = position; i < pathSplitLength; i++)
			{
				if (currentContext == null)
				{
					break;
				}

				var inpc = currentContext as INotifyPropertyChanged;

				string sourceSegment = sourcePath[i];

#if NETSTANDARD
				var sourceProperty = currentContext.GetType().GetRuntimeProperty(sourceSegment);
#else
				var sourceProperty = currentContext.GetType().GetProperty(sourceSegment,
							BindingFlags.Instance
							| BindingFlags.NonPublic
							| BindingFlags.Public);
#endif

				if (i == lastIndex) /* The value. */
				{
					/* Add a property binding between the source (the viewmodel) 
					 * and the target (the view) so we can update the target property 
					 * when the source property changes (a OneWay binding). */
					propertyBinding[0] = new PropertyBinding
					{
						SourceProperty = sourceProperty,
						TargetProperty = targetProperty,
						Converter = converter,
						ConverterParameter = bindingExpression.ConverterParameter,
						View = bindingExpression.View
					};

					{
						/* When this value changes, the value must be pushed to the target. 
						 * (Unless it's OneWayToSource or OneTime) */
						var mode = bindingExpression.Mode;
						if (sourceProperty != null && inpc != null && (mode != BindingMode.OneTime && mode != BindingMode.OneWayToSource))
						{
							object context = currentContext;

							void HandlePropertyChanged(object sender, PropertyChangedEventArgs args)
							{
								string propertyName = args.PropertyName;
								if (propertyName != sourceSegment && !string.IsNullOrEmpty(propertyName))
								{
									return;
								}

								PropertyBinding binding = propertyBinding[0];

								if (binding != null && sourceProperty != null)
								{
									if (binding.PreventUpdateForTargetProperty)
									{
										return;
									}

									try
									{
										binding.PreventUpdateForSourceProperty = true;

										if (binding.TargetMethod == null)
										{
											SetTargetProperty(sourceProperty, context, binding.View, binding.TargetProperty, binding.Converter, binding.ConverterParameter);
										}
										else
										{
											CallTargetMethod(binding.TargetMethod, sourceProperty, context, binding.View, binding.Converter, binding.ConverterParameter);
										}
									}
									finally
									{
										binding.PreventUpdateForSourceProperty = false;
									}
								}
							}

							inpc.PropertyChanged += HandlePropertyChanged;

							void RemoveHandler()
							{
								inpc.PropertyChanged -= HandlePropertyChanged;
								propertyBinding[0] = null;
							}

							localRemoveActions.Add(RemoveHandler);
							globalRemoveActions.Add(RemoveHandler);
						}
					}

					/* Determine if the target is an event, 
					 * in which case use that to trigger an update. */
#if NETSTANDARD
					string targetName = bindingExpression.Target;
					if (string.IsNullOrEmpty(targetName))
					{
						throw new BindingException($"Target is null or empty. Source: {bindingExpression.Source}, Path: {bindingExpression.Path}, View: {bindingExpression.View}");
					}

					var bindingEvent = bindingExpression.View.GetType().GetRuntimeEvent(bindingExpression.Target);
#else
					var bindingEvent = bindingExpression.View.GetType().GetEvent(bindingExpression.Target);
#endif
					if (bindingEvent != null)
					{
						/* The target is an event of the view. */
						if (sourceProperty != null)
						{
							Func<object, object> getter = ReflectionCache.GetPropertyGetter(sourceProperty);

							/* The source must be an ICommand so we can call its Execute method. */
							var command = getter(currentContext) as ICommand;
							
							if (command == null)
							{
								throw new InvalidOperationException(
									$"The source property {bindingExpression.Path}, "
									+ $"bound to the event {bindingEvent.Name}, "
									+ "needs to implement the interface ICommand.");
							}

							/* Subscribe to the specified event to execute 
							 * the command when the event is raised. */
							var invoker = ReflectionCache.GetVoidMethodInvoker(commandExecuteMethodInfo);

							void InvokeAction()
							{
								invoker(command, new object[] {null});
							}

							Action removeAction = DelegateUtility.AddHandler(bindingExpression.View, bindingExpression.Target, (Action)InvokeAction);
							localRemoveActions.Add(removeAction);
							globalRemoveActions.Add(removeAction);

							/* Subscribe to the CanExecuteChanged event of the command 
							 * to disable or enable the view associated to the command. */
							var view = bindingExpression.View;

#if NETSTANDARD
							PropertyInfo enabledProperty = view.GetType().GetRuntimeProperty(viewEnabledPropertyName);
#else
							PropertyInfo enabledProperty = view.GetType().GetProperty(viewEnabledPropertyName);
#endif

							if (enabledProperty != null)
							{
								var setter = ReflectionCache.GetPropertySetter(enabledProperty);
								setter(view, command.CanExecute(null));

								Action canExecuteChangedAction = () => setter(view, command.CanExecute(null));
								removeAction = DelegateUtility.AddHandler(
									command, nameof(ICommand.CanExecuteChanged), canExecuteChangedAction);

								localRemoveActions.Add(removeAction);
								globalRemoveActions.Add(removeAction);
							}
						}
						else /* sourceProperty == null */
						{
							/* If the Source property of the data context 
							 * is not a property, check if it's a method. */
							var contextType = currentContext.GetType();
#if NETSTANDARD
							var sourceMethod = contextType.GetRuntimeMethods().FirstOrDefault(x => x.Name == sourceSegment);
#else
							var sourceMethod = contextType.GetMethod(sourceSegment,
								BindingFlags.Public | BindingFlags.NonPublic
								| BindingFlags.Instance | BindingFlags.Static);
#endif
							if (sourceMethod == null)
							{
								throw new InvalidOperationException(
									$"No property or event named {bindingExpression.Path} "
									+ $"on object of type {contextType} "
									+ $"found to bind it to the event {bindingEvent.Name}.");
							}

							var parameterCount = sourceMethod.GetParameters().Length;
							if (parameterCount > 3)
							{
								/* Only calls to methods with 0, 1, 2 or three parameters are supported. */
								throw new InvalidOperationException(
									$"Method {sourceMethod.Name} should accept three or fewer parameters "
									+ $"to be called when event {bindingEvent.Name} is raised. The signature should resemble one of the following:" + Environment.NewLine
									+ $"void MethodName() {Environment.NewLine}"
									+ $"void MethodName(object dataContext, EventArgs e) {Environment.NewLine}"
									+ "void MethodName(object dataContext, EventArgs e, object sender)");
							}

							/* It's a method therefore subscribe to the specified event 
							 * to execute the method when the event is raised. */
							var context = currentContext;

							Action removeAction;

							Action<object, object[]> invoker = ReflectionCache.GetVoidMethodInvoker(sourceMethod);

							if (parameterCount > 1)
							{
								removeAction = DelegateUtility.AddHandler(bindingExpression.View,
									bindingExpression.Target,
									(sender, e) =>
									{
										var args = parameterCount > 2 ? new[] {originalDataContext, e, bindingExpression.View} : new[] {originalDataContext, e};
										invoker(context, args);
									});
							}
							else
							{
								removeAction = DelegateUtility.AddHandler(bindingExpression.View,
									bindingExpression.Target,
									() =>
									{
										var args = parameterCount > 0 ? new[] {originalDataContext} : null;
										invoker(context, args);
									});
							}

							localRemoveActions.Add(removeAction);
							globalRemoveActions.Add(removeAction);
						}
					}
					else /* bindingEvent == null */
					{
						if (sourceProperty == null)
						{
							throw new InvalidOperationException(
								$"Source property {bindingExpression.Path} does not exist "
								+ $"on {currentContext?.GetType().Name ?? "null"}.");
						}

						MethodInfo targetMethod = null;

						if (targetProperty == null)
						{
							/* If the Source property of the data context 
							 * is not a property, check if it's a method. */
							var viewType = bindingExpression.View.GetType();

							try
							{
#if NETSTANDARD
								targetMethod = viewType.GetRuntimeMethod(bindingExpression.Target, new[] { sourceProperty.PropertyType });
#else
								targetMethod = viewType.GetMethod(bindingExpression.Target, new[] { sourceProperty.PropertyType });
#endif
							}
							catch (Exception)
							{
								/* Ignore. */
							}

							if (targetMethod == null)
							{
#if NETSTANDARD
								targetMethod = viewType.GetRuntimeMethods().FirstOrDefault(x => x.Name == bindingExpression.Target && x.IsPublic);
#else
								targetMethod = viewType.GetMethod(bindingExpression.Target,
									BindingFlags.Public | BindingFlags.NonPublic
									| BindingFlags.Instance | BindingFlags.Static);
#endif
							}

							if (targetMethod == null)
							{
								throw new InvalidOperationException(
									$"Target property {bindingExpression.Target} does not exist "
									+ $"on {bindingExpression.View?.GetType().Name}.");
							}

							propertyBinding[0].TargetMethod = targetMethod;
						}

						var mode = bindingExpression.Mode;
						if (mode != BindingMode.OneWayToSource)
						{
							propertyBinding[0].PreventUpdateForSourceProperty = true;
							try
							{
								object closureContext = currentContext;

								UIContext.Instance.Send(() => 
								{
									/* Set initial binding value. */
									if (targetMethod == null)
									{
										SetTargetProperty(sourceProperty, closureContext, 
											bindingExpression.View, targetProperty, converter, 
											bindingExpression.ConverterParameter);
									}
									else
									{
										CallTargetMethod(targetMethod, sourceProperty, 
											closureContext, bindingExpression.View, 
											converter, bindingExpression.ConverterParameter);
									}
								});
							}
							finally
							{
								propertyBinding[0].PreventUpdateForSourceProperty = false;
							}
						}

						if (mode == BindingMode.TwoWay || mode == BindingMode.OneWayToSource)
						{
							/* TwoWay bindings require that the ViewModel property be updated 
							 * when an event is raised on the bound view. */
							string changedEvent = bindingExpression.ViewValueChangedEvent;
							if (!string.IsNullOrWhiteSpace(changedEvent))
							{
								var context = currentContext;

								Action changeAction = () =>
								{
									var pb = propertyBinding[0];
									if (pb == null)
									{
										return;
									}

									ViewValueChangedHandler.HandleViewValueChanged(pb, context);
								};

								var view = bindingExpression.View;
								var removeHandler = DelegateUtility.AddHandler(view, changedEvent, changeAction);

								localRemoveActions.Add(removeHandler);
								globalRemoveActions.Add(removeHandler);
							}
							else
							{
								var binding = propertyBinding[0];
								IViewBinder binder;
								if (ViewBinderRegistry.TryGetViewBinder(
										binding.View.GetType(), binding.TargetProperty.Name, out binder))
								{
									var unbindAction = binder.BindView(binding, currentContext);
									if (unbindAction != null)
									{
										localRemoveActions.Add(unbindAction);
										globalRemoveActions.Add(unbindAction);
									}
								}
								else
								{
									var viewInpc = binding.View as INotifyPropertyChanged;
									if (viewInpc != null)
									{
										var unbindAction = InpcTargetBinder.BindTarget(binding, currentContext);

										if (unbindAction != null)
										{
											localRemoveActions.Add(unbindAction);
											globalRemoveActions.Add(unbindAction);
										}
									}
									else if (Debugger.IsAttached)
									{
										var log = Dependency.Resolve<ILog>();
										log.Debug($"Unable to locate value changed event for two way binding with target {bindingExpression.Target} "
											+ $"on {bindingExpression.View?.GetType().Name}. Target does not implement INotifyPropertyChanged.");
										Debugger.Break();
									}
								}
							}
						}
					}
				}
				else
				{
					/* The source is a child of another object, 
					 * therefore we must subscribe to the parents PropertyChanged event 
					 * and re-bind when the child changes. */

					if (inpc != null && bindingExpression.Mode != BindingMode.OneTime)
					{
						var context = currentContext;

						var iCopy = i;

						PropertyChangedEventHandler handler
							= delegate (object sender, PropertyChangedEventArgs args)
							{
								string propertyName = args.PropertyName;
								if (propertyName != sourceSegment && !string.IsNullOrEmpty(propertyName))
								{
									return;
								}

								/* Remove existing child event subscribers. */
								var lastActionIndex = localRemoveActions.Count - 1;

								var removeActions = new List<Action>();

								for (int j = position; j < lastActionIndex; j++)
								{
									var removeAction = localRemoveActions[j];
									try
									{
										removeAction();
									}
									catch (Exception)
									{
										var log = Dependency.Resolve<ILog>();
										if (log.IsLogEnabledForLevel(LogLevel.Debug))
										{
											log.Debug("Binding failed. PropertyName: " + propertyName);
										}
									}

									removeActions.Add(removeAction);
								}

								foreach (var removeAction in removeActions)
								{
									localRemoveActions.Remove(removeAction);
									globalRemoveActions.Remove(removeAction);
								}

								propertyBinding[0] = null;

								/* Bind child bindings. */
								Bind(bindingExpression,
									context,
									originalDataContext,
									sourcePath,
									converter,
									targetProperty,
									localRemoveActions, globalRemoveActions, iCopy);
							};

						inpc.PropertyChanged += handler;

						Action removeHandler = () =>
						{
							inpc.PropertyChanged -= handler;
							propertyBinding[0] = null;
						};

						localRemoveActions.Add(removeHandler);
						globalRemoveActions.Add(removeHandler);
					}

					if (sourceProperty != null)
					{
						var getter = ReflectionCache.GetPropertyGetter(sourceProperty);
						currentContext = getter(currentContext);
					}
					else
					{
						currentContext = null;
					}
				}
			}
		}
	}
}