using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BuffEffect), true)]
public class StatusEffectDrawer : PropertyDrawer
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
            // Automatically finds StatChangeEffect, DOTEffect, etc.
            var types = TypeCache.GetTypesDerivedFrom<BuffEffect>().Where(t => !t.IsAbstract);

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

        // 2. Draw the internal fields (stat, amount, damage, etc.) if an object exists
        if (property.managedReferenceValue != null)
        {
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height);
            
            // Draw the fields inside the chosen StatusEffect class
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 2;
    }

    private string GetTypeName(SerializedProperty property)
    {
        if (string.IsNullOrEmpty(property.managedReferenceFullTypename)) return "Select Status Type...";
        string[] parts = property.managedReferenceFullTypename.Split(' ');
        return parts.Last();
    }
}