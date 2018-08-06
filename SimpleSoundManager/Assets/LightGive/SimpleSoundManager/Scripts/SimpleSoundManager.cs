using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	[SerializeField]
	public List<AudioClip> audioClipListSe = new List<AudioClip>();
	[SerializeField]
	public List<AudioClip> audioClipListBgm = new List<AudioClip>();

	[SerializeField]
	private List<SoundEffectPlayer> m_soundEffectPlayers = new List<SoundEffectPlayer>();
	[SerializeField]
	private BackGroundMusicPlayer m_mainBackgroundPlayer;
	[SerializeField]
	private BackGroundMusicPlayer m_subBackgroundPlayer;
	[SerializeField]
	private int m_sePlayerNum = 10;
	[SerializeField]
	private float m_volumeTotal = 1.0f;
	[SerializeField]
	private float m_volumeSe = 1.0f;
	[SerializeField]
	private float m_volumeBgm = 1.0f;
	[SerializeField]
	private bool m_editorIsFoldSeList = false;
	[SerializeField]
	private bool m_editorIsFoldBgmList = false;

	private Dictionary<string, AudioClip> m_audioClipDictSe = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> m_audioClipDirtBgm = new Dictionary<string, AudioClip>();

	public float volumeTotal { get { return m_volumeTotal; } }
	public float volumeSe { get { return m_volumeSe; } }
	public float volumeBgm { get { return m_volumeBgm; } }

	protected override void Init()
	{
		base.Init();
		for (int i = 0; i < m_sePlayerNum; i++)
		{
			GameObject soundPlayerObj = new GameObject("SoundPlayer" + i.ToString("0"));
			soundPlayerObj.transform.SetParent(transform);
			SoundEffectPlayer player = soundPlayerObj.AddComponent<SoundEffectPlayer>();
			player.Init();
			m_soundEffectPlayers.Add(player);
		}

		GameObject mainBackgroundPlayerObj = new GameObject("MainBackgroundMusicPlayer");
		GameObject subBackgroundPlayerObj = new GameObject("MainBackgroundMusicPlayer");
		mainBackgroundPlayerObj.transform.SetParent(transform);
		subBackgroundPlayerObj.transform.SetParent(transform);
		m_mainBackgroundPlayer = mainBackgroundPlayerObj.AddComponent<BackGroundMusicPlayer>();
		m_subBackgroundPlayer = subBackgroundPlayerObj.AddComponent<BackGroundMusicPlayer>();

		//Dictionaryに追加
		for (int i = 0; i < audioClipListSe.Count; i++)
		{
			m_audioClipDictSe.Add(audioClipListSe[i].name, audioClipListSe[i]);
		}
		for (int i = 0; i < audioClipListBgm.Count; i++)
		{
			m_audioClipDirtBgm.Add(audioClipListBgm[i].name, audioClipListBgm[i]);
		}
	}

	private void Update()
	{
		for (int i = 0; i < m_sePlayerNum; i++)
		{
			if (m_soundEffectPlayers[i].isActive)
				m_soundEffectPlayers[i].PlayerUpdate();
		}
	}

	public SoundEffectPlayer PlaySE2D(SoundNameSE _audioName)
	{
		return PlaySE(_audioName.ToString(), 1.0f, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(SoundNameSE _audioName, float _volume)
	{
		return PlaySE(_audioName.ToString(), _volume, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}

	public SoundEffectPlayer PlaySE2D(string _audioName)
	{
		return PlaySE(_audioName, 1.0f, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume)
	{
		return PlaySE(_audioName, _volume, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume, float _delay)
	{
		return PlaySE(_audioName, _volume, _delay, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume, float _delay, float _pitch)
	{
		return PlaySE(_audioName, _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume, float _delay, float _pitch, int _loopCount)
	{
		return PlaySE(_audioName, _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume, float _delay, float _pitch, int _loopCount, UnityAction _onStartBefore, UnityAction _onStart, UnityAction _onComplete, UnityAction _onCompleteAfter)
	{
		return PlaySE(_audioName, _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	public SoundEffectPlayer PlaySE2D_FadeInOut(string _audioName, float _fadeInTime, float _fadeOutTime, float _volume, float _delay, float _pitch, int _loopCount, UnityAction _onStartBefore, UnityAction _onStart, UnityAction _onComplete, UnityAction _onCompleteAfter)
	{
		return PlaySE(_audioName, _volume, _delay, _pitch, false, _loopCount, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	private SoundEffectPlayer PlaySE(
		string _audioName,
		float _volume,
		float _delay,
		float _pitch,
		bool _isLoopInfinity,
		int _loopCount,
		float _fadeInTime,
		float _fadeOutTime,
		bool _is3dSound,
		Vector3 _soundPos,
		GameObject _chaseObj,
		float _minDistance,
		float _maxDistance,
		UnityAction _onStartBefore,
		UnityAction _onStart,
		UnityAction _onComplete,
		UnityAction _onCompleteAfter)
	{
		if (!m_audioClipDictSe.ContainsKey(_audioName))
		{
			Debug.Log("SE with that name does not exist :" + _audioName);
			return null;
		}

		var clip = m_audioClipDictSe[_audioName];
		var spatialBlend = (_is3dSound) ? 1.0f : 0.0f;

		SoundEffectPlayer player = GetSoundEffectPlayer();
		player.source.clip = clip;
		player.pitch = _pitch;
		player.transform.position = _soundPos;
		player.source.spatialBlend = spatialBlend;
		player.chaseObj = _chaseObj;
		player.loopCount = _loopCount;
		player.volume = _volume * m_volumeSe;
		player.delay = _delay;

		//CallBackEntry
		player.onStartBefore = _onStartBefore;
		player.onStart = _onStart;
		player.onComplete = _onComplete;
		player.onCompleteAfter = _onCompleteAfter;

		player.isFade = (_fadeInTime > 0.0f || _fadeOutTime > 0.0f);
		player.isLoopInfinity = _isLoopInfinity;

		if (player.isFade)
		{
			_fadeInTime = Mathf.Clamp(_fadeInTime, 0.0f, clip.length);
			_fadeOutTime = Mathf.Clamp(_fadeOutTime, 0.0f, clip.length);

			//フェードインとフェードアウトの時間が長すぎる場合の対応
			Keyframe k1, k2, k3, k4;
			var p1 = 0.0f;
			var p2 = _fadeInTime;
			var p3 = clip.length - _fadeOutTime;
			var p4 = clip.length;
			if (p2 > p3)
			{
				var av = (p2 + p3) / 2.0f;
				p2 = av;
				p3 = av;
			}
			AnimationCurve animCurve;
			k1 = new Keyframe(p1, 0.0f, 0.0f, 1.0f);
			k2 = new Keyframe(p2, 1.0f, 0.0f, 0.0f);
			k3 = new Keyframe(p3, 1.0f, 0.0f, 0.0f);
			k4 = new Keyframe(p4, 0.0f, 0.0f, 1.0f);

			animCurve = new AnimationCurve(k1, k2, k3, k4);
			player.animationCurve = animCurve;
		}

		if (_is3dSound)
		{
			player.source.minDistance = _minDistance;
			player.source.maxDistance = _maxDistance;
		}

		player.Play();
		return player;
	}

	public void Stop()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			m_soundEffectPlayers[i].Stop();
		}
	}

	public void Pause()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			m_soundEffectPlayers[i].Pause();
		}
	}

	public void Resume()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			m_soundEffectPlayers[i].Resume();
		}
	}

	private SoundEffectPlayer GetSoundEffectPlayer()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			if (m_soundEffectPlayers[i].isActive)
				continue;

			return m_soundEffectPlayers[i];
		}

		int idx = 0;
		for (int i = 1; i < m_soundEffectPlayers.Count; i++)
		{
			if (m_soundEffectPlayers[i].Length > m_soundEffectPlayers[idx].Length)
			{
				idx = i;
			}
		}
		return m_soundEffectPlayers[idx];
	}


	//******************************ここからBGM




	public void PlayBGM(SoundNameBGM _soundName)
	{

	}

	private void PlayBGM(string _audioName, float _volume, bool _isLoop, float _fadeInTime, float _fadeOutTime, float _crossFadeRate, bool _isCheckLoopPoint, float _loopStartTime = 0.0f, float _loopEndTime = 0.0f)
	{
		if (!m_audioClipDirtBgm.ContainsKey(_soundName))
		{
			Debug.Log("SE with that name does not exist :" + _soundName);
			return;
		}


		_volume = Mathf.Clamp01(_volume);
		_crossFadeRate = 1.0f - Mathf.Clamp01(_crossFadeRate);
		var clip = m_audioClipDictSe[_audioName];
		BackGroundMusicPlayer player = GetDisableBgmPlayer();

		//BGM再生部分の作成
		var isFade = (_fadeInTime > 0.0f || _fadeOutTime > 0.0f);
		if (isFade)
		{
			GetDisableBgmPlayer().FadeIn(_fadeInTime, (_crossFadeRate * _fadeInTime));
			GetAbleBgmPlayer().FadeOut(_fadeOutTime);
		}
		else
		{
			StopBGM();
		}



	}

	/// <summary>
	/// BGMを停止させる
	/// </summary>
	public void StopBGM()
	{
		m_mainBackgroundPlayer.Stop();
		m_subBackgroundPlayer.Stop();
	}

	/// <summary>
	/// 使っていないBGMPlayerを取得する
	/// </summary>
	/// <returns>The bgm player.</returns>
	public BackGroundMusicPlayer GetDisableBgmPlayer()
	{
		return (m_mainBackgroundPlayer.IsPlaying) ? m_subBackgroundPlayer : m_mainBackgroundPlayer;
	}

	public BackGroundMusicPlayer GetAbleBgmPlayer()
	{
		return (m_mainBackgroundPlayer.IsPlaying) ? m_mainBackgroundPlayer : m_subBackgroundPlayer;

	}



}
