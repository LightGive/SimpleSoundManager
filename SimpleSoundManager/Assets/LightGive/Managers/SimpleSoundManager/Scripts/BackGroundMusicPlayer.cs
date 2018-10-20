using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

public class BackGroundMusicPlayer : MonoBehaviour
{
	public enum SoundPlayState
	{
		Stop,
		Playing,
		Pause,
		DelayWait
	}

	private SoundPlayState m_beforeState;
	private SoundPlayState m_state;
	private AudioSource m_source;
	private IEnumerator m_fadeMethod;
	private IEnumerator m_playMethod;
	private float m_volume;
	private float m_delay;
	private float m_fadeVolume;
	private bool m_isFadeIn;
	private bool m_isFadeOut;
	private bool m_isLoop;
	private AudioClip m_introClip;
	private AudioClip m_mainClip;
	private UnityAction m_onIntroStart;
	private UnityAction m_onIntroComplete;
	private UnityAction m_onMainStart;
	private UnityAction m_onMainComplete;

	private float m_waitTimeCnt = 0.0f;

	public AudioClip mainClip { get { return m_mainClip; } set { m_mainClip = value; } }
	public AudioClip introClip { get { return m_introClip; } set { m_introClip = value; } }
	public AudioSource source { get { return m_source; } }
	public UnityAction onIntroStart { get { return m_onIntroStart; } set { m_onIntroStart = value; } }
	public UnityAction onIntroComplete { get { return m_onIntroComplete; } set { m_onIntroComplete = value; } }
	public UnityAction onMainStart { get { return m_onMainStart; } set { m_onMainStart = value; } }
	public UnityAction onMainComplete { get { return m_onMainComplete; } set { m_onMainComplete = value; } }
	public bool isLoop { get { return m_isLoop; } set { m_isLoop = value; } }
	public bool isPlaying { get { return m_state == SoundPlayState.Playing; } }
	public float delay { get { return m_delay; } set { m_delay = value; } }
	public float volume
	{
		get
		{
			var v = m_volume * SimpleSoundManager.Instance.volumeSe;
			return v;
		}
		set
		{
			m_volume = Mathf.Clamp01(value);
		}
	}
	public bool isActive { get { return (m_state != SoundPlayState.Stop); } }
	public bool isFade
	{
		get
		{
			return (m_fadeMethod != null);
		}
	}
	public float Length
	{
		get
		{
			if (m_state == SoundPlayState.Stop || m_state == SoundPlayState.DelayWait)
				return 0.0f;

			if (m_introClip != null)
			{
				if (source.clip == m_introClip)
					return Mathf.Clamp01(source.time / (m_mainClip.length + m_introClip.length));
				else
					return Mathf.Clamp01((source.time + m_introClip.length) / (m_mainClip.length + m_introClip.length));
			}
			else
			{
				return Mathf.Clamp01(source.time / source.clip.length);
			}
		}
	}

	public void Init()
	{
		m_state = SoundPlayState.Stop;
		m_fadeVolume = 1.0f;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.outputAudioMixerGroup = SimpleSoundManager.Instance.bgmAudioMixerGroup;
		m_source.playOnAwake = false;
		m_source.loop = false;
		m_source.spatialBlend = 0.0f;
		m_source.volume = SimpleSoundManager.Instance.volumeBgm;
	}

	public void Play()
	{
		m_playMethod = _Play();
		StartCoroutine(m_playMethod);
	}

	private IEnumerator _Play()
	{
		m_state = SoundPlayState.DelayWait;
		m_waitTimeCnt = 0.0f;
		while (m_waitTimeCnt < delay)
		{
			m_waitTimeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		//状態をプレイ中に変更
		m_state = SoundPlayState.Playing;

		//イントロの曲があるかのチェック
		if (introClip != null)
		{
			//イントロ開始
			if (onIntroStart != null) { onIntroStart.Invoke(); }

			m_source.time = 0.0f;
			m_source.clip = introClip;
			ChangeVolume();
			m_source.Play();
			m_state = SoundPlayState.Playing;
			m_waitTimeCnt = 0.0f;
			while (m_waitTimeCnt < introClip.length)
			{
				m_waitTimeCnt += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			//イントロ終了
			if (onIntroComplete != null) { onIntroComplete.Invoke(); }
		}

		//メインBGM
		do
		{
			//メイン開始
			if (onMainStart != null) { onMainStart.Invoke(); }

			m_source.clip = m_mainClip;
			m_source.time = 0.0f;

			ChangeVolume();
			m_source.Play();

			m_waitTimeCnt = 0.0f;
			while (m_waitTimeCnt < m_mainClip.length)
			{
				m_waitTimeCnt += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}

			if (!isLoop)
			{
				m_state = SoundPlayState.Stop;
			}

			//メイン終了
			if (onMainComplete != null)
			{
				onMainComplete.Invoke();
			}
		} while (isLoop);

		m_playMethod = null;
	}

	public void FadeIn(float _fadeTime, float _waitTime)
	{
		if (isFade)
		{
			StopCoroutine(m_fadeMethod);
			m_fadeMethod = null;
		}

		m_fadeMethod = _FadeIn(_fadeTime, _waitTime);
		StartCoroutine(m_fadeMethod);
	}

	public void FadeOut(float _fadeTime, float _waitTime)
	{
		if (isFade)
		{
			StopCoroutine(m_fadeMethod);
			m_fadeMethod = null;
		}

		m_fadeMethod = _FadeOut(_fadeTime, _waitTime);
		StartCoroutine(m_fadeMethod);
	}

	public void Stop()
	{
		if (m_playMethod != null)
		{
			StopCoroutine(m_playMethod);
			m_playMethod = null;
		}

		m_source.Stop();
		m_state = SoundPlayState.Stop;

		if (isFade)
		{
			StopCoroutine(m_fadeMethod);
			m_fadeMethod = null;
		}
	}

	public void Pause()
	{
		if (!(m_state == SoundPlayState.Playing || m_state == SoundPlayState.DelayWait))
			return;

		m_beforeState = m_state;
		m_state = SoundPlayState.Pause;
		StopCoroutine(m_playMethod);
		m_source.Pause();

		if (isFade)
		{
			StopCoroutine(m_fadeMethod);
		}
	}

	public void Resume()
	{
		//ポーズ中ではない時は処理しない
		if (m_state != SoundPlayState.Pause)
			return;

		m_state = m_beforeState;
		StartCoroutine(m_playMethod);
		m_source.Play();

		if (isFade)
		{
			StartCoroutine(m_fadeMethod);
		}
	}

	private IEnumerator _FadeIn(float _fadeTime, float _waitTime)
	{
		var timeCnt = 0.0f;
		while (timeCnt < _waitTime)
		{
			timeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		timeCnt = 0.0f;
		while (timeCnt < _fadeTime)
		{
			timeCnt += Time.deltaTime;
			m_fadeVolume = Mathf.Clamp01(timeCnt / _fadeTime);
			ChangeVolume();
			yield return new WaitForEndOfFrame();
		}

		m_fadeMethod = null;
	}

	private IEnumerator _FadeOut(float _fadeTime, float _waitTime)
	{
		var timeCnt = 0.0f;
		while (timeCnt < _waitTime)
		{
			timeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		timeCnt = 0.0f;
		while (timeCnt < _fadeTime)
		{
			timeCnt += Time.deltaTime;
			m_fadeVolume = 1.0f - Mathf.Clamp01(timeCnt / _fadeTime);
			ChangeVolume();
			yield return new WaitForEndOfFrame();
		}

		Stop();
		m_fadeMethod = null;
	}

	public void ChangeVolume()
	{
		var v =
			m_volume *
			m_fadeVolume *
			SimpleSoundManager.Instance.volumeBgm *
			SimpleSoundManager.Instance.volumeTotal;

		m_source.volume = v;
	}
}