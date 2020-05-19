#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2013-04-03 16:04:19Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace Codon.Navigation
{
	/// <summary>
	/// This class is used for Android, iOS, or any other platform 
	/// that doesn't adhere to a navigation model using either a URI or Type.
	/// </summary>
	public class RoutingService : IRoutingService
	{
		readonly Dictionary<string, Action<object>> pathDictionary 
			= new Dictionary<string, Action<object>>();

		readonly ReaderWriterLockSlim dictionaryLock = new ReaderWriterLockSlim();

		public void RegisterPath(string url, Action<object> navigationAction)
		{
			AssertArg.IsNotNullOrWhiteSpace(url, nameof(url));
			AssertArg.IsNotNull(navigationAction, nameof(navigationAction));

			dictionaryLock.EnterWriteLock();
			try
			{
				pathDictionary[url] = navigationAction;
			}
			finally
			{
				dictionaryLock.ExitWriteLock();
			}
		}

		public virtual void Navigate(
			string relativeUrl, 
			object navigationArgument = null)
		{
			AssertArg.IsNotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl));

			bool hasAction;
			Action<object> action;

			dictionaryLock.EnterReadLock();
			try
			{
				hasAction = pathDictionary.TryGetValue(relativeUrl, out action);
			}
			finally
			{
				dictionaryLock.ExitReadLock();
			}

			if (hasAction)
			{
				action(navigationArgument);
			}
			else
			{
				throw new ArgumentException(
					"Unknown path: " + relativeUrl);
			}
		}
	}
}