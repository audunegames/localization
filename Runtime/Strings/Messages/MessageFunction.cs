namespace Audune.Localization
{
  /// <summary>
  /// Delegate that defines a function for messages.
  /// </summary>
  /// <param name="argument">The argument supplied to the function.</param>
  /// <returns>The result of executing the function.</returns>
  public delegate string MessageFunction(string argument);
}