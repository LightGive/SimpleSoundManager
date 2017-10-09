using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// オーディオのファイル名を定数で管理するクラスを自動で作成するスクリプト
/// </summary>
public class AudioNameCreator : AssetPostprocessor
{
	private const string AUDIO_SCRIPT_NAME = "AudioName.cs";
	private const string BGM_FOLDER_PATH = "\\Source\\BGM";
	private const string SE_FOLDER_PATH = "\\Source\\SE";
	private const string COMMAND_NAME = "Tools/AudioManager/Create AudioName"; // コマンド名

	//オーディオファイルが編集または追加されたら自動でAUDIO.csを作成する
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		List<string[]> assetsList = new List<string[]>(){
			importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
		};

		//for (int i = 0; i < assetsList.Count; i++)
		//{
		//	for (int ii = 0; ii < assetsList[i].Length; ii++)
		//	{
		//		Debug.Log(assetsList[i][ii].ToString());
		//	}
		//}

		Create();
	}

	//スクリプトを作成します
	[MenuItem(COMMAND_NAME)]
	private static void Create()
	{
		string[] res = System.IO.Directory.GetFiles(Application.dataPath, "AudioNameCreator.cs", SearchOption.AllDirectories);

		//スクリプトのパスを取得する
		var audioManagerScriptFolderPath = Directory.GetParent(res[0]).FullName;
		var audioNameCreatorFolderPath = Directory.GetParent(audioManagerScriptFolderPath).FullName;
		var audioManagerFolderPath = Directory.GetParent(audioNameCreatorFolderPath).FullName;
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

				//var bgmInfo = new AudioClipInfo(idx, audio);
				//bgmAudioClipListProp.arraySize++;
				//foreach (AudioManager t in targets)
				//{
				//	t.bgmAudioClipList.Add(bgmInfo);
				//	serializedObject.ApplyModifiedProperties();
				//}
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

				//var seInfo = new AudioClipInfo(idx, audio);
				//seAudioClipListProp.arraySize++;
				//foreach (AudioManager t in targets)
				//	t.seAudioClipList.Add(seInfo);
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
		strBuilder.AppendLine("\t");

		strBuilder.AppendFormat("public enum AudioNameBGM").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();
		foreach (AudioClip bgm in bgmObjList)
			strBuilder.Append("\t").AppendFormat(@"{0},", bgm.name).AppendLine();
		strBuilder.AppendLine("}");
		strBuilder.AppendLine("\t");

		strBuilder.AppendFormat("public enum AudioNameSE").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();
		foreach (AudioClip se in seObjList)
			strBuilder.Append("\t").AppendFormat(@"{0},", se.name).AppendLine();
		strBuilder.AppendLine("}");

		string directoryName = Path.GetDirectoryName(audioNameScriptPath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllText(audioNameScriptPath, strBuilder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);

	}

	/// <summary>
	/// 入力されたassetsの中に、ディレクトリのパスがdirectoryNameの物はあるか
	/// </summary>
	static bool ExistsDirectoryInAssets(List<string[]> assetsList, List<string> targetDirectoryNameList)
	{
		return assetsList
			.Any(assets => assets
			 .Select(asset => System.IO.Path.GetDirectoryName(asset))
			 .Intersect(targetDirectoryNameList)
			 .Count() > 0);
	}

	static string ConvertSystemPathToUnityPath(string _path)
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