using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UISliderSoundSetting : MonoBehaviour
{
	[SerializeField]
	private float changeValue;
	
	private int preValue;
	private int splitCount;
	private float offset = 0.0f;

	private Slider slider;

	void Start()
	{
		slider = this.gameObject.GetComponent<Slider>();
		slider.onValueChanged.AddListener(ValueChange);
		offset = -slider.minValue;
		splitCount = Mathf.FloorToInt((slider.maxValue + offset) / changeValue);

		offset = (slider.maxValue - slider.minValue);
		if (changeValue > offset)
		{
			changeValue = offset;
		}
	}

	void Update()
	{

	}

	void ValueChange(float _value)
	{
		int index = Mathf.FloorToInt((_value + offset) / changeValue);
		if(index != preValue)
		{
			SimpleSoundManager.Instance.PlaySound2D(AudioNameSE.Bound);
			preValue = index;
		}

	}
}
