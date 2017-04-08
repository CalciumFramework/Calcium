using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codon;
using Codon.ApiProfiling;
using Codon.Concurrency;

class Program
{
    static void Main(string[] args)
    {
		Dependency.Register<ISynchronizationContext>(new ConsoleSynchronizationContext());

        Console.WriteLine("Profiling started.");

	    var profilableTypes = GetTypesWithAttribute(Assembly.GetEntryAssembly(), typeof(ProfilableAttribute));
	    foreach (Type profilableType in profilableTypes)
	    {
		    Console.WriteLine("Profiling " + profilableType);

			var instance = (IProfilable)Activator.CreateInstance(profilableType);

		    var result = instance.Profile();
			Console.WriteLine("Results for " + result.Name);
		    foreach (ProfileMetric profileMetric in result.Metrics)
		    {
			    Console.WriteLine($"{profileMetric.Name}     {profileMetric.TimeSpan}");
		    }
	    }

		Console.WriteLine("Done.");
	    Console.ReadKey();
    }

	static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
	{
		foreach (Type type in assembly.GetTypes())
		{
			if (type.GetTypeInfo().GetCustomAttributes(attributeType, true).Any())
			{
				yield return type;
			}
		}
	}
}