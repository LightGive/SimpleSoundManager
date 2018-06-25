using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleSoundManager))]
public class SimpleSoundManagerEditor : Editor
{
	private SerializedObject m_serializedObj;
	private SerializedProperty m_audioClipListSeProp;
	private SerializedProperty m_audioClipListBgmProp;

	private void OnEnable()
	{
		m_serializedObj = new SerializedObject(target);
		m_audioClipListSeProp = m_serializedObj.FindProperty("audioClipListSe");
		m_audioClipListBgmProp = m_serializedObj.FindProperty("audioClipListBgm");


		List<AudioClip> bgmClipList = new List<AudioClip>();
		List<AudioClip> seClipList = new List<AudioClip>();
		bgmClipList = SimpleSoundManagerSetting.GetAudioClipListBgm();
		seClipList = SimpleSoundManagerSetting.GetAudioClipListSe();

		Debug.Log(seClipList.Count);

		m_audioClipListSeProp.arraySize = seClipList.Count;
		m_audioClipListBgmProp.arraySize = bgmClipList.Count;


		foreach (SimpleSoundManager t in targets)
		{
			t.audioClipListSe.Clear();
			t.audioClipListBgm.Clear();
		}

		for (int i = 0; i < bgmClipList.Count; i++)
		{
			foreach (SimpleSoundManager t in targets)
			{
				t.audioClipListBgm.Add(bgmClipList[i]);
				serializedObject.ApplyModifiedProperties();
			}
		}

		for (int i = 0; i < seClipList.Count; i++)
		{
			foreach (SimpleSoundManager t in targets)
			{
				t.audioClipListSe.Add(seClipList[i]);
				serializedObject.ApplyModifiedProperties();
			}
		}

		m_serializedObj.Update();
		EditorUtility.SetDirty(target);
		serializedObject.ApplyModifiedProperties();

		Debug.Log("AudioClipList Initialize");
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}