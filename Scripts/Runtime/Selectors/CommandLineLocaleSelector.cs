using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization.Selectors
{
  // Class that defines a locale selector that uses a command line argument
  public sealed class CommandLineLocaleSelector : LocaleSelector
  {
    // Locale selector settings
    [SerializeField, Tooltip("The command line argument used to select the locale")]
    private string _argument = "language";


    // Return the locale according to this selector
    public override Locale Select(IReadOnlyList<Locale> locales)
    {
      var fullArgument = $"-{_argument}=";

      if (string.IsNullOrEmpty(fullArgument))
        return null;

      var argument = Environment.GetCommandLineArgs().Where(arg => arg.StartsWith(fullArgument)).FirstOrDefault();
      if (string.IsNullOrEmpty(argument))
        return null;

      var localeCode = argument[fullArgument.Length..];
      return locales.Where(locale => locale.Code == localeCode).FirstOrDefault();
    }
  }
}
