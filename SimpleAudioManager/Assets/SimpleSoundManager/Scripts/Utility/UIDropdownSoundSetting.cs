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
			SimpleSoundManager.Instance.PlaySound2D(onValueChangedAudio, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointExitAudio, volume);
		}
		public void OnPointerClick(PointerEventData ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointClickAudio, volume);
		}
	}
}