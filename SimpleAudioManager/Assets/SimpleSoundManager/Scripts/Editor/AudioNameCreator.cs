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
	private const string BGM_CLIPLIST_PATH = "\\Data\\BGM\\BGM_ClipInfoList.asset";
	private const string SE_CLIPLIST_PATH = "\\Data\\SE\\BGM_ClipInfoList.asset";
    private const string COMMAND_NAME = "Tools/AudioManager/Create AudioName"; // コマンド名


	/// <summary>
	/// Get root folder path
	/// </summary>
	private static string ManagerRootFolderPath
	{
		get
		{
			//Find "AudioNameCreator.cs" Path
			string[] res = System.IO.Directory.GetFiles(Application.dataPath, "AudioNameCreator.cs", SearchOption.AllDirectories);
			return Directory.GetParent(Directory.GetParent(Directory.GetParent(res[0]).FullName).FullName).FullName;
		}
	}

	/// <summary>
	/// SourceFolderPath(BGM)
	/// </summary>
	private static string SourceFolderPathBGM
	{
		get
		{
			return ConvertSystemPathToUnityPath(ManagerRootFolderPath + BGM_FOLDER_PATH);
		}
	}
	/// <summary>
	/// SourceFolderPath(SE)
	/// </summary>
	private static string SourceFolderPathSE
	{
		get
		{
			return ConvertSystemPathToUnityPath(ManagerRootFolderPath + SE_FOLDER_PATH);
		}
	}

	private static string ListDataPathBGM
	{
		get
		{
			return ConvertSystemPathToUnityPath(ManagerRootFolderPath + BGM_CLIPLIST_PATH);
		}
	}

	private static string ListDataPathSE
	{
		get
		{
			return ConvertSystemPathToUnityPath(ManagerRootFolderPath + SE_CLIPLIST_PATH);
		}
	}

	/// <summary>
	/// if add sound file in project, Create file "AudioName.cs". 
	/// </summary>
	/// <param name="importedAssets"></param>
	/// <param name="deletedAssets"></param>
	/// <param name="movedAssets"></param>
	/// <param name="movedFromAssetPaths"></param>
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		Create();
	}

	/// <summary>
	/// Create audio name file
	/// </summary>
	[MenuItem(COMMAND_NAME)]
	private static void Create()
	{
		try
		{
			if (!Directory.Exists(SourceFolderPathBGM))
			{
				// If there is no BGM folder, create it.
				Directory.CreateDirectory(SourceFolderPathBGM);
				Debug.Log("I did not have the BGM folder, so I created it. \nPath:" + SourceFolderPathBGM);
			}

			if (!Directory.Exists(SourceFolderPathSE))
			{
				//If there is no SE folder, create it.
				Directory.CreateDirectory(SourceFolderPathSE);
				Debug.Log("I did not have the SE folder, so I created it. \nPath:" + SourceFolderPathSE);
			}
		}

		//When an error occurs, a log is output
		catch (IOException ex)
		{
			Debug.Log(ex.Message);
		}

		string[] fileEntriesBgm = Directory.GetFiles(SourceFolderPathBGM, "*", SearchOption.AllDirectories);
		string[] fileEntriesSe = Directory.GetFiles(SourceFolderPathSE, "*", SearchOption.AllDirectories);

		List<Object> bgmObjList = new List<Object>();
		List<Object> seObjList = new List<Object>();

		List<AudioClipInfo> bgmClipList = new List<AudioClipInfo>();
		List<AudioClipInfo> seClipList = new List<AudioClipInfo>();

		int idx = 0;

		//Search files in BGM folder one by one
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
				bgmClipList.Add(bgmInfo);
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

				var seInfo = new AudioClipInfo(idx, audio);
				
				
				//seAudioClipListProp.arraySize++;
				//foreach (AudioManager t in targets)
				//	t.seAudioClipList.Add(seInfo);
			}
		}

		new AudioClipList(bgmClipList, ListDataPathBGM);
		

		//スクリプトのパスを取得する
		var audioNameScriptPath = ManagerRootFolderPath + "/Scripts/" + AUDIO_SCRIPT_NAME;

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