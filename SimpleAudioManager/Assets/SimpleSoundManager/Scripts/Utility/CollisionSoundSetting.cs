using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CollisionSoundSetting : MonoBehaviour
{
	[SerializeField]
	private AudioNameSE playAudioName;
	[SerializeField, Range(0.0f, 1.0f)]
	private float volume = 1.0f;
	[SerializeField]
	private bool changeVolumeMag = false;
	[SerializeField]
	private float velocityMag = 1.0f;
	[SerializeField]
	private float minDistance = 1.0f;
	[SerializeField]
	private float maxDistance = 10.0f;
	[SerializeField]
	private string[] ignoreTagName;

	void OnCollisionEnter(Collision _col)
	{
		if (_col.gameObject.GetComponent<Rigidbody>().velocity.magnitude < velocityMag)
		{
			return;
		}

		//CheckIgnoreTag
		for(int i= 0; i < ignoreTagName.Length; i++)
		{
			if (_col.gameObject.tag == ignoreTagName[i])
				return;
		}
		SimpleSoundManager.Instance.Play3DSound(playAudioName, _col.contacts[0].point);
	}
}
