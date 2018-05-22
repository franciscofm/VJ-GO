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
		public RectTransform panelTutorial;
		public RectTransform panelLeftMask;
		public RectTransform panelCenterMask;
		public RectTransform panelRightMask;
		[Header("Select Level UI")]
		public GameObject selectLevelUI;

		[HideInInspector]
		public Level loadedlevel;

		Animator animatorScene;
		void Start() {
			StartMenuUI ();
			StartTutorialUI ();

			menuScene.SetActive (true);
			menuUI.SetActive (true);
			tutorialUI.SetActive (false);
			selectLevelUI.SetActive (false);

			animatorScene = menuScene.GetComponent<Animator> ();
			//asegurar que el cubo que brilla esta apagado
			cubeHidden.transform.GetChild (0).gameObject.SetActive (false);
			//Cambiar por animacion de encender foco
			StartCoroutine (SpotLightRoutine (false, delegate {
				StartCoroutine(TextsInRoutine());
			}));
		}

		IEnumerator SpotLightRoutine(bool close, Action callback = null) {
			float t = 0f;
			float d = Values.Menu.Scene.LightCloseDuration;
			float a = close ? Values.Menu.Scene.LightOpen : 0f;
			float b = close ? 0f : Values.Menu.Scene.LightOpen;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				spotLight.spotAngle = Mathf.Lerp (a, b, t / d);
			}
			lightOn = !close;
			if (callback != null) callback ();
		}
		IEnumerator WaitAnimationRoutine(Animator anim, string play, float duration, Action callback = null) {
			anim.Play (play);
			yield return new WaitForSeconds (duration);
			if (callback != null) callback ();
		}

		//menuUI
		bool lightOn;
		bool textIn;
		void StartMenuUI() {
			
		}
		IEnumerator TextsInRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Scene.TextOffset);
			StartCoroutine (TextRoutine (textSelectLevel,startSelectLevel.position,endSelectLevel.position));
			yield return offset;
			StartCoroutine (TextRoutine (textTutorial,startTutorial.position,endTutorial.position));
			yield return offset;
			StartCoroutine (TextRoutine (textExit,startExit.position,endExit.position,callback));
			textIn = true;
		}
		IEnumerator TextsOutRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Scene.TextOffset);
			StartCoroutine (TextRoutine (textSelectLevel,endSelectLevel.position,startSelectLevel.position));
			yield return offset;
			StartCoroutine (TextRoutine (textTutorial,endTutorial.position,startTutorial.position));
			yield return offset;
			StartCoroutine (TextRoutine (textExit,endExit.position,startExit.position, callback));
			textIn = false;
		}
		IEnumerator TextRoutine(Transform tran, Vector2 start, Vector2 end, Action callback = null) {
			float t = 0f, d = Values.Menu.Scene.TextDuration;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				tran.position = Vector2.Lerp (start, end, t / d);
			}
			if (callback != null) callback ();
		}

		public void ShowTutorial() {
			StartCoroutine(SpotLightRoutine(true, delegate {
				ShowTutorialRequest();
			}));
			StartCoroutine (TextsOutRoutine( delegate {
				menuScene.SetActive(false);
				menuUI.SetActive(false);
				tutorialUI.SetActive(true);
				ShowTutorialRequest();
			}));
		}
		public void ShowSelectLevel() {
			menuUI.SetActive (false);
			//reducir foco de luz
			StartCoroutine(SpotLightRoutine(true,
				delegate {
					//encender luces de los neones de los cubos
					StartCoroutine(WaitAnimationRoutine(
						animatorScene, Values.Menu.Scene.LightCubesAnimation, Values.Menu.Scene.LightCubesDuration,
						delegate {
							//mostrar selectLevelUI
							selectLevelUI.SetActive(true);
						}
					));
				}
			));
   		}

		public void Exit() {

		}

		//tutorialUI
		float height;
		void StartTutorialUI() {
			height = panelTutorial.rect.height;
		}
		void ShowTutorialRequest() {
			if(!lightOn && !textIn)
				StartCoroutine(ShowTutorialRoutine ());
		}
		IEnumerator ShowTutorialRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Tutorial.PanelOffset);
			StartCoroutine (ExpandPanelRoutine(panelLeftMask, true));
			yield return offset;
			StartCoroutine (ExpandPanelRoutine(panelCenterMask, false));
			yield return offset;
			StartCoroutine (ExpandPanelRoutine(panelRightMask, true));
			if (callback != null) callback ();
		}
		IEnumerator ExpandPanelRoutine(RectTransform mask, bool topDown) {
			float t = 0f;
			float d = Values.Menu.Tutorial.PanelDuration;
			float sizeDeltaY = -height;
			float anchoredPositionY = topDown ? -height * 0.5f : height * 0.5f;
			Rect rect = mask.rect;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				//rect.yMin = Mathf.Lerp (0, height, t / d);
				mask.sizeDelta = new Vector2(0f,Mathf.Lerp(0f, sizeDeltaY, t/d));
				mask.anchoredPosition = new Vector2(0f,Mathf.Lerp(0f, anchoredPositionY, t/d));
			}
		}

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

			yield return new WaitForSeconds (Values.Menu.SelectLevel.SelectDuration);

			//deactive menu objects
			menuScene.SetActive(false);

			//load level
			GoToLevel (level);
		}
		void GoToLevel(string level) {

		}
	}

}