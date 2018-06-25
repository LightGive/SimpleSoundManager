using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Principal;

public class ExampleSpectrum : MonoBehaviour
{
	public AudioSource audioSource;
	public int min;
	public int maximam;

	[SerializeField]
	private float m_scale = 200;
	[SerializeField]
	private float m_maxHeight = 300;

	private RectTransform m_rectTransfom;
	private float[] m_spectrum = new float[1024];

	void Start()
	{
		m_rectTransfom = this.gameObject.GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (audioSource == null)
		{
			m_rectTransfom.sizeDelta = new Vector2(20, 0);
			return;
		}


		audioSource.GetSpectrumData(m_spectrum, 0, FFTWindow.Blackman);
		float max = 0;
		for (int i = min; i < maximam; i++)
		{
			if (m_spectrum[i] >= max)
			{
				max = m_spectrum[i];
			}
		}
		float y = Mathf.Clamp(max * m_scale, 0.0f, m_maxHeight);

		m_rectTransfom.sizeDelta = new Vector2(20, y);
	}
}
