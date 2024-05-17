#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-05-17 21:15:46Z</CreationDate>
</File>
*/
#endregion

using System.Threading;
using System.Threading.Tasks;

namespace Calcium.ComponentModel
{
	/// <summary>
	/// Classes implementing this interface are able
	/// to ready themselves for use.
	/// </summary>
	public interface ISupportInitialization
	{
		/// <summary>
		/// Performs the startup activities of the class.
		/// </summary>
		/// <exception cref="System.Exception">An unknown exception may occur.</exception>
		Task InitAsync(CancellationToken cancellationToken = default);
	}
}
