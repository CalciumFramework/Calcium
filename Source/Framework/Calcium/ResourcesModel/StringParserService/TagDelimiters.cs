using System;

namespace Calcium.ResourcesModel
{
	public class TagDelimiters
	{
		public string Start { get; }
		public string End   { get; }

		public TagDelimiters(string start, string end)
		{
			if (string.IsNullOrEmpty(start))
			{
				throw new ArgumentNullException(nameof(start));
			}

			if (string.IsNullOrEmpty(end))
			{
				throw new ArgumentNullException(nameof(end));
			}

			Start = start;
			End   = end;
		}

		public static TagDelimiters Default => new("${", "}"); // Default delimiters
	}
}