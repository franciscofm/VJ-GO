using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Entity: De la que extienden todos los objetos en nivel controlados
/// por Level.cs
/// </summary>
public class Entity : MonoBehaviour {

	public static Level level;

	public Spot spot; //Donde esta (si es clase Spot = null)

	void OnMouseUp() {
		level.Select (this);
	}

	public virtual void DisplayInfo() {
		Debug.Log ("DisplayInfo not written");
	}
	public virtual void HideInfo() {
		Debug.Log ("HideInfo not written");
	}
}
