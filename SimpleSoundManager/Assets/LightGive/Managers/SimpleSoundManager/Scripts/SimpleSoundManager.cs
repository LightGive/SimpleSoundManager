using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

public class SimpleSoundManager : LightGive.SingletonMonoBehaviour<SimpleSoundManager>
{
	private const float DefaultParamVolume = 1.0f;
	private const float DefaultParamDelay = 0.0f;
	private const float DefaultParamPitch = 1.0f;
	private const float DefaultParamMinDistance = 1.0f;
	private const float DefaultParamMaxDistance = 500.0f;
	private const float DefaultParamVolumeTotal = 1.0f;
	private const float DefaultParamVolumeBgm = 0.5f;
	private const float DefaultParamVolumeSe = 0.5f;

	private const string SaveKeyVolumeTotal = "VolumeTotal";
	private const string SaveKeyVolumeBgm = "VolumeBgm";
	private const string SaveKeyVolumeSe = "VolumeSe";

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
	private float m_volumeTotal = DefaultParamVolumeTotal;
	[SerializeField]
	private float m_volumeSe = DefaultParamVolumeSe;
	[SerializeField]
	private float m_volumeBgm = DefaultParamVolumeBgm;
	[SerializeField]
	private float m_defaultMinDistance = 1.0f;
	[SerializeField]
	private float m_defaultMaxDistance = 500.0f;

	[SerializeField]
	private bool m_editorIsFoldSeList = false;
	[SerializeField]
	private bool m_editorIsFoldBgmList = false;
	[SerializeField]
	private bool m_isLoopBgm = true;
	[SerializeField]
	private bool m_isChangeToSave = false;


	private Dictionary<string, AudioClip> m_audioClipDictSe = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> m_audioClipDirtBgm = new Dictionary<string, AudioClip>();


	public float volumeTotal
	{
		set
		{
			m_volumeTotal = Mathf.Clamp01(value);
			if (m_isChangeToSave) { SaveVolume(); }
		}
		get { return m_volumeTotal; }
	}

	public float volumeSe
	{
		set
		{
			m_volumeSe = Mathf.Clamp01(value);
			if (m_isChangeToSave) { SaveVolume(); }
		}
		get { return m_volumeSe; }
	}

	public float volumeBgm
	{
		set
		{
			m_volumeBgm = Mathf.Clamp01(value);
			if (m_isChangeToSave) { SaveVolume(); }
		}
		get { return m_volumeBgm; }
	}

	protected override void Awake()
	{
		base.isDontDestroy = true;
		base.Awake();

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

	/// <summary>
	/// HashTableのパラメータを参照してSEを再生する
	/// </summary>
	/// <returns>SEPlayer</returns>
	/// <param name="_soundName">SEの名前</param>
	/// <param name="_args">パラメータ</param>
	public SoundEffectPlayer PlaySE(SoundNameSE _soundName, Hashtable _args)
	{
		return PlaySE(_soundName.ToString(), _args);
	}

	/// <summary>
	/// HashTableのパラメータを参照してSEを再生する
	/// </summary>
	/// <returns>SEPlayer</returns>
	/// <param name="_soundName">SEの名前</param>
	/// <param name="_args">パラメータ</param>
	public SoundEffectPlayer PlaySE(string _soundName, Hashtable _args)
	{
		float volume = 1.0f;
		float delay = 0.0f;
		float pitch = 1.0f;
		bool isLoopInfinity = false;
		int loopCount = 0;
		float fadeInTime = 0.0f;
		float fadeOutTime = 0.0f;

		bool is3dSound = false;
		Vector3 soundPos = Vector3.zero;
		GameObject chaseObj = null;
		float minDistance = 5.0f;
		float maxDistance = 500.0f;

		UnityAction onStartBefore = null;
		UnityAction onStart = null;
		UnityAction onComplete = null;
		UnityAction onCompleteAfter = null;

		//Volume
		if (_args.ContainsKey(HashParam.volume))
		{
			if (_args[HashParam.volume] is float)
				volume = (float)_args[HashParam.volume];
			else
				Debug.Log(HashParam.volume.ToString() + " type is different.");
		}

		//Delay
		if (_args.ContainsKey(HashParam.delay))
		{
			if (_args[HashParam.delay] is float)
				delay = (float)_args[HashParam.delay];
			else
				Debug.Log(HashParam.delay.ToString() + " type is different.");
		}

		//Pitch
		if (_args.ContainsKey(HashParam.pitch))
		{
			if (_args[HashParam.pitch] is float)
				pitch = (float)_args[HashParam.pitch];
			else
				Debug.Log(HashParam.pitch.ToString() + " type is different.");
		}

		//IsLoopInfinity
		if (_args.ContainsKey(HashParam.isLoopInfinity))
		{
			if (_args[HashParam.isLoopInfinity] is bool)
				isLoopInfinity = (bool)_args[HashParam.isLoopInfinity];
			else
				Debug.Log(HashParam.isLoopInfinity.ToString() + " type is different.");
		}

		//LoopCount
		if (_args.ContainsKey(HashParam.loopCount))
		{
			if (_args[HashParam.loopCount] is int)
				loopCount = (int)_args[HashParam.loopCount];
			else
				Debug.Log(HashParam.loopCount.ToString() + " type is different.");
		}

		//FadeInTime
		if (_args.ContainsKey(HashParam.fadeInTime))
		{
			if (_args[HashParam.fadeInTime] is float)
				fadeInTime = (float)_args[HashParam.fadeInTime];
			else
				Debug.Log(HashParam.fadeInTime.ToString() + " type is different.");
		}

		//FadeOutTime
		if (_args.ContainsKey(HashParam.fadeOutTime))
		{
			if (_args[HashParam.fadeOutTime] is float)
				fadeOutTime = (float)_args[HashParam.fadeOutTime];
			else
				Debug.Log(HashParam.fadeOutTime.ToString() + " type is different.");
		}

		//is3dSound
		if (_args.ContainsKey(HashParam.is3dSound))
		{
			if (_args[HashParam.is3dSound] is bool)
				is3dSound = (bool)_args[HashParam.is3dSound];
			else
				Debug.Log(HashParam.is3dSound.ToString() + " type is different.");
		}

		//SoundPos
		if (_args.ContainsKey(HashParam.soundPos))
		{
			if (_args[HashParam.soundPos] is Vector3)
				soundPos = (Vector3)_args[HashParam.soundPos];
			else
				Debug.Log(HashParam.soundPos.ToString() + " type is different.");
		}

		//ChaseObj
		if (_args.ContainsKey(HashParam.chaseObj))
		{
			if (_args[HashParam.chaseObj] is GameObject)
				chaseObj = (GameObject)_args[HashParam.chaseObj];
			else
				Debug.Log(HashParam.chaseObj.ToString() + " type is different.");
		}

		//MinDistance
		if (_args.ContainsKey(HashParam.minDistance))
		{
			if (_args[HashParam.minDistance] is float)
				minDistance = (float)_args[HashParam.minDistance];
			else
				Debug.Log(HashParam.minDistance.ToString() + " type is different.");
		}
		//MaxDistance
		if (_args.ContainsKey(HashParam.maxDistance))
		{
			if (_args[HashParam.maxDistance] is float)
				maxDistance = (float)_args[HashParam.maxDistance];
			else
				Debug.Log(HashParam.maxDistance.ToString() + " type is different.");
		}
		//onStartBefore
		if (_args.ContainsKey(HashParam.onStartBefore))
		{
			if (_args[HashParam.onStartBefore] is UnityAction)
				onStartBefore = (UnityAction)_args[HashParam.onStartBefore];
			else
				Debug.Log(HashParam.onStartBefore.ToString() + " type is different.");
		}
		//onStart
		if (_args.ContainsKey(HashParam.onStart))
		{
			if (_args[HashParam.onStart] is UnityAction)
				onStart = (UnityAction)_args[HashParam.onStart];
			else
				Debug.Log(HashParam.onStart.ToString() + " type is different.");
		}
		//onComplete
		if (_args.ContainsKey(HashParam.onComplete))
		{
			if (_args[HashParam.onComplete] is UnityAction)
				onComplete = (UnityAction)_args[HashParam.onComplete];
			else
				Debug.Log(HashParam.onComplete.ToString() + " type is different.");
		}
		//onCompleteAfter
		if (_args.ContainsKey(HashParam.onCompleteAfter))
		{
			if (_args[HashParam.onCompleteAfter] is UnityAction)
				onCompleteAfter = (UnityAction)_args[HashParam.onCompleteAfter];
			else
				Debug.Log(HashParam.onCompleteAfter.ToString() + " type is different.");
		}

		return PlaySE(
			_soundName,
			volume,
			delay,
			pitch,
			isLoopInfinity,
			loopCount,
			fadeInTime,
			fadeOutTime,
			is3dSound,
			soundPos,
			chaseObj,
			minDistance,
			maxDistance,
			onStartBefore,
			onStart,
			onComplete,
			onCompleteAfter);
	}

	//PlaySE_2D_Simple
	public SoundEffectPlayer PlaySE_2D(SoundNameSE _soundName, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_2D(SoundNameSE _soundName, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D(SoundNameSE _soundName, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D(SoundNameSE _soundName)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}

	//PlaySE_2D_Loop
	public SoundEffectPlayer PlaySE_2D_Loop(SoundNameSE _soundName, int _loopCount, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_2D_Loop(SoundNameSE _soundName, int _loopCount, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_Loop(SoundNameSE _soundName, int _loopCount, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_Loop(SoundNameSE _soundName, int _loopCount)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}

	//PlaySE_2D_LoopInfinity
	public SoundEffectPlayer PlaySE_2D_LoopInfinity(SoundNameSE _soundName, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_2D_LoopInfinity(SoundNameSE _soundName, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, true, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_LoopInfinity(SoundNameSE _soundName, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, true, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_LoopInfinity(SoundNameSE _soundName)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, true, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}

	//PlaySE_2D_FadeInOut
	public SoundEffectPlayer PlaySE_2D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_2D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_2D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, null, null, null, null);
	}

	//Play_3D_Simple
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, null, null, null, null);
	}

	//Play_3D_ChaseObject
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject, float _minDistance, float _maxDistance, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject, float _minDistance, float _maxDistance, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject, float _minDistance, float _maxDistance, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject, float _minDistance, float _maxDistance)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, DefaultParamMinDistance, DefaultParamMaxDistance, null, null, null, null);
	}

	//Play_3D_Loop
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos, float _minDistance, float _maxDistance)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, null, null, null, null);
	}

	//Play_3D_LoopInfinity
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos, float _minDistance, float _maxDistance)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, null, null, null, null);
	}

	//Play_3D_FadeInOut
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay, float _pitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume, float _delay)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos, float _minDistance, float _maxDistance, float _volume)
	{
		return PlaySE(_soundName.ToString(), _volume, DefaultParamDelay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos, float _minDistance, float _maxDistance)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, null, null, null, null);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos)
	{
		return PlaySE(_soundName.ToString(), DefaultParamVolume, DefaultParamDelay, DefaultParamPitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, null, null, null, null);
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
		player.source.rolloffMode = (_is3dSound) ? AudioRolloffMode.Linear : AudioRolloffMode.Logarithmic;
		player.chaseObj = _chaseObj;
		player.loopCount = _loopCount;
		player.volume = _volume * m_volumeSe;
		player.delay = _delay;

		//CallBackEntry
		player.onStartBefore = _onStartBefore;
		player.onStart = _onStart;
		player.onComplete = _onComplete;
		player.onCompleteAfter = _onCompleteAfter;

		_fadeInTime = Mathf.Clamp(_fadeInTime, 0.0f, float.MaxValue);
		_fadeOutTime = Mathf.Clamp(_fadeOutTime, 0.0f, float.MaxValue);
		player.isFade = (_fadeInTime > 0.0f || _fadeOutTime > 0.0f);
		player.isLoopInfinity = _isLoopInfinity;

		if (player.isFade)
		{
			List<Keyframe> keyframeList = new List<Keyframe>();
			if (_fadeInTime <= 0.0f)
			{
				//フェードアウトのみの場合
				keyframeList.Add(new Keyframe(0.0f, 1.0f));
				if (clip.length < _fadeOutTime)
				{
					keyframeList.Add(new Keyframe(clip.length, clip.length / _fadeOutTime));
				}
				else
				{
					keyframeList.Add(new Keyframe(clip.length - _fadeOutTime, 1.0f));
					keyframeList.Add(new Keyframe(clip.length, 0.0f));
				}
			}
			else if (_fadeOutTime <= 0.0f)
			{
				//フェードインのみの場合
				keyframeList.Add(new Keyframe(0.0f, 0.0f));
				if (clip.length < _fadeInTime)
				{
					keyframeList.Add(new Keyframe(clip.length, clip.length / _fadeInTime));
				}
				else
				{
					keyframeList.Add(new Keyframe(_fadeInTime, 1.0f));
					keyframeList.Add(new Keyframe(clip.length, 1.0f));
				}
			}
			else
			{
				keyframeList.Add(new Keyframe(0.0f, 0.0f));

				//フェードイン、フェードアウトが両方ある時
				if (clip.length < (_fadeInTime + _fadeOutTime))
				{
					var x1 = 0.0f;
					var y1 = 0.0f;
					var x2 = _fadeInTime;
					var y2 = 1.0f;
					var x3 = clip.length - _fadeOutTime;
					var y3 = 1.0f;
					var x4 = clip.length;
					var y4 = 0.0f;
					var a0 = (y2 - y1) / (x2 - x1);
					var a1 = (y4 - y3) / (x4 - x3);
					var x = (a0 * x1 - y1 - a1 * x3 + y3) / (a0 - a1);
					var y = (y2 - y1) / (x2 - 0.0f) * (x - x1) + y1;
					keyframeList.Add(new Keyframe(x, y));
				}
				else
				{
					keyframeList.Add(new Keyframe(_fadeInTime, 1.0f));
					keyframeList.Add(new Keyframe(clip.length - _fadeOutTime, 1.0f));
				}
				keyframeList.Add(new Keyframe(clip.length, 0.0f));
			}

			//フェードインとフェードアウトの時間が長すぎる場合の対応

			AnimationCurve animCurve = new AnimationCurve(keyframeList.ToArray());
			for (int i = 0; i < animCurve.keys.Length; i++)
			{
				AnimationUtility.SetKeyLeftTangentMode(animCurve, i, AnimationUtility.TangentMode.Linear);
				AnimationUtility.SetKeyRightTangentMode(animCurve, i, AnimationUtility.TangentMode.Linear);
			}

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

	private void PlayBGM(string _soundName, float _volume, bool _isLoop, float _fadeInTime, float _fadeOutTime, float _crossFadeRate)
	{
		if (!m_audioClipDirtBgm.ContainsKey(_soundName))
		{
			Debug.Log("BGM with that name does not exist :" + _soundName);
			return;
		}

		_volume = Mathf.Clamp01(_volume);
		_crossFadeRate = 1.0f - Mathf.Clamp01(_crossFadeRate);
		var clip = m_audioClipDictSe[_soundName];
		BackGroundMusicPlayer player = GetNonActiveBgmPlayer();

		//BGM再生部分の作成
		var isFade = (_fadeInTime > 0.0f || _fadeOutTime > 0.0f);
		if (isFade)
		{
			GetNonActiveBgmPlayer().FadeIn(_fadeInTime, (_crossFadeRate * _fadeInTime));
			GetActiveBgmPlayer().FadeOut(_fadeOutTime);
		}
		else
		{
			StopBGM();
		}

		player.Play(clip, _isLoop, isFade, _volume * volumeBgm * volumeTotal, false, 0.0f, 0.0f);
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
	public BackGroundMusicPlayer GetNonActiveBgmPlayer()
	{
		return (m_mainBackgroundPlayer.IsPlaying) ? m_subBackgroundPlayer : m_mainBackgroundPlayer;
	}

	/// <summary>
	/// 使用中のBGMPlayerを取得する
	/// </summary>
	/// <returns>The able bgm player.</returns>
	public BackGroundMusicPlayer GetActiveBgmPlayer()
	{
		return (m_mainBackgroundPlayer.IsPlaying) ? m_mainBackgroundPlayer : m_subBackgroundPlayer;
	}


	public void LoadVolume()
	{
		m_volumeTotal = PlayerPrefs.GetFloat(SaveKeyVolumeTotal, DefaultParamVolumeTotal);
		m_volumeBgm = PlayerPrefs.GetFloat(SaveKeyVolumeBgm, DefaultParamVolumeBgm);
		m_volumeSe = PlayerPrefs.GetFloat(SaveKeyVolumeSe, DefaultParamVolumeSe);
	}

	public void SaveVolume()
	{
		PlayerPrefs.SetFloat(SaveKeyVolumeTotal, volumeTotal);
		PlayerPrefs.SetFloat(SaveKeyVolumeBgm, volumeBgm);
		PlayerPrefs.SetFloat(SaveKeyVolumeSe, volumeSe);
		PlayerPrefs.Save();
	}


	public enum HashParam
	{
		volume,
		delay,
		pitch,
		isLoopInfinity,
		loopCount,
		fadeInTime,
		fadeOutTime,
		is3dSound,
		soundPos,
		chaseObj,
		minDistance,
		maxDistance,
		onStartBefore,
		onStart,
		onComplete,
		onCompleteAfter,
	}
}