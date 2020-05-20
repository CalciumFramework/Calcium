#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-04-07 17:24:22Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Globalization;
using Calcium.ApiProfiling;
using Calcium.MissingTypes.System.Windows.Data;

namespace Calcium.UI.Data
{
	[Profilable]
    class BindingApplicatorProfiling : IProfilable
	{
		const int iterations = 100000;

		public ProfileResult Profile()
		{
			ProfileResult result = new ProfileResult
			{
				Name = nameof(BindingApplicator)
			};

			result.Metrics.Add(Profile1());
			result.Metrics.Add(Profile2());
			result.Metrics.Add(Profile3());

			return result;
		}

		ProfileMetric Profile1()
		{
			DateTime startTime = DateTime.UtcNow;

			for (int i = 0; i < iterations; i++)
			{
				var applicator = new BindingApplicator();

				var bindingExpression = new BindingExpression
				{
					//Converter = "BooleanToVisibilityConverter",
					//ConverterParameter = "Visible,Hidden,Hidden",
					Path = nameof(ViewModelTestClass.Bool1),
					Target = nameof(ViewTestClass.Bool1)
				};

				var target = new ViewTestClass();
				var source = new ViewModelTestClass();

				applicator.ApplyBinding(bindingExpression, target, source);
			}

			DateTime endTime = DateTime.UtcNow;
			var duration = endTime - startTime;

			var metric = new ProfileMetric();
			metric.Name = "Simple";
			metric.TimeSpan = new TimeSpan(duration.Ticks / iterations);

			return metric;
		}

		ProfileMetric Profile2()
		{
			DateTime startTime = DateTime.UtcNow;

			var path = nameof(ViewModelTestClass.NestedClass1) 
				+ "." + "Bool2";

			for (int i = 0; i < iterations; i++)
			{
				var applicator = new BindingApplicator();

				var bindingExpression = new BindingExpression
				{
					Path = path,
					Target = nameof(ViewTestClass.Bool1)
				};

				var target = new ViewTestClass();
				var source = new ViewModelTestClass();

				applicator.ApplyBinding(bindingExpression, target, source);
			}

			DateTime endTime = DateTime.UtcNow;
			var duration = endTime - startTime;

			var metric = new ProfileMetric();
			metric.Name = "Nested";
			metric.TimeSpan = new TimeSpan(duration.Ticks / iterations);

			return metric;
		}

		ProfileMetric Profile3()
		{
			var converter = new DummyBooleanToVisibilityConverter();

			DateTime startTime = DateTime.UtcNow;

			for (int i = 0; i < iterations; i++)
			{
				var applicator = new BindingApplicator();

				var bindingExpression = new BindingExpression
				{
					Path = nameof(ViewModelTestClass.Bool1),
					Target = nameof(ViewTestClass.Visibility),
					//Converter = "Calcium.UI.Data.DummyBooleanToVisibilityConverter, Calcium.ApiProfiling"
				};

				var target = new ViewTestClass();
				var source = new ViewModelTestClass();
				
				applicator.ApplyBinding(bindingExpression, target, source, converter);
			}

			DateTime endTime = DateTime.UtcNow;
			var duration = endTime - startTime;

			var metric = new ProfileMetric();
			metric.Name = "Converter";
			metric.TimeSpan = new TimeSpan(duration.Ticks / iterations);

			return metric;
		}
	}
}
