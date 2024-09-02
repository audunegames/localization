namespace Audune.Localization
{
  // Class that defines the value for a localized string
  public interface IStringValue
  {
    // Return if the value is present
    public bool isPresent { get; }

    // Return if the value is empty
    public bool isEmpty => !isPresent;


    // Return if the reference can be resolved and store the value of the resolved reference
    public bool TryGetValue(ILocalizedStringTable table, out string value);


    // Class that defines a path value for a localized reference
    public class Path : IStringValue
    {
      // The path of the value
      public readonly string path;


      // Return if the value is present
      public bool isPresent => !string.IsNullOrEmpty(path);


      // Constructor
      public Path(string path)
      {
        this.path = path;
      }

      // Return if the value can be 
      public bool TryGetValue(ILocalizedStringTable table, out string value)
      {
        return table.TryFind(path, out value);
      }
    }


    // Class that defines a text value for a localized reference
    public class Text : IStringValue
    {
      // The text of the value
      public readonly string text;


      // Return if the value is present
      public bool isPresent => !string.IsNullOrEmpty(text);


      // Constructor
      public Text(string text)
      {
        this.text = text;
      }

      // Return if the value can be 
      public bool TryGetValue(ILocalizedStringTable table, out string value)
      {
        value = text;
        return true;
      }
    }
  }
}