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

	public void PlaySE(string _audioName)
	{
		if (!m_audioClipDictSe.ContainsKey(_audioName))
		{
			Debug.Log("その名前のSEは見つかりませんでした。");
			return;
		}

		var player = GetSoundEffectPlayer();
		var clip = m_audioClipDictSe[_audioName];
		player.Play(clip);
	}



	private void PlaySE(
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
			return;
		}
		var clipInfo = m_audioClipDictSe[_audioName];
		var spatialBlend = (_is3dSound) ? 1.0f : 0.0f;

		SoundEffectPlayer player = null;

		player.audioSource.clip = clipInfo.clip;
		player.Pitch = _pitch;
		player.transform.position = _soundPos;
		player.audioSource.spatialBlend = spatialBlend;
		player.chaseObj = _chaseObj;
		player.LoopCount = _loopCount;
		player.Volume = _volume;
		player.Delay = _delay;
		player.callbackOnComplete = _onComplete;
		player.callbackOnStart = _onStart;
		player.IsFade = (_fadeInTime != 0.0f || _fadeOutTime != 0.0f);
		player.IsLoopInfinity = _isLoopInfinity;

		if (player.IsFade)
		{
			_fadeInTime = Mathf.Clamp(_fadeInTime, 0.0f, clipInfo.clip.length);
			_fadeOutTime = Mathf.Clamp(_fadeOutTime, 0.0f, clipInfo.clip.length);

			Keyframe key1 = new Keyframe(0.0f, 0.0f, 0.0f, 1.0f);
			Keyframe key2 = new Keyframe(_fadeInTime, 1.0f, 0.0f, 0.0f);
			Keyframe key3 = new Keyframe(clipInfo.clip.length - _fadeOutTime, 1.0f, 0.0f, 0.0f);
			Keyframe key4 = new Keyframe(clipInfo.clip.length, 0.0f, 0.0f, 1.0f);

			AnimationCurve animCurve = new AnimationCurve(key1, key2, key3, key4);
			player.animationCurve = animCurve;
		}

		if (_is3dSound)
		{
			player.audioSource.minDistance = _minDistance;
			player.audioSource.maxDistance = _maxDistance;
		}

		player.Play();
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
