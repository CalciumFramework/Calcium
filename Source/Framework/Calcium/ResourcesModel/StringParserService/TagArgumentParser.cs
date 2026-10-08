#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2026, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://calciumframework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2026-10-08 18:14:26Z</CreationDate>
</File>
*/
#endregion

using System;
using System.Collections.Generic;

namespace Calcium.ResourcesModel
{
	/// <summary>
	/// Parses a sequence of named tag arguments expressed as
	/// <c>Name=Value</c> pairs separated by one or more configured delimiter characters.
	/// </summary>
	/// <remarks>
	/// The default argument delimiter is a semicolon (<c>;</c>).
	/// Argument names are treated as case-insensitive, and surrounding whitespace
	/// around argument names and values is ignored.
	/// </remarks>
	public class TagArgumentParser
	{
		const string defaultArgumentDelimiters = ";";

		readonly char[] argumentDelimiters;

		public TagArgumentParser()
			: this(defaultArgumentDelimiters)
		{
		}

		public TagArgumentParser(string argumentDelimiters)
		{
			if (string.IsNullOrWhiteSpace(argumentDelimiters))
			{
				throw new ArgumentException(
					"At least one argument delimiter must be specified.",
					nameof(argumentDelimiters));
			}

			HashSet<char> delimiters = new();

			foreach (char delimiter in argumentDelimiters)
			{
				if (delimiter == '=')
				{
					throw new ArgumentException(
						"'=' cannot be used as an argument delimiter.",
						nameof(argumentDelimiters));
				}

				delimiters.Add(delimiter);
			}

			this.argumentDelimiters = new char[delimiters.Count];
			delimiters.CopyTo(this.argumentDelimiters);
		}

		/// <summary>
		/// Parses the specified tag argument string into a dictionary of argument
		/// names and values.
		/// </summary>
		/// <param name="arguments">
		/// The tag arguments to parse, for example
		/// <c>Provider=Google;PermissionSet=Calendar</c>.
		/// </param>
		/// <returns>
		/// A case-insensitive dictionary containing the parsed argument names and values.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="arguments"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="FormatException">
		/// Thrown when an argument is malformed, has an empty name or value,
		/// or specifies the same argument name more than once.
		/// </exception>
		public Dictionary<string, string> Parse(string arguments)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException(nameof(arguments));
			}

			Dictionary<string, string> result
				= new(4, StringComparer.OrdinalIgnoreCase);

			int itemStart = 0;

			while (itemStart < arguments.Length)
			{
				int delimiterIndex = arguments.IndexOfAny(argumentDelimiters, itemStart);

				int itemEnd = delimiterIndex < 0
								  ? arguments.Length
								  : delimiterIndex;

				ParseItem(
					arguments,
					itemStart,
					itemEnd - itemStart,
					result);

				if (delimiterIndex < 0)
				{
					break;
				}

				itemStart = delimiterIndex + 1;
			}

			return result;
		}

		static void ParseItem(string arguments,
							  int startIndex,
							  int length,
							  Dictionary<string, string> result)
		{
			if (length < 1)
			{
				return;
			}

			string item = arguments.Substring(startIndex, length).Trim();

			if (item.Length < 1)
			{
				return;
			}

			int equalsIndex = item.IndexOf('=');

			if (equalsIndex <= 0
				|| equalsIndex == item.Length - 1)
			{
				throw new FormatException(
					$"Invalid tag argument '{item}'. "
					+ "Arguments must have the form Name=Value.");
			}

			string name = item.Substring(0, equalsIndex).Trim();

			string value = item.Substring(equalsIndex + 1).Trim();

			if (name.Length < 1 || value.Length < 1)
			{
				throw new FormatException(
					$"Invalid tag argument '{item}'. "
					+ "Argument names and values cannot be empty.");
			}

			if (result.ContainsKey(name))
			{
				throw new FormatException(
					$"Tag argument '{name}' is specified more than once.");
			}

			result.Add(name, value);
		}
	}
}