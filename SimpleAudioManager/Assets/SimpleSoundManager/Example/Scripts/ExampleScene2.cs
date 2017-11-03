using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene2 : MonoBehaviour
{
	[SerializeField]
	private Transform cameraAnchor;
	[SerializeField]
	private float width = 5.0f;
	[SerializeField]
	private GameObject chaseObject;

	public void RandomPlaySound()
	{
		var randomPos = cameraAnchor.position;
		randomPos.x += Random.Range(-width, width) ;
		SimpleSoundManager.Instance.Play3DSound(AudioNameSE.DoorKnock, randomPos);
	}

	public void ChasePlaySound()
	{
		SimpleSoundManager.Instance.Play3DSound(AudioNameSE.bird, chaseObject);
	}
}