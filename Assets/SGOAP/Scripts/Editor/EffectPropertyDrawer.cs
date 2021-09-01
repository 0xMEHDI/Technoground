using SGoap;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Effect), useForChildren: true)]
public class EffectPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var size = property.FindPropertyRelative("Action") != null ? 3 : 1;
        return base.GetPropertyHeight(property, label)*size;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var singleLine = EditorGUIUtility.singleLineHeight;
        EditorGUI.BeginProperty(position, label, property);
        label = GUIContent.none;

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        EditorGUIUtility.labelWidth = 10;

        position.height = singleLine;
        
        var width = position.width/2.5f;
        var rowPosition = position.x;
        var yPosition = position.y;

        var title = new Rect(position.x, position.y, width*2, position.height);

        var hasAction = property.FindPropertyRelative("Action") != null;
        var box = position;

        if (hasAction)
            box.y += singleLine;

        box.height = singleLine;

        EditorGUI.DrawRect(box, new Color(0, 0, 0.2f, 0.1f));

        if (hasAction)
        {
            EditorGUI.PropertyField(title, property.FindPropertyRelative("Action"), GUIContent.none);
            yPosition += singleLine;
        }
        else
        {
            if (!property.displayName.StartsWith("Element"))
                EditorGUI.LabelField(title, property.displayName);
        }

        var col0 = new Rect(rowPosition, yPosition, 50, position.height);
        rowPosition += 50;

        var col1 = new Rect(rowPosition, yPosition, width, position.height);
        rowPosition += width;
        var col2 = new Rect(rowPosition, yPosition, 40, position.height);
        rowPosition += 40;
        var col3 = new Rect(rowPosition, yPosition, 40, position.height);
        rowPosition += 40;
        var col4 = new Rect(rowPosition, yPosition, 40, position.height);

        EditorGUI.PropertyField(col0, property.FindPropertyRelative("StateType"), GUIContent.none);

        var stateTypeProp = property.FindPropertyRelative("StateType");
        var stateType = (EStateType)stateTypeProp.enumValueIndex;

        switch (stateType)
        {
            case EStateType.Ref:
                EditorGUI.PropertyField(col1, property.FindPropertyRelative("KeyReference"), GUIContent.none);
                break;
            case EStateType.Code:
                EditorGUI.PropertyField(col1, property.FindPropertyRelative("Concatenator"), GUIContent.none);
                break;

            case EStateType.Text:
                EditorGUI.PropertyField(col1, property.FindPropertyRelative("StringValue"), GUIContent.none);
                break;
        }

        var operatorProp = property.FindPropertyRelative("EffectOperator");
        var operatorType = (EChangeOperator)operatorProp.enumValueIndex;
        operatorProp.enumValueIndex = EditorGUI.Popup(col2, (int)operatorProp.enumValueIndex, new string[] { "=","x", "+=" });

        rowPosition = position.x;
        yPosition += position.height;
        var col21 = new Rect(rowPosition, yPosition, 50, position.height);
        rowPosition += 50;

        var col22 = new Rect(rowPosition, yPosition, width, position.height);
        rowPosition += width;
        var col23 = new Rect(rowPosition, yPosition, width, position.height);
        rowPosition += width;

        var spaceRect = title;
        spaceRect.x += title.width;
        spaceRect.width = 50;

        if (!hasAction)
            spaceRect.x = col4.x;

        EditorGUI.PropertyField(spaceRect, property.FindPropertyRelative("Space"), GUIContent.none);

        var operatorTypeValue = (EChangeOperator)property.FindPropertyRelative("EffectOperator").enumValueIndex;
        if (operatorTypeValue == EChangeOperator.Modify || operatorTypeValue == EChangeOperator.Set)
        {
            var hasRate = property.FindPropertyRelative("Rate") != null;
            EditorGUI.PropertyField(col3, property.FindPropertyRelative("Value"), GUIContent.none);

            if (hasRate)
                EditorGUI.PropertyField(col4, property.FindPropertyRelative("Rate"), new GUIContent("T"));
        }

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}