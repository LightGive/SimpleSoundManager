using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class SimpleSoundManagerSetting
{
	private static ConfigAsset m_configAsset;

	public static ConfigAsset configAsset
	{
		get
		{
			if (m_configAsset != null)
				return m_configAsset;

			m_configAsset = (ConfigAsset)AssetDatabase.LoadAssetAtPath(SimpleSoundManagerDefine.PathConfigData, typeof(ConfigAsset));
			return (m_configAsset != null) ? m_configAsset : CreateConfigData();
		}
	}

	public static ConfigAsset CreateConfigData()
	{
		return ConfigAsset.CreateAsset();
	}
}