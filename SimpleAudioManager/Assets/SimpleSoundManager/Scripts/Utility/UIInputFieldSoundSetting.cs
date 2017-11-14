using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LightGive
{
	[RequireComponent(typeof(InputField))]
	public class UIInputFieldSoundSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField, Range(0.0f, 1.0f)]
		private float volume = 1.0f;
		[SerializeField]
		private AudioNameSE onPointEnterAudio;
		[SerializeField]
		private AudioNameSE onPointExitAudio;
		[SerializeField]
		private AudioNameSE onPointerClickAudio;
		[SerializeField]
		private AudioNameSE onValueChangedAudio;
		[SerializeField]
		private AudioNameSE onEndEditAudio;

		private InputField inputField;

		void Awake()
		{
			inputField = this.gameObject.GetComponent<InputField>();
			inputField.onValueChanged.AddListener(OnValueChanged);
			inputField.onEndEdit.AddListener(OnEndEdit);
		}
		void OnValueChanged(string _val)
		{
			if (onValueChangedAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySound2D(onValueChangedAudio, volume);
		}
		void OnEndEdit(string _val)
		{
			if (onEndEditAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySound2D(onEndEditAudio, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointEnterAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySound2D(onPointEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointExitAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySound2D(onPointExitAudio, volume);
		}
		public void OnPointerClick(PointerEventData ped)
		{
			if (onPointerClickAudio == AudioNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySound2D(onPointerClickAudio, volume);
		}
	}
}