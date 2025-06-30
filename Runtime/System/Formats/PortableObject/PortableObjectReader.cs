using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Audune.Localization
{
  // Class that defines a portable object reader
  internal class PortableObjectReader
  {
    // Internal state of the reader
    private TextReader _reader;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="writer">The reader to use for this Portable Object reader.</param>
    public PortableObjectReader(TextReader reader)
    {
      _reader = reader;
    }


    /// <summary>
    /// Try to read a portable object message.
    /// </summary>
    /// <param name="message">The message that is stored when it has been succesfully read.</param>
    /// <returns>Whether a message could be read.</returns>
    public bool TryRead(out PortableObjectMessage message)
    {
      message = null;

      // Check if there is more text to read
      var line = _reader.ReadLine();
      if (line == null)
        return false;

      // Create a new message
      message = new PortableObjectMessage();
      string currentField = null;
      StringBuilder currentBuffer = null;

      // Function to flush the current field
      void FlushCurrentField(PortableObjectMessage message)
      {
        if (currentField == "msgctxt")
          message.context = currentBuffer?.ToString();
        else if (currentField == "msgid")
          message.untranslatedString = currentBuffer?.ToString();
        else if (currentField == "msgstr")
          message.translatedString = currentBuffer?.ToString();
      }

      // Read the next lines until an empty line is read
      while (!string.IsNullOrWhiteSpace(line))
      {
        line = line.Trim();

        // Check the type of the line
        if (line.StartsWith("#"))
        {
          // TODO: Handle comments
        }
        else if (line.StartsWith("msgctxt "))
        {
          // Flush the current field
          FlushCurrentField(message);

          // Start the msgctxt field
          currentField = "msgctxt";
          currentBuffer = new StringBuilder(UnquoteString(line[8..]));
          message.context = null;
        }
        else if (line.StartsWith("msgid "))
        {
          // Flush the current field
          FlushCurrentField(message);
          
          // Start the msgid field
          currentField = "msgid";
          currentBuffer = new StringBuilder(UnquoteString(line[6..]));
          message.untranslatedString = null;
        }
        else if (line.StartsWith("msgstr "))
        {
          // Flush the current field
          FlushCurrentField(message);
          
          // Start the msgstr field
          currentField = "msgstr";
          currentBuffer = new StringBuilder(UnquoteString(line[7..]));
          message.translatedString = null;
        }
        else
        {
          // Append the line to the current buffer
          if (currentBuffer == null)
            throw new PortableObjectException("Invalid PO format");
          currentBuffer.Append(UnquoteString(line));
        }

        // Read the next line
        line = _reader.ReadLine();
      }

      // Flush the current field
      FlushCurrentField(message);

      return true;
    }


    /// <summary>
    /// Unquote a read string
    /// </summary>
    private string UnquoteString(string input)
    {
      if (input.StartsWith("\"") && input.EndsWith("\""))
        return UnescapeString(input[1..^1]);
      else
        throw new PortableObjectException($"Invalid PO format: found unquoted string \"{input}\"");
    }

    // Unsecape a read string.
    private string UnescapeString(string input)
    {
      if (input == null)
        return null;

      // Unescape hex escape sequences
      input = Regex.Replace(input, @"\\x([0-9A-Fa-f]{2})", match => {
        string hex = match.Groups[1].Value;
        char c = (char)Convert.ToInt32(hex, 16);
        return c.ToString();
      });

      // Unescape known C escape sequences
      input = Regex.Replace(input, @"\\.", match => match.Value switch {
        "\\a" => "\a",
        "\\b" => "\b",
        "\\f" => "\f",
        "\\n" => "\n",
        "\\r" => "\r",
        "\\t" => "\t",
        "\\v" => "\v",
        "\\\\" => "\\",
        "\\\"" => "\"",
        "\\'" => "'",
        _ => match.Value,
      });

      return input;
    }
  }
}