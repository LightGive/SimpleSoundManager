using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightGive
{
	[System.Serializable]
	public class BackGroundMusicPlayer : MonoBehaviour
	{
		private const int AUDIO_SOURCE_NUM = 2;

		[SerializeField]
		private AudioSource[] audioSource = new AudioSource[2];

		private int playerIndex;
		
		public BackGroundMusicPlayer()
		{
			playerIndex = 0;
			for (int i = 0; i < AUDIO_SOURCE_NUM; i++)
			{
				audioSource[i] = this.gameObject.AddComponent<AudioSource>();
				audioSource[i].playOnAwake = false;
				audioSource[i].loop = true;
				audioSource[i].outputAudioMixerGroup = SimpleSoundManager.Instance.bgmAudioMixerGroup;
				audioSource[i].volume = SimpleSoundManager.Instance.BGMVolume;
			}
		}

		public void Play(float _fadeOutTime, float _fadeInTime)
		{
			audioSource[playerIndex].Play();
		}
	}
}