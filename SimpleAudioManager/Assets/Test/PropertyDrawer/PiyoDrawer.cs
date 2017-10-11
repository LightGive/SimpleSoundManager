using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PiyoDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		AudioClipInfoAttribute att = (AudioClipInfoAttribute)attribute;
		using (new EditorGUI.PropertyScope(position, label, property))
		{
		}
	}
}