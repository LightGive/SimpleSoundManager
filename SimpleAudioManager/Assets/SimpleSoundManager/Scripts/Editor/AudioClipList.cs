using System.Collections.Generic;
using UnityEngine;

namespace LightGive
{
	public class AudioClipList : ScriptableObject
	{
		[SerializeField]
		public List<AudioClipInfo> data = new List<AudioClipInfo>();

		public AudioClipList(List<AudioClipInfo> _clipList)
		{
			data = _clipList;
		}

		public AudioClipInfo GetAudioClipInfo(string clipName)
		{
			AudioClipInfo clipInfo;
			if (!Contains(clipName, out clipInfo))
			{
			}
			return clipInfo;
		}

		private bool Contains(string clipName, out AudioClipInfo info)
		{
			foreach (var clipInfo in data)
			{
				if (clipInfo.AudioCilp.name != clipName)
					continue;
				info = clipInfo;
				return true;
			}
			info = null;
			return false;
		}
	}
}