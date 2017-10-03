using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ExampleScene1
/// </summary>
public class ExampleScene1 : MonoBehaviour {

	[SerializeField]
	private Text volumeText;
	[SerializeField]
	private Text delayText;
	[SerializeField]
	private Text pitchText;

	private float volume = 1.0f;
	private float delay = 0.0f;
	private float pitch = 1.0f;

	void Update()
	{

	}

	public void PlaySound2D()
	{
		AudioManager.Instance.PlaySound2D(AudioName.SE_Shutter, volume, delay, pitch);
	}

	public void PlaySound2DLoop()
	{
		AudioManager.Instance.PlaySound2DLoop(AudioName.SE_Shutter, 5, volume, delay, pitch);
	}

	public void PlayBGM()
	{
		AudioManager.Instance.PlayBGM(AudioName.BGM_Main);
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
