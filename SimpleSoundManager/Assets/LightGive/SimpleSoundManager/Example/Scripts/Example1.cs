using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Example1 : MonoBehaviour
{
	[SerializeField]
	private Dropdown m_dropDownSeName;
	[SerializeField]
	private Dropdown m_dropDownBgmName;


	private string selectSeName
	{
		get
		{
			var idx = m_dropDownSeName.value;
			var itemName = m_dropDownSeName.options[idx];
			return itemName.text;
		}
	}

	private void Start()
	{
		string[] enumNames = System.Enum.GetNames(typeof(SoundNameSE));
		List<string> names = new List<string>(enumNames);
		m_dropDownSeName.ClearOptions();
		m_dropDownSeName.AddOptions(names);
	}

	public void OnButtonDownSceneReload()
	{
		SceneManager.LoadScene(0);
	}

	public void OnButtonDownPlay()
	{
		SimpleSoundManager.Instance.PlaySE(selectSeName);
	}
}