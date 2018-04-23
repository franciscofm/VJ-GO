using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Entity: De la que extienden todos los objetos en nivel controlados
/// por Level.cs
/// </summary>
public class Entity : MonoBehaviour {

	public static Level level;

	public Vector3 position { 
		get { return transform.position; }
		set { transform.position = position; } 
	}
	public Spot spot; //Donde esta (si es clase Spot = null)

	void OnMouseDown() {
		level.Select (this);
	}
}
