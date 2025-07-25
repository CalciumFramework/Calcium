#nullable enable

using System;
using System.Collections.Generic;
using System.IO;

namespace Calcium.IO.Serialization
{
	partial class SilverlightSerializer
	{
		/// <summary>
		/// Unlike the other Deserialize methods, this method
		/// does not use the internally stored typed name
		/// when determining the type of the object to deserialize.
		/// </summary>
		public static object DeserializeObjectToType(Stream inputStream, 
													 Type? targetType = null)
		{
			if (targetType == null)
			{
				return Deserialize(inputStream, instance: null);
			}

			var originalVerbose = Verbose;
			CreateStacks();

			try
			{
				_ktStack.Push(_knownTypes);
				_piStack.Push(_propertyIds);
				_loStack.Push(_loadedObjects);

				var reader = new BinaryReader(inputStream);
				var version = reader.ReadString();
				var typeCount = reader.ReadInt32();

				if (version == "SerV3")
				{
					Verbose = reader.ReadBoolean();
				}

				_propertyIds = new List<string>();
				_knownTypes = new List<Type>();
				_loadedObjects = new List<object>();

				for (var i = 0; i < typeCount; i++)
				{
					var rawName = reader.ReadString();
					Type? resolvedType;

					if (i == 0)
					{
						/* Force the root object to be your targetType */
						resolvedType = targetType;
					}
					else
					{
						/* Use the embedded AQN, with your existing fallbacks */
						resolvedType = Type.GetType(rawName);

						if (resolvedType == null)
						{
							// See if it’s pointing at the core library — e.g. mscorlib/System.Private.CoreLib
							// and if so strip off the ", Assembly=..." tail
							var nameToResolve = rawName;
							var commaIndex = nameToResolve.IndexOf("'", StringComparison.Ordinal);

							if (commaIndex >= 0)
							{
								// Grab everything after the first comma
								var assemblyPart = nameToResolve
												   .Substring(commaIndex + 1)
												   .TrimStart();

								// Get the actual core‑lib assembly name at runtime
								var coreAssemblyName = typeof(object)
													   .Assembly
													   .GetName()
													   .Name;

								if (assemblyPart.StartsWith(coreAssemblyName, StringComparison.Ordinal))
								{
									// Strip off the ", CoreLib, Version=…, Culture=…, PublicKeyToken=…"
									nameToResolve = nameToResolve.Substring(0, commaIndex);
								}
							}

							// Try again without the core‑lib info
							resolvedType = Type.GetType(nameToResolve);

							if (resolvedType == null)
							{
								// Last resort: fire your mapping event
								var mapArgs = new TypeMappingEventArgs
											  {
												  TypeName = rawName
											  };
								InvokeMapMissingType(mapArgs);
								resolvedType = mapArgs.UseType;
							}
						}

						if (!Verbose && resolvedType == null)
						{
							throw new ArgumentException(
								$"SilverlightSerializer: Cannot reference type {rawName} in this context");
						}
					}

					/* At this point resolvedType is non‑null */
					_knownTypes.Add(resolvedType!);
				}

				var propCount = reader.ReadInt32();

				for (var i = 0; i < propCount; i++)
				{
					_propertyIds.Add(reader.ReadString());
				}

				/* Deserialize the object graph, supplying your targetType */
				return DeserializeObject(
					reader,
					itemType: targetType,
					instance: null);
			}
			finally
			{
				_knownTypes = _ktStack.Pop();
				_propertyIds = _piStack.Pop();
				_loadedObjects = _loStack.Pop();
				Verbose = originalVerbose;
			}
		}
	}
}
