namespace Audune.Localization
{
  // Interface that defines a function executor for a message formatter
  public interface IMessageFunctionExecutor
  {
    // Return if a function with the specified name exists and execute it with the specified argument
    public bool TryExecuteFunction(string name, string argument, out string value);
  }
}