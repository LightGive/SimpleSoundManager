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
	private IEnumerator m_fadeInMethod;
	private IEnumerator m_fadeOutMethod;
	private float m_volume;
	private float m_fadeVolume;
	private bool m_isFadeIn;
	private bool m_isFadeOut;
	private bool m_isPlaying;
	private bool m_isPlayIntro;
	private AudioClip m_introClip;
	private AudioClip m_mainClip;

	public AudioSource source { get { return m_source; } }
	public bool IsPlaying { get { return m_isPlaying; } }
	public float volume
	{
		get
		{
			var v = m_volume * SimpleSoundManager.Instance.volumeSe;
			//if (isFade) { v *= animationCurve.Evaluate(source.time); }
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
			return (m_fadeInMethod != null || m_fadeOutMethod != null);
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
		m_isPlaying = false;
		m_isFadeIn = false;
		m_isFadeOut = false;
	}

	public void Init()
	{
		m_fadeVolume = 1.0f;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.playOnAwake = false;
		m_source.loop = true;
		m_source.spatialBlend = 0.0f;
		m_source.volume = SimpleSoundManager.Instance.volumeBgm;
	}

	public void Play(AudioClip _clip, AudioClip _introClip, float _volume, float _delay, bool _isLoop, UnityAction _onStartBefore, UnityAction _onStart, UnityAction _onComplete, UnityAction _onCompleteAfter)
	{
		m_isPlaying = true;
		this.gameObject.SetActive(true);

		m_volume = _volume;

		if (m_source.isPlaying)
			m_source.Stop();

		m_source.loop = _isLoop;
		m_source.time = 0.0f;
		m_source.clip = _clip;
		ChangeVolume();
		m_source.Play();
	}

	//private IEnumerator _Play(AudioClip _clip, AudioClip _introClip, float _volume, bool _isLoop, UnityAction _onStartBefore, UnityAction _onStart, UnityAction _onComplete, UnityAction _onCompleteAfter)
	//{


	//}


	public void PlayerUpdate()
	{

	}

	public void FadeIn(float _fadeTime, float _waitTime)
	{
		this.gameObject.SetActive(true);

		m_fadeInMethod = _FadeIn(_fadeTime, _waitTime);
		StartCoroutine(m_fadeInMethod);
	}

	public void FadeOut(float _fadeTime)
	{
		if (!IsPlaying)
			return;
		if (m_fadeInMethod != null)
			StopCoroutine(m_fadeInMethod);

		m_isPlaying = false;
		m_fadeOutMethod = _FadeOut(_fadeTime);
		StartCoroutine(m_fadeOutMethod);
	}

	public void Stop()
	{
		m_isPlaying = false;
		m_source.Stop();
		this.gameObject.SetActive(false);

		if (m_isFadeIn)
			StopCoroutine(m_fadeInMethod);
		if (m_isFadeOut)
			StopCoroutine(m_fadeOutMethod);
	}

	public void Pause()
	{
		m_isPlaying = false;
		m_source.Pause();

		if (m_isFadeIn)
			StopCoroutine(m_fadeInMethod);
		if (m_isFadeOut)
			StopCoroutine(m_fadeOutMethod);
	}

	public void Resume()
	{

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
	}

	private IEnumerator _FadeOut(float _fadeTime)
	{
		var timeCnt = 0.0f;
		while (timeCnt < _fadeTime)
		{
			timeCnt += Time.deltaTime;
			m_fadeVolume = 1.0f - Mathf.Clamp01(timeCnt / _fadeTime);
			ChangeVolume();
			yield return new WaitForEndOfFrame();
		}

		Stop();
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