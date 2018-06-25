using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Example1 : MonoBehaviour
{
	[SerializeField]
	private Dropdown m_dropDownSeName;
	[SerializeField]
	private Dropdown m_dropDownBgmName;
	[SerializeField]
	private Slider m_sliderVolumeSe;
	[SerializeField]
	private Slider m_sliderDelaySe;
	[SerializeField]
	private Slider m_sliderPitchSe;
	[SerializeField]
	private ExampleSpectrum[] m_spectrum;
	[SerializeField]
	private int m_spectrumWidth = 100;

	private string selectSeName
	{
		get
		{
			var idx = m_dropDownSeName.value;
			var itemName = m_dropDownSeName.options[idx];
			return itemName.text;
		}
	}

	private void Start()
	{
		string[] enumNames = System.Enum.GetNames(typeof(SoundNameSE));
		List<string> names = new List<string>(enumNames);
		m_dropDownSeName.ClearOptions();
		m_dropDownSeName.AddOptions(names);
		for (int i = 0; i < m_spectrum.Length; i++)
		{
			m_spectrum[i].min = i * m_spectrumWidth;
			m_spectrum[i].maximam = (i * m_spectrumWidth) + m_spectrumWidth;
		}
	}

	public void OnButtonDownSceneReload()
	{
		SceneManager.LoadScene(0);
	}

	public void OnButtonDownPlay()
	{
		var player = SimpleSoundManager.Instance.PlaySE2D(
			selectSeName,
			m_sliderVolumeSe.value,
			m_sliderDelaySe.value,
			m_sliderPitchSe.value);

		if (player == null)
			return;

		for (int i = 0; i < m_spectrum.Length;i++)
		{
			m_spectrum[i].audioSource = player.source;
		}
	}
}