using System.Reflection;
using System;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AudioClipInfoAttribute))]
internal sealed class AudioClipInfoDrawer : PropertyDrawer
{
	/// <summary>
	/// 描画処理
	/// </summary>
	/// <param name="position">表示範囲</param>
	/// <param name="property">プロパティ</param>
	/// <param name="label">ラベル</param>
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		AudioClipInfoAttribute att = (AudioClipInfoAttribute)attribute;
		using (new EditorGUI.PropertyScope(position, label, property))
		{
			var halfWid = EditorGUIUtility.currentViewWidth / 2.0f;
			var quarterWid = EditorGUIUtility.currentViewWidth / 4.0f;
			var oneEighthWid = EditorGUIUtility.currentViewWidth / 8.0f;

			//オーディオの名前を表示する範囲
			var audioNoRect = new Rect(position)
			{
				width = quarterWid
			};
			//オーディオクリップを表示する範囲
			var audioClipRect = new Rect(position)
			{
				x = oneEighthWid + 15,
				width = quarterWid * 1.5f
			};
			//AudioClipの時間を表示する範囲
			var audioTimeRect = new Rect(position)
			{
				x = oneEighthWid + (quarterWid * 1.5f) + 15
			};

			//使うかのボタンを表示する範囲
			var audioEnableButtonRect = new Rect(position)
			{
				x = position.width - oneEighthWid - oneEighthWid,
				width = oneEighthWid
			};
			//Playボタンを表示する範囲
			var audioTestPlayRect = new Rect(position)
			{
				x = position.width - oneEighthWid,
				width = oneEighthWid
			};

			//プロパティ取得
			var audioNoProp = property.FindPropertyRelative("audioNo");
			var isUseProp = property.FindPropertyRelative("isUse");
			var clipProp = property.FindPropertyRelative("clip");


			if (clipProp.objectReferenceValue == null)
			{
				EditorGUI.LabelField(position, "Missing");
			}
			else
			{
				EditorGUI.LabelField(audioNoRect, audioNoProp.intValue.ToString("00") + ".");
				EditorGUI.ObjectField(audioClipRect, "", clipProp.objectReferenceValue, typeof(AudioClip), false);
				var t = ((AudioClip)clipProp.objectReferenceValue).length;
				EditorGUI.LabelField(audioTimeRect, Mathf.FloorToInt(t / 60.0f).ToString("00") + ":" + (t % 60).ToString("00"));

				if (GUI.Button(audioTestPlayRect, ""))
				{
					if ((AudioClip)clipProp.objectReferenceValue == null)
					{
						Debug.Log("AudioClipに追加して下さい");
						return;
					}

					StopAllClips();
					PlayClip((AudioClip)clipProp.objectReferenceValue);
				}
			}
		}
	}

	/// <summary>
	/// オーディオを再生する
	/// </summary>
	/// <param name="clip">再生するAudioClip</param>
	public static void PlayClip(AudioClip clip)
	{
		System.Reflection.Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
		method.Invoke(null, new object[] { clip });
	}

	/// <summary>
	/// 再生中のオーディオを止める
	/// </summary>
	public static void StopAllClips()
	{
		Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
		Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
		MethodInfo method = audioUtilClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { }, null);
		method.Invoke(null, new object[] { });
	}
}


