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
  public sealed class LocalizationSystem : MonoBehaviour, ILocalizationSystem
  {
    // Static instance of the localization system
    private static ILocalizationSystem _current;

    // Return the static instance of the localization system
    public static ILocalizationSystem current => _current;


    // Internal state of the localization system
    private List<ILocale> _loadedLocales = new List<ILocale>();
    private ILocale _selectedLocale = null;
    private ILocale _lastLocale = null;
    
    private Dictionary<string, MessageFunction> _functions = new Dictionary<string, MessageFunction>();


    // Return all registered locale loaders
    public IEnumerable<LocaleLoader> loaders => GetComponents<LocaleLoader>().OrderBy(l => l.priority);
    
    // Return all enabled registered locale loaders
    public IEnumerable<LocaleLoader> enabledLoaders =>  loaders.Where(l => l.executionMode.ShouldExecute());

    // Return all registered locale selectors
    public IEnumerable<LocaleSelector> selectors => GetComponents<LocaleSelector>().OrderBy(s => s.priority);

    // Return all enabled registered locale selectors
    public IEnumerable<LocaleSelector> enabledSelectors => selectors.Where(l => l.executionMode.ShouldExecute());

    // Return the loaded locales
    public IReadOnlyList<ILocale> loadedLocales => _loadedLocales;

    // Return and set the selected locale
    public ILocale selectedLocale {
      get => _selectedLocale;
      set => _selectedLocale = value;
    }

    // Return and set the selected culture
    public CultureInfo selectedCulture {
      get => _selectedLocale != null ? _selectedLocale.culture : CultureInfo.InvariantCulture;
      set => _selectedLocale = _loadedLocales.Where(locale => locale.code == value.Name).FirstOrDefault();
    }


    // Event that is triggered when the selected locale is changed
    public event Action<ILocale> onSelectedLocaleChanged;


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
      // Set the static instance
      _current = this;

      // Add functions to format assets
      RegisterFunction("asset", arg => ((ILocalizationSystem)this).FormatAsset(arg));

      // Add functions to format data from the Unity application
      RegisterFunction("productName", arg => Application.productName);
      RegisterFunction("companyName", arg => Application.companyName);
      RegisterFunction("version", arg => Application.version);
      RegisterFunction("unityVersion", arg => Application.unityVersion);
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
      // Update the selected locale
      if (_selectedLocale != _lastLocale)
      {
        // Emit a locale changed event if the new locale is not null
        if (_selectedLocale != null)
          onSelectedLocaleChanged?.Invoke(_selectedLocale);

        // Update the state
        _lastLocale = _selectedLocale;
      }
    }


    #region System initialization
    // Initialize the system
    public void Initialize()
    {
      // Load the locales
      LoadLocales();

      // Select th locale
      if (!TrySelectLocale())
        Debug.LogWarning($"[{gameObject.name}] Could not select a locale using any of the registered locale selectors");
    }

    // Initialize the system if no locale has been selected
    public void InitializeIfNoLocaleSelected()
    {
      if (_selectedLocale == null)
        Initialize();
    }
    #endregion

    #region Loading and selecting locales
    // Load the locales using the registered loaders
    public void LoadLocales()
    {
      _loadedLocales.Clear();

      foreach (var loader in enabledLoaders)
      {
        var locales = loader.LoadLocales().Where(locale => locale != null).ToList();
        foreach (var locale in locales)
          _loadedLocales.Add(locale);

        if (Application.isPlaying)
          Debug.Log($"[{gameObject.name}] Loaded {locales.Count} locales using {loader.GetType()}{(locales.Count > 0 ? $": {string.Join(", ", locales)}" : "")}");
      }
    }

    // Return if a locale can be selected using the registered selectors and store the selected locale
    public bool TrySelectLocale()
    {
      foreach (var selectors in enabledSelectors)
      {
        if (selectors.TrySelectLocale(_loadedLocales, out var locale))
        {
          if (Application.isPlaying)
            Debug.Log($"[{gameObject.name}] Selected locale {locale} using {selectors.GetType()}");

          _selectedLocale = locale;
          return true;
        }
      }

      return false;
    }
    #endregion

    #region Managing message functions
    // Register a function with the specified name
    public void RegisterFunction(string name, MessageFunction func)
    {
      _functions.Add(name, func);
    }

    // Unregister a function with the specified name
    public void UnregisterFunction(string name)
    {
      _functions.Remove(name);
    }

    // Return if a function with the specified name exists and store the function
    public bool TryGetFunction(string name, out MessageFunction func)
    {
      return _functions.TryGetValue(name, out func);
    }

    // Return if a function with the specified name exists and execute it with the specified argument
    public bool TryExecuteFunction(string name, string argument, out string value)
    {
      var result = TryGetFunction(name, out var func);
      value = result ? func(argument) : null;
      return result;
    }
    #endregion

    #region Formatting and localizing references
    // Format a message with the specified arguments using the specified locale
    public string Format(IMessageFormatProvider formatProvider, string message, IReadOnlyDictionary<string, object> arguments = null)
    {
      if (formatProvider == null)
        throw new ArgumentNullException(nameof(formatProvider));
      if (message == null)
        throw new ArgumentNullException(nameof(message));
      
      return new MessageFormatter(formatProvider, this).Format(message, arguments ?? new Dictionary<string, object>());
    }
    #endregion
  }
}