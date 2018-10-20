using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class AssetPostprocessorEx : AssetPostprocessor
{
	protected static bool ExistsDirectoryInAssets(List<string[]> assetsList, List<string> targetDirectoryNameList)
	{

		return assetsList
			.Any(assets => assets                                       //入力されたassetsListに以下の条件を満たすか要素が含まれているか判定
			 .Select(asset => System.IO.Path.GetDirectoryName(asset))   //assetsに含まれているファイルのディレクトリ名だけをリストにして取得
			 .Intersect(targetDirectoryNameList)                         //上記のリストと入力されたディレクトリ名のリストの一致している物のリストを取得
			 .Count() > 0);                                              //一致している物があるか

	}
}