using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCaller : MonoBehaviour {

	public ContentAssigner contr;
	public int priority;
	void OnMouseDown() {
		contr.PlaySound (priority);
	}
}
