using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class LevelUI : MonoBehaviour {
		public Image image;
		public Text name;
		public Text description;
		public Image stars;

		public void SetValues(string name, string description, Sprite image, float stars) {
			this.name.text = name;
			this.description.text = description;
			this.image.sprite = image;
			this.stars.fillAmount = stars;
		}
	}
}