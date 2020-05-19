﻿using System;
using System.Reflection;

namespace Codon.Reflection
{
	public class PropertyWithAttribute
	{
		public Attribute Attribute { get; }
		public PropertyInfo PropertyInfo { get; }

		public PropertyWithAttribute(Attribute attribute, PropertyInfo propertyInfo)
		{
			Attribute = attribute;
			PropertyInfo = propertyInfo;
		}
	}
}