using UnityEngine;
using UnityEngine.UI;

namespace LightGive
{
	public class Example4 : MonoBehaviour
	{
		[SerializeField]
		private Text textPercent;

		public void ChangeSliderValue(float _value)
		{
			textPercent.text = (_value * 10.0f).ToString("f2") + "%";
		}
	}
}
