using Audune.Utils.UnityEditor.Editor;
using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace Audune.Localization.Editor
{
  // Class that defines an editor for locales
  [CustomEditor(typeof(Locale))]
  public sealed class LocaleEditor : UnityEditor.Editor
  {
    // Properties of the editor
    private SerializedProperty _code;
    private SerializedProperty _altCodes;
    private SerializedProperty _englishName;
    private SerializedProperty _nativeName;
    private SerializedProperty _decimalNumberFormat;
    private SerializedProperty _percentNumberFormat;
    private SerializedProperty _currencyNumberFormat;
    private SerializedProperty _shortDateFormat;
    private SerializedProperty _longDateFormat;
    private SerializedProperty _shortTimeFormat;
    private SerializedProperty _longTimeFormat;
    private SerializedProperty _strings;

    // Helper objects of the editor
    private NumberContext _number;
    private DateTime _date;

    // Foldout state of the editor
    private bool _settingsFoldout = true;
    private bool _formattingFoldout = true;
    private bool _stringTableFoldout = true;



    // Return the target object of the editor
    public new Locale target => serializedObject.targetObject as Locale;


    // Constructor
    public void OnEnable()
    {
      // Initialize the properties
      _code = serializedObject.FindProperty("_code");
      _altCodes = serializedObject.FindProperty("_altCodes");
      _englishName = serializedObject.FindProperty("_englishName");
      _nativeName = serializedObject.FindProperty("_nativeName");
      _decimalNumberFormat = serializedObject.FindProperty("_decimalNumberFormat");
      _percentNumberFormat = serializedObject.FindProperty("_percentNumberFormat");
      _currencyNumberFormat = serializedObject.FindProperty("_currencyNumberFormat");
      _shortDateFormat = serializedObject.FindProperty("_shortDateFormat");
      _longDateFormat = serializedObject.FindProperty("_longDateFormat");
      _shortTimeFormat = serializedObject.FindProperty("_shortTimeFormat");
      _longTimeFormat = serializedObject.FindProperty("_longTimeFormat");
      _strings = serializedObject.FindProperty("_strings");

      // Initialize the helper objects
      _number = NumberContext.Of(1.23f);
      _date = DateTime.Now;
    }


    // Draw the inspector GUI
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      EditorGUI.BeginChangeCheck();

      _settingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_settingsFoldout, "Settings");
      if (_settingsFoldout)
      {
        EditorGUILayout.PropertyField(_code);
        EditorGUILayout.PropertyField(_englishName);
        EditorGUILayout.PropertyField(_nativeName);
        using (new EditorGUI.DisabledGroupScope(true)) 
        {
          var culture = target.culture;
          var cultureString = !culture.Equals(CultureInfo.InvariantCulture) ? $"{culture.IetfLanguageTag} / {culture.DisplayName} / {culture.NativeName}" : "(invariant)";
          EditorGUILayout.TextField(new GUIContent("Culture", "The culture associated with the locale"), cultureString);
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_altCodes, new GUIContent("Alternative Codes", _altCodes.tooltip));

        EditorGUILayout.Space();
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      _formattingFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_formattingFoldout, "Formatting");
      if (_formattingFoldout)
      {
        Rect rect;

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Decimal Number", _decimalNumberFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _decimalNumberFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatNumber(_number, NumberFormatStyle.Decimal), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Percent Number", _percentNumberFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _percentNumberFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatNumber(_number, NumberFormatStyle.Percent), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Currency Number", _currencyNumberFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _currencyNumberFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatNumber(_number, NumberFormatStyle.Currency), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Short Date", _shortDateFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _shortDateFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatDate(_date, DateFormatStyle.Short), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Long Date", _longDateFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _longDateFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatDate(_date, DateFormatStyle.Long), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Short Time", _shortTimeFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _shortTimeFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatTime(_date, DateFormatStyle.Short), MessageType.None);

        rect = EditorGUI.PrefixLabel(EditorGUILayout.GetControlRect(true), new GUIContent("Long Time", _longTimeFormat.tooltip));
        EditorGUI.PropertyField(rect.AlignLeft(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), _longTimeFormat, GUIContent.none);
        EditorGUI.HelpBox(rect.AlignRight(0.5f * (rect.width - EditorGUIUtility.standardVerticalSpacing)), target.FormatTime(_date, DateFormatStyle.Long), MessageType.None);

        EditorGUILayout.Space();
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      _stringTableFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_stringTableFoldout, "String Table");
      if (_stringTableFoldout)
      {
        EditorGUILayout.PropertyField(_strings);
      }
      EditorGUILayout.EndFoldoutHeaderGroup();

      if (EditorGUI.EndChangeCheck())
        serializedObject.ApplyModifiedProperties();
    }
  }
}