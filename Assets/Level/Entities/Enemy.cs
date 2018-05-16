using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	public List<Spot> Route;

	int currentSpot = -1;
	public virtual void IA() {
		//Debug.Log ("Hello");
		if (actionsLeft <= 0) return;
		if (currentSpot < 0) currentSpot = Route.IndexOf (spot);
		Move (this, Route [(currentSpot + 1) % Route.Count], 0.2f, Level.instance.curveMovement, delegate {
			++currentSpot;
			Level.instance.EndActionEnemy (this);
		});
		--actionsLeft;
	}
}
