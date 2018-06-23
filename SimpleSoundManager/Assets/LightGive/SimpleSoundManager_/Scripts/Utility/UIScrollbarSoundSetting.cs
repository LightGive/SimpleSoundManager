using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LightGive
{
	[RequireComponent(typeof(Scrollbar))]
	public class UIScrollbarSoundSetting : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField, Range(0.0f, 1.0f)]
		private float changeValue = 0.1f;
		[SerializeField]
		private AudioNameSE scrollStartAudio;
		[SerializeField]
		private AudioNameSE scrollEndAudio;
		[SerializeField]
		private AudioNameSE onValueChangedAudio;

		private Scrollbar scrollbar;
		private float offset = 0.0f;
		private int preValue;
		private int splitCount;

		void Start()
		{
			scrollbar = this.gameObject.GetComponent<Scrollbar>();
			scrollbar.onValueChanged.AddListener(OnValueChange);
			splitCount = Mathf.FloorToInt(1.0f / changeValue);

			if (scrollbar.numberOfSteps != 0)
				splitCount = scrollbar.numberOfSteps;
		}

		void OnValueChange(float _value)
		{
			if (onValueChangedAudio == AudioNameSE.None)
				return;
			int index = Mathf.FloorToInt(_value / changeValue);
			if (index != preValue)
			{
				SimpleSoundManager.Instance.PlaySE2D(onValueChangedAudio, volume);
				preValue = index;
			}
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (scrollStartAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(scrollStartAudio, volume);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			if (scrollEndAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(scrollEndAudio, volume);
		}
	}
}