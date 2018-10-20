using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

[System.Serializable]
public class SoundEffectPlayer : MonoBehaviour
{
	public enum SoundPlayState
	{
		Stop,
		Playing,
		Pause,
		DelayWait
	}

	public SoundPlayState state;
	private AudioSource m_source;
	private GameObject m_chaseObj;

	private UnityAction m_onStartBefore;
	private UnityAction m_onStart;
	private UnityAction m_onComplete;
	private UnityAction m_onCompleteAfter;

	[SerializeField]
	private AnimationCurve m_animationCurve;
	private float m_volume;
	private float m_delay;
	private float m_pitch;
	private int m_loopCount;
	private bool m_isFade;
	private bool m_isLoopInfinity;
	private IEnumerator m_coroutineMethod;

	private float m_waitTimeCnt = 0.0f;

	public AudioSource source { get { return m_source; } }
	public GameObject chaseObj { get { return m_chaseObj; } set { m_chaseObj = value; } }
	public UnityAction onStartBefore { get { return m_onStartBefore; } set { m_onStartBefore = value; } }
	public UnityAction onStart { get { return m_onStart; } set { m_onStart = value; } }
	public UnityAction onComplete { get { return m_onComplete; } set { m_onComplete = value; } }
	public UnityAction onCompleteAfter { get { return m_onCompleteAfter; } set { m_onCompleteAfter = value; } }

	public AnimationCurve animationCurve { get { return m_animationCurve; } set { m_animationCurve = value; } }
	public float delay { get { return m_delay; } set { m_delay = value; } }
	public float pitch { get { return m_pitch; } set { m_pitch = value; } }
	public int loopCount { get { return m_loopCount; } set { m_loopCount = value; } }
	public bool isFade { get { return m_isFade; } set { m_isFade = value; } }
	public bool isLoopInfinity { get { return m_isLoopInfinity; } set { m_isLoopInfinity = value; } }
	public float volume
	{
		get
		{
			var v = m_volume * SimpleSoundManager.Instance.volumeSe * SimpleSoundManager.Instance.volumeTotal;
			if (isFade) { v *= animationCurve.Evaluate(source.time); }
			return v;
		}
		set
		{
			m_volume = Mathf.Clamp01(value);
		}
	}


	/// <summary>
	/// 0-1の間でどのくらい再生されているか
	/// </summary>
	/// <value>The length.</value>
	public float Length
	{
		get
		{
			if (state == SoundPlayState.Stop || state == SoundPlayState.DelayWait)
				return 0.0f;

			return Mathf.Clamp01((source.time / source.pitch) / (source.clip.length / source.pitch));
		}
	}

	/// <summary>
	/// 使用されているかどうか
	/// </summary>
	/// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
	public bool isActive { get { return (state != SoundPlayState.Stop); } }

	/// <summary>
	/// 再生されているかどうか
	/// </summary>
	/// <value><c>true</c> if is playing; otherwise, <c>false</c>.</value>
	public bool isPlaying { get { return (state == SoundPlayState.Playing || state == SoundPlayState.DelayWait); } }

	/// <summary>
	/// 初期化
	/// </summary>
	public void Init()
	{
		state = SoundPlayState.Stop;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.outputAudioMixerGroup = SimpleSoundManager.Instance.seAudioMixerGroup;
		source.loop = false;
		source.playOnAwake = false;
		m_waitTimeCnt = 0.0f;
		ResetPlayer();
	}

	public void PlayerUpdate()
	{
		if (chaseObj != null)
		{
			transform.position = chaseObj.transform.position;
		}

		if (isFade)
		{
			source.volume = volume;
		}
	}

	/// <summary>
	/// 再生する
	/// </summary>
	public void Play()
	{
		gameObject.SetActive(true);
		state = SoundPlayState.DelayWait;
		source.volume = volume;
		source.pitch = pitch;

		if (onStartBefore != null)
		{
			onStartBefore.Invoke();
		}

		m_coroutineMethod = _Play();
		StartCoroutine(m_coroutineMethod);
	}

	private IEnumerator _Play()
	{
		if (!isLoopInfinity)
			loopCount--;

		//delay wait.
		m_waitTimeCnt = 0.0f;
		while (m_waitTimeCnt < delay)
		{
			m_waitTimeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		//start callback
		if (onStart != null)
		{
			onStart.Invoke();
		}

		state = SoundPlayState.Playing;
		source.Play();


		m_waitTimeCnt = 0.0f;
		while (m_waitTimeCnt < source.clip.length / source.pitch)
		{
			m_waitTimeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}


		if (onComplete != null)
		{
			onComplete.Invoke();
		}


		//終わるかのチェック
		if (loopCount <= 0)
		{
			state = SoundPlayState.Stop;
			PlayEnd();
			m_coroutineMethod = null;
			yield break;
		}
		else
		{
			m_coroutineMethod = _Play();
			StartCoroutine(m_coroutineMethod);
			yield break;
		}
	}

	/// <summary>
	/// 再生がループも含めて完全に終わったとき
	/// </summary>
	private void PlayEnd()
	{
		if (onCompleteAfter != null)
		{
			onCompleteAfter.Invoke();
		}
		ResetPlayer();
	}

	public void Stop()
	{
		if (state == SoundPlayState.DelayWait || state == SoundPlayState.Playing || state == SoundPlayState.Pause)
		{
			source.Stop();
			StopCoroutine(m_coroutineMethod);
			state = SoundPlayState.Stop;
			gameObject.SetActive(false);
		}
	}

	public void Pause()
	{
		if (state == SoundPlayState.Playing || state == SoundPlayState.DelayWait)
		{
			StopCoroutine(m_coroutineMethod);
			state = SoundPlayState.Pause;
			source.Pause();
		}
	}

	public void Resume()
	{
		if (state == SoundPlayState.Pause)
		{
			StartCoroutine(m_coroutineMethod);
			state = SoundPlayState.Playing;
			source.UnPause();
		}
	}

	public void ResetPlayer()
	{
		gameObject.SetActive(false);
		state = SoundPlayState.Stop;

	}
}


