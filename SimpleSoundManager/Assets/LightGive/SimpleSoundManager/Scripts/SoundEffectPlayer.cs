using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
	private GameObject m_chaseObj;
	private UnityAction m_callbackOnStart;
	private UnityAction m_callbackOnComplete;
	private float m_volume;
	private float m_delay;
	private float m_pitch;
	private int m_loopCount;

	public AudioSource source { get { return m_source; } }
	public GameObject chaseObj { get { return m_chaseObj; } set { m_chaseObj = value; } }
	public UnityAction callbackOnStart { get { return m_callbackOnStart; } set { m_callbackOnStart = value; } }
	public UnityAction callbackOnComplete { get { return m_callbackOnComplete; } set { m_callbackOnComplete = value; } }
	public float volume { get { return m_volume; } set { m_volume = value; } }
	public float delay { get { return m_delay; } set { m_delay = value; } }
	public float pitch { get { return m_pitch; } set { m_pitch = value; } }
	public int loopCount { get { return m_loopCount; } set { m_loopCount = value; } }


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

	public void Play(AudioClip _clip,float _volume, float _delay)
	{
		m_source.clip = _clip;
		m_source.Play((ulong)_delay);
	}
}


