using System;
using System.Collections.Generic;
using Codon.ComponentModel;
using Codon.StatePreservation;
using Codon.Concurrency;
using Codon.DialogModel;
using Codon.Navigation;
using Codon.Services;
using Codon.SettingsModel;
using Codon.Testing;

#if __ANDROID__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Codon.InversionOfControl.Containers
{
	[TestClass]
	public class
#if WINDOWS_UWP
		FrameworkContainerResolveUwpTests
#elif WPF
		FrameworkContainerResolveWpfTests
#elif __ANDROID__
		FrameworkContainerResolveAndroidTests
#else
		FrameworkContainerResolveUnknownTests
#endif
	{
		readonly Dictionary<Type, Type> knownTypeNameMappings
			= new Dictionary<Type, Type>
			{
#if WINDOWS_UWP
				{typeof(IImplicitTypeConverter), typeof(DefaultImplicitTypeConverter)},
#else
				{typeof(IImplicitTypeConverter), typeof(ImplicitTypeConverter)},
#endif
				{typeof(ISynchronizationContext), typeof(UISynchronizationContext)},
				{typeof(IDialogService), typeof(DialogService)},
				{typeof(INavigationService), typeof(NavigationService)},
				{typeof(ISettingsService), typeof(PlatformSettingsService)},
				{typeof(IMessenger), typeof(Messaging.Messenger)},
			};

		[TestMethod]
		public void ShouldLoadDefaultTypeNameObjects()
		{
			var container = new FrameworkContainer();

			foreach (var pair in knownTypeNameMappings)
			{
				var resolvedObject = container.Resolve(pair.Key);
				Assert.IsNotNull(resolvedObject);

				AssertX.IsInstanceOfType(resolvedObject, pair.Value);
			}
		}
	}
}
