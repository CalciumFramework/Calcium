using System;

#if __ANDROID__
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Codon.Testing
{
    class AssertX
    {
		public static void IsInstanceOfType(object instance, Type type)
		{
#if __ANDROID__
			Assert.IsInstanceOfType(type, instance);
#else
			Assert.IsInstanceOfType(instance, type);
#endif
		}

	}
}
