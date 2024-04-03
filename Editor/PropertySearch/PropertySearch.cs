using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a search for properties in the project
  public static class PropertySearch
  {
    // Search in the project
    public static IEnumerable<PropertySearchResult> SearchInProject(params string[] searchInFolders)
    {
      // Initialize the assets
      var prefabAssets = EditorAsset.FindPrefabs(searchInFolders);
      var scriptableObjectAssets = EditorAsset.FindScriptableObjects(searchInFolders);
      var sceneAssets = EditorAsset.FindScenes(searchInFolders);

      // Return the results found in the assets
      return SearchInPrefabs(prefabAssets).Concat(SearchInScriptableObjects(scriptableObjectAssets)).Concat(SearchInScenes(sceneAssets));
    }

    // Search in the specified prefab assets
    public static IEnumerable<PropertySearchResult> SearchInPrefabs(IReadOnlyList<EditorAsset> assets)
    {
      // Iterate over the prefab assets
      for (var assetIndex = 0; assetIndex < assets.Count; assetIndex++)
      {
        var asset = assets[assetIndex];

        // Increment the progress
        EditorUtility.DisplayProgressBar("Searching", $"Searching for localized strings in prefabs: {asset.assetPath}", assetIndex / (float)assets.Count);

        // Return the results found in the prefab in the asset
        var prefab = asset.GetAsset<GameObject>();
        foreach (var result in SearchInGameObject(asset, prefab, prefab))
          yield return result;
      }

      // Clear the progress
      EditorUtility.ClearProgressBar();
    }

    // Search in the specified scriptable object assets
    public static IEnumerable<PropertySearchResult> SearchInScriptableObjects(IReadOnlyList<EditorAsset> assets)
    {
      // Iterate over the scriptable object assets
      for (var assetIndex = 0; assetIndex < assets.Count; assetIndex++)
      {
        var asset = assets[assetIndex];

        // Increment the progress
        EditorUtility.DisplayProgressBar("Searching", $"Searching for localized strings in scriptable objects: {asset.assetPath}", assetIndex / (float)assets.Count);

        // Iterate over the scriptable objects in the asset
        var scriptableObjects = asset.GetAllAssets<ScriptableObject>();
        foreach (var scriptableObject in scriptableObjects)
        {
          // Return the results found in the scriptable object
          foreach (var result in SearchInObject(asset, EditorComponent.FromScriptableObject(asset, scriptableObject), scriptableObject))
            yield return result;
        }
      }

      // Clear the progress
      EditorUtility.ClearProgressBar();
    }

    // Search in the specified scene assets
    public static IEnumerable<PropertySearchResult> SearchInScenes(IReadOnlyList<EditorAsset> assets)
    {
      // Save the scene manager setup
      var sceneManagerSetup = EditorSceneManager.GetSceneManagerSetup();

      // Iterate over the scene assets
      for (var assetIndex = 0; assetIndex < assets.Count; assetIndex++)
      {
        var asset = assets[assetIndex];

        // Increment the progress
        EditorUtility.DisplayProgressBar("Searching", $"Searching for localized strings in scenes: {asset.assetPath}", assetIndex / (float)assets.Count);

        // Open the scene if not done already
        var scene = asset.GetScene();
        if (!scene.IsValid())
          scene = EditorSceneManager.OpenScene(asset.assetPath, OpenSceneMode.Single);

        // Iterate over the root game objects in the scene
        var rootGameObjects = scene.GetRootGameObjects();
        foreach (var rootGameObject in rootGameObjects)
        {
          // Return the results found in the root game object
          foreach (var result in SearchInGameObject(asset, rootGameObject, null))
            yield return result;
        }
      }

      // Clear the progress
      EditorUtility.ClearProgressBar();

      // Restore the scene manager setup
      if (assets.Count > 0 && sceneManagerSetup.Length > 0)
        EditorSceneManager.RestoreSceneManagerSetup(sceneManagerSetup);
    }

    // Search in the specified game object
    private static IEnumerable<PropertySearchResult> SearchInGameObject(EditorAsset asset, GameObject gameObject, GameObject rootGameObject)
    {
      // Iterate over the mono behaviours in the game object
      var monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
      foreach (var monoBehaviour in monoBehaviours)
      {
        // Return the results found in the mono behaviour
        foreach (var result in SearchInObject(asset, EditorComponent.FromMonoBehaviour(asset, monoBehaviour, rootGameObject), monoBehaviour))
          yield return result;
      }
    }

    // Get the results in the specified target object
    private static IEnumerable<PropertySearchResult> SearchInObject(EditorAsset asset, EditorComponent component, UnityEngine.Object target)
    {
      // Check if the target object is defined
      if (target == null)
        yield break;

      // Iterate over the instance fields in the target object
      foreach (var field in target.GetType().GetFields())
      {
        // Check if the field is serialized
        if (!(field.IsPublic && field.GetCustomAttribute<NonSerializedAttribute>() == null) && field.GetCustomAttribute<SerializeField>() == null && field.GetCustomAttribute<SerializeReference>() == null)
          continue;

        // Check if the field matches the search type
        if (typeof(LocalizedString).IsAssignableFrom(field.FieldType))
        {
          // Yield a new result for the field
          yield return new PropertySearchResult(asset, component, field.Name);
        }
        else if (field.FieldType.IsGenericType && typeof(LocalizedStringDictionary<>).IsAssignableFrom(field.FieldType.GetGenericTypeDefinition()))
        {
          // Yield all results for the dictionary field
          dynamic dictionary = field.GetValue(target);
          var index = 0;
          foreach (var key in dictionary.keys)
            yield return new PropertySearchResult(asset, component, field.Name, $"{ObjectNames.NicifyVariableName(field.Name)} › {key}", index++);
        }
      }
    }
  }
}