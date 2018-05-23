using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu {
	
	public class Controller : MonoBehaviour {
		[Header("Menu Scene")]
		public GameObject menuScene;
		public GameObject cubeHiddenEdges;
		public GameObject cubesShown1Edges;
		public GameObject cubesShown2Edges;
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
		public Image fadeScreen;
		[Header("Tutorial UI")]
		public GameObject tutorialUI;
		public RectTransform panelTutorial;
		public RectTransform panelLeftMask;
		public RectTransform panelCenterMask;
		public RectTransform panelRightMask;
		[Header("Select Level UI")]
		public GameObject selectLevelUI;
		public Transform scrollViewContent;
		public GameObject scrollViewRow;
		public GameObject scrollViewLevelUI;
		public List<LevelUIEntry> scrollViewContentEntries;
		[Header("Keyboard controll")]
		public int keyboardFocus;
		public enum Focus { Menu, Tutorial, SelectLevel }
		public Focus focus;

		[Header("Public debug")]
		public Level loadedlevel;

		Animator animatorScene;
		void Awake() {
			LevelUI.controller = this;
		}
		void Start() {
			menuScene.SetActive (true);
			menuUI.SetActive (true);
			tutorialUI.SetActive (true);
			selectLevelUI.SetActive (true);

			StartMenuUI ();
			StartTutorialUI ();
			StartSelectLevelUI ();

			menuScene.SetActive (true);
			menuUI.SetActive (true);
			tutorialUI.SetActive (false);
			selectLevelUI.SetActive (false);

			focus = Focus.Menu;
			keyboardFocus = 0;
			fadeScreen.color = Values.Colors.transparentBlack;

			animatorScene = menuScene.GetComponent<Animator> ();
			//asegurar cubos apagados
			cubeHiddenEdges.SetActive (false);
			cubesShown1Edges.SetActive (false);
			cubesShown2Edges.SetActive (false);

			//Cambiar por animacion de encender foco
			StartCoroutine (SpotLightRoutine (false, delegate {
				StartCoroutine(TextsInRoutine());
			}));
		}
		void Update() {
			if (!Input.anyKeyDown) return;
			switch (focus) {
			case Focus.Menu:
				if (Input.GetKeyDown (KeyCode.S)) {
					++keyboardFocus;
					keyboardFocus %= 3;
				}
				if (Input.GetKeyDown (KeyCode.W)) {
					--keyboardFocus;
					if (keyboardFocus < 0)
						keyboardFocus = 2;
				}
				if (Input.GetKeyDown (KeyCode.Space)) {
					switch(keyboardFocus) {
					case 0:
						ShowSelectLevel();
						break;
					case 1:
						ShowTutorial ();
						break;
					case 2:
						Exit ();
						break;
					}
				}
				break;
			case Focus.Tutorial:
				if (Input.GetKeyDown (KeyCode.Escape))
					ReturnFromTutorial ();
				if (Input.GetKeyDown (KeyCode.Space))
					PlayTutorial ();
				break;
			case Focus.SelectLevel:
				if (Input.GetKeyDown (KeyCode.S)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Unfocus ();
					keyboardFocus = Math.Min (keyboardFocus + 2, scrollViewContent.childCount -1);
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Focus ();
				} else if (Input.GetKeyDown (KeyCode.W)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Unfocus ();
					keyboardFocus = Math.Max (keyboardFocus - 2, 0);
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Focus ();
				} else if (Input.GetKeyDown (KeyCode.D)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Unfocus ();
					keyboardFocus = Math.Min (keyboardFocus + 1, scrollViewContent.childCount -1);
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Focus ();
				} else if (Input.GetKeyDown (KeyCode.A)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Unfocus ();
					keyboardFocus = Math.Max (keyboardFocus - 1, 0);
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Focus ();
				} else if (Input.GetKeyDown (KeyCode.Space)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Select ();
				}
				break;
			}
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
		public bool lightOn, textIn;
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
		//Callbacks
		public void ShowTutorial() {
			StartCoroutine(SpotLightRoutine(true));
			StartCoroutine (TextsOutRoutine( delegate {
				menuScene.SetActive(false);
				menuUI.SetActive(false);
				tutorialUI.SetActive(true);
				StartCoroutine(ShowTutorialRoutine (delegate {
					focus = Focus.Tutorial;
				}));
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
							focus = Focus.SelectLevel;
							keyboardFocus = 0;
						}
					));
				}
			));
   		}
		public void Exit() {
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}

		//tutorialUI
		public float height;
		void StartTutorialUI() {
			height = panelTutorial.rect.height;
		}
		public void ReturnFromTutorial() {
			panelLeftMask.anchoredPosition = Vector2.zero;
			panelCenterMask.anchoredPosition = Vector2.zero;
			panelRightMask.anchoredPosition = Vector2.zero;
			panelLeftMask.sizeDelta = Vector2.zero;
			panelCenterMask.sizeDelta = Vector2.zero;
			panelRightMask.sizeDelta = Vector2.zero;
			menuScene.SetActive (true);
			menuUI.SetActive (true);
			tutorialUI.SetActive (false);
			StartCoroutine (SpotLightRoutine (false));
			StartCoroutine (TextsInRoutine(delegate {
				focus = Focus.Menu;
				keyboardFocus = 0;
			}));
		}
		public void PlayTutorial() {

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
		IEnumerator CloseTutorialRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Tutorial.PanelOffset);
			StartCoroutine (ExpandPanelRoutine(panelLeftMask, true));
			yield return offset;
			StartCoroutine (ExpandPanelRoutine(panelCenterMask, true));
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
				mask.sizeDelta = new Vector2(0f,Mathf.Lerp(0f, sizeDeltaY, t/d));
				mask.anchoredPosition = new Vector2(0f,Mathf.Lerp(0f, anchoredPositionY, t/d));
			}
		}

		//levelSelect
		public string levelToLoad;
		void StartSelectLevelUI() {
			Transform lastRow = null;
			for (int i = 0; i < scrollViewContentEntries.Count; ++i) {
				if (i % 2 == 0) lastRow = Instantiate (scrollViewRow, scrollViewContent).transform;
				LevelUI level = Instantiate (scrollViewLevelUI, lastRow).GetComponent<LevelUI>();
				level.SetValues (scrollViewContentEntries [i], 0f);
			}
		}
		public void SelectLevel(string level) {
			StartCoroutine (SelectLevelRoutine (level));
		}
		IEnumerator SelectLevelRoutine (string level) {
			//Hide UIs
			menuUI.SetActive (false);
			tutorialUI.SetActive (false);
			selectLevelUI.SetActive (false);
			//Precarga
			//Encender los otros dos cubos y reproducir sonido
			cubesShown1Edges.SetActive (true);
			cubesShown2Edges.SetActive (true);
			//Esperar momentaneamente
			yield return new WaitForSeconds (Values.Menu.SelectLevel.SelectWait);
			//Fade a negro
			float t = 0f;
			float d = Values.Menu.SelectLevel.SelectFade;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				fadeScreen.color = Color.Lerp (Values.Colors.transparentBlack, Color.black, t / d);
			}
			//Esperar momentaneamente
			yield return new WaitForSeconds (Values.Menu.SelectLevel.SelectWait);
			//Desactivar cosas
			menuScene.SetActive (false);

			//Cargar nivel
			UnityEngine.SceneManagement.SceneManager.LoadScene (level, UnityEngine.SceneManagement.LoadSceneMode.Additive);
		}
		public void ReturnFromSelectLevel() {
			focus = Focus.Menu;
			keyboardFocus = 0;
   		}
	}

}