using System;
using UnityEngine;

namespace Audune.Localization
{
  // Enum that defines when a component should execute
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


  // Class that defines extension methods for ean execution mode
  public static class ExecutionModeExtensions
  {
    // Return if the application is playing in a standalone player with a release build
    public static bool IsReleaseBuild()
    {
      return !Application.isEditor && !Debug.isDebugBuild;
    }

    // Return if the application is playing in a standalone player with a development build
    public static bool IsDevelopmentBuild()
    {
      return !Application.isEditor && Debug.isDebugBuild;
    }

    // Return if the application is playing in the editor
    public static bool IsEditor()
    {
      return Application.isEditor;
    }


    // Return if an execution mode should execute based on the current Unity environment
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