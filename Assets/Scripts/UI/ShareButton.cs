using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShareButton : MonoBehaviour
{
	private bool isFocus = false;
	private bool isProcessing = false;

	void OnApplicationFocus(bool focus)
	{
		isFocus = focus;
	}

	public void ShareScore()
	{
		Messenger<Sound>.Broadcast(GameEvent.PLAY_SOUND, Sound.Click);
#if UNITY_ANDROID
		if (!isProcessing)
		{
			StartCoroutine(ShareTextInAnroid());
		}
#else
		Debug.Log("No sharing set up for this platform.");
#endif
	}



#if UNITY_ANDROID
	public IEnumerator ShareTextInAnroid()
	{
		string shareSubject = Utils.GetLocalizedString("UITable", "share-title",
			new KeyValuePair<string, UnityEngine.Localization.SmartFormat.PersistentVariables.IVariable>[]
				{
					new KeyValuePair<string, UnityEngine.Localization.SmartFormat.PersistentVariables.IVariable>
					("score", new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable { Value = PlayerData.CurrentScore })
				});
		string shareMessage = shareSubject + ". " + Utils.GetLocalizedString("UITable", "share-message");

		isProcessing = true;

		if (!Application.isEditor)
		{
			//Create intent for action send
			AndroidJavaClass intentClass =
				new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject =
				new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>
				("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

			//put text and subject extra
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
			intentObject.Call<AndroidJavaObject>
				("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
			intentObject.Call<AndroidJavaObject>
				("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

			//call createChooser method of activity class
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity =
				unity.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject chooser =
				intentClass.CallStatic<AndroidJavaObject>
				("createChooser", intentObject, "Share your high score");
			currentActivity.Call("startActivity", chooser);
		}

		yield return new WaitUntil(() => isFocus);
		isProcessing = false;
	}
#endif
}