using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class Controller : MonoBehaviour {

		public GameObject menuScene;
		public GameObject floor;
		public GameObject cubeHidden;
		public GameObject cubesShown1;
		public GameObject cubesShown2;
		public Light spotLight;

		public GameObject menuUI;
		public GameObject tutorialUI;
		public GameObject selectLevelUI;

		public Level loadedlevel;

		//menuUI callbacks
		public void ShowTutorial() {

		}
		public void ShowSelectLevel() {
			menuUI.SetActive (false);
			//reducir foco de luz
			StartCoroutine(SpotLightRoutine(
				delegate {
					//encender luces de los neones de los cubos
					menuScene.GetComponent<Animator>().Play(Values.Menu.LightCubesAnimation);
					StartCoroutine(LightCubesRoutine(
						delegate {
							//mostrar selectLevelUI
							selectLevelUI.SetActive(true);
						}
					));
				}
			));
		}
		IEnumerator SpotLightRoutine(Action callback = null) {
			float t = 0f;
			float d = Values.Menu.LightCloseDuration;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				spotLight.spotAngle = Mathf.Lerp (Values.Menu.LightOpen, 0f, t / d);
			}
			if (callback != null) callback ();
		}
		IEnumerator LightCubesRoutine(Action callback = null) {
			yield return new WaitForSeconds (Values.Menu.LightCubesDuration);
			if (callback != null) callback ();
		}

		public void Exit() {

		}

		//tutorialUI callbacks

		//levelSelect callbacks
		public void SelectLevel(string level) {
			floor.SetActive (false);
			StartCoroutine (SelectLevelRoutine (level));
		}
		IEnumerator SelectLevelRoutine (string level) {
			//Hide UIs
			menuUI.SetActive (false);
			tutorialUI.SetActive (false);
			selectLevelUI.SetActive (false);

			yield return new WaitForSeconds (Values.Menu.SelectLevelDuration);

			//deactive menu objects
			menuScene.SetActive(false);

			//load level
			GoToLevel (level);
		}
		void GoToLevel(string level) {

		}
	}

}