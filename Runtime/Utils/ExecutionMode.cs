using System;
using UnityEngine;

namespace Audune.Localization
{
  /// <summary>
  /// Enum that defines when a component should execute.
  /// </summary>
  [Flags]
  public enum ExecutionMode
  {
    Never = 0,
    DevelopmentBuildOnly = 1 << 0,
    ReleaseBuildOnly = 1 << 1,
    BuildOnly = ReleaseBuildOnly | DevelopmentBuildOnly,
    EditorOnly = 1 << 2,
    Always = ReleaseBuildOnly | DevelopmentBuildOnly | EditorOnly,
  }


  /// <summary>
  /// Class that defines extension methods for ean execution mode.
  /// </summary>
  public static class ExecutionModeExtensions
  {
    /// <summary>
    /// Return if the application is playing in a standalone player with a release build.
    /// </summary>
    /// <returns>Whether the application is playing in a standalone player with a release build.</returns>
    public static bool IsReleaseBuild()
    {
      return !Application.isEditor && !Debug.isDebugBuild;
    }

    /// <summary>
    /// Return if the application is playing in a standalone player with a development build.
    /// </summary>
    /// <returns>Whether the application is playing in a standalone player with a development build.</returns>
    public static bool IsDevelopmentBuild()
    {
      return !Application.isEditor && Debug.isDebugBuild;
    }

    /// <summary>
    /// Return if the application is playing in the editor.
    /// </summary>
    /// <returns>Whether the application is playing in the editor.</returns>
    public static bool IsEditor()
    {
      return Application.isEditor;
    }


    /// <summary>
    /// Return if the specified execution mode should execute based on the current Unity environment.
    /// </summary>
    /// <param name="mode">The execution mode to check.</param>
    /// <returns>Whether the specified execution mode should execute based on the current Unity environment.</returns>
    public static bool ShouldExecute(this ExecutionMode mode)
    {
      if (mode.HasFlag(ExecutionMode.ReleaseBuildOnly) && IsReleaseBuild())
        return true;
      else if (mode.HasFlag(ExecutionMode.DevelopmentBuildOnly) && IsDevelopmentBuild())
        return true;
      else if (mode.HasFlag(ExecutionMode.EditorOnly) && IsEditor())
        return true;
      else
        return false;
    }
  }
}