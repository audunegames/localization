using Audune.Localization.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines localization settings
  [CreateAssetMenu(menuName = "Audune Localization/Localization Settings")]
  public sealed class LocalizationSettings : ScriptableObject
  {
    // Class that defines a locale loader entry
    [Serializable]
    public sealed class LocaleLoaderEntry
    {
      public LocaleLoader loader;
      public ExecutionMode executionMode;

      public LocaleLoaderEntry(LocaleLoader loader, ExecutionMode executionMode)
      {
        this.loader = loader;
        this.executionMode = executionMode;
      }
    }

    // Class that defines a locale selector entry
    [Serializable]
    public sealed class LocaleSelectorEntry
    {
      public LocaleSelector selector;
      public ExecutionMode executionMode;

      public LocaleSelectorEntry(LocaleSelector selector, ExecutionMode executionMode)
      {
        this.selector = selector;
        this.executionMode = executionMode;
      }
    }


    // Localization settings
    [SerializeField, Tooltip("The loaders to use when getting locales")]
    private List<LocaleLoaderEntry> _localeLoaders;
    [SerializeField, Tooltip("The selectors to use when getting the startup locale")]
    private List<LocaleSelectorEntry> _localeSelectors;

    
    // Load the locales using the defined loaders
    public List<Locale> LoadLocales()
    {
      var locales = new List<Locale>();

      foreach (var loaderEntry in _localeLoaders)
      {
        if (!loaderEntry.executionMode.ShouldExecute())
          continue;

        var loadedLocales = loaderEntry.loader.Load().Where(locale => locale != null).ToList();
        locales.AddRange(loadedLocales);

        if (Application.isPlaying)
          Debug.Log($"Loaded {loadedLocales.Count} locales using {loaderEntry.loader.name}{(loadedLocales.Count > 0 ? $": {string.Join(", ", loadedLocales)}" : "")}");
      }

      return locales;
    }

    // Select the startup locale using the defined selectors
    public Locale SelectStartupLocale(List<Locale> locales)
    {
      foreach (var selectorEntry in _localeSelectors)
      {
        if (!selectorEntry.executionMode.ShouldExecute())
          continue;

        if (!selectorEntry.selector.TrySelect(locales, out var locale))
          continue;

        if (Application.isPlaying)
          Debug.Log($"Selected locale {locale} using {selectorEntry.selector.name}");
        return locale;
      }

      return null;
    }
  }
}