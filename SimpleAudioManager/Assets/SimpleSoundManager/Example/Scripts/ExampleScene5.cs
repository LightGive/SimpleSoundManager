using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene5 : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem effect;

	void Start ()
	{
		
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = new RaycastHit();
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				effect.transform.position = hit.point;
				effect.Play();
				SimpleSoundManager.Instance.Play3DSound(AudioNameSE.BallBound, hit.point);
			}
		}
	}
}
