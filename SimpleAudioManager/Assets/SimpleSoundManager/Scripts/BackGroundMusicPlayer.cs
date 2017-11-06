using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightGive
{
	[System.Serializable]
	public class BackGroundMusicPlayer : MonoBehaviour
	{
		[SerializeField]
		private AudioSource audioSource;

		private bool isPlaying = false;

		public BackGroundMusicPlayer()
		{
			isPlaying = false;

			audioSource = this.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;
			audioSource.spatialBlend = 0.0f;
			audioSource.outputAudioMixerGroup = SimpleSoundManager.Instance.bgmAudioMixerGroup;
			audioSource.volume = SimpleSoundManager.Instance.BGMVolume;
		}

		public void Play(AudioClip _clip, bool _isLoop, float _volume, float _fadeInTime, float _fadeOutTime, float _loopStartTime, float _loopEndTime)
		{
			isPlaying = true;

			if (_fadeOutTime == 0.0f && _fadeInTime == 0.0f)
			{
				audioSource.Stop();
				audioSource.volume = _volume;
				audioSource.clip = _clip;
				audioSource.Play();
			}
			else
			{

			}

		}
	}
}