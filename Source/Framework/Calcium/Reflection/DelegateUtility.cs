using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Calcium.Reflection
{
	class DelegateUtility
	{
		/// <summary>
		/// Dynamically adds an event handler to a control.
		/// Source: http://stackoverflow.com/questions/5658765/create-a-catch-all-handler-for-all-events-and-delegates-in-c-sharp
		/// </summary>
		/// <param name="target">The control on which we want to add the event handler.</param>
		/// <param name="eventName">The name of the event on which we want to add a handler.</param>
		/// <param name="methodToExecute">The code we want to execute when the handler is raised.</param>
		/// <returns>An action that can be used to remove the subscription.</returns>
		internal static Action AddHandler(object target, string eventName, Action methodToExecute)
		{
#if NETSTANDARD
			var eventInfo = target.GetType().GetRuntimeEvent(eventName);
#else
			var eventInfo = target.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
#endif
			if (eventInfo == null)
			{
				throw new ArgumentException($"{nameof(eventName)} '{eventName}' does not exist on target of type '{target.GetType()}'");
			}

			var delegateType = eventInfo.EventHandlerType;
			var dynamicHandler = BuildDynamicHandler(delegateType, methodToExecute);

#if NETSTANDARD
			MethodInfo methodInfo = eventInfo.AddMethod;
#else
			MethodInfo methodInfo = eventInfo.GetAddMethod();
#endif
			var invoker = ReflectionCache.GetVoidMethodInvoker(
							methodInfo,
							DelegateCreationMode.FastCreationSlowPerformance);
			invoker(target, new object[] { dynamicHandler });

			void Result() => RemoveHandler(target, eventName, dynamicHandler);

			return Result;
		}

		internal static void RemoveHandler(object target, string eventName, Delegate dynamicHandler)
		{
#if NETSTANDARD
			var eventInfo = target.GetType().GetRuntimeEvent(eventName);
#else
			var eventInfo = target.GetType().GetEvent(eventName);
#endif
			if (eventInfo == null)
			{
				throw new ArgumentException(nameof(eventName) + "does not exist on target of type:" + target.GetType());
			}

#if NETSTANDARD
			MethodInfo methodInfo = eventInfo.RemoveMethod;
#else
			MethodInfo methodInfo = eventInfo.GetRemoveMethod();
#endif
			var invoker = ReflectionCache.GetVoidMethodInvoker(
								methodInfo,
								DelegateCreationMode.FastCreationSlowPerformance);
			invoker(target, new object[] { dynamicHandler });
		}

		/// <summary>
		/// Build a delegate for a particular type.
		/// Code by Thomas Lebrun http://bit.ly/1OQsD8L
		/// </summary>
		/// <param name="delegateType">The type of the object for which we want the delegate.</param>
		/// <param name="methodToExecute">The code we want to execute when the handler is raised.</param>
		/// <returns>A delegate object for the dedicated type, used the execute the specified code.</returns>
		static Delegate BuildDynamicHandler(Type delegateType, Action methodToExecute)
		{
#if NETSTANDARD
			MethodInfo invokeMethod = delegateType.GetRuntimeMethods().FirstOrDefault(x => x.Name == "Invoke");
#else
			MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
#endif
			ParameterExpression[] parameters = invokeMethod.GetParameters().Select(
				parameterInfo => Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name)).ToArray();

			ConstantExpression instance = methodToExecute.Target == null
				? null : Expression.Constant(methodToExecute.Target);

			MethodCallExpression call = Expression.Call(instance, methodToExecute.GetMethodInfo());
			Expression body = invokeMethod.ReturnType == typeof(void)
				? (Expression)call
				: Expression.Convert(call, invokeMethod.ReturnType);
			LambdaExpression expression = Expression.Lambda(delegateType, body, parameters);

			return expression.Compile();
		}

		internal static Action AddHandler(object target, string eventName, Action<object, EventArgs> methodToExecute)
		{
#if NETSTANDARD
			var eventInfo = target.GetType().GetRuntimeEvent(eventName);
#else
			var eventInfo = target.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
#endif
			if (eventInfo == null)
			{
				throw new ArgumentException(
					nameof(eventName) + " " + eventName
					+ " does not exist on target of type:"
					+ target.GetType());
			}

			var delegateType = eventInfo.EventHandlerType;
			var dynamicHandler = BuildDynamicHandler(delegateType, methodToExecute);

#if NETSTANDARD
			var methodInfo = eventInfo.AddMethod;
#else
			var methodInfo = eventInfo.GetAddMethod();
#endif
			var invoker = ReflectionCache.GetVoidMethodInvoker(
							methodInfo,
							DelegateCreationMode.FastCreationSlowPerformance);

			invoker(target, new object[] { dynamicHandler });

			void Result() => RemoveHandler(target, eventName, dynamicHandler);

			return Result;
		}

		static IReflectionCache reflectionCacheUseProperty;
		static IReflectionCache ReflectionCache
			=> reflectionCacheUseProperty
			?? (reflectionCacheUseProperty = Dependency.Resolve<IReflectionCache>());

		static Delegate BuildDynamicHandler(Type delegateType, Action<object, EventArgs> methodToExecute)
		{
#if NETSTANDARD
			MethodInfo invokeMethod = delegateType.GetRuntimeMethods().First(x => x.Name == "Invoke");
#else
			MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
#endif
			ParameterExpression[] parameters = invokeMethod.GetParameters().Select(
				parameterInfo => Expression.Parameter(parameterInfo.ParameterType, parameterInfo.Name)).ToArray();

			ConstantExpression instance = methodToExecute.Target == null
											? null : Expression.Constant(methodToExecute.Target);

			MethodCallExpression callExpression = Expression.Call(instance, methodToExecute.GetMethodInfo(), parameters);
			Expression body = invokeMethod.ReturnType == typeof(void)
				? (Expression)callExpression
				: Expression.Convert(callExpression, invokeMethod.ReturnType);
			LambdaExpression expression = Expression.Lambda(delegateType, body, parameters);

			return expression.Compile();
		}
	}
}
