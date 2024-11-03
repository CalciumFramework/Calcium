#region File and License Information
/*
<File>
	<License>
		Copyright © 2009 - 2017, Daniel Vaughan. All rights reserved.
		This file is part of Calcium (http://CalciumFramework.com), 
		which is released under the MIT License.
		See file /Documentation/License.txt for details.
	</License>
	<CreationDate>2010-03-25 20:43:46Z</CreationDate>
</File>
*/
#endregion

#nullable enable
using System;
using System.Collections.Generic;

using Calcium.ComponentModel;
using Calcium.InversionOfControl;
using Calcium.ResourcesModel;

namespace Calcium.Services
{
	/// <summary>
	/// This interface specifies the minimum capabilities of a string parser,
	/// which is able to take a string and resolve any embedded string tags.
	/// Tags are sections within strings having the format: 
	/// ${TagName[:Argument]} 
	/// where [:Argument] is optional.
	/// You can register a tag with its replacement text,
	/// or you can register an <see cref="IConverter"/>
	/// that is used to replace the text.
	/// </summary>
	[DefaultType(typeof(StringParserService), Singleton = true)]
	public interface IStringParserService
	{
		/// <summary>
		/// Parses the specified text. Resolves string tags within the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="tagValues">The custom tag values. 
		/// If a matching tag is found in the specified text, 
		/// then the tag will be substituted for the dictionary value.
		/// </param>
		/// <param name="delimiters">An override for the default <see cref="TagDelimiters"/>.</param>
		/// <returns>The parsed string.</returns>
		string Parse(string text, IDictionary<string, string> tagValues, TagDelimiters? delimiters = null);

		/// <summary>
		/// Parses the specified text. Resolves string tags within the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="delimiters">An override for the default <see cref="TagDelimiters"/>.</param>
		/// <returns></returns>
		string Parse(string text, TagDelimiters? delimiters = null);

		/// <summary>
		/// Registers a converter with the specified tag name. 
		/// When subsequently calling Parse, if a tag is discovered in the text, 
		/// then the specified IConverter will be used to convert the tag 
		/// and substitute the tag in the text.
		/// </summary>
		/// <param name="tagName">The tag identifier. E.g., "Country". 
		/// When calling Parse, if the text contains a string "Greeting from ${Country}." 
		/// The specified converter will be used to create a string replacement 
		/// for the ${Country} tag.</param>
		/// <param name="converter">The converter.</param>
		/// <example></example>
		void RegisterConverter(string tagName, IConverter converter);

		/// <summary>
		/// Registers a converter with the specified tag name. 
		/// When subsequently calling Parse, if a tag is discovered in the text, 
		/// then the specified IConverter will be used to convert the tag 
		/// and substitute the tag in the text.
		/// </summary>
		/// <param name="tagName">The tag identifier. E.g., "Country". 
		/// When calling Parse, if the text contains a string "Greeting from ${Country}." 
		/// The specified converter will be used to create a string replacement 
		/// for the ${Country} tag.</param>
		/// <param name="convertFunc">A func that is used to convert a value.</param>
		void RegisterConverter(string tagName, Func<object, object> convertFunc);
	}
}
