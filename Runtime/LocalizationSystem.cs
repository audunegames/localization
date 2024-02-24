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


    // Return a string for the specified key in the current locale
    public string Localize(LocalizedString reference)
    {
      // TODO: Check reference for null value properly
      if (reference != null && _currentLocale != null && reference.TryResolve(_currentLocale, out var value))
      {
        var message = new MessageFormatter(_currentLocale).Format(value, new Dictionary<string, object>(reference?.arguments));
        //message = localizedRegex.Replace(message, EvaluateLocalizedMatch);
        //message = inputRegex.Replace(message, EvaluateInputMatch);
        return message;
      }

      Debug.LogWarning($"[{gameObject.name}] Could not find string {reference} in locale {_currentLocale}");
      return $"<{reference}>";
    }


    /*
    // Evaluate a localized match
    private string EvaluateLocalizedMatch(Match match)
    {
      return Localize(new LocalizedString(match.Groups[1].Value));
    }

    // Evaluate an input match
    private string EvaluateInputMatch(Match match)
    {
      var actionString = match.Groups[2].Value;
      var schemeString = match.Groups[1].Success ? match.Groups[1].Value : Game.playerSystem.uiPlayerInput != null ? Game.playerSystem.uiPlayerInput.currentControlScheme : "Keyboard";

      var action = Game.playerSystem.uiPlayerInput.actions.FindAction(actionString);
      var scheme = Game.playerSystem.uiPlayerInput.actions.FindControlScheme(schemeString);

      if (action == null || scheme == null)
        return actionString + " (not found)";

      return string.Join("", action.GetCombinedBindingReferences(scheme.Value).Select(g => g.ToTextMeshProSpriteFirstOnly()));
    }
    */
  }
}