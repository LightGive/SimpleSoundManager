using UnityEngine;

/// <summary>
/// オーディオクリップの情報用のクラス
/// </summary>
[System.Serializable]
public class AudioClipInfo
{
	/// <summary>
	/// この音ファイルを使うかどうか
	/// </summary>
	[SerializeField]
	public bool isUse = true;
	/// <summary>
	/// オーディオの番号
	/// </summary>
	[SerializeField]
	public int audioNo;
	/// <summary>
	/// オーディオクリップ
	/// </summary>
	[SerializeField]
	public AudioClip audioClip;

	/// <summary>
	/// オーディオの名前
	/// </summary>
	public string audioName
	{
		get { return audioClip.name; }
	}

	/// <summary>
	/// コンストラクタ
	/// オーディオクリップを新しく設定
	/// </summary>
	/// <param name="_audioClip">オーディオクリップ</param>
	public AudioClipInfo(AudioClip _audioClip)
	{
		audioNo = 0;
		audioClip = _audioClip;
	}

	/// <summary>
	/// コンストラクタ
	/// 番号とオーディオクリップを設定
	/// </summary>
	/// <param name="_audioNo">番号</param>
	/// <param name="_audioClip">オーディオクリップ</param>
	public AudioClipInfo(int _audioNo, AudioClip _audioClip)
	{
		audioNo = _audioNo;
		audioClip = _audioClip;
	}
}