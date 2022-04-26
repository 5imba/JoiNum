using UnityEngine;
using UnityEditor;

namespace BlurUI
{
	[CustomEditor(typeof(BlurUI))]
	[ExecuteInEditMode]
	public class BlurUIEditor : Editor
	{
		void Awake()
		{
			BlurUI myTarget = (BlurUI)target;
			myTarget.AddBlurringComponent();
		}

		void OnEnable()
		{
			BlurUI myTarget = (BlurUI)target;
			myTarget.AddBlurringComponent();
		}

		public override void OnInspectorGUI()
		{
			BlurUI myTarget = (BlurUI)target;
			if (target == null) return;

			GUI.changed = false;

			GUILayout.Space(10);
			myTarget.ApplyOnChildren = EditorGUILayout.Toggle("ApplyOnChildren", myTarget.ApplyOnChildren);
			if (myTarget.ApplyOnChildren == true)
			{
				myTarget.AddBlurringComponentOnChildren();
			}
			else
			{
				myTarget.RemoveBlurringComponentFromChildren();
			}
			GUILayout.Space(10);
			myTarget.mainCamera = EditorGUILayout.ObjectField("MainCamera", myTarget.mainCamera, typeof(Camera), true) as Camera;
			if (myTarget.mainCamera != null)
			{
				myTarget.AddBlurUICamera();
			}
			GUILayout.Space(10);
			myTarget.blurOption = (BlurOption)EditorGUILayout.EnumPopup("BlurOption", myTarget.blurOption);
			GUILayout.Space(10);
			myTarget.MobileDevice = EditorGUILayout.Toggle("MobileDevice", myTarget.MobileDevice);
			if (myTarget.MobileDevice == true)
			{
				myTarget.RemoveBlurUICamera();
				myTarget.AddBlurUICameraMobile();
			}
			else
			{
				GUILayout.Space(10);
				myTarget.kernalSize = (BlurKernelSize)EditorGUILayout.EnumPopup("KernalSize", myTarget.kernalSize);
				myTarget.RemoveBlurUICameraMobile();
				myTarget.AddBlurUICamera();
			}
			GUILayout.Space(10);
			myTarget.DownSample = EditorGUILayout.IntSlider("DownSample", myTarget.DownSample, 0, 4);
			GUILayout.Space(10);
			myTarget.Iterations = EditorGUILayout.IntSlider("Iterations", myTarget.Iterations, 0, 4);
			GUILayout.Space(10);
			myTarget.UpdateFrameRate = EditorGUILayout.IntSlider("UpdateFrameRate", myTarget.UpdateFrameRate, 0, 60);

			GUILayout.Space(10);

			myTarget.GreyScale = EditorGUILayout.Slider("GreyScale", myTarget.GreyScale, 0.0f, 1.0f);
			GUILayout.Space(10);
			myTarget.Brightness = EditorGUILayout.Slider("Brightness", myTarget.Brightness, 0.0f, 1.0f);
			GUILayout.Space(10);



			if (GUI.changed && myTarget != null)
			{
				EditorUtility.SetDirty(myTarget);
				myTarget.ApplyCameraProperties();
			}

			if (GUILayout.Button("RemoveBlurUI"))
			{
				myTarget.RemoveBlurUI();
				GUIUtility.ExitGUI();
			}
		}
	}

}
