using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SoundPlayer : MonoBehaviour
{
	public enum PlayType
	{
		Single,
		Loop,
		Random
	}

	[SerializeField]
	private float volume = 1.0f;
	[SerializeField]
	private float pitch = 1.0f;
	[SerializeField]
	private float delay;

	[SerializeField]
	private float randomWaitTimeMin = 1.0f;
	[SerializeField]
	private float randomWaitTimeMax = 5.0f;

	[SerializeField]
	private int loopCount;
	[SerializeField]
	private bool is3dSound;
	[SerializeField]
	private bool isAwakePlay;
	[SerializeField]
	private bool isLoopInfinity = false;
	[SerializeField]
	private UnityEvent startCallbackAct;
	[SerializeField]
	private UnityEvent endCallbackAct;
	[SerializeField]
	private PlayType playType;
	[SerializeField]
	private AudioNameSE audioName;


	public bool IsPlaying
	{
		get
		{
			return true;
		}
	}

	void Start ()
	{
		if (isAwakePlay)
			Play();
	}

	public void Play()
	{
		if(playType == PlayType.Single)
		{
			loopCount = 1;
		}

		if (is3dSound)
		{
			if (isLoopInfinity)
				SimpleSoundManager.Instance.PlaySE3DLoopInfinity(audioName, this.gameObject, volume, delay, pitch, 0.0f, 0.0f, () => startCallbackAct.Invoke(), () => startCallbackAct.Invoke());
			else
				SimpleSoundManager.Instance.PlaySE3DLoop(audioName, this.gameObject, loopCount, volume, delay, pitch, 0.0f, 0.0f, () => startCallbackAct.Invoke(), () => startCallbackAct.Invoke());
			
		}
		else
		{
			if (isLoopInfinity)
				SimpleSoundManager.Instance.PlaySE2DLoopInfinity(audioName, volume, delay, pitch, 0.0f, 0.0f, () => startCallbackAct.Invoke(), () => startCallbackAct.Invoke());
			else
				SimpleSoundManager.Instance.PlaySE2DLoop(audioName, loopCount,volume, delay, pitch, 0.0f, 0.0f, () => startCallbackAct.Invoke(), () => startCallbackAct.Invoke());



		}
	}
}

#if UNITY_EDITOR

[CustomEditor(typeof(SoundPlayer))]
public class SoundPlayerEditor : Editor
{
	private SerializedObject serializedObj;
	private SerializedProperty volumeProp;
	private SerializedProperty pitchProp;
	private SerializedProperty delayProp;
	private SerializedProperty loopCountProp;
	private SerializedProperty is3dSoundProp;
	private SerializedProperty isAwakePlayProp;
	private SerializedProperty startCallbackActProp;
	private SerializedProperty endCallbackActProp;
	private SerializedProperty playTypeProp;
	private SerializedProperty randomWaitTimeMinProp;
	private SerializedProperty randomWaitTimeMaxProp;
	private SerializedProperty isLoopInfinityProp;
	private SerializedProperty audioNameProp;

	public override void OnInspectorGUI()
	{
		serializedObj.Update();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("SoundSetting", EditorStyles.boldLabel);
		audioNameProp.enumValueIndex = (int)((AudioNameSE)EditorGUILayout.EnumPopup("AudioName", (AudioNameSE)audioNameProp.enumValueIndex));
		playTypeProp.enumValueIndex = (int)((SoundPlayer.PlayType)EditorGUILayout.EnumPopup("PlayType", (SoundPlayer.PlayType)playTypeProp.enumValueIndex));
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Sound Type");
		
		is3dSoundProp.boolValue = (GUILayout.Toolbar((is3dSoundProp.boolValue) ? 1 : 0, new string[] { "2D", "3D" })) == 1;
		EditorGUILayout.Slider(volumeProp, 0.0f, 1.0f,"Volume");
		EditorGUILayout.Slider(pitchProp, 0.0f, 3.0f, "Pitch");
		delayProp.floatValue = EditorGUILayout.FloatField("Delay", delayProp.floatValue);
		isAwakePlayProp.boolValue = EditorGUILayout.Toggle("PlayAwake", isAwakePlayProp.boolValue);
		if (playTypeProp.enumValueIndex != (int)SoundPlayer.PlayType.Single)
		{
			isLoopInfinityProp.boolValue = EditorGUILayout.Toggle("LoopInfinity", isLoopInfinityProp.boolValue);
			if (!isLoopInfinityProp.boolValue)
				loopCountProp.intValue = Mathf.Clamp(EditorGUILayout.IntField("LoopCount", loopCountProp.intValue), 1, int.MaxValue);
		}
		if (playTypeProp.enumValueIndex == (int)SoundPlayer.PlayType.Random)
		{
			randomWaitTimeMinProp.floatValue = EditorGUILayout.FloatField("Wait min time", randomWaitTimeMinProp.floatValue);
			randomWaitTimeMaxProp.floatValue = Mathf.Clamp(EditorGUILayout.FloatField("Wait max time", randomWaitTimeMaxProp.floatValue), randomWaitTimeMinProp.floatValue, Mathf.Infinity);
		}
		EditorGUILayout.PropertyField(startCallbackActProp);
		EditorGUILayout.PropertyField(endCallbackActProp);
		EditorUtility.SetDirty(target);
		serializedObj.ApplyModifiedProperties();
	}

	void OnEnable()
	{
		serializedObj = new SerializedObject(target);
		volumeProp = serializedObj.FindProperty("volume");
		pitchProp = serializedObj.FindProperty("pitch");
		delayProp = serializedObj.FindProperty("delay");
		loopCountProp = serializedObj.FindProperty("loopCount");
		is3dSoundProp = serializedObj.FindProperty("is3dSound");
		isAwakePlayProp = serializedObj.FindProperty("isAwakePlay");
		startCallbackActProp = serializedObj.FindProperty("startCallbackAct");
		endCallbackActProp = serializedObj.FindProperty("endCallbackAct");
		playTypeProp = serializedObj.FindProperty("playType");
		randomWaitTimeMinProp = serializedObj.FindProperty("randomWaitTimeMin");
		randomWaitTimeMaxProp = serializedObj.FindProperty("randomWaitTimeMax");
		isLoopInfinityProp = serializedObj.FindProperty("isLoopInfinity");
		audioNameProp = serializedObj.FindProperty("audioName");
    }
}
#endif