using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LightGive
{
	
	[System.Serializable]
	public class SoundEffectPlayer : MonoBehaviour
	{
		private const float DelayMin = 0.0f;
		private const float DelayMax = 10.0f;
		private const float PitchMin = 0.0f;
		private const float PitchMax = 3.0f;

		[SerializeField]
		public AudioSource audioSource;
		[SerializeField]
		public GameObject chaseObj;
		[SerializeField]
		public AnimationCurve animationCurve;
		
		[SerializeField]
		public UnityAction callbackOnComplete;
		[SerializeField]
		public UnityAction callbackOnStart;

		private float volume;
		private float delay;
		private float pitch;
		private int loopCount;
		private bool isActive;
		private bool isPause;
		private bool isPlaying;
		private bool isFade;
		private bool isLoopInfinity;

		private IEnumerator coroutineMethod;

		public bool IsPause
		{
			get { return isPause; }
			set { isPause = value; }
		}
		public bool IsPlaying
		{
			get { return isPlaying; }
			set { isPlaying = value; }
		}
		public bool IsActive
		{
			get { return isActive; }
			set { isActive = value; }
		}
		public int LoopCount
		{
			get { return loopCount; }
			set { loopCount = value; }
		}
		public bool IsLoopInfinity
		{
			get { return isLoopInfinity; }
            set { isLoopInfinity = value; }
		}
		public bool IsFade
		{
			get { return isFade; }
            set { isFade = value; }
		}
		public float Delay
		{
			get { return delay; }
			set { delay = Mathf.Clamp(value, DelayMin, DelayMax); }
		}
		public float Pitch
		{
			get { return pitch; }
			set { pitch = Mathf.Clamp(value, PitchMin, PitchMax); }
		}
		public float Volume
		{
			get
			{
				var v = volume * SimpleSoundManager.Instance.SEVolume * SimpleSoundManager.Instance.TotalVolume;
				if (IsFade) { v *= animationCurve.Evaluate(audioSource.time); }
				return v;
			}
			set
			{
				volume = Mathf.Clamp01(value);
			}
		}

		public SoundEffectPlayer()
		{
			IsPlaying = false;
			IsPause = false;
			isActive = false;
			audioSource = null;
			chaseObj = null;
			delay = 0.0f;
			loopCount = 0;
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
			audioSource.time = 0.0f;
			audioSource.pitch = Pitch;
			audioSource.volume = Volume;

			if (callbackOnStart != null)
				callbackOnStart.Invoke();
			
			coroutineMethod = AudioPlayCheck();
			StartCoroutine(coroutineMethod);
		}

		public void Stop()
		{
			if(audioSource.isPlaying)
			audioSource.Stop();
			loopCount = 0;
			IsPause = false;
			IsFade = false;
			IsPlaying = false;
			isActive = false;
			this.gameObject.SetActive(false);
		}

		public void Pause()
		{
			IsPause = true;
			IsPlaying = false;
			audioSource.Pause();
			StopCoroutine(coroutineMethod);

		}

		public void Resume()
		{
			IsPause = false;
			IsPlaying = true;
			audioSource.Play();
			StartCoroutine(coroutineMethod);
		}

		private IEnumerator AudioPlayCheck()
		{
			if (LoopCount <= 0 && !IsLoopInfinity)
				AudioPlayEnd();

			float timeCnt = 0.0f;
			while (timeCnt < delay)
			{
				timeCnt += Time.deltaTime;
				yield return 0;
			}

			audioSource.Play();
			loopCount--;
			timeCnt = 0.0f;

			float waitTime = (audioSource.clip.length / audioSource.pitch);
			while (timeCnt < waitTime)
			{
				timeCnt += Time.deltaTime;
				yield return 0;
			}

			coroutineMethod = AudioPlayCheck();
			StartCoroutine(coroutineMethod);
			if (IsLoopInfinity)
				loopCount = 1;
			yield break;
		}

		private void AudioPlayEnd()
		{
			if (callbackOnComplete != null)
			{
				callbackOnComplete.Invoke();
			}

			IsActive = false;
			IsPlaying = false;
			IsPause = false;
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

		public void ChangeVolume()
		{
			audioSource.volume = Volume;
		}
	}
}