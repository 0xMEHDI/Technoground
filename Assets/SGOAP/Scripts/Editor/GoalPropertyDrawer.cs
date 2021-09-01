using SGoap;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Goal))]
public class GoalPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        label = GUIContent.none;

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 45;

        // Calculate rects
        var keyReference = new Rect(position.x, position.y, position.width * 0.5f, position.height);
        var once = new Rect(position.x + keyReference.width + EditorGUIUtility.labelWidth, position.y, position.width * 0.3f, position.height);
        var priority = new Rect(position.x + keyReference.width + once.width, position.y, position.width * 0.2f, position.height);

        var operatorProp = property.FindPropertyRelative("Operator");
        var operatorValue = (EOperator)operatorProp.enumValueIndex;
        var rowPosition = position.x;
        var yPosition = position.y;
        var width = position.width / 2.0f;

        var col1 = new Rect(rowPosition, yPosition, 50, position.height);
        rowPosition += 50;
        var col2 = new Rect(rowPosition, yPosition, width, position.height);
        rowPosition += width;
        var col3 = new Rect(rowPosition, yPosition, 30, position.height);
        rowPosition += 40 + 30;
        var col4 = new Rect(rowPosition, yPosition, Screen.width * 0.3f, position.height);


        EditorGUI.PropertyField(col1, property.FindPropertyRelative("StateType"), GUIContent.none);

        var stateTypeProp = property.FindPropertyRelative("StateType");
        var stateType = (EStateType)stateTypeProp.enumValueIndex;

        switch (stateType)
        {
            case EStateType.Ref:
                EditorGUI.PropertyField(col2, property.FindPropertyRelative("KeyReference"), GUIContent.none);
                break;
            case EStateType.Code:
                EditorGUI.PropertyField(col2, property.FindPropertyRelative("Concatenator"), GUIContent.none);
                break;

            case EStateType.Text:
                EditorGUI.PropertyField(col2, property.FindPropertyRelative("StringValue"), GUIContent.none);
                break;
        }

        EditorGUI.PropertyField(col3, property.FindPropertyRelative("Once"),new GUIContent("Once"));
        EditorGUI.PropertyField(col4, property.FindPropertyRelative("Priority"), new GUIContent("Priority"));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}