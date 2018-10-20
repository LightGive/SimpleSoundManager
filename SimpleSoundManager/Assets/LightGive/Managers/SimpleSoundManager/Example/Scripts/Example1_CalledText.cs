using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example1_CalledText : MonoBehaviour
{
	[SerializeField]
	private Toggle m_toggleIsActive;
	[SerializeField]
	private float m_colorChangeTime = 0.5f;

	private Text m_text;
	private float m_timeCnt = 0.0f;
	private bool m_isActive = false;

	void Start ()
	{
		m_text = gameObject.GetComponent<Text>();
		m_text.color = Color.clear;
	}
	
	void Update ()
	{
		if (!m_isActive || !m_toggleIsActive.isOn)
			return;
		m_timeCnt += Time.deltaTime;
		var lerp = Mathf.Clamp01(m_timeCnt / m_colorChangeTime);
		m_text.color = Color.Lerp(Color.black, Color.clear, lerp);

		if (lerp >= 1.0f)
			m_isActive = false;
	}

	public void  Show()
	{
		if (!m_toggleIsActive.isOn)
			return;

		m_timeCnt = 0.0f;
		m_isActive = true;
	}
}
