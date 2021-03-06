﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Audio;

namespace Menu {
	
	public class Controller : MonoBehaviour {

		public GameObject soundSource;

		[Header("Menu Scene")]
		public GameObject menuScene;
		public GameObject cubeHiddenEdges;
		public GameObject cubesShown1Edges;
		public GameObject cubesShown2Edges;
		public Light spotLight;
		public Animator animatorScene;
		public AudioClip selectLevelSound;
		public AudioSource sceneSource;
		[Header("Menu UI")]
		public GameObject menuUI;
		public Transform textSelectLevel;
		public Transform startSelectLevel, endSelectLevel;
		public Transform textTutorial;
		public Transform startTutorial, endTutorial;
		public Transform textExit;
		public Transform startExit, endExit;
		public Image fadeScreen;
		public Transform[] selectables;
		public Transform selection;
		[Header("Tutorial UI")]
		public GameObject tutorialUI;
		public RectTransform panelTutorial;
		public RectTransform panelLeftMask;
		public RectTransform panelCenterMask;
		public RectTransform panelRightMask;
		public LevelUIEntry tutorialEntry;
		[Header("Select Level UI")]
		public GameObject selectLevelUI;
		public Transform scrollViewContent;
		public GameObject scrollViewRow;
		public GameObject scrollViewLevelUI;
		[Tooltip("Tienen que estar en orden")]
		public List<LevelUIEntry> scrollViewContentEntries;
		[Header("Keyboard controll")]
		public int keyboardFocus;
		public enum Focus { Menu, Tutorial, SelectLevel }
		public Focus focus;
		[Header("Options UI")]
		public GameObject optionsUI;
		public Dropdown lenguageDropdown;
		public Slider globalSlider;
		public Slider voiceSlider;
		public Slider effectsSlider;
		public Slider musicSlider;
		public GameObject licensePanel;

		[Header("Public debug")]
		public bool inGame;
		public bool lightOn, textIn;
		public float height;
		public Level loadedlevel;
		public string loadedLevelName;
		public static Controller instance;
		public List<LevelUI> levelUIs;

		void Awake() {
			LevelUI.controller = this;
			Level.menu = this;
			instance = this;

			DataManager.LoadData ();
			ManagerSound.Init (soundSource);
		}
		IEnumerator Start() {
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
			inGame = false;
			spotLight.spotAngle = 0f;
			sceneSource.clip = selectLevelSound;

			//asegurar cubos apagados
			cubeHiddenEdges.SetActive (false);
			cubesShown1Edges.SetActive (false);
			cubesShown2Edges.SetActive (false);

			yield return new WaitForSeconds (Values.Menu.Scene.StartWait);

			//Cambiar por animacion de encender foco
			StartCoroutine (SpotLightRoutine (false, delegate {
				StartCoroutine(TextsInRoutine());
			}));
		}
		void ReturnFromLevel () {
			menuScene.SetActive (true);
			menuUI.SetActive (true);
			tutorialUI.SetActive (false);
			selectLevelUI.SetActive (false);

			animatorScene.Play (Values.Menu.Scene.IdleHiddenAnimation);
			cubeHiddenEdges.SetActive (false);
			cubesShown1Edges.SetActive (false);
			cubesShown2Edges.SetActive (false);

			textSelectLevel.position = startSelectLevel.position;
			textTutorial.position = startTutorial.position;
			textExit.position = startExit.position;
			StartCoroutine (TextsInRoutine ());
			StartCoroutine (SpotLightRoutine (false));
			StartCoroutine (ScreenFadeRoutine (Color.black, Values.Colors.transparentBlack));

			focus = Focus.Menu;
			keyboardFocus = 0;
		}
		void Update() {
			if (inGame) return;
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
				selection.parent = selectables [keyboardFocus];
				selection.transform.localPosition = Vector2.zero;
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
				if (Input.GetKeyDown (KeyCode.Space)) {
					scrollViewContent.GetChild (keyboardFocus).GetComponent<LevelUI> ().Select ();
				} else if (Input.GetKeyDown (KeyCode.Escape)) {
					ReturnFromSelectLevel ();
				} else {
					int previousFocus = keyboardFocus;
					bool changed = false;
					if (Input.GetKeyDown (KeyCode.S)) {
						keyboardFocus = Math.Min (keyboardFocus + 2, ScrollViewSize() - 1);
						changed = true;
					} else if (Input.GetKeyDown (KeyCode.W)) {
						keyboardFocus = Math.Max (keyboardFocus - 2, 0);
						changed = true;
					} else if (Input.GetKeyDown (KeyCode.D)) {
						keyboardFocus = Math.Min (keyboardFocus + 1, ScrollViewSize() - 1);
						changed = true;
					} else if (Input.GetKeyDown (KeyCode.A)) {
						keyboardFocus = Math.Max (keyboardFocus - 1, 0);
						changed = true;
					}
					if (changed) {
						levelUIs [previousFocus].Unfocus ();
						levelUIs [keyboardFocus].Focus ();
					}
				}
				break;
			}
		}
		int ScrollViewSize() {
			return (scrollViewContent.childCount * 2) + scrollViewContent.GetChild(scrollViewContent.childCount - 1).childCount;
		}
		public void MouseFocus(int focus) {
			Debug.Log (focus);
			levelUIs [keyboardFocus].Unfocus ();
			keyboardFocus = focus;
		}

		IEnumerator WaitRoutine(float t, Action callback) {
			yield return new WaitForSeconds (t);
			callback ();
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
		IEnumerator ScreenFadeRoutine(Color a, Color b, Action callback = null) {
			float t = 0f;
			float d = Values.Menu.SelectLevel.SelectFade;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				fadeScreen.color = Color.Lerp (a, b, t / d);
			}
			if (callback != null) callback ();
		}

		//menuUI
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
						animatorScene, Values.Menu.Scene.LightHiddenAnimation, Values.Menu.Scene.LightHiddenDuration,
						delegate {
							//mostrar selectLevelUI
							selectLevelUI.SetActive(true);
							focus = Focus.SelectLevel;
							keyboardFocus = 0;
							for(int i=0; i<levelUIs.Count; ++i) {
								levelUIs[i].canvasGroup.alpha = 0f;
								levelUIs[i].FadeIn(Values.Menu.SelectLevel.LevelUIOffset * (1+i));
								//--> StartCoroutine(levelUIs[i].FadeIn());
							}
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
		void StartTutorialUI() {
			//height = panelTutorial.rect.height;
			height = Screen.height;
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
			StartCoroutine (CloseTutorialRoutine (delegate {
				StartCoroutine(ScreenFadeRoutine(Values.Colors.transparentBlack, Color.black, delegate {
					LoadLevel (tutorialEntry.Scene);
					tutorialUI.SetActive(false);
				}));
			}));
		}
		IEnumerator ShowTutorialRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Tutorial.PanelOffset);
			StartCoroutine (ExpandPanelRoutine(panelLeftMask, true));
			yield return offset;
			StartCoroutine (ExpandPanelRoutine(panelCenterMask, false));
			yield return offset;
			StartCoroutine (ExpandPanelRoutine(panelRightMask, true, callback));
		}
		IEnumerator CloseTutorialRoutine(Action callback = null) {
			YieldInstruction offset = new WaitForSeconds(Values.Menu.Tutorial.PanelOffset);
			StartCoroutine (CollapsePanelRoutine(panelLeftMask, true));
			yield return offset;
			StartCoroutine (CollapsePanelRoutine(panelCenterMask, true));
			yield return offset;
			StartCoroutine (CollapsePanelRoutine(panelRightMask, true, callback));
		}
		IEnumerator ExpandPanelRoutine(RectTransform mask, bool topDown, Action callback = null) {
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
			if (callback != null) callback ();
		}
		IEnumerator CollapsePanelRoutine(RectTransform mask, bool topDown, Action callback = null) {
			float t = 0f;
			float d = Values.Menu.Tutorial.PanelDuration;
			float sizeDeltaY = -height;
			float anchoredPositionY = topDown ? -height * 0.5f : height * 0.5f;
			Rect rect = mask.rect;
			while (t < d) {
				yield return null;
				t += Time.deltaTime;
				mask.sizeDelta = new Vector2(0f,Mathf.Lerp(sizeDeltaY, 0f, t/d));
				mask.anchoredPosition = new Vector2(0f,Mathf.Lerp(anchoredPositionY, 0f, t/d));
			}
			if (callback != null) callback ();
		}

		//optionsUI
		public void StartOptionsUI() {

		}
		public void ShowOptionsUI() {
			for (int i = 0; i < lenguageDropdown.options.Count; ++i) {
				if (lenguageDropdown.options [i].text == DataManager.ConfState.Lenguage) {
					lenguageDropdown.value = i;
					i = lenguageDropdown.options.Count + 1;
				}
			}
			globalSlider.value = DataManager.ConfState.GlobalVolume;
			voiceSlider.value = DataManager.ConfState.VoiceVolume;
			effectsSlider.value = DataManager.ConfState.EffectVolume;
			musicSlider.value = DataManager.ConfState.MusicVolume;
			optionsUI.SetActive (true);
		}
		public void EraseOptions() {
			DataManager.EraseData ();
			DataManager.LoadData ();
			ShowOptionsUI ();
		}
		public void ExitOptions() {
			optionsUI.SetActive (false);
		}
		public void ApplyOptions() {
			DataManager.ConfState.Lenguage = lenguageDropdown.options [lenguageDropdown.value].text;
			DataManager.ConfState.GlobalVolume = globalSlider.value;
			DataManager.ConfState.VoiceVolume = voiceSlider.value;
			DataManager.ConfState.EffectVolume = effectsSlider.value;
			DataManager.ConfState.MusicVolume = musicSlider.value;
			DataManager.SaveData ();
			optionsUI.SetActive (false);
		}
		public void LicenseOptions(bool active) {
			licensePanel.SetActive (active);
		}

		//levelSelect
		public string levelToLoad;
		void StartSelectLevelUI() {
			levelUIs = new List<LevelUI> ();
			List<LevelRegistry> registries = DataManager.SaveState.Registries;
			Transform lastRow = null;
			for (int i = 0; i < scrollViewContentEntries.Count; ++i) {
				if (i % 2 == 0) lastRow = Instantiate (scrollViewRow, scrollViewContent).transform;
				LevelUI level = Instantiate (scrollViewLevelUI, lastRow).GetComponent<LevelUI>();
				float completion = 0f;
				if (i < registries.Count)
					completion = registries [i].Completion;
				level.SetValues (scrollViewContentEntries [i], completion);
				levelUIs.Add (level);
			}
		}
		public void SelectLevel(LevelUI lUI) {
			if (lUI.completion > 0f)
				StartCoroutine (SelectLevelRoutine (lUI.Scene));
			else {
				for(int i=0; i<levelUIs.Count; ++i) {
					if (lUI.Scene == levelUIs [i].Scene) {
						if (i == 0) { StartCoroutine (SelectLevelRoutine (lUI.Scene)); return; }
						if(levelUIs[i-1].completion > 0f) { StartCoroutine (SelectLevelRoutine (lUI.Scene)); return; }
						return;
					}
				}
			}
		}
		IEnumerator SelectLevelRoutine (string level) {
			//Hide UIs
			menuUI.SetActive (false);
			tutorialUI.SetActive (false);
			sceneSource.Play ();
			for (int i = 0; i < levelUIs.Count; ++i)
				levelUIs [i].FadeOut ();
			//Precarga
			//Encender los otros dos cubos y reproducir sonido
			cubesShown1Edges.SetActive (true);
			cubesShown2Edges.SetActive (true);
			//Esperar momentaneamente
			yield return new WaitForSeconds (Values.Menu.SelectLevel.SelectWait);
			//Fade a negro
			StartCoroutine(ScreenFadeRoutine(Values.Colors.transparentBlack, Color.black, delegate {
				//Esperar momentaneamente
				StartCoroutine(WaitRoutine(Values.Menu.SelectLevel.SelectWait, delegate {
					//Desactivar cosas
					selectLevelUI.SetActive(false);
					menuScene.SetActive (false);
					//Cargar nivel
					LoadLevel(level);
				}));
			}));
		}
		public void ReturnFromSelectLevel() {
			for (int i = 0; i < levelUIs.Count; ++i)
				levelUIs [i].FadeOut ();
			StartCoroutine (WaitRoutine (Values.Menu.SelectLevel.LevelUIFadeOut, delegate {
				ReturnFromLevel();
			}));
			focus = Focus.Menu;
			keyboardFocus = 0;
   		}
		void UpdateSelectLevelUI(string level, float completion) {
			for (int i = 0; i < levelUIs.Count; ++i)
				if (levelUIs [i].Scene == level)
					levelUIs [i].UpdateCompletion (completion);
		}

		void LoadLevel(string level) {
			loadedLevelName = level;
			inGame = true;
			SceneManager.LoadScene (level, UnityEngine.SceneManagement.LoadSceneMode.Additive);
			StartCoroutine (ScreenFadeRoutine (Color.black, Values.Colors.transparentBlack, delegate {
				loadedlevel.debug = false;
				loadedlevel.StartLevel();
			}));
		}
		public void FinishLevel() {
			inGame = false;
			ScoreGame ();
			StartCoroutine(ScreenFadeRoutine(Values.Colors.transparentBlack, Color.black, delegate {
				//Unload current level
				SceneManager.UnloadSceneAsync (loadedLevelName);
				//Load next level
				bool found = false;
				for (int i = 0; i < scrollViewContentEntries.Count && !found; ++i) {
					if (scrollViewContentEntries [i].Scene == loadedLevelName) {
						found = true;
						if (i + 1 < scrollViewContentEntries.Count) {
							loadedLevelName = scrollViewContentEntries [i + 1].Scene;
							LoadLevel (loadedLevelName);
						} else {
							ReturnFromLevel ();
						}
					}
				}
				if (!found) ReturnFromLevel ();
			}));
		}
		public void ExitLevel() {
			inGame = false;
			StartCoroutine(ScreenFadeRoutine(Values.Colors.transparentBlack, Color.black, delegate {
				//Unload current level
				SceneManager.UnloadSceneAsync (loadedLevelName);
				//Load menu 
				ReturnFromLevel ();
			}));
		}
		void ScoreGame() {
			List<LevelRegistry> registries = DataManager.SaveState.Registries;
			float completion = loadedlevel.GetCompletion ();
			bool found = false;
			bool update = false;
			for (int i = 0; i < registries.Count && !found; ++i) {
				if (registries [i].Level == loadedLevelName) {
					found = true;
					if (registries [i].Completion < completion) {
						registries [i].Completion = completion;
						update = true;
					}
				}
			}
			if (!found) {
				update = true;
				LevelRegistry newRegistry = new LevelRegistry ();
				newRegistry.Level = loadedLevelName;
				newRegistry.Completion = completion;
				registries.Add (newRegistry);
			}
			DataManager.SaveData ();
			if (update)
				UpdateSelectLevelUI (loadedLevelName, completion);
		}
	}

}