using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BackGroundMusicPlayer : MonoBehaviour
{
	public AudioSource audioSource;
	private IEnumerator fadeInMethod;
	private IEnumerator fadeOutMethod;
	private float volume;
	private float fadeVolume;
	private float loopStartTime;
	private float loopEndTime;
	private bool isFade;
	private bool isPlaying;
	private AudioClip introClip;

	public bool IsPlaying { get { return isPlaying; } }

	public BackGroundMusicPlayer()
	{
		loopStartTime = 0.0f;
		loopEndTime = 0.0f;

		isPlaying = false;
		isFade = false;
	}

	public void Init()
	{
		fadeVolume = 1.0f;
		audioSource = this.gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = true;
		audioSource.spatialBlend = 0.0f;
		audioSource.volume = SimpleSoundManager.Instance.volumeBgm;
	}

	public void Play(AudioClip _clip, AudioClip _introClip, float _volume, bool _isLoop, float _fadeInTime, float _fadeOutTime, float _crossFadeRate, UnityAction _onStartBefore, UnityAction _onStart, UnityAction _onComplete, UnityAction _onCompleteAfter)
	{
		isPlaying = true;
		this.gameObject.SetActive(true);
		isFade = (_fadeInTime >= 0.0f || _fadeOutTime >= 0.0f);
		fadeVolume = (isFade) ? 0.0f : 1.0f;
		volume = _volume;

		if (audioSource.isPlaying)
			audioSource.Stop();

		audioSource.loop = _isLoop;
		audioSource.time = 0.0f;
		audioSource.clip = _clip;
		ChangeVolume();
		audioSource.Play();
	}


	public void PlayerUpdate()
	{

	}

	public void FadeIn(float _fadeTime, float _waitTime)
	{
		this.gameObject.SetActive(true);

		fadeInMethod = _FadeIn(_fadeTime, _waitTime);
		StartCoroutine(fadeInMethod);
	}

	public void FadeOut(float _fadeTime)
	{
		if (!IsPlaying)
			return;
		if (fadeInMethod != null)
			StopCoroutine(fadeInMethod);

		isPlaying = false;
		fadeOutMethod = _FadeOut(_fadeTime);
		StartCoroutine(fadeOutMethod);
	}

	public void Stop()
	{
		isPlaying = false;
		audioSource.Stop();
		this.gameObject.SetActive(false);
	}

	public void Pause()
	{
		isPlaying = false;
		audioSource.Pause();
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
			fadeVolume = Mathf.Clamp01(timeCnt / _fadeTime);
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
			fadeVolume = 1.0f - Mathf.Clamp01(timeCnt / _fadeTime);
			ChangeVolume();
			yield return new WaitForEndOfFrame();
		}

		Stop();
	}

	public void ChangeVolume()
	{
		var v =
			volume *
			fadeVolume *
			SimpleSoundManager.Instance.volumeBgm *
			SimpleSoundManager.Instance.volumeTotal;

		audioSource.volume = v;
	}
}