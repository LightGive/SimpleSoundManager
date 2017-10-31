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
		public bool isActive;
		[SerializeField]
		public bool isFade;

		[SerializeField]
		public UnityAction callbackOnComplete;
		[SerializeField]
		public UnityAction callbackOnStart;

		private bool isPause;
		private IEnumerator coroutineMethod;

		/// <summary>
		/// ポーズしているか
		/// </summary>
		public bool IsPause
		{
			get { return isPause; }
		}


		public SoundEffectPlayer()
		{
			isPause = false;
			isActive = false;
			audioSource = null;
			chaseObj = null;
			delay = 0.0f;
			loopCnt = 0;
		}

		public void Play()
		{
			isActive = true;
			this.gameObject.SetActive(true);
			audioSource.volume = SimpleSoundManager.Instance.TotalVolume * volume;
			audioSource.PlayDelayed(delay);
			if (callbackOnStart != null)
				callbackOnStart.Invoke();

			var waitTime = (audioSource.clip.length / audioSource.pitch) + delay;
			coroutineMethod = AudioPlayCheck(waitTime);
			StartCoroutine(coroutineMethod);
		}

		public void Stop()
		{
			audioSource.Stop();
			this.gameObject.SetActive(false);
			CancelInvoke();
			isActive = false;
			loopCnt = 0;
		}


		public void Pause()
		{
			audioSource.Pause();
			StopCoroutine(coroutineMethod);
			isPause = true;
		}

		public void Resume()
		{
			audioSource.Play();
			isPause = false;
			Debug.Log(this.gameObject.name);
			StartCoroutine(coroutineMethod);
		}

		private IEnumerator AudioPlayCheck(float _waitTime)
		{
			yield return new WaitForSeconds(_waitTime);

			loopCnt--;

			if (loopCnt > 0)
			{
				audioSource.Play();
				coroutineMethod.Reset();
				coroutineMethod = AudioPlayCheck(_waitTime);
				yield break ;
			}

			if (callbackOnComplete != null)
			{
				callbackOnComplete.Invoke();
			}
			this.gameObject.SetActive(false);
		}


		public void PlayerUpdate()
		{
			if (chaseObj != null)
				transform.position = chaseObj.transform.position;

			if (isFade)
			{
				audioSource.volume =
					SimpleSoundManager.Instance.TotalVolume *
					animationCurve.Evaluate(audioSource.time) *
					volume;
			}
		}

		public SoundEffectPlayer(AudioClip _audioClip)
		{
			audioSource = new AudioSource();
			audioSource.clip = _audioClip;
		}

		public void ChangeTotalVolume(float _val)
		{
			audioSource.volume = SimpleSoundManager.Instance.TotalVolume * volume;
		}
	}
}