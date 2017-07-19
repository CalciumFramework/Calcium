#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2008-11-03 23:22:06Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Codon.InversionOfControl;

namespace Codon.Concurrency
{
	/// <summary>
	/// Used to allow calls to be marshaled 
	/// to a particular thread.
	/// <seealso cref="System.Threading.SynchronizationContext"/>
	/// </summary>
	[DefaultType(typeof(UISynchronizationContext), Singleton = true)]
	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Concurrency) + 
		".UISynchronizationContext, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface ISynchronizationContext
	{
		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method returns immediately upon queuing the request.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <param name="memberName">The caller member name, 
		/// which indicates the property or method location of the method call.</param>
		/// <param name="filePath">The caller file path, 
		/// which indicates the file path location of the method call.</param>
		/// <param name="lineNumber">The caller line number, 
		/// which indicates the line number of the method call.</param>
		void Post(Action action,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0);

		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method blocks until the specified callback completes.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <param name="memberName">The caller member name, 
		/// which indicates the property or method location of the method call. 
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="filePath">The caller file path, 
		/// which indicates the file path location of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="lineNumber">The caller line number, 
		/// which indicates the line number of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		Task PostAsync(
			Action action,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0);

		/// <summary>
		/// If the calling thread is matches the thread used by the synchronization context
		/// then the action is invoked on the calling thread; if not, the action is invoked 
		/// on the synchronization context thread.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="memberName">The caller member name, 
		/// which indicates the property or method location of the method call. 
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="filePath">The caller file path, 
		/// which indicates the file path location of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="lineNumber">The caller line number, 
		/// which indicates the line number of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		void Send(
			Action action,
			[CallerMemberName]string memberName = null,
			[CallerFilePath]string filePath = null,
			[CallerLineNumber]int lineNumber = 0);

		/// <summary>
		/// Invokes the specified callback asynchronously.
		/// Method blocks until the specified callback completes.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <param name="memberName">The caller member name, 
		/// which indicates the property or method location of the method call. 
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="filePath">The caller file path, 
		/// which indicates the file path location of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		/// <param name="lineNumber">The caller line number, 
		/// which indicates the line number of the method call.
		/// This parameter is optional and in most cases should not be supplied.</param>
		Task SendAsync(
			Func<Task> action, 
			[CallerMemberName] string memberName = null, 
			[CallerFilePath] string filePath = null, 
			[CallerLineNumber] int lineNumber = 0);

		/// <summary>
		/// Gets a value indicating whether invocation is required.
		/// That is, it determines whether the call was made from a thread other 
		/// than the one that the current instance was created on.
		/// </summary>
		/// <value><c>true</c> if the calling thread was not the thread that the current instance 
		/// was initialized on; otherwise, <c>false</c>.</value>
		bool InvokeRequired { get; }

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Indicates whether the initialize method needs 
		/// to be called on the main thread before use.
		/// </summary>
		bool InitializeRequired { get; }
	}
}
