using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Audune.Localization
{
  // Class that defines a portable object writer
  internal class PortableObjectWriter
  {
    // Internal state of the writer
    private TextWriter _writer;
    private bool _append = false;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="writer">The writer to use for this Portable Object writer.</param>
    public PortableObjectWriter(TextWriter writer)
    {
      _writer = writer;
      _writer.NewLine = "\n";
    }


    /// <summary>
    /// Write a portable object message.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public void Write(PortableObjectMessage message)
    {
      // Write an empty line is the message should be appended
      if (_append)
        _writer.WriteLine();
      else
        _append = true;

      // Write the message context
      if (!string.IsNullOrEmpty(message.context))
      {
        _writer.Write("msgctxt ");
        _writer.WriteLine(QuoteString(message.context));
      }

      // Write the message identifier
      _writer.Write("msgid ");
      _writer.WriteLine(QuoteString(message.untranslatedString));

      // Write the message string
      _writer.Write("msgstr ");
      _writer.WriteLine(QuoteString(message.translatedString));
    }

    /// <summary>
    /// Write a portable object message format.
    /// </summary>
    /// <param name="messageFormat">The message format to write.</param>
    public void Write(IPortableObjectMessageFormat messageFormat)
    {
      Write(messageFormat.EncodeMessage());
    }


    /// <summary>
    /// Quote a string to be written.
    /// </summary>
    private string QuoteString(string input)
    {
      if (input == null)
        return "\"\"";

      // Split the string in multiple lines if necessary
      if (input.IndexOf("\n") < 0)
        return $"\"{EscapeString(input)}\"";

      // Iterate over the lines in the string
      var lines = input.Split('\n');
      var sb = new StringBuilder("\"\"\n");
      for (var i = 0; i < lines.Length; i++)
        sb.Append($"\"{EscapeString(lines[i])}{(i < lines.Length - 1 ? "\\n" : "")}\"\n");
      var output = sb.ToString();

      // Omit the last line if it is empty
      if (output.EndsWith("\"\"\n"))
        output = output[..^3];

      // Omit the last newline
      if (output.EndsWith("\n"))
        output = output[..^1];

      return output;
    }

    // Escape a string to be written.
    private string EscapeString(string input)
    {
      if (input == null)
        return null;
      
      // Escape backslashes
      input = input.Replace("\\\\", "\\\\");

      // Escape known C escape sequences
      input = input.Replace("\a", "\\a");
      input = input.Replace("\b", "\\b");
      input = input.Replace("\f", "\\f");
      input = input.Replace("\n", "\\n");
      input = input.Replace("\r", "\\r");
      input = input.Replace("\t", "\\t");
      input = input.Replace("\v", "\\v");
      input = input.Replace("\"", "\\\"");
      input = input.Replace("'", "\\'");

      // Escape unprintable characters
      input = Regex.Replace(input, "[^\x20-\x7E]", match => $"\\x{(int)match.Value[0]:X2}");

      return input;
    }
  }
}