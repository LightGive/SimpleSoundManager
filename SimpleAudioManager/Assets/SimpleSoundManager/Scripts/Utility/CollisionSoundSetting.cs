using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionSoundSetting : MonoBehaviour
{
	[SerializeField]
	private float velocityMag = 1.0f;
	[SerializeField]
	private AudioNameSE playAudioName;
	[SerializeField]
	private float minDistance;
	[SerializeField]
	private float maxDistance;

	void OnCollisionEnter(Collision _col)
	{
		if (_col.gameObject.GetComponent<Rigidbody>().velocity.magnitude < velocityMag)
			return;
		SimpleSoundManager.Instance.Play3DSound(playAudioName, _col.contacts[0].point);
	}
}
