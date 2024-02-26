using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines the system for localization
  [AddComponentMenu("Audune/Localization/Localization System")]
  [DefaultExecutionOrder(-100)]
  public sealed class LocalizationSystem : MonoBehaviour
  {
    // Regex constants
    private static readonly Regex localizedRegex = new Regex(@"\[\[((?:[^\\\]]|\\.)+)\]\]", RegexOptions.Compiled);
    private static readonly Regex inputRegex = new Regex(@"\[%(?:([^:]*):)?([a-zA-Z_][a-zA-Z0-9_\/]*)%]", RegexOptions.Compiled);


    // Internal state of the localization 
    private List<Locale> _definedLocales = new List<Locale>();
    private Locale _currentLocale = null;
    private Locale _lastLocale = null;

    // Localization system events
    public event Action<Locale> OnLocaleChanged;


    // Return the defined locales in the localization system
    public IEnumerable<Locale> definedLocales => _definedLocales;

    // Return and set the current locale
    public Locale currentLocale {
      get => _currentLocale;
      set => _currentLocale = value;
    }

    // Return and set the current culture
    public CultureInfo currentCulture {
      get => _currentLocale != null ? _currentLocale.culture : CultureInfo.InvariantCulture;
      set => _currentLocale = _definedLocales.Where(locale => locale.code == value.TwoLetterISOLanguageName).FirstOrDefault();
    }


    // Start is called before the first frame update
    private void Start()
    {
      // Initialize the system
      Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
      // Update the current locale
      if (_currentLocale != _lastLocale)
      {
        // Emit a locale changed event if the new locale is not null
        if (_currentLocale != null)
          OnLocaleChanged?.Invoke(_currentLocale);

        // Update the state
        _lastLocale = _currentLocale;
      }
    }

    #region System initialization
    // Initialize the system
    public void Initialize()
    {
      // Load the locales
      _definedLocales = LoadLocales().ToList();

      // Select th locale
      if (!TrySelectLocale(_definedLocales, out _currentLocale))
        Debug.LogWarning($"[LocalizationSystem] Could not select a locale using any of the registered locale selectors");
    }

    // Initialize the system if no locale has been selected
    public void InitializeIfNoLocaleSelected()
    {
      if (_currentLocale == null)
        Initialize();
    }
    #endregion

    #region Locale loader management
    // Return all registered locale loaders
    public IEnumerable<LocaleLoader> GetLocaleLoaders()
    {
      return GetComponents<LocaleLoader>().OrderBy(l => l.priority);
    }

    // Return all enabled registered locale loaders
    public IEnumerable<LocaleLoader> GetEnabledLocaleLoaders()
    {
      return GetLocaleLoaders().Where(l => l.executionMode.ShouldExecute());
    }

    // Load the locales using the registered loaders
    public IEnumerable<Locale> LoadLocales()
    {
      foreach (var localeLoader in GetEnabledLocaleLoaders())
      {
        var locales = localeLoader.LoadLocales().Where(locale => locale != null).ToList();
        foreach (var locale in locales)
          yield return locale;

        if (Application.isPlaying)
          Debug.Log($"[LocalizationSystem] Loaded {locales.Count} locales using {localeLoader.GetType()}{(locales.Count > 0 ? $": {string.Join(", ", locales)}" : "")}");
      }
    }
    #endregion

    #region Locale selector management
    // Return all registered locale selectors
    public IEnumerable<LocaleSelector> GetLocaleSelectors()
    {
      return GetComponents<LocaleSelector>().OrderBy(s => s.priority);
    }

    // Return all enabled registered locale selectors
    public IEnumerable<LocaleSelector> GetEnabledLocaleSelectors()
    {
      return GetLocaleSelectors().Where(l => l.executionMode.ShouldExecute());
    }

    // Return if a locale can be selected using the registered selectors and store the selected locale
    public bool TrySelectLocale(IReadOnlyList<Locale> locales, out Locale locale)
    {
      foreach (var localeSelector in GetEnabledLocaleSelectors())
      {
        if (localeSelector.TrySelectLocale(locales, out locale))
        {
          if (Application.isPlaying)
            Debug.Log($"[LocalizationSystem] Selected locale {locale} using {localeSelector.GetType()}");

          return true;
        }
      }

      locale = null;
      return false;
    }
    #endregion

    #region Localizing references
    // Return a string for the specified key in the current locale
    public string Localize(LocalizedString reference)
    {
      if (reference == null)
        throw new ArgumentNullException(nameof(reference));

      if (_currentLocale != null && reference.TryResolve(_currentLocale, out var value))
        return currentLocale.Format(value, reference.arguments);
      else
        return $"<{reference}>";
    }
    #endregion
  }
}