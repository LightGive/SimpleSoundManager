using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipList : ScriptableObject
{
	[SerializeField]
	private List<AudioClipInfo> data = new List<AudioClipInfo>();

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
