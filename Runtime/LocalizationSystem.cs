using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Class that defines a localization system.
  /// </summary>
  [AddComponentMenu("Audune/Localization/Localization System")]
  [DefaultExecutionOrder(-100)]
  public sealed class LocalizationSystem : MonoBehaviour, ILocalizationSystem
  {
    // Static instance of the localization system
    private static ILocalizationSystem _current;

    /// <summary>
    /// Return the static instance of the localization system.
    /// </summary>
    public static ILocalizationSystem current => _current;


    // Internal state of the localization system
    private List<ILocale> _loadedLocales = new List<ILocale>();
    private ILocale _selectedLocale = null;
    private ILocale _lastLocale = null;
    
    private Dictionary<string, MessageFunction> _functions = new Dictionary<string, MessageFunction>();


    /// <summary>
    /// Return all registered locale loaders.
    /// </summary>
    public IEnumerable<LocaleLoader> loaders => GetComponents<LocaleLoader>().OrderBy(l => l.priority);
    
    /// <summary>
    /// Return all enabled registered locale loaders.
    /// </summary>
    public IEnumerable<LocaleLoader> enabledLoaders =>  loaders.Where(l => l.executionMode.ShouldExecute());

    /// <summary>
    /// Return all registered locale selectors.
    /// </summary>
    public IEnumerable<LocaleSelector> selectors => GetComponents<LocaleSelector>().OrderBy(s => s.priority);

    /// <summary>
    /// Return all enabled registered locale selectors.
    /// </summary>
    public IEnumerable<LocaleSelector> enabledSelectors => selectors.Where(l => l.executionMode.ShouldExecute());

    /// <summary>
    /// Return the loaded locales.
    /// </summary>
    public IReadOnlyList<ILocale> loadedLocales => _loadedLocales;

    /// <summary>
    /// Return and set the selected locale.
    /// </summary>
    public ILocale selectedLocale {
      get => _selectedLocale;
      set => _selectedLocale = value;
    }

    /// <summary>
    /// Return and set the selected culture.
    /// </summary>
    public CultureInfo selectedCulture {
      get => _selectedLocale != null ? _selectedLocale.culture : CultureInfo.InvariantCulture;
      set => _selectedLocale = _loadedLocales.Where(locale => locale.code == value.Name).FirstOrDefault();
    }


    /// <summary>
    /// Event that is triggered when the selected locale is changed.
    /// </summary>
    public event Action<ILocale> onSelectedLocaleChanged;


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
      // Set the static instance
      if (_current == null)
        _current = this;
      else
        Destroy(gameObject);

      // Add functions to format assets
      RegisterFunction("asset", arg => ((ILocalizationSystem)this).FormatAsset(arg));

      // Add functions to format data from the Unity application
      RegisterFunction("productName", arg => Application.productName);
      RegisterFunction("companyName", arg => Application.companyName);
      RegisterFunction("version", arg => Application.version);
      RegisterFunction("unityVersion", arg => Application.unityVersion);
    }

    // OnDestroy is called when the component will be destroyed
    private void OnDestroy()
    {
      // Reset the static instancce
      if ((object)_current == this)
        _current = null;
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
    /// <summary>
    /// Initialize the system.
    /// </summary>
    public void Initialize()
    {
      // Load the locales
      LoadLocales();

      // Select th locale
      if (!TrySelectLocale())
        Debug.LogWarning($"[{gameObject.name}] Could not select a locale using any of the registered locale selectors");
    }

    /// <summary>
    /// Initialize the system if no locale has been selected.
    /// </summary>
    public void InitializeIfNoLocaleSelected()
    {
      if (_selectedLocale == null)
        Initialize();
    }
    #endregion

    #region Loading and selecting locales
    /// <summary>
    /// Load the locales using the registered loaders.
    /// </summary>
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

    /// <summary>
    /// Return if a locale can be selected using the registered selectors and store the selected locale.
    /// </summary>
    /// <returns>Whether a locale could be selected using the registered selectors.</returns>
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
    /// <summary>
    /// Register a function with the specified name.
    /// </summary>
    /// <param name="name">The name of the function to register.</param>
    /// <param name="func">The function to register under the specified name</param>
    public void RegisterFunction(string name, MessageFunction func)
    {
      _functions.Add(name, func);
    }

    /// <summary>
    /// Unregister a function with the specified name.
    /// </summary>
    /// <param name="name">The name of the function to unregister.</param>
    public void UnregisterFunction(string name)
    {
      _functions.Remove(name);
    }

    /// <summary>
    /// Return if a function with the specified name exists and store the function.
    /// </summary>
    /// <param name="name">The name of the function to get.</param>
    /// <param name="func">The function corresponding to the specified name.</param>
    /// <returns>If a function with the specified name exists.</returns>
    public bool TryGetFunction(string name, out MessageFunction func)
    {
      return _functions.TryGetValue(name, out func);
    }

    /// <summary>
    /// Return if a function with the specified name exists and execute it with the specified argument.
    /// </summary>
    /// <param name="name">The name of the function to execute.</param>
    /// <param name="argument">The argument to supply to the function.</param>
    /// <param name="value">The result of executing the function corresponding to the specified name with the specified argument.</param>
    /// <returns>If a function with the specified name exists.</returns>
    public bool TryExecuteFunction(string name, string argument, out string value)
    {
      var result = TryGetFunction(name, out var func);
      value = result ? func(argument) : null;
      return result;
    }
    #endregion

    #region Formatting and localizing references
    /// <summary>
    /// Format a message with the specified arguments using the specified format provider.
    /// </summary>
    /// <param name="formatProvider">The format provider to format the message with.</param>
    /// <param name="message">The message to format using the format provider.</param>
    /// <param name="arguments">The arguments to format inside of the message.</param>
    /// <returns>The formatted message, using the specified format provider.</returns>
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