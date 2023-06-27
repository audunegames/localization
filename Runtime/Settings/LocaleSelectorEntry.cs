using Audune.Utils;
using System;
using UnityEngine;

namespace Audune.Localization.Settings
{
  // Class that defines a locale selector entry
  [Serializable]
  public sealed class LocaleSelectorEntry
  {
    // Locale selector entry settings
    [Tooltip("The selector of the entry")]
    public LocaleSelector selector;
    [Tooltip("The execution mode of the entry")]
    public ExecutionMode executionMode;


    // Constructor
    public LocaleSelectorEntry(LocaleSelector selector, ExecutionMode executionMode)
    {
      this.selector = selector;
      this.executionMode = executionMode;
    }
  }
}