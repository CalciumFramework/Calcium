#if !NET6_0_OR_GREATER
//NETSTANDARD2_0
namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.ReturnValue
					| AttributeTargets.Parameter
					| AttributeTargets.Field
					| AttributeTargets.Property
					| AttributeTargets.Event)]
	sealed class NotNullAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
	sealed class MaybeNullAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class NotNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public NotNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class MaybeNullWhenAttribute : Attribute
	{
		public bool ReturnValue { get; }

		public MaybeNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}
	}

	[AttributeUsage(AttributeTargets.ReturnValue)]
	sealed class NotNullIfNotNullAttribute : Attribute
	{
		public string ParameterName { get; }

		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	sealed class DoesNotReturnAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class DoesNotReturnIfAttribute : Attribute
	{
		public bool ParameterValue { get; }

		public DoesNotReturnIfAttribute(bool parameterValue)
		{
			ParameterValue = parameterValue;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	sealed class NotNullMembersAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	sealed class MemberNotNullAttribute : Attribute
	{
		public string[] Members { get; }

		public MemberNotNullAttribute(params string[] members)
		{
			Members = members;
		}
	}

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	sealed class MemberNotNullWhenAttribute : Attribute
	{
		public bool     ReturnValue { get; }
		public string[] Members     { get; }

		public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
		{
			ReturnValue = returnValue;
			Members     = members;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class CallerArgumentExpressionAttribute : Attribute
	{
		public CallerArgumentExpressionAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}
#endif