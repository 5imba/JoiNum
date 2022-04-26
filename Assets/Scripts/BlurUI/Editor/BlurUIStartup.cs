using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class BlurUIStartup
{
	static BlurUIStartup()
	{
		////////////creating layers ///////////////////
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

		SerializedProperty layerProp = tagManager.FindProperty("layers");
		for (int i = 8; i <= 29; i++)
		{
			SerializedProperty sp0 = layerProp.GetArrayElementAtIndex(i);

			if (sp0.stringValue.Equals("BlurUI"))
			{
				break;
			}
			if (sp0 != null)
			{
				if (sp0.stringValue.Trim().Length == 0)
				{
					sp0.stringValue = "BlurUI";
					Debug.Log("BlurUI layer added");
					break;
				}
			}
		}
		tagManager.ApplyModifiedProperties();
		/////////////////////////////////////////////
	}
}