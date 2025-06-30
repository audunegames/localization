namespace Audune.Localization
{
  /// <summary>
  /// Delegate that defines a resolver for a localized string.
  /// </summary>
  /// <param name="formatter">The formatter to use to resolve the localized string.</param>
  /// <returns>The resolved localized string.</returns>
  public delegate string LocalizedStringResolver(MessageFormatterDelegate formatter);
}