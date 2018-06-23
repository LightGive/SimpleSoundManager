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
		private AudioNameSE onPointerEnterAudio;
		[SerializeField]
		private AudioNameSE onPointerExitAudio;
		[SerializeField]
		private AudioNameSE onPointerClickAudio;

		private Button button;

		void Awake()
		{
			button = this.gameObject.GetComponent<Button>();
			button.onClick.AddListener(OnPointerClick);
		}
		public void OnPointerClick()
		{
			if (onPointerClickAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointerClickAudio, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointerEnterAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointerEnterAudio, volume);
        }
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointerExitAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE2D(onPointerExitAudio, volume);
		}
	}
}
