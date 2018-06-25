using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleSoundManager))]
public class SimpleSoundManagerEditor : Editor
{
	private SerializedObject m_serializedObj;


	private void OnEnable()
	{
		m_serializedObj = new SerializedObject(target);
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}

	[InitializeOnLoadMethod]
	private void Init()
	{

	}
}