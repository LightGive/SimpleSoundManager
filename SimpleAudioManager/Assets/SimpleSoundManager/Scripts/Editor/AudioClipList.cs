using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AudioClipList : ScriptableObject
{
	[SerializeField]
	private List<AudioClipInfo> data = new List<AudioClipInfo>();

	public AudioClipList(List<AudioClipInfo> _clipList,string _path)
	{
		var clipListAsset = CreateInstance<AudioClipList>();
		clipListAsset.data = _clipList;
		AssetDatabase.CreateAsset(clipListAsset, _path);
		AssetDatabase.Refresh();
	}

	public AudioClipInfo GetAudioClipInfo(string clipName)
	{
		AudioClipInfo clipInfo;
		if (!Contains(clipName, out clipInfo))
		{
			Debug.Log("hoge hoge");
		}
		return clipInfo;
	}

	private bool Contains(string clipName, out AudioClipInfo info)
	{
		foreach (var clipInfo in data)
		{
			if (clipInfo.Clip.name != clipName)
				continue;
			info = clipInfo;
			return true;
		}
		info = null;
		return false;
	}
}
