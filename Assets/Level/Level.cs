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

	public AnimationCurve curveMovement;

	uint turn;
	bool playerTurn;
	int teamTurn;
	float levelDuration;

	// Use this for initialization
	void Start () {
		Entity.level = this;

		turn = 0;
		levelDuration = 0f;
		playerTurn = true;

		DrawBridges ();
		Init ();
	}
	protected virtual void Init() {

	}


	protected virtual void EndAction() {

	}

	protected virtual void EndTurn() {
		++turn;
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

	//Mover una entidad de forma generica
	public void Move(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction = null) {
		if (duration == 0f) {
			obj.position = newPos;
		} else {
			StartCoroutine (MoveRoutine (obj, newPos, duration, curve, endAction));
		}
	}
	IEnumerator MoveRoutine(Entity obj, Vector3 newPos, float duration, AnimationCurve curve, Action endAction = null) {
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
	List<Spot> selectedPlayerDestinations;
	Enemy selectedEnemy;

	public void Select<T>(T t) {
		//Type type = typeof(T);
		Debug.Log(t);
		if (t is Player)
			SelectPlayer (t as Player);
		else if (t is Spot)
			SelectSpot (t as Spot);
		else if (t is Enemy)
			SelectEnemy (t as Enemy);
		else
			Debug.Log ("Selected entity, ERROR");
	}

	public void SelectPlayer(Player player) {
		if (selectedPlayer == null) { 	//si no hay jugador seleccionado
			selectedPlayer = player;
			selectedPlayerDestinations = player.spot.bridges;
			if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
			player.DisplayInfo ();
		} else { 						//si hay un jugador seleccionado
			if (playerTurn) UnmarkBridges (player);
			player.HideInfo ();
			if (selectedPlayer == player) { //si es el mismo -> no hay
				selectedPlayer = null;
				selectedPlayerDestinations = null;
			} else {
				selectedPlayer = player;	//si es otro -> cambiamos
				selectedPlayerDestinations = player.spot.bridges;
				if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
				player.DisplayInfo ();
			}
		}
	}
	void MarkBridges(Player player) {
		List<Spot> bridges = player.spot.bridges;
		for (int i = 0; i < bridges.Count; ++i) {
			if (!bridges [i].occupied)
				Debug.Log ("Implement me"); // Remarcar dibujo del camino
		}
	}
	void UnmarkBridges(Player player) {
		List<Spot> bridges = player.spot.bridges;
		for (int i = 0; i < bridges.Count; ++i) {
			if (!bridges [i].occupied)
				Debug.Log ("Implement me"); // Remarcar dibujo del camino
		}
	}

	public void SelectEnemy(Enemy enemy) {
		if (selectedEnemy == null) { 	//si no hay jugador seleccionado
			selectedEnemy = enemy;
			enemy.DisplayInfo ();
		} else { 						//si hay un jugador seleccionado
			enemy.HideInfo ();
			if (selectedEnemy == enemy) { //si es el mismo -> no hay
				selectedEnemy = null;
			} else {
				selectedEnemy = enemy;	//si es otro -> cambiamos
				enemy.DisplayInfo ();
			}
		}
	}
	public void SelectSpot(Spot spot) {
		if (!playerTurn) return;
		if (selectedPlayer == null || selectedPlayerDestinations == null) return;
		if (selectedPlayer.actionsLeft <= 0) return;

		for (int i = 0; i < selectedPlayerDestinations.Count; ++i) {
			if (spot == selectedPlayerDestinations [i]) {
				Move (selectedPlayer, spot.transform.position, 1f, curveMovement);
				--selectedPlayer.actionsLeft;
			}
		}
	}
}
