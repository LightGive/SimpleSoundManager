using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConfigAsset : ScriptableObject
{
	static public void CreateAsset()
	{
		var configAsset = CreateInstance<ConfigAsset>();
		AssetDatabase.CreateAsset(configAsset, SimpleSoundManagerDefine.PathConfigData);
		AssetDatabase.Refresh();
	}
}