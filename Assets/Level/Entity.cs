using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Entity: De la que extienden todos los objetos en nivel controlados
/// por Level.cs
/// </summary>
public class Entity : MonoBehaviour {

	public static Level level;

	public string Name = "Unknown";
	public string description = "";
	public int actionsPerTurn = 1;
	public int actionsLeft;
	public int team = 0;
	public Color color = Color.white;
	public Spot spot; //Donde esta (si es clase Spot = null)

	void OnMouseUp() {
		level.Select (this);
	}
}
