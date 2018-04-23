using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour {

	public GameObject line;
	public Transform spotsParent;
	public List<Player> players;
	public List<Enemy> enemies;
	public List<Spot> spots;

	uint turn;
	bool playerTurn;
	int teamTurn;
	float levelDuration;

	// Use this for initialization
	void Start () {
		turn = 0;
		levelDuration = 0f;
		DrawBridges ();
		Init ();
	}
	protected virtual void Init() {

	}
	protected virtual void DrawBridges() {
		spotsParent.GetComponentsInChildren<Spot> (spots);
		List<Spot> drawnSpots = new List<Spot> ();
		for (int i = 0; i < spots.Count; ++i) {
			List<Spot> toDraw = spots [i].bridges;
			for (int n = 0; n < toDraw.Count; ++n)
				if (!drawnSpots.Contains (toDraw [n])) {
					DrawBridge (spots [i], toDraw [n]);
				}
			drawnSpots.Add (spots [i]);
		}
	}
	protected virtual void DrawBridge(Spot start, Spot end) {
		GameObject t = GameObject.Instantiate (this.line, transform);
		LineRenderer line = t.GetComponent<LineRenderer> ();
		line.positionCount = 2;
		line.SetPosition (0, start.position);
		line.SetPosition (1, end.position);
		start.AddLine (line);
		end.AddLine (line);
		//Utils.CopyLineRenderer (this.line, line);
	}
	
	// Update is called once per frame
	void Update () {
		float delta = Time.deltaTime;
		levelDuration += delta;
		Update2 (delta);	
	}
	protected virtual void Update2(float delta) {

	}

	public void Move(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction) {
		if (duration == 0f) {
			obj.position = newPos;
		} else {
			StartCoroutine (MoveRoutine (obj, newPos, duration, curve, endAction));
		}
	}
	IEnumerator MoveRoutine(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction) {
		float t = 0f;
		Vector3 startPos = obj.position;
		while (t < duration) {
			yield return null;
			t += Time.deltaTime;
			obj.position = Vector3.Lerp(obj.position, newPos, curve.Evaluate(t/duration));
		}
		obj.position = newPos;
		if (endAction != null)
			endAction ();
	}

	Player selectedPlayer;
	Enemy selectedEnemy;

	public void Select<T>(T t) {
		//Type type = typeof(T);
		if (t is Player)
			SelectPlayer (t as Player);
		else if (t is Spot)
			SelectSpot (t as Spot);
		else if (t is Enemy)
			SelectEnemy (t as Enemy);
		else
			Debug.Log ("Selected entity, wrong coding");
	}

	public void SelectPlayer(Player player) {
		if (playerTurn) {
			MarkBridges (player);
		}
		selectedPlayer = player;
		//Display info panel
	}
	void MarkBridges(Player player) {
		List<Spot> bridges = player.spot.bridges;
		for (int i = 0; i < bridges.Count; ++i) {
			if (!bridges [i].occupied)
				Debug.Log ("Implement me"); // Remarcar dibujo del camino
		}
	}

	public void SelectEnemy(Enemy enemy) {
		selectedEnemy = enemy;
		//Display info panel
	}
	public void SelectSpot(Spot spot) {
		//Si es el turno de
	}
}
