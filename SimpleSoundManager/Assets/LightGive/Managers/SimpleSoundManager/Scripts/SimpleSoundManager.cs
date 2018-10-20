using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Audio;

public class SimpleSoundManager : LightGive.SingletonMonoBehaviour<SimpleSoundManager>
{
	private const bool DefaultParamIsLoop = true;
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
	private AudioMixerGroup m_seAudioMixerGroup;
	[SerializeField]
	private AudioMixerGroup m_bgmAudioMixerGroup;
	[SerializeField]
	private int m_sePlayerNum = 10;
	[SerializeField]
	private float m_volumeTotal = DefaultParamVolumeTotal;
	[SerializeField]
	private float m_volumeSe = DefaultParamVolumeSe;
	[SerializeField]
	private float m_volumeBgm = DefaultParamVolumeBgm;

	[SerializeField]
	private bool m_editorIsFoldSeList = false;
	[SerializeField]
	private bool m_editorIsFoldBgmList = false;
	[SerializeField]
	private bool m_isLoopBgm = true;
	[SerializeField]
	private bool m_isChangeToSave = false;
	[SerializeField]
	private bool m_isChangeSceneToStopSe = false;


	private Dictionary<string, AudioClip> m_audioClipDictSe = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> m_audioClipDirtBgm = new Dictionary<string, AudioClip>();

	#region Prop

	public AudioMixerGroup seAudioMixerGroup { get { return m_seAudioMixerGroup; } }
	public AudioMixerGroup bgmAudioMixerGroup { get { return m_bgmAudioMixerGroup; } }

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

	public bool isPlayingBgm { get { return m_mainBackgroundPlayer.isPlaying; } }

	public bool isPlayingSE
	{
		get
		{
			for (int i = 0; i < m_soundEffectPlayers.Count; i++)
			{
				if (m_soundEffectPlayers[i].isPlaying)
					return true;
			}
			return false;
		}
	}

	#endregion

	protected override void Awake()
	{
		base.isDontDestroy = true;
		base.Awake();

		SceneManager.sceneUnloaded += OnSceneUnloaded;

		for (int i = 0; i < m_sePlayerNum; i++)
		{
			GameObject soundPlayerObj = new GameObject("SoundPlayer" + i.ToString("0"));
			soundPlayerObj.transform.SetParent(transform);
			SoundEffectPlayer player = soundPlayerObj.AddComponent<SoundEffectPlayer>();
			player.Init();
			m_soundEffectPlayers.Add(player);
		}

		GameObject mainBackgroundPlayerObj = new GameObject("MainBackgroundMusicPlayer");
		GameObject subBackgroundPlayerObj = new GameObject("SubBackgroundMusicPlayer");
		mainBackgroundPlayerObj.transform.SetParent(transform);
		subBackgroundPlayerObj.transform.SetParent(transform);
		m_mainBackgroundPlayer = mainBackgroundPlayerObj.AddComponent<BackGroundMusicPlayer>();
		m_subBackgroundPlayer = subBackgroundPlayerObj.AddComponent<BackGroundMusicPlayer>();

		//初期化
		m_mainBackgroundPlayer.Init();
		m_subBackgroundPlayer.Init();

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
		if (_args.ContainsKey(HashParam_SE.volume))
		{
			if (_args[HashParam_SE.volume] is float)
				volume = (float)_args[HashParam_SE.volume];
			else
				Debug.Log(HashParam_SE.volume.ToString() + " type is different.");
		}

		//Delay
		if (_args.ContainsKey(HashParam_SE.delay))
		{
			if (_args[HashParam_SE.delay] is float)
				delay = (float)_args[HashParam_SE.delay];
			else
				Debug.Log(HashParam_SE.delay.ToString() + " type is different.");
		}

		//Pitch
		if (_args.ContainsKey(HashParam_SE.pitch))
		{
			if (_args[HashParam_SE.pitch] is float)
				pitch = (float)_args[HashParam_SE.pitch];
			else
				Debug.Log(HashParam_SE.pitch.ToString() + " type is different.");
		}

		//IsLoopInfinity
		if (_args.ContainsKey(HashParam_SE.isLoopInfinity))
		{
			if (_args[HashParam_SE.isLoopInfinity] is bool)
				isLoopInfinity = (bool)_args[HashParam_SE.isLoopInfinity];
			else
				Debug.Log(HashParam_SE.isLoopInfinity.ToString() + " type is different.");
		}

		//LoopCount
		if (_args.ContainsKey(HashParam_SE.loopCount))
		{
			if (_args[HashParam_SE.loopCount] is int)
				loopCount = (int)_args[HashParam_SE.loopCount];
			else
				Debug.Log(HashParam_SE.loopCount.ToString() + " type is different.");
		}

		//FadeInTime
		if (_args.ContainsKey(HashParam_SE.fadeInTime))
		{
			if (_args[HashParam_SE.fadeInTime] is float)
				fadeInTime = (float)_args[HashParam_SE.fadeInTime];
			else
				Debug.Log(HashParam_SE.fadeInTime.ToString() + " type is different.");
		}

		//FadeOutTime
		if (_args.ContainsKey(HashParam_SE.fadeOutTime))
		{
			if (_args[HashParam_SE.fadeOutTime] is float)
				fadeOutTime = (float)_args[HashParam_SE.fadeOutTime];
			else
				Debug.Log(HashParam_SE.fadeOutTime.ToString() + " type is different.");
		}

		//is3dSound
		if (_args.ContainsKey(HashParam_SE.is3dSound))
		{
			if (_args[HashParam_SE.is3dSound] is bool)
				is3dSound = (bool)_args[HashParam_SE.is3dSound];
			else
				Debug.Log(HashParam_SE.is3dSound.ToString() + " type is different.");
		}

		//SoundPos
		if (_args.ContainsKey(HashParam_SE.soundPos))
		{
			if (_args[HashParam_SE.soundPos] is Vector3)
				soundPos = (Vector3)_args[HashParam_SE.soundPos];
			else
				Debug.Log(HashParam_SE.soundPos.ToString() + " type is different.");
		}

		//ChaseObj
		if (_args.ContainsKey(HashParam_SE.chaseObj))
		{
			if (_args[HashParam_SE.chaseObj] is GameObject)
				chaseObj = (GameObject)_args[HashParam_SE.chaseObj];
			else
				Debug.Log(HashParam_SE.chaseObj.ToString() + " type is different.");
		}

		//MinDistance
		if (_args.ContainsKey(HashParam_SE.minDistance))
		{
			if (_args[HashParam_SE.minDistance] is float)
				minDistance = (float)_args[HashParam_SE.minDistance];
			else
				Debug.Log(HashParam_SE.minDistance.ToString() + " type is different.");
		}
		//MaxDistance
		if (_args.ContainsKey(HashParam_SE.maxDistance))
		{
			if (_args[HashParam_SE.maxDistance] is float)
				maxDistance = (float)_args[HashParam_SE.maxDistance];
			else
				Debug.Log(HashParam_SE.maxDistance.ToString() + " type is different.");
		}
		//onStartBefore
		if (_args.ContainsKey(HashParam_SE.onStartBefore))
		{
			if (_args[HashParam_SE.onStartBefore] is UnityAction)
				onStartBefore = (UnityAction)_args[HashParam_SE.onStartBefore];
			else
				Debug.Log(HashParam_SE.onStartBefore.ToString() + " type is different.");
		}
		//onStart
		if (_args.ContainsKey(HashParam_SE.onStart))
		{
			if (_args[HashParam_SE.onStart] is UnityAction)
				onStart = (UnityAction)_args[HashParam_SE.onStart];
			else
				Debug.Log(HashParam_SE.onStart.ToString() + " type is different.");
		}
		//onComplete
		if (_args.ContainsKey(HashParam_SE.onComplete))
		{
			if (_args[HashParam_SE.onComplete] is UnityAction)
				onComplete = (UnityAction)_args[HashParam_SE.onComplete];
			else
				Debug.Log(HashParam_SE.onComplete.ToString() + " type is different.");
		}
		//onCompleteAfter
		if (_args.ContainsKey(HashParam_SE.onCompleteAfter))
		{
			if (_args[HashParam_SE.onCompleteAfter] is UnityAction)
				onCompleteAfter = (UnityAction)_args[HashParam_SE.onCompleteAfter];
			else
				Debug.Log(HashParam_SE.onCompleteAfter.ToString() + " type is different.");
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
	public SoundEffectPlayer PlaySE_2D(SoundNameSE _soundName, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//PlaySE_2D_Loop
	public SoundEffectPlayer PlaySE_2D_Loop(SoundNameSE _soundName, int _loopCount, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//PlaySE_2D_LoopInfinity
	public SoundEffectPlayer PlaySE_2D_LoopInfinity(SoundNameSE _soundName, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//PlaySE_2D_FadeInOut
	public SoundEffectPlayer PlaySE_2D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _volume = DefaultParamVolume, float _delay = DefaultParamVolume, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, 0.0f, 0.0f, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//Play_3D_Simple
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, float _minDistance, float _maxDistance, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D(SoundNameSE _soundName, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}


	//Play_3D_ChaseObject
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, float _minDistance, float _maxDistance, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_ChaseObject(SoundNameSE _soundName, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}


	//Play_3D_Loop
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, float _minDistance, float _maxDistance, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, float _minDistance, float _maxDistance, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_Loop(SoundNameSE _soundName, int _loopCount, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//Play_3D_LoopInfinity
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, float _minDistance, float _maxDistance, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, float _minDistance, float _maxDistance, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_LoopInfinity(SoundNameSE _soundName, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, true, 1, 0.0f, 0.0f, true, _chaseObject.transform.position, _chaseObject, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}

	//Play_3D_FadeInOut
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _minDistance, float _maxDistance, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, Vector3 _soundPos, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, float _minDistance, float _maxDistance, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _chaseObject.transform.position, _chaseObject, _minDistance, _maxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
	}
	public SoundEffectPlayer PlaySE_3D_FadeInOut(SoundNameSE _soundName, float _fadeInTime, float _fadeOutTime, GameObject _chaseObject, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, float _pitch = DefaultParamPitch, UnityAction _onStartBefore = null, UnityAction _onStart = null, UnityAction _onComplete = null, UnityAction _onCompleteAfter = null)
	{
		return PlaySE(_soundName.ToString(), _volume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _chaseObject.transform.position, _chaseObject, DefaultParamMinDistance, DefaultParamMaxDistance, _onStartBefore, _onStart, _onComplete, _onCompleteAfter);
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
				animCurve.keys[i].inTangent = 0.0f;
				animCurve.keys[i].outTangent = 0.0f;
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

	public void StopSE()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			m_soundEffectPlayers[i].Stop();
		}
	}

	public void PauseSE()
	{
		for (int i = 0; i < m_soundEffectPlayers.Count; i++)
		{
			m_soundEffectPlayers[i].Pause();
		}
	}


	public void ResumeSE()
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


	//ここからBGM


	public BackGroundMusicPlayer PlayBGM(string _mainSoundName, Hashtable _args)
	{
		string introSoundName = "";
		float volume = DefaultParamVolume;
		float delay = DefaultParamDelay;
		bool isLoop = DefaultParamIsLoop;
		float fadeInTime = 0.0f;
		float fadeOutTime = 0.0f;
		float crossFadeRate = 0.0f;
		UnityAction onStartBefore = null;
		UnityAction onStart = null;
		UnityAction onComplete = null;
		UnityAction onCompleteAfter = null;


		//IntroSoundName
		if (_args.ContainsKey(HashParam_BGM.introSoundName))
		{
			if (_args[HashParam_BGM.introSoundName] is string)
				introSoundName = (string)_args[HashParam_BGM.introSoundName];
			else if (_args[HashParam_BGM.introSoundName] is SoundNameBGM)
				introSoundName = ((SoundNameBGM)_args[HashParam_BGM.introSoundName]).ToString();
			else
				Debug.Log(HashParam_BGM.introSoundName.ToString() + " type is different.");
		}

		//Volume
		if (_args.ContainsKey(HashParam_BGM.volume))
		{
			if (_args[HashParam_BGM.volume] is float)
				volume = (float)_args[HashParam_BGM.volume];
			else
				Debug.Log(HashParam_BGM.volume.ToString() + " type is different.");
		}

		//Delay
		if (_args.ContainsKey(HashParam_BGM.delay))
		{
			if (_args[HashParam_BGM.delay] is float)
				delay = (float)_args[HashParam_BGM.delay];
			else
				Debug.Log(HashParam_BGM.delay.ToString() + " type is different.");
		}

		//IsLoop
		if (_args.ContainsKey(HashParam_BGM.isLoop))
		{
			if (_args[HashParam_BGM.isLoop] is bool)
				isLoop = (bool)_args[HashParam_BGM.isLoop];
			else
				Debug.Log(HashParam_BGM.isLoop.ToString() + " type is different.");
		}

		//FadeIn
		if (_args.ContainsKey(HashParam_BGM.fadeInTime))
		{
			if (_args[HashParam_BGM.fadeOutTime] is float)
				fadeInTime = (float)_args[HashParam_BGM.fadeInTime];
			else
				Debug.Log(HashParam_BGM.fadeInTime.ToString() + " type is different.");
		}

		//FadeOut
		if (_args.ContainsKey(HashParam_BGM.fadeInTime))
		{
			if (_args[HashParam_BGM.fadeOutTime] is float)
				fadeOutTime = (float)_args[HashParam_BGM.fadeOutTime];
			else
				Debug.Log(HashParam_BGM.fadeOutTime.ToString() + " type is different.");
		}

		//CrossFadeRate
		if (_args.ContainsKey(HashParam_BGM.crossFadeRate))
		{
			if (_args[HashParam_BGM.crossFadeRate] is float)
				crossFadeRate = (float)_args[HashParam_BGM.crossFadeRate];
			else
				Debug.Log(HashParam_BGM.crossFadeRate.ToString() + " type is different.");
		}

		//onStartBefore
		if (_args.ContainsKey(HashParam_BGM.onIntroStart))
		{
			if (_args[HashParam_BGM.onIntroStart] is UnityAction)
				onStartBefore = (UnityAction)_args[HashParam_BGM.onIntroStart];
			else
				Debug.Log(HashParam_BGM.onIntroStart.ToString() + " type is different.");
		}

		//onStart
		if (_args.ContainsKey(HashParam_BGM.onIntroComplete))
		{
			if (_args[HashParam_BGM.onIntroComplete] is UnityAction)
				onStart = (UnityAction)_args[HashParam_BGM.onIntroComplete];
			else
				Debug.Log(HashParam_BGM.onIntroComplete.ToString() + " type is different.");
		}

		//onComplete
		if (_args.ContainsKey(HashParam_BGM.onMainStart))
		{
			if (_args[HashParam_BGM.onMainStart] is UnityAction)
				onComplete = (UnityAction)_args[HashParam_BGM.onMainStart];
			else
				Debug.Log(HashParam_BGM.onMainStart.ToString() + " type is different.");
		}

		//onCompleteAfter
		if (_args.ContainsKey(HashParam_BGM.onMainComplete))
		{
			if (_args[HashParam_BGM.onMainComplete] is UnityAction)
				onCompleteAfter = (UnityAction)_args[HashParam_BGM.onMainComplete];
			else
				Debug.Log(HashParam_BGM.onMainComplete.ToString() + " type is different.");
		}

		return PlayBGM(
			_mainSoundName,
			introSoundName,
			volume,
			delay,
			isLoop,
			fadeInTime,
			fadeOutTime,
			crossFadeRate,
			onStartBefore,
			onStart,
			onComplete,
			onCompleteAfter);
	}


	public BackGroundMusicPlayer PlayBGM(SoundNameBGM _mainSoundName, bool _isLoop = DefaultParamIsLoop, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, UnityAction _onIntroStart = null, UnityAction _onIntroComplete = null, UnityAction _onMainStart = null, UnityAction _onMainComplete = null)
	{
		return PlayBGM(_mainSoundName.ToString(), "", _volume, _delay, _isLoop, 0.0f, 0.0f, 0.0f, _onIntroStart, _onIntroComplete, _onMainStart, _onMainComplete);
	}

	public BackGroundMusicPlayer PlayBGM_FadeInOut(SoundNameBGM _mainSoundName, float _fadeInTime, float _fadeOutTime, float _crossFadeRate, bool _isLoop = DefaultParamIsLoop, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, UnityAction _onIntroStart = null, UnityAction _onIntroComplete = null, UnityAction _onMainStart = null, UnityAction _onMainComplete = null)
	{
		return PlayBGM(_mainSoundName.ToString(), "", _volume, _delay, _isLoop, _fadeInTime, _fadeOutTime, _crossFadeRate, _onIntroStart, _onIntroComplete, _onMainStart, _onMainComplete);
	}

	public BackGroundMusicPlayer PlayBGM_Intro(SoundNameBGM _mainSoundName, SoundNameBGM _introSoundName, bool _isLoop = DefaultParamIsLoop, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, UnityAction _onIntroStart = null, UnityAction _onIntroComplete = null, UnityAction _onMainStart = null, UnityAction _onMainComplete = null)
	{
		return PlayBGM(_mainSoundName.ToString(), _introSoundName.ToString(), _volume, _delay, _isLoop, 0.0f, 0.0f, 0.0f, _onIntroStart, _onIntroComplete, _onMainStart, _onMainComplete);
	}

	public BackGroundMusicPlayer PlayBGM_IntroFadeInOut(SoundNameBGM _mainSoundName, SoundNameBGM _introSoundName, float _fadeInTime, float _fadeOutTime, float _crossFadeRate, bool _isLoop = DefaultParamIsLoop, float _volume = DefaultParamVolume, float _delay = DefaultParamDelay, UnityAction _onIntroStart = null, UnityAction _onIntroComplete = null, UnityAction _onMainStart = null, UnityAction _onMainComplete = null)
	{
		return PlayBGM(_mainSoundName.ToString(), _introSoundName.ToString(), _volume, _delay, _isLoop, _fadeInTime, _fadeOutTime, _crossFadeRate, _onIntroStart, _onIntroComplete, _onMainStart, _onMainComplete);
	}

	private BackGroundMusicPlayer PlayBGM(
		string _mainSoundName,
		string _introSoundName,
		float _volume,
		float _delay,
		bool _isLoop,
		float _fadeInTime,
		float _fadeOutTime,
		float _crossFadeRate,
		UnityAction _onIntroStart,
		UnityAction _onIntroComplete,
		UnityAction _onMainStart,
		UnityAction _onMainComplete)
	{
		AudioClip introClip = null;
		AudioClip mainClip = null;

		//イントロのサウンドの名前に文字が入っているかのチェック
		if (!string.IsNullOrEmpty(_introSoundName))
		{
			//文字が入っていた時
			if (!m_audioClipDirtBgm.ContainsKey(_introSoundName))
			{
				Debug.Log("BGM with that name does not exist :" + _introSoundName);
				return null;
			}
			introClip = m_audioClipDirtBgm[_introSoundName];
		}

		//音楽ファイルが存在するかの判定
		if (!m_audioClipDirtBgm.ContainsKey(_mainSoundName))
		{
			Debug.Log("BGM with that name does not exist :" + _mainSoundName);
			return null;
		}

		//音量を規定値に制限
		_volume = Mathf.Clamp01(_volume);

		//フェードをどの程度被せるかを規定値に制限
		_crossFadeRate = 1.0f - Mathf.Clamp01(_crossFadeRate);

		//AudioClip取得
		mainClip = m_audioClipDirtBgm[_mainSoundName];

		//BGM再生部分の作成
		var isFadeIn = _fadeInTime > 0.0f;
		var isFadeOut = _fadeOutTime > 0.0f;

		if (!isFadeIn && !isFadeOut)
		{
			//BGMを止める
			StopBGM();
		}
		else
		{
			//待ち時間
			var waitTime = (_fadeInTime > _fadeOutTime) ?
				_crossFadeRate * _fadeOutTime :
				Mathf.Clamp(_fadeOutTime - ((1.0f - _crossFadeRate) * _fadeInTime), 0.0f, float.PositiveInfinity);

			//メインがプレイ中と何も再生していない時で分ける
			if (m_mainBackgroundPlayer.isPlaying)
				m_subBackgroundPlayer.FadeIn(_fadeInTime, waitTime + _delay);
			else
				m_subBackgroundPlayer.FadeIn(_fadeInTime, _delay);


			//メインがプレイ中と何も再生していない時で分ける
			m_mainBackgroundPlayer.FadeOut(_fadeOutTime, _delay);

		}

		//使っていない方のBGMPlayerに色々設定する
		m_subBackgroundPlayer.mainClip = mainClip;
		m_subBackgroundPlayer.introClip = introClip;
		m_subBackgroundPlayer.isLoop = _isLoop;
		m_subBackgroundPlayer.delay = _delay;
		m_subBackgroundPlayer.volume = _volume;
		m_subBackgroundPlayer.onIntroStart = _onIntroStart;
		m_subBackgroundPlayer.onIntroComplete = _onIntroComplete;
		m_subBackgroundPlayer.onMainStart = _onMainStart;
		m_subBackgroundPlayer.onMainComplete = _onMainComplete;
		m_subBackgroundPlayer.Play();

		//SubとMainを交換
		var tmp = m_subBackgroundPlayer;
		m_subBackgroundPlayer = m_mainBackgroundPlayer;
		m_mainBackgroundPlayer = tmp;

		return m_mainBackgroundPlayer;
	}

	/// <summary>
	/// 全てのBGMを停止させる
	/// </summary>
	public void StopBGM()
	{
		m_mainBackgroundPlayer.Stop();
		m_subBackgroundPlayer.Stop();
	}

	/// <summary>
	/// BGMをポーズさせる
	/// </summary>
	public void PauseBGM()
	{
		m_mainBackgroundPlayer.Pause();
		m_subBackgroundPlayer.Pause();
	}

	/// <summary>
	/// ポーズ中のBGMを再開させる
	/// </summary>
	public void ResumeBGM()
	{
		m_mainBackgroundPlayer.Resume();
		m_subBackgroundPlayer.Resume();
	}


	void OnSceneUnloaded(Scene scene)
	{
		if (m_isChangeSceneToStopSe)
			StopSE();
	}


	/// <summary>
	/// 音量をロードする
	/// </summary>
	public void LoadVolume()
	{
		m_volumeTotal = PlayerPrefs.GetFloat(SaveKeyVolumeTotal, DefaultParamVolumeTotal);
		m_volumeBgm = PlayerPrefs.GetFloat(SaveKeyVolumeBgm, DefaultParamVolumeBgm);
		m_volumeSe = PlayerPrefs.GetFloat(SaveKeyVolumeSe, DefaultParamVolumeSe);
	}

	/// <summary>
	/// 音量を保存しておく
	/// </summary>
	public void SaveVolume()
	{
		PlayerPrefs.SetFloat(SaveKeyVolumeTotal, volumeTotal);
		PlayerPrefs.SetFloat(SaveKeyVolumeBgm, volumeBgm);
		PlayerPrefs.SetFloat(SaveKeyVolumeSe, volumeSe);
		PlayerPrefs.Save();
	}

	/// <summary>
	/// ハッシュ値(SE) 
	/// </summary>
	public enum HashParam_SE
	{
		/// <summary>
		/// 音量(0.0f-1.0f)(float)
		/// </summary>
		volume,

		/// <summary>
		/// 遅延時間(float)
		/// </summary>
		delay,

		/// <summary>
		/// ピッチ(float)
		/// </summary>
		pitch,

		/// <summary>
		/// 永続的にループさせるか(bool)
		/// </summary>
		isLoopInfinity,

		/// <summary>
		/// ループ回数(int)
		/// </summary>
		loopCount,

		/// <summary>
		/// フェードインの時間(float)
		/// </summary>
		fadeInTime,

		/// <summary>
		/// フェードアウトの時間(float)
		/// </summary>
		fadeOutTime,

		/// <summary>
		/// 3Dサウンドか(bool)
		/// </summary>
		is3dSound,

		/// <summary>
		/// サウンドを鳴らす座標(Vector3)
		/// </summary>
		soundPos,

		/// <summary>
		/// サウンドを追従させるゲームオブジェクト(GameObject)
		/// </summary>
		chaseObj,

		/// <summary>
		/// 音が完全に聞こえなくなる距離(float)
		/// </summary>
		minDistance,

		/// <summary>
		/// 音が減衰を停止する距離(float)
		/// </summary>
		maxDistance,

		/// <summary>
		/// 鳴り始める時に呼ばれるコールバック。一度しか呼ばれない(UnityAction)
		/// </summary>
		onStartBefore,

		/// <summary>
		/// 鳴り始めた時に呼ばれるコールバック。ループされるたびに呼ばれる(UnityAction)
		/// </summary>
		onStart,

		/// <summary>
		/// 鳴り終えた時に呼ばれるコールバック。ループされるたびに呼ばれる(UnityAction)
		/// </summary>
		onComplete,

		/// <summary>
		/// 完全に鳴り終えた時に呼ばれるコールバック。一度しか呼ばれない(UnityAction)
		/// </summary>
		onCompleteAfter,
	}

	/// <summary>
	/// ハッシュ値(BGM)
	/// </summary>
	public enum HashParam_BGM
	{
		/// <summary>
		/// イントロの曲名(SoundName,string)
		/// </summary>
		introSoundName,

		/// <summary>
		/// 音量(0.0f-1.0f)(float)
		/// </summary>
		volume,

		/// <summary>
		/// 遅延時間(float)
		/// </summary>
		delay,

		/// <summary>
		/// ループをするか(bool)
		/// </summary>
		isLoop,

		/// <summary>
		/// フェードインの時間(float)
		/// </summary>
		fadeInTime,

		/// <summary>
		/// フェードアウトの時間(float)
		/// </summary>
		fadeOutTime,

		/// <summary>
		/// フェードを重ねる割合(0.0f-1.0f)(float)
		/// </summary>
		crossFadeRate,

		/// <summary>
		/// イントロが開始されるタイミングで呼ばれるコールバック(UnityAction)
		/// </summary>
		onIntroStart,

		/// <summary>
		/// イントロが終わったタイミングで呼ばれるコールバック(UnityAction)
		/// </summary>
		onIntroComplete,

		/// <summary>
		/// メイン曲が開始されるタイミングで呼ばれるコールバック(UnityAction)
		/// </summary>
		onMainStart,

		/// <summary>
		/// メイン曲が終わったタイミングで呼ばれるコールバック(UnityAction)
		/// </summary>
		onMainComplete,
	}
}