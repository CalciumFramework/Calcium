#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-04-22 17:26:58Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Reflection;

namespace Calcium.Concurrency
{
	/// <summary>
	/// Allows a <see cref="Delegate"/> to be referenced directly
	/// or using a <see cref="WeakReference"/>.
	/// </summary>
	public class DelegateReference
	{
		readonly Delegate delegateToReference;
		readonly WeakReference delegateWeakReference;
		readonly MethodInfo method;
		readonly Type delegateType;

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateReference"/> class.
		/// </summary>
		/// <param name="delegateToReference">The target delegate.</param>
		/// <param name="useWeakReference">
		/// if set to <c>true</c> a weak reference 
		/// will be used for the target of the <see cref="Delegate"/>.
		/// </param>
		public DelegateReference(Delegate delegateToReference, bool useWeakReference)
		{
			AssertArg.IsNotNull(delegateToReference, nameof(delegateToReference));

			if (useWeakReference)
			{
				delegateWeakReference = new WeakReference(delegateToReference.Target);
#if NETSTANDARD || NETFX_CORE
				method = delegateToReference.GetMethodInfo();
#else
				method = delegateToReference.Method;
#endif
				delegateType = delegateToReference.GetType();
			}
			else
			{
				this.delegateToReference = delegateToReference;
			}
		}

		public Delegate Delegate => delegateToReference ?? GetDelegate();


#if NETSTANDARD || NETFX_CORE
		Delegate GetDelegate()
		{
			Delegate result = null;
			if (method.IsStatic)
			{
				result = method.CreateDelegate(delegateType, null);
			}
			else
			{
				object target = delegateWeakReference.Target;
				if (target != null)
				{
					result = method.CreateDelegate(delegateType, target);
				}
			}
			return result;
		}
#else
		Delegate GetDelegate()
		{
			Delegate result = null;
			if (method.IsStatic)
			{
				result = Delegate.CreateDelegate(delegateType, null, method);
			}
			else
			{
				object target = delegateWeakReference.Target;
				if (target != null)
				{
					result = Delegate.CreateDelegate(delegateType, target, method);
				}
			}
			return result;
		}
#endif
	}
}
