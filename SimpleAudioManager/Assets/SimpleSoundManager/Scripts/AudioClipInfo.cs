using UnityEngine;
namespace LightGive
{
	[System.Serializable]
	public class AudioClipInfo
	{
		[SerializeField]
		public bool isUse = true;
		[SerializeField]
		public int audioNo;
		[SerializeField]
		public AudioClip clip;
		[SerializeField]
		private float volume;
		[SerializeField]
		private float pitch;

		public AudioClip AudioCilp { get { return clip; } }
		public float Volume { get { return volume; } }
		public float Picth { get { return pitch; } }


		public string audioName
		{
			get
			{
				return clip.name;
			}
		}

		public AudioClipInfo(int _audioNo, AudioClip _clip)
		{
			audioNo = _audioNo;
			clip = _clip;
		}
	}
}