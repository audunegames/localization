using Audune.Utils.UnityEditor.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Enum that defines the type of an asset in the editor
  public enum EditorAssetType
  {
    Prefab,
    PrefabModel,
    PrefabVariant,
    ScriptableObject,
    Scene,
  };


  // Class that defines extension methods for asset types
  public static class EditorAssetTypeExtensions
  {
    // Return if the asset type is a prefab
    public static bool IsPrefab(this EditorAssetType type)
    {
      return type == EditorAssetType.Prefab || type == EditorAssetType.PrefabModel || type == EditorAssetType.PrefabVariant;
    }

    // Return if the asset type is a scriptable object
    public static bool IsScriptableObject(this EditorAssetType type) 
    {
      return type == EditorAssetType.ScriptableObject;
    }

    // Return if the asset type is a scene
    public static bool IsScene(this EditorAssetType type)
    {
      return type == EditorAssetType.Scene;
    }

    // Return the prefab type for the specified game object
    public static EditorAssetType GetPrefabAssetType(GameObject gameObject)
    {
      return PrefabUtility.GetPrefabAssetType(gameObject) switch {
        PrefabAssetType.Model => EditorAssetType.PrefabModel,
        PrefabAssetType.Variant => EditorAssetType.PrefabVariant,
        _ => EditorAssetType.Prefab,
      };
    }

    // Return the prefab type for the specified asset GUID
    public static EditorAssetType GetPrefabAssetType(string guid)
    {
      return GetPrefabAssetType(AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid)));
    }

    // Return the icon for the asset type
    public static Texture2D GetAssetIcon(this EditorAssetType type)
    {
      return type switch {
        EditorAssetType.Prefab => EditorIcons.prefab,
        EditorAssetType.PrefabModel => EditorIcons.prefabModel,
        EditorAssetType.PrefabVariant => EditorIcons.prefabVariant,
        EditorAssetType.ScriptableObject => EditorIcons.scriptableObject,
        EditorAssetType.Scene => EditorIcons.scene,
        _ => throw new ArgumentException($"Unsupported asset type {type}"),
      };
    }
  }
}