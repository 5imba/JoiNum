using UnityEngine;
using UnityEditor;

namespace BlurUI
{
	[CustomEditor(typeof(Blurring))]
	[ExecuteInEditMode]
	public class BlurringEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(10);
		}
	}
}