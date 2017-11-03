using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CollisionSoundSetting : MonoBehaviour
{
	[Header("AudioSetting")]
	[SerializeField]
	private AudioNameSE playAudioName;
	[Header("VolumeSetting")]
	[SerializeField, Range(0.0f, 1.0f)]
	private float volume = 1.0f;
	[SerializeField]
	private bool changeVolumeMag = false;
	[SerializeField]
	private float minVelocityMag = 1.0f;
	[SerializeField]
	private float maxVelocityMag = 10.0f;
	[SerializeField]
	private float minDistance = 1.0f;
	[SerializeField]
	private float maxDistance = 10.0f;
	[Header("ignoreSetting")]
	[SerializeField]
	private string[] ignoreTagName;
	[SerializeField]
	private LayerMask ignoreLayer;

	void Start()
	{

	}

	void OnCollisionEnter(Collision _col)
	{
		var vec = _col.gameObject.GetComponent<Rigidbody>().velocity;
        if (vec.magnitude < minVelocityMag)
		{
			return;
		}

		//CheckIgnoreTag
		for(int i= 0; i < ignoreTagName.Length; i++)
		{
			if (_col.gameObject.tag == ignoreTagName[i] || _col.gameObject.layer != ignoreLayer)
				return;
		}

		var vol = volume;
		if (changeVolumeMag)
		{
			vol = Mathf.Clamp01(vec.magnitude / (minVelocityMag + maxVelocityMag)) * vol;
        }
		SimpleSoundManager.Instance.Play3DSound(playAudioName, _col.contacts[0].point, vol);
	}
}