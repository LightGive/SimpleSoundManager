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
		[SerializeField]
		private AudioNameSE m_EnterAudioName;
		[SerializeField]
		private AudioNameSE m_ExitAudioName;
		[SerializeField]
		private AudioNameSE m_ClickOnAudioName;
		[SerializeField]
		private AudioNameSE m_ClickOffAudioName;

		private UnityEvent eventPointerEnter = new UnityEvent();
		private UnityEvent eventPointerExit = new UnityEvent();
		private UnityEvent eventPointerDown = new UnityEvent();
		private Toggle toggle;

		void Awake()
		{
			toggle = this.gameObject.GetComponent<Toggle>();

			if (m_EnterAudioName != AudioNameSE.None)
				eventPointerEnter.AddListener(() => SimpleSoundManager.Instance.PlaySound2D(m_EnterAudioName));
			if (m_ExitAudioName != AudioNameSE.None)
				eventPointerExit.AddListener(() => SimpleSoundManager.Instance.PlaySound2D(m_ExitAudioName));
			if (m_ClickOffAudioName != AudioNameSE.None && m_ClickOnAudioName != AudioNameSE.None)
				toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		public void OnPointerEnter(PointerEventData ped)
		{
			if (eventPointerEnter != null)
				eventPointerEnter.Invoke();
		}
		public void OnPointerExit(PointerEventData ped)
		{
			if (eventPointerExit != null)
				eventPointerExit.Invoke();
		}
		public void OnToggleChanged(bool _val)
		{
			if (_val)
				SimpleSoundManager.Instance.PlaySound2D(m_ClickOnAudioName);
			else
				SimpleSoundManager.Instance.PlaySound2D(m_ClickOffAudioName);
		}
	}
}