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
	private AudioSource m_source;

	/// <summary>
	/// 0-1の間でどのくらい再生されているか
	/// </summary>
	/// <value>The length.</value>
	public float Length 
	{
		get 
		{
			if (state == SoundPlayState.Stop)
				return 0.0f;

			return Mathf.Clamp01(m_source.time / m_audioClip.length); 
		} 
	}
	/// <summary>
	/// 使用されているかどうか
	/// </summary>
	/// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
	public bool isActive { get { return (state == SoundPlayState.Stop); } }

	public void Init()
	{
		state = SoundPlayState.Stop;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.loop = false;
	}

	public void Play(AudioClip _clip)
	{

	}
}


