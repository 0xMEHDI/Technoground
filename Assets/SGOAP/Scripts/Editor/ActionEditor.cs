using SGoap;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Action), true)]
[CanEditMultipleObjects]
public class ActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var it = serializedObject.GetIterator();
        it.NextVisible(true);

        var costEvaluator = serializedObject.FindProperty("CostEvaluator");
        var cost = serializedObject.FindProperty("Cost");

        var action = (Action)target;
        
        while (it.NextVisible(false))
        {
            if (it.propertyPath == "Cost")
                continue;

            if (it.propertyPath == "CostEvaluator")
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.PropertyField(action.CostEvaluationType == 0 ? cost : it, new GUIContent(cost.displayName), true);

                var options = new string[]
                {
                    "Default", "Evaluator"
                };

                action.CostEvaluationType = EditorGUILayout.Popup("", action.CostEvaluationType, options, GUILayout.Width(Screen.width * 0.2f));

                if (GUI.changed)
                    EditorUtility.SetDirty(action);
                
                GUILayout.EndHorizontal();

                continue;
            }

            if (it.propertyPath == "UsableEvaluator")
            {
                GUILayout.BeginHorizontal();

                var options = new string[] {"Default", "Evaluator"};
                
                if (action.AchievableEvaluationType == 1)
                    EditorGUILayout.PropertyField(it, new GUIContent(it.displayName), true);
                else
                    GUILayout.Label($"{it.displayName}");
                action.AchievableEvaluationType = EditorGUILayout.Popup("", action.AchievableEvaluationType, options, GUILayout.Width(Screen.width * 0.2f));

                if (GUI.changed)
                    EditorUtility.SetDirty(action);

                GUILayout.EndHorizontal();
                
                continue;
            }

            if (it.propertyPath == "AbortEvaluator")
            {
                GUILayout.BeginHorizontal();

                var options = new string[] { "Default", "Evaluator" };

                if (action.AbortEvaluatorType == 1)
                    EditorGUILayout.PropertyField(it, new GUIContent(it.displayName), true);
                else
                    GUILayout.Label($"{it.displayName}");
                action.AbortEvaluatorType = EditorGUILayout.Popup("", action.AbortEvaluatorType, options, GUILayout.Width(Screen.width * 0.2f));

                if (GUI.changed)
                    EditorUtility.SetDirty(action);

                GUILayout.EndHorizontal();

                continue;
            }

            if (it.propertyPath == "PreConditions" || it.propertyPath == "AfterEffects")
                GUILayout.BeginVertical("Box", GUILayout.Height(30));

            EditorGUILayout.PropertyField(it, new GUIContent(it.displayName), true);

            if (it.propertyPath == "PreConditions" || it.propertyPath == "AfterEffects")
                GUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}