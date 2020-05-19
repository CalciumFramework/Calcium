using System;
using Calcium.InversionOfControl;

namespace Calcium.UI.Data
{
	[DefaultType(typeof(MarkupTypeResolver), Singleton = true)]
	public interface IMarkupTypeResolver
	{
		Type Resolve(string qualifiedTypeName);

		bool TryResolve(string qualifiedTypeName, out Type type);
	}
}
