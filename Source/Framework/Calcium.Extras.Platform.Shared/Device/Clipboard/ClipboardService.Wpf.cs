#if WPF
#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://codonfx.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2017-03-15 09:28:02Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Threading.Tasks;
using System.Windows;
using Calcium.Services;

namespace Calcium.Device
{
	/// <summary>
	/// WPF implementation of the <see cref="IClipboardService"/>.
	/// </summary>
    public class ClipboardService : IClipboardService
    {
	    public void CopyToClipboard(object content, string description)
	    {
		    string text = content as string;
		    if (text != null)
		    {
				Clipboard.SetText(text);
			}
		    else
		    {
			   throw new NotImplementedException(
				   "Data formats for content have not yet been implemented.");
		    }
	    }

	    public Task<object> GetClipboardContentsAsync()
	    {
		    object result = null;

			if (Clipboard.ContainsText())
			{
				result = Clipboard.GetText();
				
			}
			else if (Clipboard.ContainsImage())
			{
				result = Clipboard.GetImage();
			}
			else if (Clipboard.ContainsFileDropList())
			{
				result = Clipboard.GetFileDropList();
			}
			else if (Clipboard.ContainsAudio())
			{
				result = Clipboard.GetAudioStream();
			}
			
			return Task.FromResult(result);
		}
    }
}
#endif
