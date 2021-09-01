using SGoap;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EffectAndValueAttribute))]
[CustomPropertyDrawer(typeof(EffectAttribute))]
[CustomPropertyDrawer(typeof(State))]
public class StatePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EffectAttribute effectAttribute = attribute as EffectAttribute;
        EffectAndValueAttribute effectAndValue = attribute as EffectAndValueAttribute;

        label = GUIContent.none;

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var operatorProp = property.FindPropertyRelative("Operator");
        var operatorValue = (EOperator)operatorProp.enumValueIndex;
        var rowPosition = position.x;
        var yPosition = position.y;
        var width = position.width/2.0f;

        var col1 = new Rect(rowPosition, yPosition, 50, position.height);
        rowPosition += 50;
        var col2 = new Rect(rowPosition, yPosition, width, position.height);
        rowPosition += width;
        var col3 = new Rect(rowPosition, yPosition, 40, position.height);
        rowPosition += 40;
        var col4 = new Rect(rowPosition, yPosition, 60, position.height);

        rowPosition = position.x;
        yPosition += EditorGUIUtility.singleLineHeight;


        if(effectAttribute != null)
            col2.width = position.width-col1.width;

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


        if (effectAndValue != null)
        {
            EditorGUI.LabelField(col3, "Value");
            EditorGUI.PropertyField(col4, property.FindPropertyRelative("Value"), GUIContent.none);
        }
        else
        {
            if (effectAttribute == null)
            {
                var v = EditorGUI.Popup(col3, (int)operatorValue, new string[] { "*", "==", "<", ">", "!=" });
                operatorProp.enumValueIndex = v;

                if (operatorValue != EOperator.Contains && operatorValue != EOperator.DoesNotContain)
                    EditorGUI.PropertyField(col4, property.FindPropertyRelative("Value"), GUIContent.none);
            }
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}