using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PassiveEffect), true)]
public class PassiveEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Dropdown button
        Rect buttonRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        string typeName = GetTypeName(property);

        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(typeName), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();

            var types = TypeCache
                .GetTypesDerivedFrom<PassiveEffect>()
                .Where(t => !t.IsAbstract);

            menu.AddItem(
                new GUIContent("None (Null)"),
                property.managedReferenceValue == null,
                () =>
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedProperties();
                });

            foreach (Type type in types)
            {
                menu.AddItem(
                    new GUIContent(type.Name),
                    false,
                    () =>
                    {
                        property.managedReferenceValue = Activator.CreateInstance(type);
                        property.serializedObject.ApplyModifiedProperties();
                    });
            }

            menu.ShowAsContext();
        }

        // Draw child fields
        if (property.managedReferenceValue != null)
        {
            Rect fieldRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + 2,
                position.width,
                position.height
            );

            EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        return EditorGUI.GetPropertyHeight(property, true)
             + EditorGUIUtility.singleLineHeight
             + 2;
    }

    private string GetTypeName(SerializedProperty property)
    {
        if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            return "Select Passive Effect...";

        string[] parts = property.managedReferenceFullTypename.Split(' ');
        return parts.Last();
    }
}