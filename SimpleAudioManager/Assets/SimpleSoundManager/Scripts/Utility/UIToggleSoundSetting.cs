using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
		private AudioNameSE cursorEnterAudioName;
		[SerializeField]
		private AudioNameSE cursorExitAudioName;
		[SerializeField]
		private AudioNameSE checkOnAudioName;
		[SerializeField]
		private AudioNameSE checkOffAudioName;

		private UnityEvent eventPointerEnter = new UnityEvent();
		private UnityEvent eventPointerExit = new UnityEvent();
		private UnityEvent eventPointerDown = new UnityEvent();
		private Toggle toggle;

		void Awake()
		{
			toggle = this.gameObject.GetComponent<Toggle>();

			if (cursorEnterAudioName != AudioNameSE.None)
				eventPointerEnter.AddListener(() => SimpleSoundManager.Instance.PlaySound2D(cursorEnterAudioName));
			if (cursorExitAudioName != AudioNameSE.None)
				eventPointerExit.AddListener(() => SimpleSoundManager.Instance.PlaySound2D(cursorExitAudioName));
			if (checkOffAudioName != AudioNameSE.None && checkOnAudioName != AudioNameSE.None)
				toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		public void OnPointerEnter(PointerEventData _ped)
		{
			if (eventPointerEnter != null)
				eventPointerEnter.Invoke();
		}
		public void OnPointerExit(PointerEventData _ped)
		{
			if (eventPointerExit != null)
				eventPointerExit.Invoke();
		}
		public void OnToggleChanged(bool _isToggle)
		{
			if (_isToggle)
			{
				SimpleSoundManager.Instance.PlaySound2D(checkOnAudioName, volume);
			}
			else
			{
				SimpleSoundManager.Instance.PlaySound2D(checkOffAudioName, volume);
			}
		}
	}
}