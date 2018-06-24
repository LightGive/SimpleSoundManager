using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	[SerializeField]
	private List<SoundEffectPlayer> m_soundEffectPlayers = new List<SoundEffectPlayer>();
	[SerializeField]
	private List<AudioClip> m_audioClipList = new List<AudioClip>();
	[SerializeField]
	private int m_sePlayerNum = 10;

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
	}

	public void Play()
	{
		var player = GetSoundEffectPlayer();
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
