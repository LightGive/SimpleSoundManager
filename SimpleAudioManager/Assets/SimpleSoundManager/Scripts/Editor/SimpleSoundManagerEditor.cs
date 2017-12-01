using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

namespace LightGive
{
	/// <summary>
	/// AudioManagerのインスペクタの操作
	/// </summary>
	[CustomEditor(typeof(SimpleSoundManager))]
	public class SimpleSoundManagerEditor : Editor
	{
		private const string AUDIO_SCRIPT_NAME = "AudioName.cs";
		private const string BGM_FOLDER_PATH = "\\Source\\BGM";
		private const string SE_FOLDER_PATH = "\\Source\\SE";

		private int listSize;
		private Object thisScript;
		private SerializedObject serializedObj;
		private SerializedProperty totalVolumeProp;
		private SerializedProperty bgmVolumeProp;
		private SerializedProperty seVolumeProp;
		private SerializedProperty volumeChangeToSaveProp;
		private SerializedProperty volumeLoadAwakeProp;

		private SerializedProperty sePlayerNumProp;
		private SerializedProperty bgmAudioClipListProp;
		private SerializedProperty seAudioClipListProp;
		private SerializedProperty bgmSourceInfoListProp;
		private SerializedProperty seSourceInfoListProp;
		private SerializedProperty bgmAudioMixerProp;
		private SerializedProperty seAudioMixerProp;


		private AudioClipList bgmClipList;
		private AudioClipList seClipList;

		/// <summary>
		/// Inspector拡張
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObj.Update();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Volume", EditorStyles.boldLabel);

			totalVolumeProp.floatValue = EditorGUILayout.Slider("Total", totalVolumeProp.floatValue, 0.0f, 1.0f);
			EditorGUILayout.Slider(bgmVolumeProp, 0.0f, 1.0f, "BGM");
			EditorGUILayout.Slider(seVolumeProp, 0.0f, 1.0f, "SE");
			volumeChangeToSaveProp.boolValue = EditorGUILayout.Toggle("Change to Save", volumeChangeToSaveProp.boolValue);
			volumeLoadAwakeProp.boolValue = EditorGUILayout.Toggle("Awake load volume ", volumeLoadAwakeProp.boolValue);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
			sePlayerNumProp.intValue = EditorGUILayout.IntField("Create SE Player Num", sePlayerNumProp.intValue);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("AudioMixer", EditorStyles.boldLabel);

			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				bgmAudioMixerProp.objectReferenceValue = EditorGUILayout.ObjectField("BGM", bgmAudioMixerProp.objectReferenceValue, typeof(AudioMixerGroup), false);
				seAudioMixerProp.objectReferenceValue = EditorGUILayout.ObjectField("SE", seAudioMixerProp.objectReferenceValue, typeof(AudioMixerGroup), false);

			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			//オーディオクリップのリストを表示する
			EditorGUILayout.LabelField("AudioClipList", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical(GUI.skin.box);
			{
				EditorGUILayout.LabelField("BGM");
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					if (bgmAudioClipListProp.arraySize == 0)
					{
						EditorGUILayout.LabelField("Nothing");
					}
					else
					{
						for (int i = 0; i < bgmAudioClipListProp.arraySize; i++)
						{
							SerializedProperty prop = bgmAudioClipListProp.GetArrayElementAtIndex(i);
							EditorGUILayout.PropertyField(prop);
						}
					}
				}
				EditorGUILayout.EndVertical();


				EditorGUILayout.LabelField("SE");
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					if (seAudioClipListProp.arraySize == 0)
					{
						EditorGUILayout.LabelField("Nothing");
					}
					else
					{
						for (int i = 0; i < seAudioClipListProp.arraySize; i++)
						{
							SerializedProperty prop = seAudioClipListProp.GetArrayElementAtIndex(i);
							EditorGUILayout.PropertyField(prop);
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndVertical();

			EditorUtility.SetDirty(target);
			serializedObj.ApplyModifiedProperties();
		}

		void ResetAudioClipInfo()
		{
			serializedObj.Update();

			foreach (SimpleSoundManager t in targets)
			{
				t.bgmAudioClipList.Clear();
				t.seAudioClipList.Clear();
			}
			serializedObj.ApplyModifiedProperties();
		}

		void OnEnable()
		{
			serializedObj = new SerializedObject(target);
			totalVolumeProp = serializedObj.FindProperty("totalVolume");
			bgmVolumeProp = serializedObj.FindProperty("bgmVolume");
			seVolumeProp = serializedObj.FindProperty("seVolume");

			volumeChangeToSaveProp = serializedObj.FindProperty("volumeChangeToSave");
			volumeLoadAwakeProp = serializedObj.FindProperty("volumeLoadAwake");
			bgmAudioMixerProp = serializedObj.FindProperty("bgmAudioMixerGroup");
			seAudioMixerProp = serializedObj.FindProperty("seAudioMixerGroup");
			sePlayerNumProp = serializedObj.FindProperty("sePlayerNum");
			bgmSourceInfoListProp = serializedObj.FindProperty("bgmSourceList");
			seSourceInfoListProp = serializedObj.FindProperty("seSourceList");
			bgmAudioClipListProp = serializedObj.FindProperty("bgmAudioClipList");
			seAudioClipListProp = serializedObj.FindProperty("seAudioClipList");

			bgmClipList = AudioNameCreator.BgmClipList;
			seClipList = AudioNameCreator.SeClipList;

			ResetAudioClipInfo();


			seAudioClipListProp.arraySize = seClipList.data.Count;
			bgmAudioClipListProp.arraySize = bgmClipList.data.Count;

			for (int i = 0; i < bgmClipList.data.Count; i++)
			{
				//bgmAudioClipListProp.arraySize++;
				foreach (SimpleSoundManager t in targets)
				{
					t.bgmAudioClipList.Add(bgmClipList.data[i]);
					serializedObject.ApplyModifiedProperties();
				}
			}
			
			for (int i = 0; i < seClipList.data.Count; i++)
			{
				//seAudioClipListProp.arraySize++;
				foreach (SimpleSoundManager t in targets)
				{
					t.seAudioClipList.Add(seClipList.data[i]);
					serializedObject.ApplyModifiedProperties();
				}
			}
			serializedObj.Update();

			EditorUtility.SetDirty(target);
			serializedObject.ApplyModifiedProperties();

			Repaint();
		}


		string ConvertSystemPathToUnityPath(string _path)
		{
			int index = _path.IndexOf("Assets");
			if (index > 0)
			{
				_path = _path.Remove(0, index);
			}
			_path.Replace("\\", "/");
			return _path;
		}
	}

	[CustomPropertyDrawer(typeof(SoundEffectPlayer))]
	public class AudioSourceInfoDrawer : PropertyDrawer
	{
		private SoundEffectPlayer audioSourceInfo;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using (new EditorGUI.PropertyScope(position, label, property))
			{
				EditorGUIUtility.labelWidth = 10;

				var audioSourceProp = property.FindPropertyRelative("audioSource");
				var audioSourceRect = new Rect(position)
				{
					width = position.width,
					y = position.y
				};
			}
		}
	}
}