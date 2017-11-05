using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LightGive
{
	[RequireComponent(typeof(Slider))]
	public class UISliderSoundSetting : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField]
		private float changeValue = 0.1f;
		[SerializeField]
		private AudioNameSE sliderStartAudio;
		[SerializeField]
		private AudioNameSE sliderEndAudio;
		[SerializeField]
		private AudioNameSE onValueChangedAudio;

		private int preValue;
		private int splitCount;
		private float offset = 0.0f;

		private Slider slider;

		void Start()
		{
			slider = this.gameObject.GetComponent<Slider>();
			slider.onValueChanged.AddListener(ValueChange);
			offset = -slider.minValue;
			if (changeValue > (slider.maxValue - slider.minValue))
			{
				changeValue = offset;
			}

			splitCount = Mathf.FloorToInt((slider.maxValue + offset) / changeValue);
		}

		void ValueChange(float _value)
		{
			if (changeValue == 0.0f)
				return;

			int index = Mathf.FloorToInt((_value + offset) / changeValue);
			if (index != preValue)
			{
				SimpleSoundManager.Instance.PlaySound2D(onValueChangedAudio, volume);
				preValue = index;
			}
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			SimpleSoundManager.Instance.PlaySound2D(sliderStartAudio, volume);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			SimpleSoundManager.Instance.PlaySound2D(sliderEndAudio, volume);
		}
	}
}
