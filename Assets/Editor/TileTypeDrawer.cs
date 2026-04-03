using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(TileType), true)]
public class TileTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Draw the Type Selection Button
        Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        string typeName = property.managedReferenceValue?.GetType().Name ?? "None (Select Type)";

        if (GUI.Button(buttonRect, new GUIContent(typeName, "Click to change Tile Type"), EditorStyles.popup))
        {
            ShowTypeMenu(property);
        }

        // 2. Draw the Child Properties (the variables inside the class)
        if (property.managedReferenceValue != null)
        {
            // Move down by one line height + spacing
            Rect childRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, 0);
            
            // This loop iterates through the variables inside AbilityTile, StatTile, etc.
            SerializedProperty endProperty = property.GetEndProperty();
            SerializedProperty child = property.Copy();
            child.NextVisible(true); // Enter the class

            while (!SerializedProperty.EqualContents(child, endProperty))
            {
                float height = EditorGUI.GetPropertyHeight(child);
                childRect.height = height;
                
                EditorGUI.PropertyField(childRect, child, true);
                
                childRect.y += height + 2;
                if (!child.NextVisible(false)) break;
            }
        }

        EditorGUI.EndProperty();
    }

    // Tells Unity how tall this whole block should be in the Inspector
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight; // Height of the button

        if (property.managedReferenceValue != null)
        {
            SerializedProperty endProperty = property.GetEndProperty();
            SerializedProperty child = property.Copy();
            child.NextVisible(true);

            while (!SerializedProperty.EqualContents(child, endProperty))
            {
                height += EditorGUI.GetPropertyHeight(child) + 2;
                if (!child.NextVisible(false)) break;
            }
        }

        return height;
    }

    private void ShowTypeMenu(SerializedProperty property)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("None"), property.managedReferenceValue == null, () => {
            property.managedReferenceValue = null;
            property.serializedObject.ApplyModifiedProperties();
        });

        var types = Assembly.GetAssembly(typeof(TileType)).GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TileType)));

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            });
        }
        menu.ShowAsContext();
    }
}