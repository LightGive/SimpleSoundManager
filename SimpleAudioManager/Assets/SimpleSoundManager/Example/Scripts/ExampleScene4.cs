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
		SimpleSoundManager.Instance.PlayBGM(selectAudioName);
	}

	public void OnPlayFadeButtonDown()
	{
		SimpleSoundManager.Instance.PlayCrossFadeBGM(
			selectAudioName,
			float.Parse(fadeInTimeInputField.text),
			float.Parse(fadeOutTimeInputField.text));
	}
}
