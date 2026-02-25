using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AbilityEffect), true)]
public class AbilityEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Calculate the rect for the dropdown button
        Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        string typeName = GetTypeName(property);

        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(typeName), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            // Automatically finds AtkAbilityEffect, DrainAtkAbilityEffect, etc.
            var types = TypeCache.GetTypesDerivedFrom<AbilityEffect>().Where(t => !t.IsAbstract);

            menu.AddItem(new GUIContent("None (Null)"), property.managedReferenceValue == null, () => 
            {
                property.managedReferenceValue = null;
                property.serializedObject.ApplyModifiedProperties();
            });

            foreach (var type in types)
            {
                menu.AddItem(new GUIContent(type.Name), false, () => 
                {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        // 2. Draw the internal fields (scalingStat, multiplier, etc.) if selected
        if (property.managedReferenceValue != null)
        {
            // Move position down so fields don't overlap the dropdown button
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height);
            
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        // Returns button height + height of all visible fields in the effect class
        return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 2;
    }

    private string GetTypeName(SerializedProperty property)
    {
        if (string.IsNullOrEmpty(property.managedReferenceFullTypename)) return "Select Effect Type...";
        string[] parts = property.managedReferenceFullTypename.Split(' ');
        return parts.Last();
    }
}