using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Example3 : MonoBehaviour
{
	[Header("Ball")]
	[SerializeField]
	private Example3_Ball m_ball;
	[SerializeField]
	private Transform m_anchorShot;

	[Header("Helicopter")]
	[SerializeField]
	private Vector3 m_helicopterOffset;
	[SerializeField]
	private GameObject m_helicopterObj;
	[SerializeField]
	private GameObject m_propellerObj;
	[SerializeField]
	private float m_helicopterSpeed;
	[SerializeField]
	private float m_propellerSpeed;
	[SerializeField]
	private float m_radius;

	private float m_preRad;



	private void Start()
	{
		SimpleSoundManager.Instance.PlaySE_3D_LoopInfinity(SoundNameSE.Helicopter, 0.0f, 15.0f, m_propellerObj);
	}

	private void Update()
	{
		var rad = Mathf.Repeat(Time.time * m_helicopterSpeed, 360.0f) * Mathf.Deg2Rad;
		var pos = new Vector3(Mathf.Cos(rad) * m_radius, 0.0f, Mathf.Sin(rad) * m_radius) + m_helicopterOffset;
		//var prePos = new Vector3(Mathf.Cos(m_preRad) * m_radius, 0.0f, Mathf.Sin(m_preRad) * m_radius) + m_helicopterOffset;
		m_helicopterObj.transform.LookAt(pos);
		m_helicopterObj.transform.position = new Vector3(Mathf.Cos(rad) * m_radius, 0.0f, Mathf.Sin(rad) * m_radius) + m_helicopterOffset;
		m_propellerObj.transform.Rotate(new Vector3(0.0f, m_propellerSpeed, 0.0f));
		//m_preRad = rad;
	}

	void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
	{
		Debug.Log(prevScene.name + "->" + nextScene.name);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Debug.Log(scene.name + " scene loaded");
	}

	void OnSceneUnloaded(Scene scene)
	{
		Debug.Log(scene.name + " scene unloaded");
	}

	public void OnButtonDownRandomShot()
	{
		m_ball.transform.position = m_anchorShot.position;
		m_ball.rigid.velocity = Vector3.zero;
		m_ball.rigid.AddForce(new Vector3(Random.Range(-2.0f, 2.0f), 7.0f, 1.5f), ForceMode.VelocityChange);
	}

	public void OnButtonDownNextScene()
	{
		var no = SceneManager.GetActiveScene().buildIndex;
		no++;
		if (no >= SceneManager.sceneCountInBuildSettings) { no = 0; }
		SceneManager.LoadScene(no);
	}
}