using System.Collections.Generic;
using UnityEngine;
using LightGive;


namespace LightGive
{
	[System.Serializable]
	public class AudioClipInfoAttribute : PropertyAttribute
	{
		public List<string> loopList = new List<string>();
		public List<string> playList = new List<string>();

		public AudioClipInfoAttribute()
		{
		}
	}
}
