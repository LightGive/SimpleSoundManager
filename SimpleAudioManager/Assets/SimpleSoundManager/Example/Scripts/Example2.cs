using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// ExampleScene1
/// </summary>
public class Example2 : MonoBehaviour
{
	[SerializeField]
	private Toggle isLoopToggle;
	[SerializeField]
	private Toggle isStartCallBack;
	[SerializeField]
	private Toggle isEndCallBack;
	[SerializeField]
	private InputField loopCountInput;
	[SerializeField]
	private InputField fadeInTimeInput;
	[SerializeField]
	private InputField fadeOutTimeInput;
	[SerializeField]
	private Image playButtonImage;
	[SerializeField]
	private Image pauseButtonImage;
	[SerializeField]
	private Text totalVolumeText;
	[SerializeField]
	private Text volumeText;
	[SerializeField]
	private Text delayText;
	[SerializeField]
	private Text pitchText;
	[SerializeField]
	private Dropdown seNameDropDown;

	private bool isPlayingSE;
	private float volume = 1.0f;
	private float delay = 0.0f;
	private float pitch = 1.0f;

	void Start()
	{
		Application.targetFrameRate = 60;
		string[] enumNames = System.Enum.GetNames(typeof(AudioNameSE));
		List<string> names = new List<string>(enumNames);
		seNameDropDown.AddOptions(names);
	}

	void Update()
	{
		if (!isPlayingSE)
			return;

		var idx = seNameDropDown.value;
		var itemName = seNameDropDown.options[idx];
		if (!SimpleSoundManager.Instance.IsPlayingSE(itemName.text))
		{
			isPlayingSE = false;
			playButtonImage.gameObject.SetActive(true);
			pauseButtonImage.gameObject.SetActive(false);
		}			
	}

	public void PlaySound2D()
	{
		isPlayingSE = true;
		var idx = seNameDropDown.value;
		var itemName = seNameDropDown.options[idx];
		var fadeInTime = float.Parse(fadeInTimeInput.text);
		var fadeOutTime = float.Parse(fadeOutTimeInput.text);
		UnityAction callBackStart = null;
		UnityAction callBackEnd = null;
		playButtonImage.gameObject.SetActive(false);
		pauseButtonImage.gameObject.SetActive(true);

		//ポーズ中
		if (SimpleSoundManager.Instance.IsPauseSE(itemName.text))
		{
			SimpleSoundManager.Instance.Resume(itemName.text);
			return;
		}

		if (isStartCallBack.isOn)
			callBackStart = DebugStartCallback;
		if (isEndCallBack.isOn)
			callBackEnd = DebugEndCallback;
		
		if (isLoopToggle.isOn)
		{
			if (loopCountInput.text == "")
				return;
			var loopCnt = int.Parse(loopCountInput.text);
			SimpleSoundManager.Instance.PlaySE2DLoop(itemName.text, loopCnt, volume, delay, pitch, callBackStart, callBackEnd);
		}
		else
		{
			SimpleSoundManager.Instance.PlaySE2D(itemName.text, volume, delay, pitch, fadeInTime, fadeOutTime, callBackStart, callBackEnd);
		}
	}


	public void PauseSound2D()
	{
		var idx = seNameDropDown.value;
		var itemName = seNameDropDown.options[idx];
		playButtonImage.gameObject.SetActive(true);
		pauseButtonImage.gameObject.SetActive(false);
		SimpleSoundManager.Instance.PauseSE(itemName.text);
	}

	public void DebugStartCallback()
	{
		//デバッグのコールバック
		Debug.Log("Start Call Back");
	}
	public void DebugEndCallback()
	{
		//デバッグのコールバック
		Debug.Log("End Call Back");
	}

	public void StopSE()
	{
		SimpleSoundManager.Instance.StopAllSE();
	}

	public void ChangeTotalVolume(float sliderValue)
	{
		SimpleSoundManager.Instance.TotalVolume = sliderValue;
		totalVolumeText.text = (sliderValue * 100).ToString("f1") + "%";
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
