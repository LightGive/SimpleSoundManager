using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LightGive
{
	[RequireComponent(typeof(Dropdown))]
	public class UIDropdownSoundSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField]
		private AudioNameSE onPointEnterAudio;
		[SerializeField]
		private AudioNameSE onPointExitAudio;
		[SerializeField]
		private AudioNameSE onPointClickAudio;
		[SerializeField]
		private AudioNameSE onValueChangedAudio;

		private Dropdown dropdown;

		void Awake()
		{
			dropdown = this.gameObject.GetComponent<Dropdown>();
			dropdown.onValueChanged.AddListener(OnValueChanged);
		}

		public void OnValueChanged(int _val)
		{
			if (onValueChangedAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onValueChangedAudio, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointEnterAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointExitAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointExitAudio, volume);
		}
		public void OnPointerClick(PointerEventData ped)
		{
			if (onPointClickAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointClickAudio, volume);
		}
	}
}