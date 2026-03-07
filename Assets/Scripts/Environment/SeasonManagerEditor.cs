using UnityEngine;
using UnityEditor;
using Environment;

[CustomEditor(typeof(SeasonManager))]
public class SeasonManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty sp = serializedObject.FindProperty("seasons");
        for (int i = 0; i < (int)SeasonManager.Season.Count; i++)
        {
            string name = ((SeasonManager.Season)i).ToString();
            EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(i), new GUIContent(name));
        }

        SerializedProperty dayLabelProp = serializedObject.FindProperty("dayLabel");
        SerializedProperty seasonLabelProp = serializedObject.FindProperty("seasonLabel");
        SerializedProperty calendarLabelProp = serializedObject.FindProperty("calendarLabel");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI Labels", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(dayLabelProp, new GUIContent("Day Label"));
        EditorGUILayout.PropertyField(seasonLabelProp, new GUIContent("Season Label"));
        EditorGUILayout.PropertyField(calendarLabelProp, new GUIContent("Calendar Label"));

        SeasonManager manager = (SeasonManager)target;
        SeasonData data = manager.RuntimeData;

        if (data != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Data", EditorStyles.boldLabel);

            SerializedObject so = new SerializedObject(data);
            EditorGUILayout.PropertyField(so.FindProperty("avgTemp"));
            EditorGUILayout.PropertyField(so.FindProperty("dayLength"));
            EditorGUILayout.PropertyField(so.FindProperty("sunColor"));
            so.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
    }
}