using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity {

	public int actionsPerTurn = 1;
	public int actionsLeft;

	void Start() {
		actionsLeft = actionsPerTurn;
	}
}
