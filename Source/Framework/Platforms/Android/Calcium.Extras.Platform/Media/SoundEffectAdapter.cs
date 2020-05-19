#region File and License Information
/*
<File>
	<License Type="BSD">
		Copyright © 2009 - 2012, Outcoder. All rights reserved.
	
		This file is part of Calcium (http://calciumsdk.net).

		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
		ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="danielvaughan@outcoder.com" />
	<CreationDate>2013-03-21 19:32:50Z</CreationDate>
</File>
*/
#endregion

using System;
using Android.Media;

namespace Outcoder.Audio
{
	public class SoundEffectAdapter : ISoundEffect
	{
		MediaPlayer mediaPlayer = new MediaPlayer();

		public SoundEffectAdapter(string filePath)
		{
			//ArgumentValidator.AssertNotNull(audioStream, "audioStream");
			//using (BinaryReader binaryReader = new BinaryReader(audioStream))
			//{
			//	long totalBytes = audioStream.Length;
			//	buffer = binaryReader.ReadBytes((Int32)totalBytes);
			//}
			mediaPlayer.SetDataSource(filePath);
			mediaPlayer.Prepare();
		}

		public void Play(float volume = 1, float pitch = 0, float pan = 0)
		{
			mediaPlayer.Start();
		}

		public void Stop(bool immediate = true)
		{
			mediaPlayer.Stop();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (mediaPlayer != null)
				{
					mediaPlayer.Dispose();
					mediaPlayer = null;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~SoundEffectAdapter()
		{
			Dispose(false);
		}
	}
}