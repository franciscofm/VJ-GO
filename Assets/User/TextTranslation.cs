using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextTranslation : MonoBehaviour {

	public Translation[] translations = new Translation[] { new Translation("English", ""), new Translation("Español","") };
	IEnumerator Start () {
		yield return new WaitForEndOfFrame ();
		string lan = DataManager.ConfState.Lenguage;
		Text text = GetComponent<Text> ();
		switch (lan) {
		case "English":
			text.text = translations [0].text;
			break;
		case "Español":
			text.text = translations [1].text;
			break;
		}
	}
}

[System.Serializable]
public class Translation { 
	public string lan; 
	public string text; 
	public Translation(string l, string t) {
		lan = l;
		text = t;
	}
}