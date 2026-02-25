using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UniqueResourceCost), true)]
public class UniqueResourceCostDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Calculate the rect for the dropdown button
        Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        string typeName = GetTypeName(property);

        // The Dropdown Button allows you to pick the subclass (VoidCost, ShieldCost, etc.)
        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(typeName), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            // This searches your project for everything that inherits from UniqueResourceCost
            var types = TypeCache.GetTypesDerivedFrom<UniqueResourceCost>().Where(t => !t.IsAbstract);

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

        // 2. Only draw the internal fields (like 'amount') if an object actually exists
        if (property.managedReferenceValue != null)
        {
            Rect fieldRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, position.height);
            
            // This draws the "amount" field and any other variables inside the selected cost class
            EditorGUI.PropertyField(fieldRect, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        // button height + height of the class fields + small gap
        return EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 2;
    }

    private string GetTypeName(SerializedProperty property)
    {
        if (string.IsNullOrEmpty(property.managedReferenceFullTypename)) return "Select Cost Type...";
        string[] parts = property.managedReferenceFullTypename.Split(' ');
        return parts.Last();
    }
}