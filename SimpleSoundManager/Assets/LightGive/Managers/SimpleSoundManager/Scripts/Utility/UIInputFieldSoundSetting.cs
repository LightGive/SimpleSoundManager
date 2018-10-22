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
		private SoundNameSE onPointEnterAudio;
		[SerializeField]
		private SoundNameSE onPointExitAudio;
		[SerializeField]
		private SoundNameSE onPointerClickAudio;
		[SerializeField]
		private SoundNameSE onValueChangedAudio;
		[SerializeField]
		private SoundNameSE onEndEditAudio;

		private InputField inputField;

		void Awake()
		{
			inputField = this.gameObject.GetComponent<InputField>();
			inputField.onValueChanged.AddListener(OnValueChanged);
			inputField.onEndEdit.AddListener(OnEndEdit);
		}
		void OnValueChanged(string _val)
		{
			if (onValueChangedAudio == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onValueChangedAudio, volume);
		}
		void OnEndEdit(string _val)
		{
			if (onEndEditAudio == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onEndEditAudio, volume);
		}
		public void OnPointerEnter(PointerEventData ped)
		{
			if (onPointEnterAudio == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointEnterAudio, volume);
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (onPointExitAudio == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointExitAudio, volume);
		}
		public void OnPointerClick(PointerEventData ped)
		{
			if (onPointerClickAudio == SoundNameSE.None)
				return;
			SimpleSoundManager.Instance.PlaySE_2D(onPointerClickAudio, volume);
		}
	}
}