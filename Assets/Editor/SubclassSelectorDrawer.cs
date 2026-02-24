using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemEffect), true)]
public class ItemEffectDrawer : PropertyDrawer
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
            var types = TypeCache.GetTypesDerivedFrom<ItemEffect>().Where(t => !t.IsAbstract);

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

        // 2. Only draw the internal fields if an object actually exists
        if (property.managedReferenceValue != null)
        {
            // Move the drawing position down so it doesn't overlap the button
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height);
            
            // Draw the children of the class (amount, duration, etc.)
            EditorGUI.PropertyField(fieldRect, property, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // If empty, just show the button height. 
        // If populated, show button height + height of the class fields.
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 2;
    }

    private string GetTypeName(SerializedProperty property)
    {
        if (string.IsNullOrEmpty(property.managedReferenceFullTypename)) return "Select Effect Type...";
        string[] parts = property.managedReferenceFullTypename.Split(' ');
        return parts.Last();
    }
}