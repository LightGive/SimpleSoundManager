using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene5 : MonoBehaviour
{
	private ParticleSystem effect;

	void Start ()
	{
		
	}

	void Update()
	{
		var hit = new RaycastHit();
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
		{
			effect.transform.position = hit.point;
			effect.Play();
		}
	}
}
