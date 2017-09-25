using UnityEngine;

public class ShowActivity : MonoBehaviour {
	private void OnGUI() {
		if(GUILayout.Button("Show Activity", GUILayout.Width(400), GUILayout.Height(100))) {
			Debug.LogError("showActivity");

#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
			jo.Call("showActivity");
#endif
		}
	}
}
