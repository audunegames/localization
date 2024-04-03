using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audune.Localization.Editor
{
  // Class that defines an asset in the editor
  public sealed class EditorAsset
  {
    // Type of the asset
    public readonly EditorAssetType type;

    // Unique identifier of the asset
    public readonly string guid;


    // Return the path of the asset
    public string assetPath => AssetDatabase.GUIDToAssetPath(guid);

    // Return the name of the asset
    public string assetName => Path.GetFileNameWithoutExtension(assetPath);

    // Return the directory name of the asset
    public string assetDirectoryName => Path.GetDirectoryName(assetPath).Replace('\\', '/');

    // Return the icon of the asset
    public Texture2D assetIcon => type.GetAssetIcon();


    // Constructor
    public EditorAsset(EditorAssetType type, string guid)
    {
      this.type = type;
      this.guid = guid;
    }


    // Return the asset of the specified type based on the asset
    public T GetAsset<T>() where T : Object
    {
      return AssetDatabase.LoadAssetAtPath<T>(assetPath);
    }

    // Return all assets of the specified type based on the asset
    public IReadOnlyList<T> GetAllAssets<T>() where T : Object
    {
      return AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<T>().ToList();
    }

    // Get the scene based on the asset
    public Scene GetScene()
    {
      return SceneManager.GetSceneByPath(assetPath);
    }
    
    // Return the string representation of the component
    public override string ToString()
    {
      return assetName;
    }


    #region Editor methods
    // Select the asset in the editor
    public void SetAsSelection()
    {
      var targetObject = GetAsset<Object>();
      if (targetObject != null)
      {
        Selection.SetActiveObjectWithContext(targetObject, targetObject);
        EditorGUIUtility.PingObject(targetObject);
      }
    }
    #endregion

    #region Finding assets
    // Find all prefab assets
    public static IReadOnlyList<EditorAsset> FindPrefabs(params string[] searchInFolders)
    {
      return AssetDatabase.FindAssets("t:GameObject", searchInFolders)
        .Select(guid => new EditorAsset(EditorAssetTypeExtensions.GetPrefabAssetType(guid), guid))
        .ToList();
    }

    // Find all scriptable object assets
    public static IReadOnlyList<EditorAsset> FindScriptableObjects(params string[] searchInFolders)
    {
      return AssetDatabase.FindAssets("t:ScriptableObject", searchInFolders)
        .Distinct()
        .Select(guid => new EditorAsset(EditorAssetType.ScriptableObject, guid))
        .ToList();
    }

    // Find all scene assets
    public static IReadOnlyList<EditorAsset> FindScenes(params string[] searchInFolders)
    {
      return AssetDatabase.FindAssets("t:Scene", searchInFolders)
        .Select(guid => new EditorAsset(EditorAssetType.Scene, guid))
        .ToList();
    }
    #endregion
  }
}