using System;

namespace Codon.UI.Data
{
	public interface IMarkupTypeResolver
	{
		Type Resolve(string qualifiedTypeName);

		bool TryResolve(string qualifiedTypeName, out Type type);
	}
}