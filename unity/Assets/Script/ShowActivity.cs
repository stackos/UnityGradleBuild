using UnityEngine;

public class ShowActivity : MonoBehaviour {
	private void OnGUI() {
		if(GUILayout.Button("Show Activity", GUILayout.Width(200), GUILayout.Height(50))) {
			Debug.LogError("Show Activity");
		}
	}
}
