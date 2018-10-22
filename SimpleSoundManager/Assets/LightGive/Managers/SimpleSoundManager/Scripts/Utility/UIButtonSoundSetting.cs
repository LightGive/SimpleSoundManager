using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LightGive
{
	[RequireComponent(typeof(Button))]
	public class UIButtonSoundSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField]
		private SoundNameSE onPointerEnterSound;
		[SerializeField]
		private SoundNameSE onPointerExitSound;
		[SerializeField]
		private SoundNameSE onPointerClickSound;

		private Button button;

		void Awake()
		{
			button = this.gameObject.GetComponent<Button>();
			button.onClick.AddListener(OnPointerClick);
		}
		public void OnPointerClick()
		{
			if (onPointerClickSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerClickSound, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointerEnterSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerEnterSound, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointerExitSound == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerExitSound, volume);
		}
	}
}
