using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Example3 : MonoBehaviour
{
	[SerializeField]
	private Example3_Ball m_ball;
	[SerializeField]
	private Transform m_anchorShot;


	public void OnButtonDownRandomShot()
	{
		m_ball.transform.position = m_anchorShot.position;
		m_ball.rigid.velocity = Vector3.zero;
		m_ball.rigid.AddForce(new Vector3(Random.Range(-2.0f, 2.0f), 8.0f, Random.Range(1.0f, 5.0f)), ForceMode.VelocityChange);
	}

	public void OnButtonDownNextScene()
	{
		var no = SceneManager.GetActiveScene().buildIndex;
		no++;
		if (no >= SceneManager.sceneCountInBuildSettings) { no = 0; }
		SceneManager.LoadScene(no);
	}
}