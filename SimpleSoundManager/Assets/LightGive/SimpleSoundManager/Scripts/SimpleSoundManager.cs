using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	[SerializeField]
	private List<SoundEffectPlayer> m_soundEffectPlayers = new List<SoundEffectPlayer>();
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
			m_soundEffectPlayers.Add(player);
		}
	}
}
