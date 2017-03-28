using System.Collections.Generic;
using System.Linq;

namespace Codon.UserOptionsModel
{
	public interface IUserOptionGroupings 
		: IEnumerable<IGrouping<IOptionCategory, IUserOptionReaderWriter>>
	{
		void Refresh();
	}
}