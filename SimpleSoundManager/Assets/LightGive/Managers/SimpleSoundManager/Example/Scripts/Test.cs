using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	private Coroutine m_coroutine;

	void Start()
	{

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			m_coroutine = StartCoroutine(_test());
		}

		if (Input.GetKeyDown(KeyCode.D))
		{
			Debug.Log(m_coroutine);
		}
	}

	private IEnumerator _test()
	{
		yield return new WaitForSeconds(5.0f);
	}
}
