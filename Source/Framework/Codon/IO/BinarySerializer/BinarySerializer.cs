#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Codon (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>$CreationDate$</CreationDate>
</File>
*/
#endregion

using System.IO;
using Codon.IO.Serialization;

namespace Codon.IO
{
	/// <summary>
	/// This class is the default implementation of the
	/// <see cref="IBinarySerializer"/> interface
	/// and is used, for example, by the settings system 
	/// to save and restore objects.
	/// See the <see cref="IBinarySerializer"/> for API
	/// documentation.
	/// </summary>
	public class BinarySerializer : IBinarySerializer
	{
		public byte[] Serialize(object item, bool makeVerbose)
		{
			return SilverlightSerializer.Serialize(item, makeVerbose);
		}

		public byte[] Serialize(object item)
		{
			return SilverlightSerializer.Serialize(item);
		}

		public T Deserialize<T>(byte[] array) where T : class
		{
			return (T)SilverlightSerializer.Deserialize(array);
		}

		public T Deserialize<T>(Stream stream) where T : class
		{
			return (T)SilverlightSerializer.Deserialize(stream);
		}

		public object Deserialize(Stream inputStream)
		{
			return SilverlightSerializer.Deserialize(inputStream);
		}

		public void Serialize(object item, Stream outputStream)
		{
			SilverlightSerializer.Serialize(item, outputStream);
		}
	}
}