#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-13 11:54:25Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;
using Codon.InversionOfControl;

namespace Codon.Platform
{
	/// <summary>
	/// .NET Standard currently does not provide
	/// a straight forward way to determine the platform
	/// that it is running on. This class probes 
	/// for a well-known type in platform specific assemblies
	/// to determine the platform. This then allows
	/// the <c>FrameworkContainer</c> IoC container 
	/// to automatically locate platform specific types
	/// at run-time.
	/// </summary>
	class PlatformDetector
	{
		internal class PlatformNameString
		{
			internal const string Android = "Android";
			internal const string Ios = "Ios";
			internal const string Uwp = "Uwp";
			internal const string Wpf = "Wpf";
		}

		static readonly List<string> platformNames
			= new List<string>
			{
				PlatformNameString.Android,
				PlatformNameString.Ios,
				PlatformNameString.Uwp,
				PlatformNameString.Wpf
			};

		static string platformName;
		static bool searchedForName;

		public static string PlatformName
		{
			get
			{
				if (!searchedForName && platformName == null)
				{
					searchedForName = true;

					var id = PlatformId;

					platformName = id.ToIdString();
				}

				return platformName;
			}
		}

		static PlatformId? platformId_UseProperty;

		public static PlatformId PlatformId
		{
			get
			{
				if (!platformId_UseProperty.HasValue)
				{
					IPlatformIdentifier identifier;
					if (Dependency.TryResolve<IPlatformIdentifier>(out identifier))
					{
						platformId_UseProperty = identifier.PlatformId;
					}
					else
					{
						platformId_UseProperty = PlatformId.Unknown;
					}
				}

				return platformId_UseProperty.Value;
			}
		}

		static PlatformId ConvertToId(string idString)
		{
			switch (idString)
			{
				case PlatformNameString.Android:
					return PlatformId.Android;
				case PlatformNameString.Ios:
					return PlatformId.Ios;
				case PlatformNameString.Uwp:
					return PlatformId.Uwp;
				case PlatformNameString.Wpf:
					return PlatformId.Wpf;
				default:
					return PlatformId.Unknown;
			}
		}
	}

	public enum PlatformId
	{
		Unknown,
		Android,
		Ios,
		Uwp,
		Wpf
	}

	static class PlatformIdExtensions
	{
		internal static string ToIdString(this PlatformId id)
		{
			switch (id)
			{
				case PlatformId.Android:
					return PlatformDetector.PlatformNameString.Android;
				case PlatformId.Ios:
					return PlatformDetector.PlatformNameString.Ios;
				case PlatformId.Uwp:
					return PlatformDetector.PlatformNameString.Uwp;
				case PlatformId.Wpf:
					return PlatformDetector.PlatformNameString.Wpf;
				default:
					return null;
			}
		}
	}

	[DefaultTypeName(AssemblyConstants.Namespace + "." + nameof(Codon.Platform)
					+ ".PlatformIdentifier, " + AssemblyConstants.PlatformAssembly, Singleton = true)]
	public interface IPlatformIdentifier
	{
		PlatformId PlatformId { get; }
	}
}
