using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace LightGive
{
	public static class AssemblyAudioPlayer
	{
		public static Type AudioUtilityClass
		{
			get
			{
				Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
				return unityEditorAssembly.GetType("UnityEditor.AudioUtil");
			}
		}

		public static void PlayClip(AudioClip clip)
		{

			MethodInfo method = AudioUtilityClass.GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
			method.Invoke(null, new object[] { clip });
		}


		public static bool IsPlayClip(AudioClip _clip)
		{
			MethodInfo method = AudioUtilityClass.GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
			return (bool)method.Invoke(null, new object[] { _clip });
		}

		public static void StopClip(AudioClip clip)
		{
			MethodInfo method = AudioUtilityClass.GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
			method.Invoke(null, new object[] { clip });
		}

		public static void StopAllClips()
		{
			MethodInfo method = AudioUtilityClass.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { }, null);
			method.Invoke(null, new object[] { });
		}
	}
}