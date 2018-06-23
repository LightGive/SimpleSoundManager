using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPostprocessorExSound : AssetPostprocessorEx
{
	private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		List<string[]> assetsList = new List<string[]>()
		{
			importedAssets, deletedAssets,
		};

		List<string> targetDirectoryNameList = new List<string>()
		{
			SimpleSoundManagerDefine.PathBgmSourceFolder,
			SimpleSoundManagerDefine.PathSeSourceFolder
		};

		if (ExistsDirectoryInAssets(assetsList, targetDirectoryNameList))
		{
			SimpleSoundManagerSetting.CreateSoundName();

			//該当するファイルがあった場合の処理
			Debug.Log("変更がありました");
		}
	}
}
