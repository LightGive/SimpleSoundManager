using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LightGive
{
	[RequireComponent(typeof(Toggle))]
	public class UIToggleSoundSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField]
		private AudioNameSE onPointerEnterAudio;
		[SerializeField]
		private AudioNameSE onPointerExitAudio;
		[SerializeField]
		private AudioNameSE onValueChangedOnAudio;
		[SerializeField]
		private AudioNameSE onValueChangedOffAudio;

		private Toggle toggle;

		void Awake()
		{
			toggle = this.gameObject.GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		public void OnPointerEnter(PointerEventData _ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointerEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData _ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointerExitAudio, volume);
		}
		public void OnToggleChanged(bool _isToggle)
		{
			if (_isToggle)
			{
				SimpleSoundManager.Instance.PlaySound2D(onValueChangedOnAudio, volume);
			}
			else
			{
				SimpleSoundManager.Instance.PlaySound2D(onValueChangedOffAudio, volume);
			}
		}
	}
}