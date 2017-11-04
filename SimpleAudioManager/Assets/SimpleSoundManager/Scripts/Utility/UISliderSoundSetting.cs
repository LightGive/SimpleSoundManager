using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class UISliderSoundSetting : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField, Range(0.0f, 1.0f)]
	private float volume = 1.0f;
	[SerializeField]
	private float changeValue = 0.1f;
	[SerializeField]
	private AudioNameSE sliderStartAudioName;
	[SerializeField]
	private AudioNameSE sliderEndAudioName;
	[SerializeField]
	private AudioNameSE sliderChangeAudioName;

	private int preValue;
	private int splitCount;
	private float offset = 0.0f;

	private Slider slider;

	void Start()
	{
		slider = this.gameObject.GetComponent<Slider>();
		slider.onValueChanged.AddListener(ValueChange);
		offset = -slider.minValue;
		if (changeValue > (slider.maxValue-slider.minValue))
		{
			changeValue = offset;
		}

		splitCount = Mathf.FloorToInt((slider.maxValue + offset) / changeValue);
	}

	void ValueChange(float _value)
	{
		int index = Mathf.FloorToInt((_value + offset) / changeValue);
		if (index != preValue)
		{
			SimpleSoundManager.Instance.PlaySound2D(sliderChangeAudioName, volume);
			preValue = index;
		}
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		SimpleSoundManager.Instance.PlaySound2D(sliderStartAudioName, volume);
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		SimpleSoundManager.Instance.PlaySound2D(sliderEndAudioName, volume);
	}
}
