#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-08 19:02:21Z</CreationDate>
</File>
*/
#endregion

using Calcium.InversionOfControl;
using Calcium.UI.Data;

namespace Calcium.UI.Elements
{
	public class StaticResourceExtension : MarkupExtensionBase
	{
		readonly string resourceId;

		static IStaticResourceRegistry resourceRegistry_UseProperty;

		IStaticResourceRegistry ResourceRegistry
		{
			get
			{
				if (resourceRegistry_UseProperty == null)
				{
					resourceRegistry_UseProperty = Dependency.Resolve<IStaticResourceRegistry>();
				}

				return resourceRegistry_UseProperty;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resourceId">
		/// A string that identifies the resource that 
		/// was registered using the <see cref="IStaticResourceRegistry"/> instance. 
		/// </param>
		public StaticResourceExtension(string resourceId)
		{
			this.resourceId = AssertArg.IsNotNull(resourceId, nameof(resourceId));
		}
		
		public override object ProvideValue(IContainer iocContainer/*, object[] parameters*/)
		{
			return ResourceRegistry[resourceId];
		}
	}
}
