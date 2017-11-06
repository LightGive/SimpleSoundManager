using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleScene4 : MonoBehaviour
{
	[SerializeField]
	private Dropdown bgmNameDropDown;

	void Start ()
	{
		string[] enumNames = System.Enum.GetNames(typeof(AudioNameBGM));
		List<string> names = new List<string>(enumNames);
		bgmNameDropDown.AddOptions(names);
	}
	
	void Update ()
	{
		
	}
}
