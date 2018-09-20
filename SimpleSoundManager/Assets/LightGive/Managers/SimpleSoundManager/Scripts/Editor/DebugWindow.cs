using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DebugWindow : EditorWindow
{
	[MenuItem("Tools/LightGive/SimpleSoundManager/DebugWindow")]
	static void Open()
	{
		var exampleWindow = CreateInstance<DebugWindow>();
		exampleWindow.Show();
	}
}
