using Calcium.Services;

namespace Calcium.ResourcesModel.Extensions
{
	/// <summary>
	/// Extension methods for the parsing strings using the <see cref="IStringParserService"/>.
	/// </summary>
    public static class StringParserExtensionMethods
    {
		/// <summary>
		/// Uses the IoC registered <see cref="IStringParserService"/>
		/// to parse the specified text.
		/// </summary>
		/// <param name="text">The text to be parsed.</param>
		/// <returns>The parsed string, with any tag transformed.</returns>
	    public static string Parse(this string text)
	    {
		    if (string.IsNullOrWhiteSpace(text))
		    {
			    return null;
		    }

		    var stringParserService = Dependency.Resolve<IStringParserService>();
		    var result = stringParserService.Parse(text);
		    return result;
	    }
    }
}
