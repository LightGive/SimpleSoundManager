using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ConfigAsset : ScriptableObject
{
	public int SEVolume;

	static public ConfigAsset CreateAsset()
	{
		var configAsset = CreateInstance<ConfigAsset>();
		AssetDatabase.CreateAsset(configAsset, SimpleSoundManagerDefine.PathConfigData);
		AssetDatabase.Refresh();
		return configAsset;
	}
}