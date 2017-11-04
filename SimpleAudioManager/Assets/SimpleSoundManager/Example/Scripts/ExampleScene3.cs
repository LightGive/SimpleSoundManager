using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleScene3 : MonoBehaviour
{
	[SerializeField]
	private Text textPercent;


	public void ChangeSliderValue(float _value)
	{
		textPercent.text = (_value * 10.0f).ToString("f2") + "%";
	}
}
