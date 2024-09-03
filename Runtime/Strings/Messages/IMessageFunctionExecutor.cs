namespace Audune.Localization
{
  // Interface that defines a function executor for a message formatter
  public interface IMessageFunctionExecutor
  {
    // Register a function with the specified name
    public void RegisterFunction(string name, MessageFunction func);

    // Unregister a function with the specified name
    public void UnregisterFunction(string name);

    // Return if a function with the specified name exists and store the function
    public bool TryGetFunction(string name, out MessageFunction func);

    // Return if a function with the specified name exists and execute it with the specified argument
    public bool TryExecuteFunction(string name, string argument, out string value);
  }
}