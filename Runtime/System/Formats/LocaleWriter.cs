using System.IO;
using System.Text;

namespace Audune.Localization
{
  /// <summary>
  /// Base class that defines a locale writer.
  /// </summary>
  public abstract class LocaleWriter
  {
    /// <summary>
    /// Write a locale to a text writer.
    /// </summary>
    /// <param name="textWriter">The text writer to write the locale to.</param>
    public abstract void Write(Locale locale, TextWriter textWriter);


    /// <summary>
    /// Write a locale to a file with the specified encoding.
    /// </summary>
    /// <param name="path">The path to the file to write the locale to.</param>
    /// <param name="encoding">The encoding of the file.</param>
    public void Write(Locale locale, string path, Encoding encoding)
    {
      using var textWriter = new StreamWriter(path, false, encoding);
      Write(locale, textWriter);
    }

    /// <summary>
    /// Write a locale to a file.
    /// </summary>
    /// <param name="path">The path to the file to write the locale to.</param>
    public void Write(Locale locale, string path)
    {
      using var textWriter = new StreamWriter(path, false);
      Write(locale, textWriter);
    }
  }
}