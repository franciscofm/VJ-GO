using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Spot : MonoBehaviour {

	public enum Type { Start, Normal, End };
	public Type type = Type.Normal;
	public bool occupied;
	public Entity occupation;
	public List<Spot> bridges = new List<Spot>();

	public List<BridgeToLine> bridgesToLines = new List<BridgeToLine> ();
	public class BridgeToLine {
		public Spot spot;
		public LineRenderer line;
	}

	void Start() {
		if(type == Type.Start) GetComponent<MeshRenderer> ().material.color = Color.yellow;
		if(type == Type.End) GetComponent<MeshRenderer> ().material.color = Color.cyan;
	}

	void OnMouseUp() {
		Level.instance.SelectSpot (this);
	}

	public void CreateBridges(List<Spot> spots) {
		for (int i = 0; i < spots.Count; ++i) {
			bridges.Add (spots [i]);
			spots [i].AddBridge (this);
		}
	}
	public void AddBridge(Spot spot) {
		bridges.Add (spot);
	}

	public void DestroyBridges() {
		while (bridges.Count > 0) {
			bridges [0].RemoveBridge (this);
			bridges.RemoveAt (0);
		}
	}
	public void RemoveBridge(Spot spot) {
		bridges.Remove (spot);
	}

	public void AddLine(LineRenderer line, Spot spot) {
		BridgeToLine newLine = new BridgeToLine ();
		newLine.line = line;
		newLine.spot = spot;
		bridgesToLines.Add (newLine);
	}

//	public void RemoveLine(LineRenderer line) {
//		lines.Remove (line);
//		for(int i=0; i<bridges.Count; ++i)
//			if(bridges[i].lines.Contains(line)) {
//				bridges [i].lines.Remove(line);
//				return;
//			}
//	}

	public delegate void SpotEvent();

	public event SpotEvent OnEnter;
	public void Entered() {
		Debug.Log ("Entered event raised: " + gameObject.name);
		if(OnEnter!=null) OnEnter ();
	}
	public event SpotEvent OnLeave;
	public void Leave() {
		Debug.Log ("Leave event raised: " + gameObject.name);
		if(OnLeave!=null) OnLeave ();
	}
	public event SpotEvent OnStay;
	public void Stay() {
		Debug.Log ("Stay event raised: " + gameObject.name);
		if(OnStay!=null) OnStay ();
	}
}
