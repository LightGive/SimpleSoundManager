using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ExampleScene1
/// </summary>
public class ExampleScene1 : MonoBehaviour {

	[SerializeField]
	private Toggle isLoopToggle;
	[SerializeField]
	private InputField loopCountInput;
	[SerializeField]
	private Text volumeText;
	[SerializeField]
	private Text delayText;
	[SerializeField]
	private Text pitchText;
	[SerializeField]
	private Dropdown seNameDropDown;

	private float volume = 1.0f;
	private float delay = 0.0f;
	private float pitch = 1.0f;

	void Start()
	{
		string[] enumNames = System.Enum.GetNames(typeof(AudioNameSE));
		List<string> names = new List<string>(enumNames);
		seNameDropDown.AddOptions(names);
	}

	void Update()
	{

	}

	public void PlaySound2D()
	{
		var idx = seNameDropDown.value;
		var itemName = seNameDropDown.options[idx];

		if (isLoopToggle.isOn)
		{
			if (loopCountInput.text == "")
				return;

			var loopCnt = int.Parse(loopCountInput.text);
			AudioManager.Instance.PlaySound2DLoop(itemName.text, loopCnt, volume, delay, pitch);
		}
		else
		{
			AudioManager.Instance.PlaySound2D(itemName.text, volume, delay, pitch);
		}
	}

	public void PlaySound2DLoop()
	{
		AudioManager.Instance.PlaySound2DLoop(seNameDropDown.itemText.text, 5, volume, delay, pitch);
	}

	public void StopSE()
	{
		AudioManager.Instance.StopSeAll();
	}

	public void PlayBGM()
	{
		AudioManager.Instance.PlayBGM(AudioName.BGM_Main);
	}

	public void ChangeTotalVolume(float sliderValue)
	{
		AudioManager.Instance.TotalVolume = sliderValue;
	}

	public void ChangeVolume(float sliderValue)
	{
		volume = sliderValue;
		volumeText.text = (sliderValue * 100).ToString("f1") + "%";
	}

	public void ChangeDelay(float sliderValue)
	{
		delay = sliderValue;
		delayText.text = sliderValue.ToString("f2") + " sec";
	}

	public void ChangePitch(float sliderValue)
	{
		pitch = sliderValue;
		pitchText.text = sliderValue.ToString("f2");
	}
}
