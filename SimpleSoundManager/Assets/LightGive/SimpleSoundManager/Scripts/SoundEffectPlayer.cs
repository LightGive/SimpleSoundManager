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
		Pause,
		DelayWait
	}

	public SoundPlayState state;
	private AudioSource m_source;
	private GameObject m_chaseObj;
	private UnityAction m_callbackOnStart;
	private UnityAction m_callbackOnComplete;
	private AnimationCurve m_animationCurve;
	private float m_volume;
	private float m_delay;
	private float m_pitch;
	private int m_loopCount;
	private bool m_isFade;
	private bool m_isLoopInfinity;

	public AudioSource source { get { return m_source; } }
	public GameObject chaseObj { get { return m_chaseObj; } set { m_chaseObj = value; } }
	public UnityAction callbackOnStart { get { return m_callbackOnStart; } set { m_callbackOnStart = value; } }
	public UnityAction callbackOnComplete { get { return m_callbackOnComplete; } set { m_callbackOnComplete = value; } }
	public AnimationCurve animationCurve { get { return m_animationCurve; } set { m_animationCurve = value; } }
	public float volume { get { return m_volume; } set { m_volume = value; } }
	public float delay { get { return m_delay; } set { m_delay = value; } }
	public float pitch { get { return m_pitch; } set { m_pitch = value; } }
	public int loopCount { get { return m_loopCount; } set { m_loopCount = value; } }
	public bool isFade { get { return m_isFade; } set { m_isFade = value; } }
	public bool isLoopInfinity { get { return m_isLoopInfinity; } set { m_isLoopInfinity = value; } }


	/// <summary>
	/// 0-1の間でどのくらい再生されているか
	/// </summary>
	/// <value>The length.</value>
	public float Length 
	{
		get 
		{
			if (state == SoundPlayState.Stop || state == SoundPlayState.DelayWait)
				return 0.0f;

			return Mathf.Clamp01((source.time / source.pitch) / (source.clip.length / source.pitch)); 
		} 
	}

	/// <summary>
	/// 使用されているかどうか
	/// </summary>
	/// <value><c>true</c> if is active; otherwise, <c>false</c>.</value>
	public bool isActive { get { return (state == SoundPlayState.Playing || state == SoundPlayState.DelayWait); } }

	public void Init()
	{
		state = SoundPlayState.Stop;
		m_source = this.gameObject.AddComponent<AudioSource>();
		m_source.loop = false;
	}

	/// <summary>
	/// 再生する
	/// </summary>
	public void Play()
	{
		state = SoundPlayState.DelayWait;
		source.volume = volume;
		source.pitch = pitch;
		StartCoroutine(_Play());
	}

	public void Pause()
	{
		source.Pause();
	}

	private IEnumerator _Play()
	{
		yield return new WaitForSeconds(delay);

		if (callbackOnStart != null)
		{
			callbackOnStart.Invoke();
		}

		state = SoundPlayState.Playing;
		source.Play();
		yield return new WaitForSeconds(source.clip.length / source.pitch);

		if (callbackOnComplete != null)
		{
			callbackOnComplete.Invoke();
		}

		state = SoundPlayState.Stop;
	}
}


