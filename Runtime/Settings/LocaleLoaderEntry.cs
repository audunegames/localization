using Audune.Utils;
using System;
using UnityEngine;

namespace Audune.Localization.Settings
{
  // Class that defines a locale loader entry
  [Serializable]
  public sealed class LocaleLoaderEntry
  {
    // Locale loader entry settings
    [Tooltip("The loader of the entry")]
    public LocaleLoader loader;
    [Tooltip("The execution mode of the entry")]
    public ExecutionMode executionMode;


    // Constructor
    public LocaleLoaderEntry(LocaleLoader loader, ExecutionMode executionMode)
    {
      this.loader = loader;
      this.executionMode = executionMode;
    }
  }
}