using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene2_MoveObj : MonoBehaviour
{
	[SerializeField]
	private float distance;
	private float angle;
	private Vector3 anchorPos;

	void Start ()
	{
		anchorPos = transform.position;
	}
	
	void Update ()
	{
		angle += Time.deltaTime;
		transform.position = new Vector3(Mathf.Sin(angle) * distance, 0.0f, 0.0f) + anchorPos;
	}
}
