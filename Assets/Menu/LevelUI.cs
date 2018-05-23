using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class LevelUI : MonoBehaviour {
		public Image Image;
		public Text Name;
		public Text Description;
		public Image Stars;
		public string scene;

		public static Controller controller;

		public void SetValues(LevelUIEntry entry, float stars) {
			scene = entry.Scene;
			SetValues (entry.Name, entry.Description, entry.Image, stars);
		}
		public void SetValues(string name, string description, Sprite image, float stars) {
			Name.text = name;
			Description.text = description;
			Image.sprite = image;
			Stars.fillAmount = stars;
		}

		public void Focus() {

		}
		public void Unfocus() {

		}

		public void Select() {
			Debug.Log ("Selected");
			controller.SelectLevel (scene);
		}
	}
}