using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (T)FindObjectOfType(typeof(T));
				if (instance == null)
				{
					Debug.LogError(typeof(T) + "is nothing");
				}
			}
			return instance;
		}
	}

	protected void Awake()
	{
		if(CheckInstance())
		{
			Init();
		}
	}

	protected virtual void Init() { }

	protected bool CheckInstance()
	{
		if (this == Instance) { return true; }
		Destroy(this.gameObject);
		return false;
	}
}