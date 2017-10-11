using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PiyoAttribute))]
public class PiyoDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		PiyoAttribute att = (PiyoAttribute)attribute;
		using (new EditorGUI.PropertyScope(position, label, property))
		{
			var buttonLabelRect = new Rect(position)
			{
				width = position.width / 2.0f
			};
			var buttonRect = new Rect(position)
			{
				x = position.width / 2.0f,
				width = position.width / 2.0f
			};

			EditorGUI.LabelField(buttonLabelRect, "Test");
			att.isButtonOn = GUI.Toggle(buttonRect, att.isButtonOn, att.isButtonOn ? "On" : "Off", GUI.skin.button);

		}
	}
}