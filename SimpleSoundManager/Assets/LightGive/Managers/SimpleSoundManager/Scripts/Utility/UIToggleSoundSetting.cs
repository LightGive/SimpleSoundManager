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
		private SoundNameSE onPointerEnterSound;
		[SerializeField]
		private SoundNameSE onPointerExitSound;
		[SerializeField]
		private SoundNameSE onValueChangedOnSound;
		[SerializeField]
		private SoundNameSE onValueChangedOffSound;

		private Toggle toggle;

		void Awake()
		{
			toggle = this.gameObject.GetComponent<Toggle>();
			toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		public void OnPointerEnter(PointerEventData _ped)
		{
			if (onPointerEnterSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerEnterSound, volume);
		}
		public void OnPointerExit(PointerEventData _ped)
		{
			if (onPointerExitSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerExitSound, volume);
		}
		public void OnToggleChanged(bool _isToggle)
		{
			if (onValueChangedOffSound == SoundNameSE.None & onValueChangedOnSound == SoundNameSE.None)
				return;

			if (_isToggle)
			{
				if (onValueChangedOnSound == SoundNameSE.None)
					return;
				SimpleSoundManager.Instance.PlaySE_2D(onValueChangedOnSound, volume);
			}
			else
			{
				if (onValueChangedOffSound == SoundNameSE.None)
					return;
				SimpleSoundManager.Instance.PlaySE_2D(onValueChangedOffSound, volume);
			}
		}
	}
}