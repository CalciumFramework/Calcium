#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-10-29 17:45:22Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Codon.ComponentModel;
using Codon.Reflection;
using Codon.Services;

namespace Codon.Messaging
{
	/// <summary>
	/// Default implementation of the <see cref="IMessenger"/> interface.
	/// See the <see cref="IMessenger"/> interface for
	/// API documentation.
	/// </summary>
	public class Messenger : IMessenger
	{
		readonly Dictionary<Type, List<WeakReference>> subscriberLists 
			= new Dictionary<Type, List<WeakReference>>();

		readonly Dictionary<Type, List<Type>> registeredTypes 
			= new Dictionary<Type, List<Type>>();

		readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

		readonly Dictionary<Type, List<Type>> typeInterfaceCache 
			= new Dictionary<Type, List<Type>>();

		public void Subscribe(object subscriber)
		{
			AssertArg.IsNotNull(subscriber, nameof(subscriber));

			var weakReference = new WeakReference(subscriber);

			Type subscriberType = subscriber.GetType();

			List<Type> subscriptionInterfaces;
			if (!typeInterfaceCache.TryGetValue(subscriberType, out subscriptionInterfaces))
			{
				var implementedInterfaces = subscriberType.GetTypeInfo().ImplementedInterfaces;
				
				foreach (Type implementedInterface in implementedInterfaces)
				{
					if (implementedInterface.IsGenericType()
						&& implementedInterface.GetGenericTypeDefinition()
								== typeof(IMessageSubscriber<>))
					{
						if (subscriptionInterfaces == null)
						{
							subscriptionInterfaces = new List<Type>();
						}

						subscriptionInterfaces.Add(implementedInterface);
					}
				}

				typeInterfaceCache[subscriberType] = subscriptionInterfaces;
			}

			if (subscriptionInterfaces == null)
			{
				/* No IMessageSubscriber interfaces implemented by the subsciber type. */
				return;
			}

			lockSlim.EnterWriteLock();
			try
			{
				foreach (Type interfaceType in subscriptionInterfaces)
				{
					bool possibleDuplicate = false;
					List<Type> typesForThisMessage;
					if (registeredTypes.TryGetValue(interfaceType, out typesForThisMessage))
					{
						if (typesForThisMessage.Contains(subscriberType))
						{
							possibleDuplicate = true;
						}
					}
					else
					{
						typesForThisMessage = new List<Type>();
						registeredTypes[interfaceType] = typesForThisMessage;
					}

					typesForThisMessage.Add(subscriberType);

					List<WeakReference> subscribers = GetSubscribersNonLocking(interfaceType);

					if (possibleDuplicate)
					{
						/* We may want to improve search complexity here for large sets; 
						* perhaps using a ConditionalWeakTable. */
						foreach (WeakReference reference in subscribers)
						{
							if (reference.Target == subscriber)
							{
								return;
							}
						}
					}

					subscribers.Add(weakReference);
				}
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public void Unsubscribe(object subscriber)
		{
			lockSlim.EnterWriteLock();
			try
			{
				foreach (KeyValuePair<Type, List<WeakReference>> pair in subscriberLists.ToList())
				{
					List<WeakReference> list = pair.Value;

					list?.RemoveAll(x => x.Target == subscriber);
				}
			}
			finally
			{
				lockSlim.ExitWriteLock();
			}
		}

		public async Task PublishAsync<TEvent>(TEvent eventToPublish,
			bool requireUIThread = false,
			Type recipientType = null,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0)
		{
			var subscriberType = typeof(IMessageSubscriber<>).MakeGenericType(typeof(TEvent));
			var subscribers = GetSubscribersLocking(subscriberType);
			List<WeakReference> subscribersToRemove = new List<WeakReference>();

			if (requireUIThread)
			{
				await UIContext.Instance.SendAsync(
					() => PublishCoreAsync(eventToPublish, subscribers, 
								subscribersToRemove, true, recipientType, 
								memberName, filePath, lineNumber));
			}
			else
			{
				await PublishCoreAsync(eventToPublish, subscribers, 
					subscribersToRemove, false, recipientType, 
					memberName, filePath, lineNumber).ConfigureAwait(false);
			}

			if (subscribersToRemove.Any())
			{
				lockSlim.EnterWriteLock();
				try
				{
					foreach (var remove in subscribersToRemove)
					{
						subscribers.Remove(remove);
					}
				}
				finally
				{
					lockSlim.ExitWriteLock();
				}
			}
		}

		async Task PublishCoreAsync<TEvent>(
			TEvent eventToPublish, 
			List<WeakReference> subscribers, 
			List<WeakReference> subscribersToRemove, 
			bool continueOnCapturedContext, 
			Type recipientType, 
			string memberName, string filePath, int lineNumber)
		{
			var exceptionHandler = ExceptionHandler;

			foreach (WeakReference weakSubscriber in subscribers.ToArray())
			{
				var subscriber = (IMessageSubscriber<TEvent>)weakSubscriber.Target;
				if (subscriber != null)
				{
					var subscriberType = subscriber.GetType();

					if (recipientType != null 
						&& !recipientType.IsAssignableFromEx(subscriberType))
					{
						continue;
					}

					if (exceptionHandler == null)
					{
						await subscriber.ReceiveMessageAsync(eventToPublish).ConfigureAwait(continueOnCapturedContext);
					}
					else
					{
						try
						{
							await subscriber.ReceiveMessageAsync(eventToPublish).ConfigureAwait(continueOnCapturedContext);
						}
						catch (Exception ex)
						{
							bool rethrow = exceptionHandler.ShouldRethrowException(ex, this, memberName, filePath, lineNumber);
							if (rethrow)
							{
								throw;
							}
						}
					}
				}
				else
				{
					subscribersToRemove.Add(weakSubscriber);
				}
			}
		}

		List<WeakReference> GetSubscribersLocking(Type subscriberType)
		{
			lockSlim.EnterReadLock();
			try
			{
				return GetSubscribersNonLocking(subscriberType);
			}
			finally
			{
				lockSlim.ExitReadLock();
			}
		}

		List<WeakReference> GetSubscribersNonLocking(Type subscriberType)
		{
			List<WeakReference> subscribers;

			if (!subscriberLists.TryGetValue(subscriberType, out subscribers))
			{
				subscribers = new List<WeakReference>();
				subscriberLists.Add(subscriberType, subscribers);
			}

			return subscribers;
		}

		IExceptionHandler settableExceptionHandler;
		int retrieveAttempts;

		/// <summary>
		/// When an exception occurs during execution or during evaluating 
		/// if the command can execute, then the exception is passed to the exception manager.
		/// If <c>null</c> the IoC container is used to locate an instance.
		/// </summary>
		public IExceptionHandler ExceptionHandler
		{
			get
			{
				/* We limit the attempts to retrieve the exception handler; 
				 * otherwise it would slow things down unnecessarily. */
				if (settableExceptionHandler == null 
					&& retrieveAttempts++ < 3)
				{
					if (Dependency.Initialized)
					{
						Dependency.TryResolve(out settableExceptionHandler);
					}
				}

				return settableExceptionHandler;
			}
			set => settableExceptionHandler = value;
		}
	}
}
