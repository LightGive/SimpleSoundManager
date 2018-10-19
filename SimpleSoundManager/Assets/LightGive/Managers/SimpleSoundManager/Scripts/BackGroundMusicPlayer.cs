using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BackGroundMusicPlayer : MonoBehaviour
{
	public enum SoundPlayState
	{
		Stop,
		Playing,
		Pause,
		DelayWait
	}

	private SoundPlayState state;
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
	public bool isPlaying { get { return state == SoundPlayState.Playing; } }
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
	public bool isActive { get { return (state != SoundPlayState.Stop); } }
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
			if (state == SoundPlayState.Stop || state == SoundPlayState.DelayWait)
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

	public BackGroundMusicPlayer()
	{
		state = SoundPlayState.Stop;
		m_isFadeIn = false;
		m_isFadeOut = false;
	}

	public void Init()
	{
		state = SoundPlayState.Stop;
		m_fadeVolume = 1.0f;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.playOnAwake = false;
		m_source.loop = true;
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
		state = SoundPlayState.DelayWait;
		m_waitTimeCnt = 0.0f;
		yield return new WaitForSeconds(delay);

		//イントロの曲があるかのチェック
		if (introClip != null)
		{
			m_source.loop = false;
			m_source.time = 0.0f;
			m_source.clip = introClip;
			ChangeVolume();
			m_source.Play();
			state = SoundPlayState.Playing;
			yield return new WaitForSeconds(introClip.length);

		}

		m_source.clip = m_mainClip;
		m_source.loop = m_isLoop;
		m_source.time = 0.0f;

		ChangeVolume();
		m_source.Play();

		Debug.Log("再生");


	}

	public void FadeIn(float _fadeTime, float _waitTime)
	{
		if (isFade)
			StopCoroutine(m_fadeMethod);

		this.gameObject.SetActive(true);
		m_fadeMethod = _FadeIn(_fadeTime, _waitTime);
		StartCoroutine(m_fadeMethod);
	}

	public void FadeOut(float _fadeTime, float _waitTime)
	{
		if (isFade)
			StopCoroutine(m_fadeMethod);

		m_fadeMethod = _FadeOut(_fadeTime, _waitTime);
		StartCoroutine(m_fadeMethod);
	}

	public void Stop()
	{
		state = SoundPlayState.Stop;
		m_source.Stop();

		if (isFade)
		{
			StopCoroutine(m_fadeMethod);
			m_fadeMethod = null;
		}
	}

	public void Pause()
	{
		if (!(state == SoundPlayState.Playing || state == SoundPlayState.DelayWait))
			return;

		state = SoundPlayState.Pause;
		m_source.Pause();

		if (m_fadeMethod != null)
			StopCoroutine(m_fadeMethod);
	}

	public void Resume()
	{
		if (m_fadeMethod != null)
			StartCoroutine(m_fadeMethod);
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