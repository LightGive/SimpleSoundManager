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
		private SoundNameSE scrollStartSound;
		[SerializeField]
		private SoundNameSE scrollEndSound;
		[SerializeField]
		private SoundNameSE onValueChangedSound;

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
			if (onValueChangedSound == SoundNameSE.None)
				return;
			int index = Mathf.FloorToInt(_value / changeValue);
			if (index != preValue)
			{
				SimpleSoundManager.Instance.PlaySE_2D(onValueChangedSound, volume);
				preValue = index;
			}
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			if (scrollStartSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(scrollStartSound, volume);
		}
		public void OnPointerUp(PointerEventData eventData)
		{
			if (scrollEndSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(scrollEndSound, volume);
		}
	}
}