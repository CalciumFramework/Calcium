using System;

namespace Codon.UI.Elements.ValueConverters
{
	partial class BooleanToVisibilityConverterTests
	{
		class ConvertParameters : IEquatable<ConvertParameters>
		{
			public bool? Value { get; }
			public string Parameter { get; }

			public ConvertParameters(bool? value, string parameter)
			{
				Value = value;
				Parameter = parameter;
			}

			public bool Equals(ConvertParameters other)
			{
				if (ReferenceEquals(null, other))
					return false;
				if (ReferenceEquals(this, other))
					return true;

				return Value == other.Value && string.Equals(Parameter, other.Parameter);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != this.GetType())
					return false;

				return Equals((ConvertParameters)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Value.GetHashCode() * 397) ^ (Parameter != null ? Parameter.GetHashCode() : 0);
				}
			}
		}
	}
}
