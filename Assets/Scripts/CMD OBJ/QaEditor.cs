using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        // Calculate layout positions
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        Rect dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        // Draw standard field label
        EditorGUI.LabelField(labelRect, label);

        // Get actual type from the managed reference path
        Type baseType = GetPropertyType(property);
        string currentTypeName = property.managedReferenceFullTypename;
        string currentDisplay = string.IsNullOrEmpty(currentTypeName) ? "Null (Empty)" : currentTypeName.Split(' ').Last().Split('.').Last();

        // Draw the type selector dropdown button
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(currentDisplay), FocusType.Keyboard))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Null"), string.IsNullOrEmpty(currentTypeName), () => ClearManagedReference(property));

            // Find all valid subclasses in the project
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && baseType.IsAssignableFrom(t));

            foreach (Type type in types)
            {
                string menuPath = type.FullName.Replace('.', '/');
                bool isSelected = currentTypeName.EndsWith(type.FullName);

                menu.AddItem(new GUIContent(menuPath), isSelected, () => SetManagedReference(property, type));
            }
            menu.DropDown(dropdownRect);
        }

        // Draw the rest of the child properties beneath the dropdown
        EditorGUI.indentLevel++;
        Rect childPosition = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, position.width, position.height);

        // Show fields if an object is selected
        if (!string.IsNullOrEmpty(currentTypeName))
        {
            SerializedProperty endProperty = property.GetEndProperty();
            SerializedProperty iterator = property.Copy();
            bool enterChildren = true;

            float currentY = childPosition.y;
            while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
            {
                enterChildren = false;
                float height = EditorGUI.GetPropertyHeight(iterator, true);
                Rect fieldRect = new Rect(childPosition.x, currentY, childPosition.width, height);
                EditorGUI.PropertyField(fieldRect, iterator, true);
                currentY += height + EditorGUIUtility.standardVerticalSpacing;
            }
        }
        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference || string.IsNullOrEmpty(property.managedReferenceFullTypename))
        {
            return EditorGUIUtility.singleLineHeight;
        }

        float totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        SerializedProperty endProperty = property.GetEndProperty();
        SerializedProperty iterator = property.Copy();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            enterChildren = false;
            totalHeight += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }

    private void SetManagedReference(SerializedProperty property, Type type)
    {
        object instance = Activator.CreateInstance(type);
        property.managedReferenceValue = instance;
        property.serializedObject.ApplyModifiedProperties();
    }

    private void ClearManagedReference(SerializedProperty property)
    {
        property.managedReferenceValue = null;
        property.serializedObject.ApplyModifiedProperties();
    }

    private Type GetPropertyType(SerializedProperty property)
    {
        // Extract type information from Unity's type string format: "Type Namespace.ClassName"
        string[] typeInfo = property.managedReferenceFieldTypename.Split(' ');
        if (typeInfo.Length < 2) return typeof(object);
        Assembly assembly = Assembly.Load(typeInfo[0]);
        return assembly.GetType(typeInfo[1]);
    }
}
