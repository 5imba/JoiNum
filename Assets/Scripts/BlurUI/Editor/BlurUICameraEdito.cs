using UnityEngine;
using UnityEditor;

namespace BlurUI
{
	[CustomEditor(typeof(BlurUICamera))]
	[ExecuteInEditMode]
	public class BlurUICameraEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(10);
		}
	}
}
