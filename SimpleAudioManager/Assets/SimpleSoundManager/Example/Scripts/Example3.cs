using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example3 : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem effect;

	[SerializeField]
	private AudioNameSE audioName;

	void Start ()
	{
		
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
			{
				effect.transform.position = hit.point;
				effect.Play();
				SimpleSoundManager.Instance.PlaySE3D(audioName, hit.point);
			}
		}
	}
}
