using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour {

	public bool occupied;
	public Entity occupation;
	public List<Spot> bridges = new List<Spot>();
	public List<LineRenderer> lines = new List<LineRenderer>();

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

	public void AddLine(LineRenderer line) {
		lines.Add (line);
	}
	public void RemoveLine(LineRenderer line) {
		lines.Remove (line);
		for(int i=0; i<bridges.Count; ++i)
			if(bridges[i].lines.Contains(line)) {
				bridges [i].lines.Remove(line);
				return;
			}
	}
}
