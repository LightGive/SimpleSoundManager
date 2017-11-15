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
		private SerializedProperty bgmDefaultVolumeProp;
		private SerializedProperty seDefaultVolumeProp;
		private SerializedProperty volumeChangeToSaveProp;

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
			serializedObject.Update();
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Volume", EditorStyles.boldLabel);

			EditorGUILayout.Slider(totalVolumeProp, 0.0f, 1.0f,"VolumeAll");
			EditorGUILayout.Slider(bgmVolumeProp, 0.0f, 1.0f, "Volume BGM");
			EditorGUILayout.Slider(seVolumeProp, 0.0f, 1.0f, "Volume SE");
			volumeChangeToSaveProp.boolValue = EditorGUILayout.Toggle("Change to Save", volumeChangeToSaveProp.boolValue);
			EditorGUILayout.Space();

			EditorGUILayout.LabelField("DefaultSetting", EditorStyles.boldLabel);
			EditorGUILayout.Slider(bgmDefaultVolumeProp, 0.0f, 1.0f, "Default Volume BGM");
			EditorGUILayout.Slider(seDefaultVolumeProp, 0.0f, 1.0f, "Default Volume BGM");

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Other", EditorStyles.boldLabel);
			sePlayerNumProp.intValue = EditorGUILayout.IntField("Create Player Num", sePlayerNumProp.intValue);
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

			//プレイ中
			//BGMのリストを作成する
			EditorGUILayout.LabelField("BGMList");
			//リセットボタンを押したとき
			if (GUILayout.Button("Reset"))
			{
				ResetAudioClipInfo();
				OnEnable();
				Repaint();
			}

			//Sourceフォルダにある音ファイルをインポートする
			if (GUILayout.Button("Import Audio Source"))
			{
				//Undoに登録
				Undo.RecordObjects(targets, "Import Audio");

				//一度現在のAudioClipのリストを消す
				ResetAudioClipInfo();

				//スクリプトのパスを取得する
				thisScript = MonoScript.FromMonoBehaviour((SimpleSoundManager)target);
				var audioManagerScriptPath = AssetDatabase.GetAssetPath(thisScript);
				var audioManagerScriptFolderPath = Directory.GetParent(audioManagerScriptPath).FullName;
				var audioManagerFolderPath = Directory.GetParent(audioManagerScriptFolderPath).FullName;
				var audioNameScriptPath = audioManagerFolderPath + "/Scripts/" + AUDIO_SCRIPT_NAME;

				//BGMとSEのファイルパス
				var bgmSourcePath = ConvertSystemPathToUnityPath(audioManagerFolderPath + BGM_FOLDER_PATH);
				var seSourcePath = ConvertSystemPathToUnityPath(audioManagerFolderPath + SE_FOLDER_PATH);

				//SEとBGMのフォルダを読み込む
				try
				{
					//フォルダがあるか確認
					if (!Directory.Exists(bgmSourcePath))
					{
						//BGMフォルダが無いとき、フォルダを新しく作成
						Directory.CreateDirectory(bgmSourcePath);
						Debug.Log("BGMフォルダが存在しないため作成しました。\nPath:" + bgmSourcePath);
					}
					if (!Directory.Exists(seSourcePath))
					{
						//SEフォルダが無いとき、フォルダを新しく作成
						Directory.CreateDirectory(seSourcePath);
						Debug.Log("SEフォルダが存在しないため作成しました。\nPath:" + seSourcePath);
					}
				}

				//エラーの時、デバッグを出す
				catch (IOException ex)
				{
					Debug.Log(ex.Message);
				}

				string[] fileEntriesBgm = Directory.GetFiles(bgmSourcePath, "*", SearchOption.AllDirectories);
				string[] fileEntriesSe = Directory.GetFiles(seSourcePath, "*", SearchOption.AllDirectories);
				List<Object> bgmObjList = new List<Object>();
				List<Object> seObjList = new List<Object>();

				//番号を表示する用の変数
				int idx = 0;

				//BGMのフォルダ内ファイル1つずつ検索
				for (int i = 0; i < fileEntriesBgm.Length; i++)
				{
					var filePath = fileEntriesBgm[i];
					filePath = ConvertSystemPathToUnityPath(filePath);
					var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(object));
					if (obj != null)
					{
						if (obj.GetType() != typeof(AudioClip))
							continue;
						idx++;
						AudioClip audio = (AudioClip)obj;
						bgmObjList.Add(obj);

						var bgmInfo = new AudioClipInfo(idx, audio);
						bgmAudioClipListProp.arraySize++;
						foreach (SimpleSoundManager t in targets)
						{
							t.bgmAudioClipList.Add(bgmInfo);
							serializedObject.ApplyModifiedProperties();
						}
					}
				}

				//番号を初期化
				idx = 0;

				//SEのフォルダ内ファイル1つずつ検索
				for (int i = 0; i < fileEntriesSe.Length; i++)
				{
					var filePath = fileEntriesSe[i];
					filePath = ConvertSystemPathToUnityPath(filePath);
					var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(object));
					if (obj != null)
					{
						if (obj.GetType() != typeof(AudioClip))
							continue;

						idx++;
						AudioClip audio = (AudioClip)obj;
						seObjList.Add(obj);

						var seInfo = new AudioClipInfo(idx, audio);
						seAudioClipListProp.arraySize++;
						foreach (SimpleSoundManager t in targets)
							t.seAudioClipList.Add(seInfo);
					}
				}

				OnEnable();
				Repaint();
			}
			

			EditorUtility.SetDirty(target);
			serializedObject.ApplyModifiedProperties();

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
			bgmDefaultVolumeProp = serializedObj.FindProperty("bgmDefaultVolume");
			seDefaultVolumeProp = serializedObj.FindProperty("seDefaultVolume");

			volumeChangeToSaveProp = serializedObj.FindProperty("volumeChangeToSave");
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

				//Debug.Log("Prop" + property);
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