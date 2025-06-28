namespace Audune.Localization
{
  /// <summary>
  /// Interface that defines a function executor for a message formatter.
  /// </summary>
  public interface IMessageFunctionExecutor
  {
    /// <summary>
    /// Register a function with the specified name.
    /// </summary>
    /// <param name="name">The name of the function to register.</param>
    /// <param name="func">The function to register under the specified name</param>
    public void RegisterFunction(string name, MessageFunction func);

    /// <summary>
    /// Unregister a function with the specified name.
    /// </summary>
    /// <param name="name">The name of the function to unregister.</param>
    public void UnregisterFunction(string name);

    /// <summary>
    /// Return if a function with the specified name exists and store the function.
    /// </summary>
    /// <param name="name">The name of the function to get.</param>
    /// <param name="func">The function corresponding to the specified name.</param>
    /// <returns>If a function with the specified name exists.</returns>
    public bool TryGetFunction(string name, out MessageFunction func);

    /// <summary>
    /// Return if a function with the specified name exists and execute it with the specified argument.
    /// </summary>
    /// <param name="name">The name of the function to execute.</param>
    /// <param name="argument">The argument to supply to the function.</param>
    /// <param name="value">The result of executing the function corresponding to the specified name with the specified argument.</param>
    /// <returns>If a function with the specified name exists.</returns>
    public bool TryExecuteFunction(string name, string argument, out string value);
  }
}