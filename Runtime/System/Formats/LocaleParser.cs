using System.IO;
using System.Text;

namespace Audune.Localization
{
  /// <summary>
  /// Base class that defines a locale parser.
  /// </summary>
  public abstract class LocaleParser
  {
    /// <summary>
    /// Parse a locale from a text reader.
    /// </summary>
    /// <param name="textReader">The text reader to parse the locale from.</param>
    /// <returns>The parsed locale.</returns>
    public abstract Locale Parse(TextReader textReader);


    /// <summary>
    /// Parse a locale from a file with the specified encoding.
    /// </summary>
    /// <param name="path">The path to the file to parse the locale from.</param>
    /// <param name="encoding">The encoding of the file.</param>
    /// <returns>The parsed locale.</returns>
    public Locale Parse(string path, Encoding encoding)
    {
      using var textReader = new StreamReader(path, encoding);
      return Parse(textReader);
    }

    /// <summary>
    /// Parse a locale from a file.
    /// </summary>
    /// <param name="path">The path to the file to parse the locale from.</param>
    /// <returns>The parsed locale.</returns>
    public Locale Parse(string path)
    {
      using var textReader = new StreamReader(path);
      return Parse(textReader);
    }
  }
}