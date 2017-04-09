using System;
using Codon.InversionOfControl;

namespace Codon.UI.Data
{
	[DefaultType(typeof(MarkupTypeResolver), Singleton = true)]
	public interface IMarkupTypeResolver
	{
		Type Resolve(string qualifiedTypeName);

		bool TryResolve(string qualifiedTypeName, out Type type);
	}
}