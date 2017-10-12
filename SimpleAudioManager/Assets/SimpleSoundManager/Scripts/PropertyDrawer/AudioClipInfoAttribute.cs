using System.Collections.Generic;
using UnityEngine;
using LightGive;


namespace LightGive
{
	[System.Serializable]
	public class AudioClipInfoAttribute : PropertyAttribute
	{
		[SerializeField]
		public bool isRoute;

		public List<string> clipList = new List<string>();
		public List<string> loopList = new List<string>();

		public class AudioStateCheck
		{
			public bool isLoop = false;
			public string clipName = "";
			public AudioStateCheck(string _clipName)
			{
				clipName = _clipName;
			}
		}

		public Dictionary<string, AudioStateCheck> audioStateCheckDic = new Dictionary<string, AudioStateCheck>();

		
		public void CheckContainsDic(string _key,string _clipName)
		{

			if (audioStateCheckDic.ContainsKey(_key))
				return;
			else
				audioStateCheckDic.Add(_key, new AudioStateCheck(_clipName));

		}

		public AudioClipInfoAttribute()
		{
		}
	}
}
