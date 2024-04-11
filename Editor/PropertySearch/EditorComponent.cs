using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines a component in the editor
  internal sealed class EditorComponent
  {
    // Component variables
    public readonly EditorAsset asset;
    public readonly GlobalObjectId gameObjectId;
    public readonly Type componentType;
    public readonly MonoScript componentScript;
    public readonly string componentPath;

    // Return the script path of the component
    public string componentScriptPath => AssetDatabase.GetAssetPath(componentScript);

    // Return the icon of the component
    public Texture2D componentIcon => AssetDatabase.GetCachedIcon(componentScriptPath) as Texture2D;

    // Return the target object of the component
    public UnityEngine.Object targetObject {
      get {
        var targetObject = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gameObjectId);
        return targetObject is GameObject targetGameObject && targetGameObject.TryGetComponent(componentType, out var targetComponent) ? targetComponent : targetObject;
      }
    }

    // Return a scriptable object for the target object
    public SerializedObject targetSerializedObject {
      get {
        var targetObject = this.targetObject;
        return targetObject != null ? new SerializedObject(targetObject) : null;
      }
    }


    // Constructor
    private EditorComponent(EditorAsset asset, GlobalObjectId gameObjectId, Type componentType, MonoScript componentScript, string componentPath)
    {
      this.asset = asset;
      this.gameObjectId = gameObjectId;
      this.componentType = componentType;
      this.componentScript = componentScript;
      this.componentPath = componentPath;
    }

    // Return the string representation of the component
    public override string ToString()
    {
      var stringBuilder = new StringBuilder(asset.ToString());
      if (!string.IsNullOrEmpty(componentPath))
        stringBuilder.Append(" › ").Append(componentPath.Replace("/", " › "));
      return stringBuilder.ToString();
    }


    #region Editor methods
    // Select the component in the editor
    public void SetAsSelection()
    {
      var targetObject = this.targetObject;
      if (targetObject != null)
      {
        Selection.SetActiveObjectWithContext(targetObject, targetObject);
        EditorGUIUtility.PingObject(targetObject);
      }
    }
    #endregion

    #region Creating components
    // Create a component from a MonoBehaviour
    public static EditorComponent FromMonoBehaviour(EditorAsset asset, MonoBehaviour monoBehaviour, GameObject rootGameObject)
    {
      return new EditorComponent(asset,
        GlobalObjectId.GetGlobalObjectIdSlow(monoBehaviour.gameObject),
        monoBehaviour.GetType(),
        MonoScript.FromMonoBehaviour(monoBehaviour),
        GetComponentPath(monoBehaviour, rootGameObject));
    }

    // Create a component from a ScriptableObject
    public static EditorComponent FromScriptableObject(EditorAsset asset, ScriptableObject scriptableObject)
    {
      return new EditorComponent(asset,
        GlobalObjectId.GetGlobalObjectIdSlow(scriptableObject),
        scriptableObject.GetType(),
        MonoScript.FromScriptableObject(scriptableObject),
        null);
    }

    // Return the path of a component relative to the root game object
    public static string GetComponentPath(Component component, GameObject rootGameObject = null)
    {
      var transform = component.transform;
      var path = new StringBuilder();

      while (transform != null && transform.gameObject != rootGameObject)
      {
        if (path.Length > 0)
          path.Insert(0, "/");

        path.Insert(0, transform.name);
        transform = transform.parent;
      }

      return path.ToString();
    }
    #endregion
  }
}