using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotTutorial : Spot {

	public enum When { Enter, Leave, Exit };
	public When when = When.Enter;
	public List<Phrase> dialog;

	public GameObject endSpot;

	IEnumerator Start() {
		endSpot.SetActive (false);
		while (!Level.instance.bridgesDrawn)
			yield return null;
		endSpot.GetComponent<Spot> ().bridgesToLines [0].line.gameObject.SetActive (false);
		switch (when) {
		case When.Enter:
			OnEnter += StartDialog;
			break;
		case When.Leave:
			OnLeave += StartDialog;
			break;
		case When.Exit:
			OnStay += StartDialog;
			break;
		}
	}

	void StartDialog() {
		Level.instance.chat.Init (dialog);
		endSpot.SetActive (true);
		endSpot.GetComponent<Spot> ().bridgesToLines [0].line.gameObject.SetActive (true);
	}
}
