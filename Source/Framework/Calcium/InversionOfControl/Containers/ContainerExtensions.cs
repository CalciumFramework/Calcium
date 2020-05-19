#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-12-27 15:13:08Z</CreationDate>
</File>
*/
#endregion

namespace Calcium.InversionOfControl
{
	/// <summary>
	/// Extension methods for classes implementing the 
	/// <see cref="IContainer"/> interface.
	/// </summary>
	public static class ContainerExtensions
	{
		static bool Initialized { get; set; }

		/// <summary>
		/// Sets up the specified container to be used 
		/// as the principal container by the framework infrastructure.
		/// </summary>
		/// <param name="container">The container to use
		/// with the framework infrastructure.</param>
		public static void InitializeContainer(this IContainer container)
		{
			AssertArg.IsNotNull(container, nameof(container));

			container.Register<IContainer>(container);

			Initialized = true;

			Dependency.Container = container;
			Dependency.Initialized = true;
		}
	}
}
