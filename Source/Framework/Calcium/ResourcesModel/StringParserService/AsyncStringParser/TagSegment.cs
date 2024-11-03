#region File and License Information

/*
<File>
	<License>
		Copyright © 2009 - 2024, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com),
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2024-11-03 23:44:11Z</CreationDate>
</File>
*/

#endregion

#nullable enable

namespace Calcium.ResourcesModel.Experimental
{
	public class TagSegment
	{
		public uint   Index   { get; set; }
		public uint   Length  { get; set; }
		public string TagName { get; set; }
		public string TagArg  { get; set; }
		public string Tag     { get; set; }

		public object? TagValue { get; set; }

		public TagSegment()
		{
		}

		public TagSegment(uint index, uint length, string tagName, string tagArg, string tag)
		{
			Index   = index;
			Length  = length;
			TagName = tagName;
			TagArg  = tagArg;
			Tag     = tag;
		}
	}
}