using UnityEngine;

[System.Serializable]
public class AudioClipInfoAttribute : PropertyAttribute
{
	[SerializeField]
	public bool isRoute;

	public string clip = string.Empty;
	public string loop = string.Empty;

	public AudioClipInfoAttribute()
	{

	}
}
