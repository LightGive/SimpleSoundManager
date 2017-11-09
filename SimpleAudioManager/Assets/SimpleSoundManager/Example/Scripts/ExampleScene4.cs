using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleScene4 : MonoBehaviour
{
	[SerializeField]
	private Dropdown bgmNameDropDown;
	[SerializeField]
	private InputField fadeOutTimeInputField;
	[SerializeField]
	private InputField fadeInTimeInputField;
	[SerializeField]
	private Slider volumeSlider;
	[SerializeField]
	private Slider crossFadeRateSlider;
	[SerializeField]
	private Text volumeText;
	[SerializeField]
	private Text crossFadeRateText;

	private string selectAudioName
	{
		get
		{
			var idx = bgmNameDropDown.value;
			var itemName = bgmNameDropDown.options[idx];
			return itemName.text;
		}
	}

	void Start ()
	{
		string[] enumNames = System.Enum.GetNames(typeof(AudioNameBGM));
		List<string> names = new List<string>(enumNames);
		bgmNameDropDown.ClearOptions();
		bgmNameDropDown.AddOptions(names);
	}
	
	void Update ()
	{
		
	}

	public void OnPlayButtonDown()
	{
		Debug.Log(selectAudioName);
		SimpleSoundManager.Instance.PlayBGM(selectAudioName);
	}

	public void OnPlayFadeButtonDown()
	{
		SimpleSoundManager.Instance.PlayCrossFadeBGM(
			selectAudioName,
			volumeSlider.value,
			true,
			float.Parse(fadeInTimeInputField.text),
			float.Parse(fadeOutTimeInputField.text),
			crossFadeRateSlider.value);
	}

	public void OnCrossFadeRateSliderValueChange(float _val)
	{
		crossFadeRateText.text = (_val * 100.0f).ToString("0") + "%";
	}
	public void OnVolumeSliderValueChange(float _val)
	{
		volumeText.text = (_val * 100.0f).ToString("0") + "%";
	}
}
