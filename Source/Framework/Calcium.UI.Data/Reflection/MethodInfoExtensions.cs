using System;
using System.Reflection;

namespace Codon.Reflection
{
    static class MethodInfoExtensions
    {
		public static MethodInfo GetMethodInfoEx(this Delegate d)
		{
#if NETSTANDARD || WINDOWS_UWP
			return d.GetMethodInfo();
#else
			return d.Method;
#endif
		}
	}
}
