using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using LightGive;

/// <summary>
/// シンプルなサウンドを管理するマネージャー
/// </summary>
public class SimpleSoundManager : LightGive.SingletonMonoBehaviour<SimpleSoundManager>
{
	private const int DefaultSePlayerNum = 10;
	private const int DefaultBgmPlayerNum = 2;
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

	//セーブキー
	private const string SaveKeyVolumeTotal = "SaveKeyVolumeTotal";
	private const string SaveKeyVolumeBgm = "SaveKeyVolumeBgm";
	private const string SaveKeyVolumeSe = "SaveKeyVolumeSe";

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

	//実際に使用するプレイヤーのリスト
	public List<SoundEffectPlayer> sePlayerList = new List<SoundEffectPlayer>();
	//実際に使用するBGMのプレイヤーのリスト
	public List<BackGroundMusicPlayer> bgmPlayerList = new List<BackGroundMusicPlayer>();


	[SerializeField]
	private float bgmDefaultVolume = DefaultVolume;
	[SerializeField]
	private float seDefaultVolume = DefaultVolume;
	
	/// <summary>
	/// オーディオミキサー
	/// </summary>
	[SerializeField]
	public AudioMixerGroup bgmAudioMixerGroup;
	[SerializeField]
	public AudioMixerGroup seAudioMixerGroup;

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

	[SerializeField]
	public int bgmPlayerNum = DefaultSePlayerNum;
	
	/// <summary>
	/// Volumeを変更した時に保存するか
	/// </summary>
	[SerializeField]
	public bool volumeChangeToSave = false;
	
	private int bgmPlayerIndex = 0;
	
	public float TotalVolume
	{
		get { return totalVolume; }
		set
		{
			totalVolume = value;
			ChangeAllVolume();

			if (volumeChangeToSave)
				SaveVolume();
		}
	}

	public float SEVolume
	{
		get { return seVolume; }
		set
		{
			seVolume = value;
			ChangeAllVolume();

			if (volumeChangeToSave)
				SaveVolume();
		}
	}

	public float BGMVolume
	{
		get { return bgmVolume; }
		set
		{
			bgmVolume = value;
			ChangeAllVolume();

			if (volumeChangeToSave)
				SaveVolume();
		}
	}
	
	protected override void Awake()
	{
		base.Awake();

		bgmPlayerList.Clear();
		sePlayerList.Clear();

		for(int i= 0; i < bgmPlayerNum; i++)
		{
			GameObject bgmPlayerObj = new GameObject("BGMPlayerObj" + i.ToString());
			bgmPlayerObj.transform.SetParent(this.gameObject.transform);
			BackGroundMusicPlayer bgmPlayer = bgmPlayerObj.AddComponent<BackGroundMusicPlayer>();
			bgmPlayerList.Add(bgmPlayer);
			bgmPlayerObj.SetActive(false);
		}

		for (int i = 0; i < sePlayerNum; i++)
		{
			GameObject sePlayerObj = new GameObject("SEPlayerObj" + i.ToString());
			sePlayerObj.transform.SetParent(this.gameObject.transform);
			SoundEffectPlayer sePlayer = sePlayerObj.AddComponent<SoundEffectPlayer>();
			sePlayerList.Add(sePlayer);
			sePlayerObj.SetActive(false);
		}

		//Dictionaryを初期化
		bgmDictionary = new Dictionary<string, AudioClipInfo>();
		seDictionary = new Dictionary<string, AudioClipInfo>();
		for (int i = 0; i < bgmAudioClipList.Count; i++)
			bgmDictionary.Add(bgmAudioClipList[i].audioName, bgmAudioClipList[i]);
		for (int i = 0; i < seAudioClipList.Count; i++)
			seDictionary.Add(seAudioClipList[i].audioName, seAudioClipList[i]);

		LoadVolume();
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
		
		for (int i = 0; i < bgmPlayerList.Count; i++)
		{
			var player = bgmPlayerList[i];
			if (!player.IsPlaying)
				continue;
			player.PlayerUpdate();
		}
	}

	public void PlayBGM(AudioNameBGM _audioName)
	{
		PlayBGM(_audioName.ToString(), bgmVolume * totalVolume, true, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
	}
	

	/// <summary>
	/// BGMを再生する
	/// </summary>
	/// <param name="_audioName">SEの名前</param>
	public void PlayBGM(string _audioName)
	{
		PlayBGM(_audioName, bgmVolume * totalVolume, true, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
	}

	public void StopBGM()
	{
		for(int i = 0; i < bgmPlayerList.Count; i++)
		{
			var bgmPlayer = bgmPlayerList[i];
			bgmPlayer.Stop();
		}
	}


	/// <summary>
	/// クロスフェードしながらBGMを再生する
	/// </summary>
	/// <param name="_audioName">流すBGMの名前</param>
	/// <param name="_volume"></param>
	/// <param name="_isLoop"></param>
	/// <param name="_fadeTime"></param>
	/// <param name="_crossFadeRate"></param>
	public void PlayCrossFadeBGM(string _audioName, float _volume, bool _isLoop, float _fadeTime, float _crossFadeRate = 1.0f)
	{
		PlayBGM(_audioName, bgmVolume * totalVolume * _volume, _isLoop, _fadeTime, _fadeTime, _crossFadeRate, 0.0f, 0.0f);
	}

	public void PlayCrossFadeBGM(string _audioName, float _volume, bool _isLoop, float _fadeInTime, float _fadeOutTime, float _crossFadeRate)
	{
		PlayBGM(_audioName, bgmVolume * totalVolume * _volume, _isLoop, _fadeInTime, _fadeOutTime, _crossFadeRate, 0.0f, 0.0f);
	}

	private void PlayBGM(string _audioName, float _volume, bool _isLoop, float _fadeInTime, float _fadeOutTime, float _crossFadeRate, float _loopStartTime =0.0f, float _loopEndTime = 0.0f)
	{
		if (!bgmDictionary.ContainsKey(_audioName))
		{
			Debug.Log("その名前のBGMは見つかりませんでした。");
			return;
		}

		_volume = Mathf.Clamp01(_volume);
		_crossFadeRate = 1.0f - Mathf.Clamp01(_crossFadeRate);
		
		var clipInfo = bgmDictionary[_audioName];
		var player = GetBgmPlayer();
		var isFade = (_fadeInTime != 0.0f || _fadeOutTime != 0.0f);
		var isCheckLoopPoint = (_loopStartTime != 0.0f || _loopEndTime != 0.0f);

		if (isFade)
		{
			player.FadeIn(_fadeInTime, (_crossFadeRate * _fadeInTime));
			for (int i = 0; i < bgmPlayerList.Count; i++)
			{
				if (bgmPlayerList[i].IsPlaying)
					bgmPlayerList[i].FadeOut(_fadeOutTime);
			}
		}
		else
		{
			StopBGM();
		}

		player.Play(clipInfo.clip, _isLoop, isFade, isCheckLoopPoint, _volume, _loopStartTime, _loopEndTime);
	}

	public void PauseBGM()
	{
		for(int i = 0; i < bgmPlayerList.Count; i++)
		{
			bgmPlayerList[i].Pause();
		}
	}

	public bool IsPlayingBGM()
	{
		for(int i = 0; i < bgmPlayerList.Count; i++)
		{
			if (bgmPlayerList[i].IsPlaying)
				return true;
		}
		return false;
	}

	public void PlaySound2D(AudioNameSE _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		if (_audioName == AudioNameSE.None)
			return;
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}

	public void PlaySound2D(string _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, float _fadeInTime = DefaultSeFadeTime, float _fadeOutTime = DefaultSeFadeTime, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, 1, _fadeInTime, _fadeOutTime, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2DLoop(AudioNameSE _audioName, int _loopCount, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName.ToString(), _seVolume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2DLoop(string _audioName, int _loopCount, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch, UnityAction _onStart = null, UnityAction _onComplete = null)
	{
		PlaySE(_audioName, _seVolume, _delay, _pitch, false, _loopCount, 0.0f, 0.0f, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, _onStart, _onComplete);
	}
	public void PlaySound2DLoopInfinity(AudioNameSE _audioName, float _seVolume = DefaultVolume, float _delay = DefaultSeDelay, float _pitch = DefaultSePitch)
	{
		PlaySE(_audioName.ToString(), seVolume, DefaultSeDelay, DefaultSePitch, false, 1, 0.0f, 0.0f, false, Vector3.zero, null, DefaultMinDistance, DefaultMaxDistance, null, null);
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
		SoundEffectPlayer sePlayer = GetSePlayer();
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

		//sePlayer.AudioPlayCheckStart();
		sePlayer.Play();
	}
	
	/// <summary>
	/// SEがポーズしているかどうか、
	/// </summary>
	/// <param name="_audioName"></param>
	/// <returns></returns>
	public bool IsPauseSE(AudioNameSE _audioName)
	{
		return IsPauseSE(_audioName.ToString());
	}


	/// <summary>
	/// SEがポーズしているかどうか。
	/// </summary>
	/// <param name="_audioName"></param>
	/// <returns></returns>
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



	public bool IsPlayingSE(AudioNameSE _audioName)
	{
		return IsPlayingSE(_audioName.ToString());
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

	public void PauseSE(AudioNameSE _audioName)
	{

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
	/// 
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
			sePlayerList[i].ChangeVolume();
		}
	}

	private BackGroundMusicPlayer GetBgmPlayer()
	{
		for (int i = 0; i < bgmPlayerList.Count; i++)
		{
			if (bgmPlayerList[i].IsPlaying)
				continue;
			return bgmPlayerList[i];
		}

		Debug.Log("AudioSourceのリストの全てが再生中");
		return bgmPlayerList[0];
	}

	/// <summary>
	/// Playerを検索する
	/// </summary>
	/// <param name="_is3dSound">3Dサウンドかどうか</param>
	/// <returns></returns>
	private SoundEffectPlayer GetSePlayer()
	{
		for (int i = 0; i < sePlayerList.Count; i++)
		{
			if (sePlayerList[i].IsPlaying)
				continue;
			return sePlayerList[i];
		}

		Debug.Log("AudioSourceのリストの全てが再生中");
		return sePlayerList[0];
	}




	/// <summary>
	/// 音量を保存しておく。
	/// </summary>
	public void SaveVolume()
	{
		PlayerPrefs.SetFloat(SaveKeyVolumeTotal, TotalVolume);
		PlayerPrefs.SetFloat(SaveKeyVolumeBgm, BGMVolume);
		PlayerPrefs.SetFloat(SaveKeyVolumeSe, SEVolume);
	}

	/// <summary>
	/// Volumeをロードする
	/// </summary>
	public void LoadVolume()
	{
		totalVolume = PlayerPrefs.GetFloat(SaveKeyVolumeTotal, DefaultVolume);
		bgmVolume = PlayerPrefs.GetFloat(SaveKeyVolumeBgm, DefaultVolume);
		seVolume = PlayerPrefs.GetFloat(SaveKeyVolumeSe,DefaultVolume);
	}
}