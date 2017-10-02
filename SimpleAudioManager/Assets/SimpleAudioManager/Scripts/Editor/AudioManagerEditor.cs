using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

/// <summary>
/// AudioManagerのインスペクタの操作
/// </summary>
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
	private const string AUDIO_SCRIPT_NAME = "AudioName.cs";
	private const string BGM_FOLDER_PATH = "\\Source\\BGM";
	private const string SE_FOLDER_PATH = "\\Source\\SE";

    private int listSize;
	private Object thisScript;
	private SerializedObject serializedObj;

	private SerializedProperty totalVolumeProp;
	private SerializedProperty sePlayerNumProp;
	private SerializedProperty bgmVolumeProp;
	private SerializedProperty seVolumeProp;
	private SerializedProperty bgmAudioClipListProp;
	private SerializedProperty seAudioClipListProp;
	private SerializedProperty bgmSourceInfoListProp;
	private SerializedProperty seSourceInfoListProp;
	private SerializedProperty bgmAudioMixerProp;
	private SerializedProperty seAudioMixerProp;


	/// <summary>
	/// Inspector拡張
	/// </summary>
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.LabelField("Volume Setting");
		EditorGUILayout.Slider(totalVolumeProp, 0.0f, 1.0f, "Total");
		EditorGUILayout.Slider(bgmVolumeProp, 0.0f, 1.0f, "BGM");
		EditorGUILayout.Slider(seVolumeProp, 0.0f, 1.0f, "SE");
		EditorGUILayout.Space();
		sePlayerNumProp.intValue = EditorGUILayout.IntField("オーディオの数", sePlayerNumProp.intValue);
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("AudioMixer");
		EditorGUILayout.BeginVertical(GUI.skin.box);
		{
			bgmAudioMixerProp.objectReferenceValue = EditorGUILayout.ObjectField("BGM", bgmAudioMixerProp.objectReferenceValue, typeof(AudioMixerGroup), false);
			seAudioMixerProp.objectReferenceValue = EditorGUILayout.ObjectField("SE", seAudioMixerProp.objectReferenceValue, typeof(AudioMixerGroup), false);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		//オーディオクリップのリストを表示する
		EditorGUILayout.LabelField("AudioClipList");
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
		if(GUILayout.Button("Reset"))
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
			thisScript = MonoScript.FromMonoBehaviour((AudioManager)target);
			var audioManagerScriptPath = AssetDatabase.GetAssetPath(thisScript);
			var audioManagerScriptFolderPath = Directory.GetParent(audioManagerScriptPath).FullName;
			var audioManagerFolderPath = Directory.GetParent(audioManagerScriptFolderPath).FullName;
			var audioNameScriptPath = audioManagerFolderPath + "/Scripts/" + AUDIO_SCRIPT_NAME;


			//BGMとSEのファイルパス
			var bgmSourcePath = ConvertSystemPathToUnityPath( audioManagerFolderPath + BGM_FOLDER_PATH);
			var seSourcePath = ConvertSystemPathToUnityPath( audioManagerFolderPath + SE_FOLDER_PATH);

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
				var obj = AssetDatabase.LoadAssetAtPath(filePath,typeof(object));
				if(obj != null)
				{
					if (obj.GetType() != typeof(AudioClip))
						continue;
					idx++;
					AudioClip audio = (AudioClip)obj;
					bgmObjList.Add(obj);

					var bgmInfo = new AudioClipInfo(idx, audio);
					bgmAudioClipListProp.arraySize++;
					foreach (AudioManager t in targets)
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
					foreach (AudioManager t in targets)
						t.seAudioClipList.Add(seInfo);
				}
			}

			//AudioNameを作成
			//Sourceフォルダに入っている音楽ファイルのファイル名の変数が入ったAudioNameファイル作成
			string audioFileNameExtension = Path.GetFileNameWithoutExtension(audioNameScriptPath);
			StringBuilder strBuilder = new StringBuilder();
			strBuilder.AppendFormat("public static class {0}", audioFileNameExtension).AppendLine();
			strBuilder.AppendLine("{");
			foreach (AudioClip bgm in bgmObjList)
				strBuilder.Append("\t").AppendFormat(@"public const string BGM_{0} = ""{1}"";", bgm.name, bgm.name).AppendLine();
			strBuilder.AppendLine("\t");
			foreach (AudioClip se in seObjList)
				strBuilder.Append("\t").AppendFormat(@"public const string SE_{0} = ""{1}"";", se.name, se.name).AppendLine();
			strBuilder.AppendLine("}");
			string directoryName = Path.GetDirectoryName(audioNameScriptPath) + "\\Scripts";
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(audioNameScriptPath, strBuilder.ToString(), Encoding.UTF8);
			AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
			//AudioNameの作成終了

			OnEnable();
			Repaint();
		}

		EditorUtility.SetDirty(target);
		serializedObject.ApplyModifiedProperties();

	}

	void ResetAudioClipInfo()
	{
		serializedObj.Update();

		foreach (AudioManager t in targets)
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
		bgmAudioMixerProp = serializedObj.FindProperty("bgmAudioMixerGroup");
		seAudioMixerProp = serializedObj.FindProperty("seAudioMixerGroup");
		sePlayerNumProp = serializedObj.FindProperty("sePlayerNum");
		bgmSourceInfoListProp = serializedObj.FindProperty("bgmSourceList");
		seSourceInfoListProp = serializedObj.FindProperty("seSourceList");
		bgmAudioClipListProp = serializedObj.FindProperty("bgmAudioClipList");
		seAudioClipListProp = serializedObj.FindProperty("seAudioClipList");

		//bgmReorderableList = new ReorderableList(serializedObj, bgmSourceInfoListProp);
		//bgmReorderableList.drawElementCallback = (rect, index, isActive, isFocused) => {
		//	var element = bgmSourceInfoListProp.GetArrayElementAtIndex(index);
		//	rect.height -= 10;
		//	rect.y += 10;
		//	EditorGUI.PropertyField(rect, element);
		//};
		//seReorderableList = new ReorderableList(serializedObj, seSourceInfoListProp);
		//seReorderableList.drawElementCallback = (rect, index, isActive, isFocused) => {
		//	var element = seSourceInfoListProp.GetArrayElementAtIndex(index);
		//	rect.height -= 10;
		//	rect.y += 10;
		//	EditorGUI.PropertyField(rect, element);
		//};


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


[CustomPropertyDrawer(typeof(SoundPlayer))]
public class AudioSourceInfoDrawer : PropertyDrawer
{
	private SoundPlayer audioSourceInfo;

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