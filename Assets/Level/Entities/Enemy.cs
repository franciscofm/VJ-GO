using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity {

	public List<Spot> Route;
	public float moveSpeed = 0.6f;
	int currentSpot = -1;
	public virtual void IA() {
		//Debug.Log ("Hello");
		if (actionsLeft <= 0) return;
		if (currentSpot < 0) currentSpot = Route.IndexOf (spot);
		--actionsLeft;
		if(Route [(currentSpot + 1) % Route.Count].occupied)
			Level.instance.EndActionEnemy (this);
		else
			Move (this, Route [(currentSpot + 1) % Route.Count], moveSpeed, Level.instance.curveMovement, delegate {
				++currentSpot;
				Level.instance.EndActionEnemy (this);
			});
	}

	public Entity target;
	public Spot next;
	void Seek() { //Move to closest, no pathfinding
		List<Spot> bridges = spot.bridges;
		Vector3 targetPos = target.transform.position;
		float d = Vector3.Distance (targetPos, bridges [0].transform.position);
		int index = 0;
		for (int i = 1; i < bridges.Count; ++i) {
			float d2 = Vector3.Distance (targetPos, bridges [i].transform.position);
			if (d2 < d) {
				index = i;
				d = d2;
			}
		}
		Move (this, bridges [index], 0.2f, Level.instance.curveMovement, delegate {
			Level.instance.EndActionEnemy (this);
		});
		--actionsLeft;
	}

	void A() { //Move to closest, with pathfinding
		--actionsLeft;
		bool success = BacktrackA (spot);
		if (success) 
			Move (this, next, 0.2f, Level.instance.curveMovement, delegate {
				Level.instance.EndActionEnemy (this);
			});
		else 
			Pass ();
		
	}
	bool BacktrackA(Spot spot) {
		if (spot.occupation == target) return true;

		List<Spot> bridges = spot.bridges;
		if (bridges.Count == 0) return false;
		List<Spot> toVisit = new List<Spot> ();

		toVisit.Add (bridges [0]);
		for (int i = 1; i < bridges.Count; ++i) {
			bool added = false;
			float d1 = Vector3.Distance (bridges[i].transform.position, target.transform.position);
			for (int j = 0; j < toVisit.Count; ++j) {
				float d2 = Vector3.Distance (toVisit[j].transform.position, target.transform.position);
				if (d2 < d1) {
					added = true;
					toVisit.Insert (j, bridges [i]);
				}
			}
			if (!added) toVisit.Add (bridges [i]);
		}
		for (int i = 0; i < toVisit.Count; ++i) {
			bool success = BacktrackA (toVisit [i]);
			if (success) {
				next = toVisit [i];
				return true;
			}
		}

		return false;
	}
}
