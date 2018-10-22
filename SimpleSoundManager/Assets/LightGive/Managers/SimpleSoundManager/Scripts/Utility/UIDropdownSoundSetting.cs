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
		private SoundNameSE onPointEnterSound;
		[SerializeField]
		private SoundNameSE onPointExitSound;
		[SerializeField]
		private SoundNameSE onPointClickSound;
		[SerializeField]
		private SoundNameSE onValueChangedSound;

		private Dropdown dropdown;

		void Awake()
		{
			dropdown = this.gameObject.GetComponent<Dropdown>();
			dropdown.onValueChanged.AddListener(OnValueChanged);
		}

		public void OnValueChanged(int _val)
		{
			if (onValueChangedSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onValueChangedSound, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointEnterSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointEnterSound, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointExitSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointExitSound, volume);
		}
		public void OnPointerClick(PointerEventData ped)
		{
			if (onPointClickSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointClickSound, volume);
		}
	}
}