using System.IO;
using System.Text;

namespace Audune.Localization
{
  // Class that parses a locale file
  public abstract class LocaleParser
  {
    // Parse a locale from a text reader
    public abstract Locale Parse(TextReader textReader);

    // Parse a locale from a file with the specified encoding
    public Locale Parse(string path, Encoding encoding)
    {
      using var textReader = new StreamReader(path, encoding);
      return Parse(textReader);
    }

    // Parse a locale from a file
    public Locale Parse(string path)
    {
      using var textReader = new StreamReader(path);
      return Parse(textReader);
    }
  }
}