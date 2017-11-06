using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LightGive
{
	[System.Serializable]
	public class SoundEffectPlayer : MonoBehaviour
	{
		[SerializeField]
		public AudioSource audioSource;
		[SerializeField]
		public GameObject chaseObj;
		[SerializeField]
		public AnimationCurve animationCurve;
		[SerializeField]
		public float delay;
		[SerializeField]
		public float volume;
		[SerializeField]
		public int loopCnt;
		[SerializeField]
		public bool isFade;

		[SerializeField]
		public UnityAction callbackOnComplete;
		[SerializeField]
		public UnityAction callbackOnStart;

		private bool isActive;
		private bool isPause;
		private bool isPlaying;
		private IEnumerator coroutineMethod;

		public bool IsPause
		{
			get { return isPause; }
		}
		public bool IsPlaying
		{
			get { return isPlaying; }
		}
		public bool IsActive
		{
			get { return isActive; }
		}

		private float Volume
		{
			get
			{
				return
					volume *
					SimpleSoundManager.Instance.SEVolume *
					SimpleSoundManager.Instance.TotalVolume;
			}
		}

		public SoundEffectPlayer()
		{
			isPlaying = false;
			isPause = false;
			isActive = false;

			audioSource = null;
			chaseObj = null;
			delay = 0.0f;
			loopCnt = 0;
		}

		void Awake()
		{
			audioSource = this.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = false;
			audioSource.outputAudioMixerGroup = SimpleSoundManager.Instance.seAudioMixerGroup;
		}

		public void Play()
		{
			isPlaying = true;
			isActive = true;
			this.gameObject.SetActive(true);

			audioSource.volume = SimpleSoundManager.Instance.TotalVolume * SimpleSoundManager.Instance.SEVolume * volume;
			audioSource.Play();
			if (callbackOnStart != null)
				callbackOnStart.Invoke();
			
			coroutineMethod = AudioPlayCheck();
			StartCoroutine(coroutineMethod);
		}

		public void Stop()
		{
			audioSource.Stop();
			CancelInvoke();
			loopCnt = 0;

			isPlaying = false;
			isActive = false;
			this.gameObject.SetActive(false);
		}

		public void Pause()
		{
			isPause = true;
			isPlaying = false;
			audioSource.Pause();
			StopCoroutine(coroutineMethod);

		}

		public void Resume()
		{
			isPause = false;
			isPlaying = true;
			audioSource.Play();
			StartCoroutine(coroutineMethod);
		}

		private IEnumerator AudioPlayCheck()
		{
			float timeCnt = 0.0f;
			float waitTime = (audioSource.clip.length / audioSource.pitch) + delay;
            while (timeCnt < waitTime)
			{
				timeCnt += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			loopCnt--;

			if (loopCnt > 0)
			{
				audioSource.Play();
				coroutineMethod = AudioPlayCheck();
				StartCoroutine(coroutineMethod);
				yield break;
			}

			if (callbackOnComplete != null)
			{
				callbackOnComplete.Invoke();
			}

			isActive = false;
			isPlaying = false;
			this.gameObject.SetActive(false);

		}

		public void PlayerUpdate()
		{
			if (chaseObj != null)
			{
				transform.position = chaseObj.transform.position;
			}

			if (isFade)
			{
				ChangeVolume();
			}
		}

		public SoundEffectPlayer(AudioClip _audioClip)
		{
			audioSource = new AudioSource();
			audioSource.clip = _audioClip;
		}


		public void ChangeVolume()
		{
			var fadeVolume = (isFade) ? animationCurve.Evaluate(audioSource.time) : 1.0f;
            audioSource.volume =
				volume *
				fadeVolume *
				SimpleSoundManager.Instance.SEVolume *
				SimpleSoundManager.Instance.TotalVolume;
		}
	}
}