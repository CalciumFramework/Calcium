#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-08-12 14:13:55Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Calcium.Concurrency
{
	/// <summary>
	/// Extension methods for the <see cref="ReaderWriterLockSlim"/>
	/// class. These extension methods allow you to replace
	/// the verbose <c>ReaderWriterLockSlim</c> Enter* and Exit* calls.
	/// </summary>
	public static class ReaderWriterLockSlimExtensions
	{
		/// <summary>
		/// Executes the specified <c>Action</c> 
		/// within a <c>EnterReadLock</c> and <c>ExitReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="action">
		/// The action to execute using the lock.</param>
		public static void ReadLock(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Action action)
		{
			readerWriterLockSlim.EnterReadLock();
			try
			{
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		/// <summary>
		/// Executes the specified asynchronous <c>Action</c> 
		/// within a <c>EnterReadLock</c> and <c>ExitReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="action">
		/// The action to execute using the lock.</param>
		public static async Task ReadLockAsync(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task> action)
		{
			readerWriterLockSlim.EnterReadLock();
			try
			{
				await action();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Func</c> 
		/// within a <c>EnterReadLock</c> and <c>ExitReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified func being invoked.</param>
		/// <param name="func">
		/// The action to execute using the lock.</param>
		/// <returns>The result of the Func.</returns>
		public static T ReadLock<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<T> func)
		{
			readerWriterLockSlim.EnterReadLock();
			try
			{
				return func();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		/// <summary>
		/// Executes the specified asynchronous <c>Func</c> 
		/// within a <c>EnterReadLock</c> and <c>ExitReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified func being invoked.</param>
		/// <param name="func">
		/// The action to execute using the lock.</param>
		/// <returns>The result of the Func.</returns>
		public static async Task<T> ReadLockAsync<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task<T>> func)
		{
			readerWriterLockSlim.EnterReadLock();
			try
			{
				return await func();
			}
			finally
			{
				readerWriterLockSlim.ExitReadLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Action</c> 
		/// within a <c>EnterWriteLock</c> and <c>ExitWriteLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="action">
		/// The action to execute using the lock.</param>
		public static void WriteLock(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Action action)
		{
			readerWriterLockSlim.EnterWriteLock();
			try
			{
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// Executes the specified asynchronous <c>Action</c> 
		/// within a <c>EnterWriteLock</c> and <c>ExitWriteLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		public static async Task WriteLockAsync(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task> func)
		{
			readerWriterLockSlim.EnterWriteLock();
			try
			{
				await func();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Action</c> 
		/// within a <c>EnterWriteLock</c> and <c>ExitWriteLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		/// <returns>The result of the invoked <c>Func</c>.</returns>
		public static T WriteLock<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<T> func)
		{
			readerWriterLockSlim.EnterWriteLock();
			try
			{
				return func();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// Executes the specified asychronous <c>Action</c> 
		/// within a <c>EnterWriteLock</c> and <c>ExitWriteLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		/// <returns>The result of the invoked <c>Func</c>.</returns>
		public static async Task<T> WriteLockAsync<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task<T>> func)
		{
			readerWriterLockSlim.EnterWriteLock();
			try
			{
				return await func();
			}
			finally
			{
				readerWriterLockSlim.ExitWriteLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Action</c> 
		/// within a <c>EnterUpgradeableReadLock</c> 
		/// and <c>ExitUpgradeableReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="action">
		/// The <c>Action</c> to invoke within the lock.</param>
		public static void UpgradeableReadLock(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Action action)
		{
			readerWriterLockSlim.EnterUpgradeableReadLock();
			try
			{
				action();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Func</c> 
		/// within a <c>EnterUpgradeableReadLock</c> 
		/// and <c>ExitUpgradeableReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		/// <returns>The result of the invoked <c>Func</c>.</returns>
		public static async Task UpgradeableReadLockAsync(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task> func)
		{
			readerWriterLockSlim.EnterUpgradeableReadLock();
			try
			{
				await func();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Executes the specified <c>Func</c> 
		/// within a <c>EnterUpgradeableReadLock</c> 
		/// and <c>ExitUpgradeableReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		/// <returns>The result of the invoked <c>Func</c>.</returns>
		public static T UpgradeableReadLock<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<T> func)
		{
			readerWriterLockSlim.EnterUpgradeableReadLock();
			try
			{
				return func();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}

		/// <summary>
		/// Executes the specified asychronous <c>Func</c> 
		/// within a <c>EnterUpgradeableReadLock</c> 
		/// and <c>ExitUpgradeableReadLock</c>
		/// block using the specified <c>ReaderWriterLockSlim</c>.
		/// </summary>
		/// <param name="readerWriterLockSlim">
		/// The lock that is used to prevent access 
		/// when the specified action being invoked.</param>
		/// <param name="func">
		/// The <c>Func</c> to invoke within the lock.</param>
		/// <returns>The result of the invoked <c>Func</c>.</returns>
		public static async Task<T> UpgradeableReadLockAsync<T>(
			this ReaderWriterLockSlim readerWriterLockSlim, 
			Func<Task<T>> func)
		{
			readerWriterLockSlim.EnterUpgradeableReadLock();
			try
			{
				return await func();
			}
			finally
			{
				readerWriterLockSlim.ExitUpgradeableReadLock();
			}
		}
	}
}
