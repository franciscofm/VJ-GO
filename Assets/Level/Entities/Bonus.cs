using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour {

	public Spot spot;
	public static Level level;

	void Start() {
		spot.OnEnter += GetBonus;
		++level.totalBonus;
		GetComponent<MeshRenderer> ().material.color = Color.green;
	}

	void GetBonus() {
		if (!spot.occupied) return; //No deberia por Entity.cs
		if (spot.occupation == null) return; //No deberia por Entity.cs
		if (spot.occupation is Enemy) return; //Que no sea un enemigo
		level.PickBonus();
		//Animacion de ser cogido
		//Animar personaje
		Destroy(gameObject);
	}
}
