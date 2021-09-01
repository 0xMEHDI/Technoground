using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MinMaxAttribute))]
public class MinMaxPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxAttribute minMax = attribute as MinMaxAttribute;

        if (property.type == nameof(RangeValue))
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var minProperty = property.FindPropertyRelative("Min");
            var maxProperty = property.FindPropertyRelative("Max");

            var minValue = minProperty.floatValue;
            var maxValue = maxProperty.floatValue;

            var sliderRect = position;
            var floatFieldSize = 50;
            sliderRect.width -= floatFieldSize + floatFieldSize;
            sliderRect.x += floatFieldSize;

            var max = Mathf.Max(minMax.Max, maxValue);
            EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref minValue, ref maxValue, minMax.Min, max);

            minProperty.floatValue = minValue;
            maxProperty.floatValue = maxValue;

            var left = position;
            left.width = floatFieldSize;

            var right = left;
            right.x = position.x + position.width - left.width;

            minProperty.floatValue = EditorGUI.FloatField(left, GUIContent.none, minProperty.floatValue);
            maxProperty.floatValue = EditorGUI.FloatField(right, GUIContent.none, maxProperty.floatValue);



            EditorGUI.EndProperty();
        }
    }
}
