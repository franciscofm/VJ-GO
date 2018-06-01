using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour {

	public Spot spot;
	public static Level level;
    public Animator anim;

    void Start() {
		spot.OnEnter += GetBonus;
		++level.totalBonus;
	}

	void GetBonus() {
		if (!spot.occupied) return; //No deberia por Entity.cs
		if (spot.occupation == null) return; //No deberia por Entity.cs
		if (spot.occupation is Enemy) return; //Que no sea un enemigo
		level.PickBonus();
        StartCoroutine ( WaitAnimationRoutine(anim, "bonusDestroyed", 1f, delegate {
            Destroy(gameObject);
        }));
        //anim.Play("bonusDestroyed");
        //Wait();
        //Destroy(gameObject);
        //Animacion de ser cogido
        //Animar personaje
        spot.OnEnter -= GetBonus;
	}

    IEnumerator WaitAnimationRoutine(Animator anim, string play, float duration, Action callback = null)
    {
        anim.Play(play);
        yield return new WaitForSeconds(duration);
        if (callback != null) callback();
    }
}
