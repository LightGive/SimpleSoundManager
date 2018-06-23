using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoundManager : SingletonMonoBehaviour<SimpleSoundManager>
{
	protected override void Awake()
	{
		base.Awake();

		Debug.Log("アイヤー");
	}
}
