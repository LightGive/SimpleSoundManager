using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;

public static class SimpleSoundManagerSetting
{
	private static List<AudioClip> m_audioClipListSe = new List<AudioClip>();
	private static List<AudioClip> m_audioClipListBgm = new List<AudioClip>();

	/// <summary>
	/// ファイルからAudioClipを取得する（BGM）
	/// </summary>
	/// <returns>The audio clip list bgm.</returns>
	public static List<AudioClip> GetAudioClipListBgm()
	{
		List<AudioClip> audioClipList = new List<AudioClip>();
		string[] fileEntriesBgm = Directory.GetFiles(SimpleSoundManagerDefine.PathBgmSourceFolder, "*", SearchOption.AllDirectories);

		int idx = 0;
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
				audioClipList.Add(audio);
			}
		}
		return audioClipList;
	}


	/// <summary>
	/// ファイルからAudioClipを取得する（SE）
	/// </summary>
	/// <returns>The audio clip list se.</returns>
	public static List<AudioClip> GetAudioClipListSe()
	{
		List<AudioClip> audioClipList = new List<AudioClip>();
		string[] fileEntriesSe = Directory.GetFiles(SimpleSoundManagerDefine.PathSeSourceFolder, "*", SearchOption.AllDirectories);

		int idx = 0;
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
				audioClipList.Add(audio);
			}
		}
		return audioClipList;
	}

	[MenuItem("Tools/LightGive/SimpleSoundManager/DebugSoundNameCreate")]
	public static void CreateSoundName()
	{
		List<AudioClip> bgmObjList = GetAudioClipListBgm();
		List<AudioClip> seObjList = GetAudioClipListSe();

		int bgmMaxNo = System.Enum.GetValues(typeof(SoundNameBGM)).Cast<int>().Max();
		int seMaxNo = System.Enum.GetValues(typeof(SoundNameSE)).Cast<int>().Max();

		//Create "AudioName.cs"
		string audioFileNameExtension = Path.GetFileNameWithoutExtension(SimpleSoundManagerDefine.PathSoundName);
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

		strBuilder.AppendFormat("public enum SoundNameBGM").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();

		var bgmNames = System.Enum.GetNames(typeof(SoundNameBGM));
		foreach (AudioClip bgm in bgmObjList)
		{
			var idx = 0;
			for (int i = 0; i < bgmNames.Length; i++)
			{
				if (bgmNames[i] == bgm.name)
				{
					idx = (int)System.Enum.Parse(typeof(SoundNameBGM), bgmNames[i]);
					break;
				}
			}

			if (idx == 0)
			{
				bgmMaxNo++;
				idx = bgmMaxNo;
			}
			strBuilder.Append("\t").AppendFormat(@"{0},", bgm.name + " = " + idx.ToString()).AppendLine();
		}
		strBuilder.AppendLine("}");
		strBuilder.AppendLine("\t");
		strBuilder.AppendFormat("public enum SoundNameSE").AppendLine();
		strBuilder.AppendLine("{");
		strBuilder.Append("\t").AppendFormat(@"None,").AppendLine();

		var seNames = System.Enum.GetNames(typeof(SoundNameSE));
		foreach (AudioClip se in seObjList)
		{
			var idx = 0;
			for (int i = 0; i < seNames.Length; i++)
			{
				if (seNames[i] == se.name)
				{
					idx = (int)System.Enum.Parse(typeof(SoundNameSE), seNames[i]);
					break;
				}
			}

			if (idx == 0)
			{
				seMaxNo++;
				idx = seMaxNo;
			}

			strBuilder.Append("\t").AppendFormat(@"{0},", se.name + " = " + idx.ToString()).AppendLine();
		}
		strBuilder.AppendLine("}");

		string directoryName = Path.GetDirectoryName(SimpleSoundManagerDefine.PathSoundName);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllText(SimpleSoundManagerDefine.PathSoundName, strBuilder.ToString(), Encoding.UTF8);
		AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
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