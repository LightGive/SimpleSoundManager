using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	/// <summary>
	/// TestMethodを呼び出すまでの時間
	/// </summary>
	[SerializeField]
	private float callTime = 5.0f;

	/// <summary>
	/// ポーズしているかどうか
	/// </summary>
	private bool isPause = false;

	void Start ()
	{
		Invoke("TestMethod", callTime);	
	}
	
	void Update ()
	{
		if (!Input.GetKeyDown(KeyCode.Space))
			return;

		if (isPause)
		{
			//Resume処理
		}
		else
		{
			//Pause処理
		}
	}

	void TestMethod()
	{
		Debug.Log("呼ばれました");
	}
}
