using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Example1 : MonoBehaviour
{
	public void OnButtonDownSceneReload()
	{
		SceneManager.LoadScene(0);
	}

	public void OnButtonDownPlay()
	{
		SimpleSoundManager.Instance.PlaySE()
	}
}
