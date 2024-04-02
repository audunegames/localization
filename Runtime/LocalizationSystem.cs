using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  // Class that defines the system for localization
  [AddComponentMenu("Audune/Localization/Localization System")]
  [DefaultExecutionOrder(-100)]
  public sealed class LocalizationSystem : MonoBehaviour, IMessageFormatter, IMessageFunctionExecutor
  {
    // Internal state of the localization 
    private Dictionary<string, Func<string, string>> _functions = new Dictionary<string, Func<string, string>>();

    private List<Locale> _loadedLocales = new List<Locale>();
    private Locale _selectedLocale = null;
    private Locale _lastLocale = null;

    // Localization system events
    public event Action<Locale> OnLocaleChanged;
    public event Action<LocalizedString> OnLocalizedStringMissing;
    public event Action<string> OnAssetMissing;


    // Return the loaded locales in the localization system
    public IEnumerable<Locale> loadedLocales => _loadedLocales;

    // Return and set the selected locale in the localization system
    public Locale selectedLocale {
      get => _selectedLocale;
      set {
        _selectedLocale = value;
      }
    }

    // Return and set the selected culture
    public CultureInfo selectedCulture {
      get => _selectedLocale != null ? _selectedLocale.culture : CultureInfo.InvariantCulture;
      set {
        _selectedLocale = _loadedLocales.Where(locale => locale.code == value.Name).FirstOrDefault();
      }
    }


    // Start is called before the first frame update
    private void Start()
    {
      // Add functions to format data from the Unity application
      RegisterFunction("productName", arg => Application.productName);
      RegisterFunction("companyName", arg => Application.companyName);
      RegisterFunction("version", arg => Application.version);
      RegisterFunction("unityVersion", arg => Application.unityVersion);

      // Add helper functions
      RegisterFunction("asset", arg => FormatAsset(arg));

      // Initialize the system
      Initialize();
    }

    // Update is called once per frame
    private void Update()
    {
      // Update the selected locale
      if (_selectedLocale != _lastLocale)
      {
        // Emit a locale changed event if the new locale is not null
        if (_selectedLocale != null)
          OnLocaleChanged?.Invoke(_selectedLocale);

        // Update the state
        _lastLocale = _selectedLocale;
      }
    }


    #region System initialization
    // Initialize the system
    public void Initialize()
    {
      // Load the locales
      _loadedLocales.Clear();
      _loadedLocales.AddRange(LoadLocales());

      // Select th locale
      if (!TrySelectLocale(_loadedLocales, out _selectedLocale))
        Debug.LogWarning($"[LocalizationSystem] Could not select a locale using any of the registered locale selectors");
    }

    // Initialize the system if no locale has been selected
    public void InitializeIfNoLocaleSelected()
    {
      if (_selectedLocale == null)
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

    #region Message function management
    // Register a function with the specified name
    public void RegisterFunction(string name, Func<string, string> func)
    {
      _functions.Add(name, func);
    }

    // Unregister a function with the specified name
    public void UnregisterFunction(string name)
    {
      _functions.Remove(name);
    }

    // Return if a function with the specified name exists and execute it with the specified argument
    bool IMessageFunctionExecutor.TryExecuteFunction(string name, string argument, out string value)
    {
      var result = _functions.TryGetValue(name, out var func);
      value = result ? func(argument) : null;
      return result;
    }
    #endregion

    #region Formatting and localizing references
    // Format the specified message message using the current locale
    public string Format(string message, IReadOnlyDictionary<string, object> arguments)
    {
      // Check if the arguments are not null
      if (message == null)
        throw new ArgumentNullException(nameof(message));

      // Check if a locale has been selected
      if (_selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");
      
      // Format the message using the formatter of the locale
      return new MessageFormatter(_selectedLocale, this).Format(message, arguments);
    }

    // Format the message for the specified localized string reference using the current locale
    public string Format(LocalizedString reference)
    {
      if (reference == null)
        throw new ArgumentNullException(nameof(reference));

      // Check if a locale has been selected
      if (_selectedLocale == null)
        throw new LocalizationException("No locale has ben selected");

      // Check if the reference can be resolved
      if (!reference.TryResolve(_selectedLocale.strings, out var message))
      {
        OnLocalizedStringMissing?.Invoke(reference);
        Debug.LogWarning($"[LocalizationSystem] Could not find string \"{reference}\" in locale {_selectedLocale}");
        return $"<{reference}>";
      }

      // Format the message using the formatter of the locale
      return Format(reference.Format(message), reference.arguments);
    }

    // Format the contents of the specified text asset resource
    public string FormatAsset(string path, IReadOnlyDictionary<string, object> arguments = null)
    {
      arguments ??= new Dictionary<string, object>();

      // Check if the text asset can be loaded
      var textAsset = Resources.Load<TextAsset>(path);
      if (textAsset == null)
      {
        OnAssetMissing?.Invoke(path);
        Debug.LogWarning($"[LocalizationSystem] Could not find asset \"{path}\"");
        return $"<asset: {path}>";
      }

      // Format the text of the text asset using the formatter of the locale
      return Format(textAsset.text, arguments);
    }
    #endregion
  }
}