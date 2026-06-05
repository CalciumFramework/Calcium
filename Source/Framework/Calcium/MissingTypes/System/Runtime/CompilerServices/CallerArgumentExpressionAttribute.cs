namespace System.Runtime.CompilerServices
{
#if !NET6_0_OR_GREATER

	[AttributeUsage(AttributeTargets.Parameter,
					AllowMultiple = false,
					Inherited = false)]
	sealed class CallerArgumentExpressionAttribute : Attribute
	{
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}

#endif
}
