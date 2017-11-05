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
		private AudioNameSE onPointEnterAudio;
		[SerializeField]
		private AudioNameSE onPointExitAudio;
		[SerializeField]
		private AudioNameSE onPointerClickAudio;

		private Button button;

		void Awake()
		{
			button = this.gameObject.GetComponent<Button>();
			button.onClick.AddListener(() => SimpleSoundManager.Instance.PlaySound2D(onPointerClickAudio, volume));
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointEnterAudio, volume);
        }
		public void OnPointerExit(PointerEventData ped)
		{
			SimpleSoundManager.Instance.PlaySound2D(onPointExitAudio, volume);
		}
	}
}
