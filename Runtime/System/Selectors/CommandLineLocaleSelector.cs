using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a locale selector that uses a command line argument.
  /// </summary>
  [AddComponentMenu("Audune/Localization/Locale Selectors/Command Line Locale Selector")]
  public sealed class CommandLineLocaleSelector : LocaleSelector
  {
    // Locale selector properties
    [SerializeField, Tooltip("The command line argument used to select the locale")]
    private string _argument = "language";


    /// <summary>
    /// Return if a locale could be selected according to this selector and store the selected locale.
    /// </summary>
    /// <param name="locales">The list of locales to select a locale from.</param>
    /// <param name="locale">The locale in which the selected locale will be stored if a locale could be selected.</param>
    /// <returns>Whether a locale could be selected.</returns>
    public override bool TrySelectLocale(IReadOnlyList<ILocale> locales, out ILocale locale)
    {
      locale = null;

      var fullArgument = $"-{_argument}=";
      if (string.IsNullOrEmpty(fullArgument))
        return false;

      var argument = Environment.GetCommandLineArgs().Where(arg => arg.StartsWith(fullArgument)).FirstOrDefault();
      if (string.IsNullOrEmpty(argument))
        return false;

      var localeCode = argument[fullArgument.Length..];
      locale = locales.Where(locale => locale.code == localeCode).FirstOrDefault();
      return locale != null;
    }
  }
}
