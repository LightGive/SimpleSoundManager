using System;
using UnityEngine;

/// <summary>
/// シングルトンクラス
/// GameControllerタグが付いているオブジェクトを参照する
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	/// <summary>
	/// タグの名前
	/// </summary>
	protected static readonly string MANAGER_TAG = "GameController";
	
	//インスタンス
	protected static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				Type type = typeof(T);
				GameObject[] objs = GameObject.FindGameObjectsWithTag(MANAGER_TAG);
				for (int j = 0; j < objs.Length; j++)
				{
					instance = (T)objs[j].GetComponent(type);
					if (instance != null)
						return instance;
				}

				Debug.LogWarning(string.Format("{0} がありません", type.Name));
			}
			return instance;
		}
	}

	/// <summary>
	/// 開始処理
	/// </summary>
	virtual protected void Awake()
	{
		CheckInstance();
	}

	/// <summary>
	/// インスタンスがあるかをチェックする
	/// </summary>
	/// <returns></returns>
	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (T)this;
			return true;
		}
		else if (Instance == this)
		{
			return true;
		}

		Destroy(this);
		return false;
	}
}