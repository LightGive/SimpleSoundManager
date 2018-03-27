using System.Collections.Generic;
using UnityEngine;
using LightGive;

namespace LightGive
{
	[System.Serializable]
	public class AudioClipInfoAttribute : PropertyAttribute
	{
		public List<string> loopList = new List<string>();
		public string playPropPath;

		public AudioClipInfoAttribute()
		{
		}
	}
}
