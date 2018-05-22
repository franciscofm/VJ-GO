using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class Controller : MonoBehaviour {
		[Header("Menu Scene")]
		public GameObject menuScene;
		public GameObject floor;
		public GameObject cubeHidden;
		public GameObject cubesShown1;
		public GameObject cubesShown2;
		public Light spotLight;
		public Camera camera;
		[Header("Menu UI")]
		public GameObject menuUI;
		public Transform textSelectLevel;
		public Transform startSelectLevel, endSelectLevel;
		public Transform textTutorial;
		public Transform startTutorial, endTutorial;
		public Transform textExit;
		public Transform startExit, endExit;
		[Header("Tutorial UI")]
		public GameObject tutorialUI;
		[Header("Select Level UI")]
		public GameObject selectLevelUI;

		public Level loadedlevel;

		Animator animatorScene;
		void Start() {
			animatorScene = menuScene.GetComponent<Animator> ();
			cubeHidden.transform.GetChild (0).gameObject.SetActive (false);
			StartCoroutine (SpotLightRoutine (false, delegate {
				StartCoroutine(TextsRoutine());
			}));
		}

		IEnumerator SpotLightRoutine(bool close, Action callback = null) {
			float t = 0f;
			float d = Values.Menu.LightCloseDuration;
			float a = close ? Values.Menu.LightOpen : 0f;
			float b = close ? 0f : Values.Menu.LightOpen;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				spotLight.spotAngle = Mathf.Lerp (a, b, t / d);
			}
			if (callback != null) callback ();
		}

		//menuUI
		IEnumerator TextsRoutine() {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.TextOffset);
			StartCoroutine (TextRoutine (textSelectLevel,startSelectLevel.position,endSelectLevel.position));
			yield return offset;
			StartCoroutine (TextRoutine (textTutorial,startTutorial.position,endTutorial.position));
			yield return offset;
			StartCoroutine (TextRoutine (textExit,startExit.position,endExit.position));
		}
		IEnumerator TextRoutine(Transform tran, Vector2 start, Vector2 end) {
			float t = 0f, d = Values.Menu.TextDuration;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				tran.position = Vector2.Lerp (start, end, t / d);
			}
		}
		public void ShowTutorial() {

		}
		public void ShowSelectLevel() {
			menuUI.SetActive (false);
			//reducir foco de luz
			StartCoroutine(SpotLightRoutine(true,
				delegate {
					//encender luces de los neones de los cubos
					animatorScene.Play(Values.Menu.LightCubesAnimation);
					StartCoroutine(LightCubesRoutine(
						delegate {
							//mostrar selectLevelUI
							selectLevelUI.SetActive(true);
						}
					));
				}
			));
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