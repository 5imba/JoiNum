using UnityEngine;
using UnityEditor;

namespace BlurUI
{
	[CustomEditor(typeof(BlurUICameraMobile))]
	[ExecuteInEditMode]
	public class BlurUICameraMobileEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(10);
		}
	}
}
