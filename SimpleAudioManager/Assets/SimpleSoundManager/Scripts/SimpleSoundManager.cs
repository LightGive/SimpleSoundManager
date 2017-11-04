using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using LightGive;

[System.Serializable]
public class SimpleSoundManager : LightGive.SingletonMonoBehaviour<SimpleSoundManager>
{
	private const int DefaultSePlayerNum = 10;
	private const float DefaultVolume = 1.0f;
	private const float DefaultSePitch = 1.0f;
	private const float DefaultSeDelay = 0.0f;
	private const float DefaultSeFadeTime = 0.0f;

	private const float PitchMin = 0.0f;
	private const float PitchMax = 3.0f;
	private const float DelayMin = 0.0f;
	private const float DelayMax = 10.0f;
	private const float DefaultMinDistance = 1.0f;
	private const float DefaultMaxDistance = 500.0f;

	private readonly Vector3 DefaultPos = Vector3.zero;

	/// <summary>
	/// 全てのBGM音声ファイルのリスト
	/// </summary>
	[AudioClipInfo, SerializeField]
	public List<AudioClipInfo> bgmAudioClipList = new List<AudioClipInfo>();
	/// <summary>
	/// 全てのSE音声ファイルのリスト
	/// </summary>
	[AudioClipInfo, SerializeField]
	public List<AudioClipInfo> seAudioClipList = new List<AudioClipInfo>();
	/// <summary>
	/// 使用するBGMのDictionary
	/// </summary>
	public Dictionary<string, AudioClipInfo> bgmDictionary;
	/// <summary>
	/// 使用するSEのDictionary
	/// </summary>
	public Dictionary<string, AudioClipInfo> seDictionary;

	public List<SoundEffectPlayer> sePlayerList = new List<SoundEffectPlayer>();
	/// <summary>
	/// オーディオミキサー
	/// </summary>
	[SerializeField]
	public AudioMixerGroup bgmAudioMixerGroup;
	[SerializeField]
	public AudioMixerGroup seAudioMixerGroup;


	public SoundEffectPlayer bgmPlayer;

	/// <summary>
	/// 全体の音量
	/// </summary>
	[SerializeField]
	private float totalVolume = DefaultVolume;
	/// <summary>
	/// BGMの音量
	/// </summary>
	[SerializeField]
	private float bgmVolume = DefaultVolume;
	/// <summary>
	/// SEの音量
	/// </summary>
	[SerializeField]
	private float seVolume = DefaultVolume;
	/// <summary>
	/// オーディオソースの数
	/// </summary>
	[SerializeField]
	public int sePlayerNum = DefaultSePlayerNum;


	public float TotalVolume
	{
		get { return totalVolume; }
		set
		{
			totalVolume = value;
			ChangeAllVolume();
		}
	}


	protected override void Awake()
	{
		base.Awake();

		//Dictionaryを初期化
		bgmDictionary = new Dictionary<string, AudioClipInfo>();
		seDictionary = new Dictionary<string, AudioClipInfo>();
		sePlayerList.Clear();

		//BGMのAudioPlayerの初期化
		GameObject bgmPlayerObj = new GameObject("BGMPlayerObj");
		bgmPlayerObj.transform.SetParent(this.gameObject.transform);
		bgmPlayerObj.SetActive(false);

		bgmPlayer = bgmPlayerObj.AddComponent<SoundEffectPlayer>();
		bgmPlayer.audioSource = bgmPlayerObj.AddComponent<AudioSource>();
		bgmPlayer.audioSource.playOnAwake = false;
		bgmPlayer.audioSource.loop = true;
		bgmPlayer.audioSource.outputAudioMixerGroup = bgmAudioMixerGroup;

		//AudioPlayerのGameObjectを作成
		for (int i = 0; i < sePlayerNum; i++)
		{
			GameObject sePlayerObj = new GameObject("SEPlayerObj" + i.ToString());
			sePlayerObj.transform.SetParent(this.gameObject.transform);
			sePlayerObj.SetActive(false);

			SoundEffectPlayer audioInfo = sePlayerObj.AddComponent<SoundEffectPlayer>();
			audioInfo.audioSource = sePlayerObj.AddComponent<AudioSource>();
			audioInfo.audioSource.playOnAwake = false;
			audioInfo.audioSource.loop = false;
			audioInfo.audioSource.outputAudioMixerGroup = seAudioMixerGroup;
			sePlayerList.Add(audioInfo);
		}


		for (int i = 0; i < bgmAudioClipList.Count; i++)
		{
			bgmDictionary.Add(bgmAudioClipList[i].audioName, bgmAudioClipList[i]);
		}
		for (int i = 0; i < seAudioClipList.Count; i++)
		{
			seDictionary.Add(seAudioClipList[i].audioName, seAudioClipList[i]);
		}
	}

	private void Update()
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			var player = sePlayerList[i];
			if (!player.IsPlaying)
				continue;

			player.PlayerUpdate();
		}
	}

	/// <summary>
	/// BGMを再生する
	/// </summary>
	/// <param name="_audioName">SEの名前</param>
	public void PlayBGM(string _audioName)
	{
		PlayBGM(_audioName, bgmVolume, true, 0.0f, 1.0f);
	}
	private void PlayBGM(string _audioName, float _volume, bool _isLoop, float _loopStartTime, float _loopEndTime)
	{
		if (!bgmDictionary.ContainsKey(_audioName))
		{
			Debug.Log("そんな名前のBGMはねーよ");
			return;
		}
		var clipInfo = bgmDictionary[_audioName];

		_volume = Mathf.Clamp01(_volume);
		bgmPlayer.audioSource.clip = clipInfo.clip;
		bgmPlayer.audioSource.volume = _volume;
		bgmPlayer.audioSource.spatialBlend = 0.0f;
		bgmPlayer.Play();
	}

	public void PlaySound2D(AudioNameSE _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2D(string _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2DLoop(string _audioName, int _loopCount, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2DLoopInfinity(string _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch)
	{
		PlaySE(_audioName, seVolume, DefaultSeDelay, DefaultSePitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, null, null);
	}

	public void Play3DSound(AudioNameSE _audioName, Vector3 _soundPos, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, DefaultMinDistance, DefaultMaxDistance, null, null);
	}
	public void Play3DSound(AudioNameSE _audioName, Vector3 _soundPos, float _minDistance = DefaultMinDistance, float _maxDistance = DefaultMaxDistance, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, null, null);
	}
	public void Play3DSound(AudioNameSE _audioName, GameObject _chaseObj, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _chaseObj.transform.position, _chaseObj, DefaultMinDistance, DefaultMaxDistance, null, null);
	}
	public void Play3DSound(string _audioName, Vector3 _soundPos, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, DefaultMinDistance, DefaultMaxDistance, null, null);
	}
	public void Play3DSound(string _audioName, Vector3 _soundPos, float _minDistance = DefaultMinDistance, float _maxDistance = DefaultMaxDistance, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _soundPos, null, _minDistance, _maxDistance, null, null);
	}
	public void Play3DSound(string _audioName, GameObject _chaseObj, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, true, _chaseObj.transform.position, _chaseObj, DefaultMinDistance, DefaultMaxDistance, null, null);
	}


	private void PlaySE(string _audioName, float _volume, float _delay, float _pitch, bool _isLoop, int _loopCount, float _fadeInTime, float _fadeOutTime, bool _is3dSound, Vector3 _soundPos, GameObject _chaseObj, float _minDistance, float _maxDistance, UnityAction _onStart, UnityAction _onComplete)
	{
		if (!seDictionary.ContainsKey(_audioName))
		{
			Debug.Log("その名前のSEは存在しません：" + _audioName);
			return;
		}
		var clipInfo = seDictionary[_audioName];
		var spatialBlend = (_is3dSound) ? 1.0f : 0.0f;

		//値に制限をかける
		_volume = Mathf.Clamp01(_volume);
		_delay = Mathf.Clamp(_delay, DelayMin, DelayMax);
		_pitch = Mathf.Clamp(_pitch, PitchMin, PitchMax);
		_fadeInTime = Mathf.Clamp(_fadeInTime, 0.0f, clipInfo.clip.length);
		_fadeOutTime = Mathf.Clamp(_fadeOutTime, 0.0f, clipInfo.clip.length);

		//オーディオプレイヤーを取得
		SoundEffectPlayer sePlayer = GetSoundPlayer(_is3dSound);
		sePlayer.audioSource.clip = clipInfo.clip;
		sePlayer.audioSource.pitch = _pitch;
		sePlayer.transform.position = _soundPos;
		sePlayer.audioSource.spatialBlend = spatialBlend;
		sePlayer.chaseObj = _chaseObj;
		sePlayer.volume = _volume;
		sePlayer.loopCnt = _loopCount;
		sePlayer.delay = _delay;
		sePlayer.isFade = (_fadeInTime != 0.0f || _fadeOutTime != 0.0f);
		sePlayer.callbackOnComplete = _onComplete;
		sePlayer.callbackOnStart = _onStart;

		if (sePlayer.isFade)
		{
			Keyframe key1 = new Keyframe(0.0f, 0.0f, 0.0f, 1.0f);
			Keyframe key2 = new Keyframe(_fadeInTime, 1.0f, 0.0f, 0.0f);
			Keyframe key3 = new Keyframe(clipInfo.clip.length - _fadeOutTime, 1.0f, 0.0f, 0.0f);
			Keyframe key4 = new Keyframe(clipInfo.clip.length, 0.0f, 0.0f, 1.0f);

			AnimationCurve animCurve = new AnimationCurve(key1, key2, key3, key4);
			sePlayer.animationCurve = animCurve;
		}


		if (_is3dSound)
		{
			sePlayer.audioSource.minDistance = _minDistance;
			sePlayer.audioSource.maxDistance = _maxDistance;
		}

		sePlayer.Play();
	}

	public bool IsPauseSE(string _audioName)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].audioSource.clip.name == _audioName && sePlayerList[i].IsPause)
				return true;
		}
		return false;
	}

	public bool IsPlayingSE()
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].IsPlaying )
				return true;
		}
		return false;
	}

	public bool IsPlayingSE(string _audioName)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].IsPlaying && sePlayerList[i].audioSource.clip.name == _audioName)
				return true;
		}
		return false;
	}

	/// <summary>
	/// SEをポーズさせる
	/// </summary>
	/// <param name="_audioName"></param>
	public void PauseSe(string _audioName)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].audioSource.clip.name == _audioName && !sePlayerList[i].IsPause)
				sePlayerList[i].Pause();
		}
	}


	/// <summary>
	/// SEをリスームさせる
	/// </summary>
	/// <param name="_audioName"></param>
	public void Resume(string _audioName)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].audioSource.clip.name == _audioName && sePlayerList[i].IsPause)
				sePlayerList[i].Resume();
		}
	}

	/// <summary>
	/// SEを停止させる
	/// </summary>
	/// <param name="_audioName"></param>
	public void StopSe(string _audioName)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			if (sePlayerList[i].audioSource.clip.name == _audioName)
				sePlayerList[i].Stop();
		}
	}

	/// <summary>
	/// 全てのSEを停止させる
	/// </summary>
	public void StopSeAll()
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			sePlayerList[i].Stop();
		}
	}

	/// <summary>
	/// 全ての音の大きさを変更する
	/// </summary>
	/// <param name="_val"></param>
	public void ChangeAllVolume()
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (!sePlayerList[i].IsActive)
				continue;
			sePlayerList[i].ChangeTotalVolume(totalVolume);
		}
	}

	/// <summary>
	/// Playerを検索する
	/// </summary>
	/// <param name="_is3dSound">3Dサウンドかどうか</param>
	/// <returns></returns>
	private SoundEffectPlayer GetSoundPlayer(bool _is3dSound)
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (sePlayerList[i].IsPlaying)
				continue;
			//Debug.Log(i.ToString() + "番のPlayerを使います");
			return sePlayerList[i];
		}

		Debug.Log("AudioSourceのリストの全てが再生中");
		return sePlayerList[0];
	}
}