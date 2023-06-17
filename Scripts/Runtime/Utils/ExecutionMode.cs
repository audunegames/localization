using UnityEngine;

namespace Audune.Localization.Utils
{
  // Enum that defines when a component should execute
  public enum ExecutionMode
  {
    Always,           // Always run the locale selector
    PlayerAndEditor,  // Only run the locale selector in dedicated players and playing in the editor
    PlayerOnly,       // Only run the locale selector in dedicated players
  }


  // Class that defines extensions methods for execution modes
  public static class ExecutionModeExtensions
  {
    // Return if an execution mode should execute based on the current Unity environment
    public static bool ShouldExecute(this ExecutionMode executionMode)
    {
      return executionMode switch {
        ExecutionMode.PlayerAndEditor => Application.isPlaying,
        ExecutionMode.PlayerOnly => Application.isPlaying && !Application.isEditor,
        _ => true,
      };
    }
  }
}