using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	[SerializeField]
	public List<AudioClip> audioClipListSe = new List<AudioClip>();
	[SerializeField]
	public List<AudioClip> audioClipListBgm = new List<AudioClip>();

	[SerializeField]
	private List<SoundEffectPlayer> m_soundEffectPlayers = new List<SoundEffectPlayer>();
	[SerializeField]
	private int m_sePlayerNum = 10;
	[SerializeField]
	private float volumeSe = 1.0f;
	[SerializeField]
	private float volumeBgm = 1.0f;


	private Dictionary<string, AudioClip> m_audioClipDictSe = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> m_audioClipDirtBgm = new Dictionary<string, AudioClip>();


	protected override void Init()
	{
		base.Init();
		for (int i = 0; i < m_sePlayerNum;i++)
		{
			GameObject soundPlayerObj = new GameObject("SoundPlayer" + i.ToString("0"));
			soundPlayerObj.transform.SetParent(transform);
			SoundEffectPlayer player = soundPlayerObj.AddComponent<SoundEffectPlayer>();
			player.Init();
			m_soundEffectPlayers.Add(player);
		}

		//Dictionaryに追加
		for (int i = 0; i < audioClipListSe.Count;i++)
		{
			m_audioClipDictSe.Add(audioClipListSe[i].name, audioClipListSe[i]);
		}
		for (int i = 0; i < audioClipListBgm.Count; i++)
		{
			m_audioClipDirtBgm.Add(audioClipListBgm[i].name, audioClipListBgm[i]);
		}
	}

	public SoundEffectPlayer PlaySE2D(SoundNameSE _audioName)
	{
		return PlaySE(_audioName.ToString(), 1.0f, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null);
	}
	public SoundEffectPlayer PlaySE2D(SoundNameSE _audioName ,float _volume)
	{
		return PlaySE(_audioName.ToString(), _volume, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName)
	{
		return PlaySE(_audioName, 1.0f, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null);
	}
	public SoundEffectPlayer PlaySE2D(string _audioName, float _volume)
	{
		return PlaySE(_audioName, _volume, 0.0f, 1.0f, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null);
	}

	public SoundEffectPlayer Play2D_FadeInOut(SoundNameSE _audioName)
	{

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
		UnityAction _onStart, 
		UnityAction _onComplete)
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
		player.volume = _volume * volumeSe;
		player.delay = _delay;
		player.callbackOnComplete = _onComplete;
		player.callbackOnStart = _onStart;
		player.isFade = (_fadeInTime >= 0.0f || _fadeOutTime >= 0.0f);
		player.isLoopInfinity = _isLoopInfinity;

		if (player.isFade)
		{
			_fadeInTime = Mathf.Clamp(_fadeInTime, 0.0f, clip.length);
			_fadeOutTime = Mathf.Clamp(_fadeOutTime, 0.0f, clip.length);
			Keyframe key1 = new Keyframe(0.0f, 0.0f, 0.0f, 1.0f);
			Keyframe key2 = new Keyframe(_fadeInTime, 1.0f, 0.0f, 0.0f);
			Keyframe key3 = new Keyframe(clip.length - _fadeOutTime, 1.0f, 0.0f, 0.0f);
			Keyframe key4 = new Keyframe(clip.length, 0.0f, 0.0f, 1.0f);

			AnimationCurve animCurve = new AnimationCurve(key1, key2, key3, key4);
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

	private SoundEffectPlayer GetSoundEffectPlayer()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count;i++)
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
}
