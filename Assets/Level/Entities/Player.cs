using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : Entity { 
	public Material bodyMaterial;
	public float disolveDuration = 1f;
	public float disolveShown = 0f;
	public float disolveHidden = 1f;
	public AudioClip disolveSound;
	public AudioSource audioSource;

	protected override void Start2 () {
		_enabled = false;
		bodyMaterial.SetColor ("_Emission", color);
		bodyMaterial.SetFloat("_DissolveAmount", disolveHidden);
		lines = new GameObject[linesParent.childCount];
		for (int i = 0; i < linesParent.childCount; ++i) {
			lines [i] = linesParent.GetChild (i).gameObject;
			lines [i].SetActive (false);
		}
		StartCoroutine (DisolveRoutine (disolveHidden, disolveShown, false, delegate {
			_enabled = true;
		}));
		if (disolveSound != null) {
			if (audioSource == null)
				audioSource = GetComponent<AudioSource> ();
			if (audioSource == null)
				audioSource = gameObject.AddComponent<AudioSource> ();
		}
	}

    protected override IEnumerator MoveRoutine(Entity entity, Spot spot, float duration, AnimationCurve curve, Action endAction = null) {
        anim.Play("moveAnimation");
        return base.MoveRoutine(entity, spot, duration, curve, endAction);
    }

	public override void Finish (Action callback) {
		StartCoroutine (DisolveRoutine (disolveShown, disolveHidden, true, delegate {
			base.Finish(callback);
		}));
	}
	IEnumerator DisolveRoutine(float start, float end, bool hide, Action callback) {
		if(hide)
			for (int i = 0; i < lines.Length; ++i) {
				lines [i].SetActive (false);
				yield return null;
			}
		float t = 0f;
		while (t < disolveDuration) {
			yield return null;
			t += Time.deltaTime;
			bodyMaterial.SetFloat("_DissolveAmount", Mathf.Lerp(start, end, t/disolveDuration));
		}
		if(!hide)
			for (int i = 0; i < lines.Length; ++i) {
				lines [i].SetActive (true);
				yield return null;
			}
		if (callback != null) callback ();
	}
}
