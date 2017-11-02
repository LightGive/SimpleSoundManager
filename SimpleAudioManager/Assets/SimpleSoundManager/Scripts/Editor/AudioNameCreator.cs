using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LightGive
{
	/// <summary>
	/// オーディオのファイル名を定数で管理するクラスを自動で作成するスクリプト
	/// </summary>
	public class AudioNameCreator : AssetPostprocessor
	{
		private const string AUDIO_SCRIPT_NAME = "AudioName.cs";
		private const string BGM_CLIPLIST_DATA_NAME = "ClipInfoListBGM.asset";
		private const string SE_CLIPLIST_DATA_NAME = "ClipInfoListSE.asset";
		private const string BGM_FOLDER_PATH = "\\Source\\BGM";
		private const string SE_FOLDER_PATH = "\\Source\\SE";
		private const string BGM_CLIPLIST_FOLDER_PATH = "\\Data\\BGM";
		private const string SE_CLIPLIST_FOLDER_PATH = "\\Data\\SE";
		private const string COMMAND_NAME = "Tools/AudioManager/Create AudioName";

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
		private static string SourceFolderPathBGM { get { return ConvertSystemPathToUnityPath(ManagerRootFolderPath + BGM_FOLDER_PATH); } }
		/// <summary>
		/// SourceFolderPath(SE)
		/// </summary>
		private static string SourceFolderPathSE { get { return ConvertSystemPathToUnityPath(ManagerRootFolderPath + SE_FOLDER_PATH); } }
		/// <summary>
		/// ClipListFolderPath(BGM)
		/// </summary>
		private static string CliplistFolderPathBGM { get { return ConvertSystemPathToUnityPath(ManagerRootFolderPath + BGM_CLIPLIST_FOLDER_PATH); } }
		/// <summary>
		/// ClipListFolderPath(SE)
		/// </summary>
		private static string CliplistFolderPathSE { get { return ConvertSystemPathToUnityPath(ManagerRootFolderPath + SE_CLIPLIST_FOLDER_PATH); } }
		/// <summary>
		/// ClipListDataPath(BGM)
		/// </summary>
		public static string CliplistDataPathBGM { get { return CliplistFolderPathBGM + "/" + BGM_CLIPLIST_DATA_NAME; } }
		/// <summary>
		/// ClipListDataPath(SE)
		/// </summary>
		public static string CliplistDataPathSE { get { return CliplistFolderPathSE + "/" + SE_CLIPLIST_DATA_NAME; } }

		public static AudioClipList BgmClipList { get { return (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathBGM, typeof(AudioClipList)); } }
		public static AudioClipList SeClipList { get { return (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathSE, typeof(AudioClipList)); } }

		public static string IconPlayTexture { get { return ManagerRootFolderPath + "/EditorIcons/IconPlay.png"; } }

		/// <summary>
		/// if add sound file in project, Create file "AudioName.cs". 
		/// </summary>
		/// <param name="importedAssets"></param>
		/// <param name="deletedAssets"></param>
		/// <param name="movedAssets"></param>
		/// <param name="movedFromAssetPaths"></param>
		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			//Create();
		}

		/// <summary>
		/// Create audio name file
		/// </summary>
		[MenuItem(COMMAND_NAME)]
		private static void Create()
		{
			//Create SourceFolder, ClipListFolder
			CreateFolder(SourceFolderPathBGM, "BGM source folder");
			CreateFolder(SourceFolderPathSE, "SE source folder");
			CreateFolder(CliplistFolderPathBGM, "BGM list data folder");
			CreateFolder(CliplistFolderPathSE, "SE list data folder");

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
					var seInfo = new AudioClipInfo(idx, audio);
					seObjList.Add(obj);
					seClipList.Add(seInfo);

					//seAudioClipListProp.arraySize++;
					//foreach (AudioManager t in targets)
					//	t.seAudioClipList.Add(seInfo);
				}
			}

			//CreateClipListAsset
			var bgmClipListInstance = Editor.CreateInstance<AudioClipList>();
			var seClipListInstance = Editor.CreateInstance<AudioClipList>();
			bgmClipListInstance = new AudioClipList(bgmClipList);
			seClipListInstance = new AudioClipList(seClipList);


			var bgmClipListOld = (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathBGM, typeof(AudioClipList));
			var seClipListOld = (AudioClipList)AssetDatabase.LoadAssetAtPath(CliplistDataPathSE, typeof(AudioClipList));

			//*********************************************要修正*********************************************
			//現状作ったものともともと作ってあったものとの比較、同じなら処理しない
			if (bgmClipListOld == bgmClipListInstance && seClipListOld == seClipListInstance)
				return;
			//******************************************************************************************

			AssetDatabase.CreateAsset(bgmClipListInstance, CliplistDataPathBGM);
			AssetDatabase.CreateAsset(seClipListInstance, CliplistDataPathSE);
			AssetDatabase.Refresh();

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

		static void CreateFolder(string _createFolderPath, string _folderName = "")
		{
			try
			{
				if (!Directory.Exists(_createFolderPath))
				{
					// If there is no BGM folder, create it.
					Directory.CreateDirectory(_createFolderPath);
					Debug.Log("I did not have the " + _folderName + ", so I created it. \nPath:" + _createFolderPath);
				}
			}

			//When an error occurs, a log is output
			catch (IOException ex)
			{
				Debug.Log(ex.Message);
			}
		}
	}
}