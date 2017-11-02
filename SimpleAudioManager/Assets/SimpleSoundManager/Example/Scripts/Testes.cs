using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testes : MonoBehaviour {

	private IEnumerator coroutine;
	private bool isPause;

	void Start ()
	{
		coroutine = test();
		//StartCoroutine(coroutine);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (isPause)
			{
				StartCoroutine(coroutine);
			}
			else
			{
				Debug.Log("ポーズ");
				StopCoroutine(coroutine);
			}
			isPause = !isPause;
		}
	}

	
	private IEnumerator test()
	{
		Debug.Log("開始");
		float timeCnt = 0.0f;
		while(timeCnt < 5.0f)
		{
			timeCnt += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		Debug.Log("終わったよ");
	}

}
