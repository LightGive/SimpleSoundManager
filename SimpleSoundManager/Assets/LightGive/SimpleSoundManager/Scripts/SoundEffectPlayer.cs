using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
	public enum SoundPlayState
	{
		Stop,
		Playing,
		Pause
	}

	public SoundPlayState state;

	private AudioClip m_audioClip;


	public void Init()
	{
		state = SoundPlayState.Stop;
	}

	void Play()
	{

	}
}


