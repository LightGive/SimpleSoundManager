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
			if (onPointerEnterAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointerEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData _ped)
		{
			if (onPointerExitAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointerExitAudio, volume);
		}
		public void OnToggleChanged(bool _isToggle)
		{
			if (onValueChangedOffAudio == AudioNameSE.None & onValueChangedOnAudio == AudioNameSE.None)
				return;

			if (_isToggle)
			{
				if (onValueChangedOnAudio == AudioNameSE.None)
					return;
				SimpleSoundManager.Instance.PlaySE2D(onValueChangedOnAudio, volume);
			}
			else
			{
				if (onValueChangedOffAudio == AudioNameSE.None)
					return;
				SimpleSoundManager.Instance.PlaySE2D(onValueChangedOffAudio, volume);
			}
		}
	}
}