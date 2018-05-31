using UnityEngine;
using UnityEditor;

public class Editor : MonoBehaviour {

	[MenuItem("DataManager/Erase data")]
	private static void EraseData() {
		DataManager.EraseData ();
	}
	[MenuItem("DataManager/Save data")]
	private static void SaveData() {
		DataManager.SaveData ();
	}
}
