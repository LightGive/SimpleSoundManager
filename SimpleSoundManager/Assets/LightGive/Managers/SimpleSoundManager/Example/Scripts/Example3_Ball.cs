using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example3_Ball : MonoBehaviour
{
	[SerializeField]
	private Rigidbody m_rigid;
	[SerializeField]
	private float m_playSeMinVelocity;

	public Rigidbody rigid { get { return m_rigid; } }

	private void OnCollisionEnter(Collision _col)
	{
		if (rigid.velocity.magnitude < m_playSeMinVelocity)
			return;
		SimpleSoundManager.Instance.PlaySE_3D(SoundNameSE.BoundBall, 0.0f, 20.0f, transform.position);
	}

}