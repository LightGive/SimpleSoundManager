using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveSoundIcon : MonoBehaviour
{
	#region Field
	[SerializeField]
	private float width = 0.0f;

	private float angle;
	private Vector3 firstPos;
	#endregion

	#region Properties
	#endregion

	#region MonoBehaviour Functions
	
	void Awake ()
	{
		
	}
	
	void Start ()
	{
		firstPos = transform.localPosition;
	}
	
	void Update ()
	{
		angle += Time.deltaTime;
		transform.localPosition = new Vector3(Mathf.Sin(angle) * width, firstPos.y, firstPos.z);
	}
	#endregion

	#region Methods
	#endregion
}
