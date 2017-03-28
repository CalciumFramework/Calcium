using System;

namespace Codon.UI.Data
{
	public class BindingException : Exception
	{
		public BindingException(string message)
			: base(message)
		{

		}

		public BindingException(string message, Exception innerException)
			: base(message, innerException)
		{
			
		}
	}
}