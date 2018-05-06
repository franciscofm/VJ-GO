using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Level : MonoBehaviour {

	public static Level instance;

	public TeamInfo teamInfo;
	public GameObject line;
	public Transform spotsParent;
	public List<Player> players;
	public List<Enemy> enemies;
	public List<Spot> spots;

	public AnimationCurve curveMovement;
	public Material bridgeNormalMaterial;
	public Material bridgeMarkedMaterial;

	uint turn;
	bool playerTurn;
	int teamTurn;
	float levelDuration;

	void Awake() {
		if (instance != null) Destroy (instance.gameObject);
		instance = this;
	}

	// Use this for initialization
	void Start () {
		Entity.level = this;

		turn = 0;
		levelDuration = 0f;
		playerTurn = true;

		StartCoroutine(DrawBridges ());
		Start2 ();
		EndTurn ();
	}
	protected virtual void Start2() {

	}

	protected virtual void EndAction() {
		if (playerTurn) {
			for (int i = 0; i < players.Count; ++i)
				if (players [i].actionsLeft > 0)
					return;
			playerTurn = false;
			EndTurn ();
		} else {
			for (int i = 0; i < enemies.Count; ++i)
				if (enemies [i].actionsLeft > 0) {
					EnemyIA (enemies [i]);
					return;
				}
			playerTurn = true;
			EndTurn ();
		}
	}
	protected virtual void EndTurn() {
		++turn;
		if (playerTurn) {
			for (int i = 0; i < players.Count; ++i)
				players [i].actionsLeft = players [i].actionsPerTurn;
			//Animacion de turno de jugador
			return;
		}
		if (!playerTurn) {
			for (int i = 0; i < enemies.Count; ++i)
				enemies [i].actionsLeft = enemies [i].actionsPerTurn;
			if (enemies.Count > 0)
				EnemyIA (enemies [0]);
			else {
				playerTurn = true;
				EndTurn ();
			}
		}
	}

	protected virtual void EnemyIA(Enemy enemy) {
		Debug.Log ("EnemyIA");
	}

	protected virtual IEnumerator DrawBridges() {
		spotsParent.GetComponentsInChildren<Spot> (spots);
		List<Spot> drawnSpots = new List<Spot> ();
		for (int i = 0; i < spots.Count; ++i) {
			drawnSpots.Add (spots [i]);
			List<Spot> toDraw = spots [i].bridges;
			for (int n = 0; n < toDraw.Count; ++n)
				if (!drawnSpots.Contains (toDraw [n])) {
					DrawBridge (spots [i], toDraw [n]);
					yield return null;
				}
		}
	}
	protected virtual void DrawBridge(Spot start, Spot end) {
		GameObject t = GameObject.Instantiate (this.line, start.transform);
		t.transform.position = (start.transform.position + end.transform.position) * 0.5f;
			#if UNITY_EDITOR 
			t.name = start.gameObject.name + " to " + end.gameObject.name; 
			#endif
		LineRenderer line = t.GetComponent<LineRenderer> ();
		line.positionCount = 2;
		line.material = bridgeNormalMaterial;
		line.SetPosition (0, start.transform.position);
		line.SetPosition (1, end.transform.position);
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

	Player selectedPlayer;
	List<Spot> selectedPlayerDestinations;
	Enemy selectedEnemy;

	public void Select<T>(T t) {
		if (t is Player) SelectPlayer (t as Player);
		else if (t is Enemy) SelectEnemy (t as Enemy);
		else
			Debug.Log ("Selected entity, ERROR");
	}

	public void SelectPlayer(Player player) {
		if (selectedPlayer == null) { 	//si no hay jugador seleccionado
			selectedPlayer = player;
			selectedPlayerDestinations = player.spot.bridges;
			if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
			teamInfo.SelectPlayer (player);
			player.Select ();
		} else { 						//si hay un jugador seleccionado
			if (playerTurn) UnmarkBridges (player);
			teamInfo.ClearSelectPlayer ();
			player.Unselect ();
			if (selectedPlayer == player) { //si es el mismo -> no hay
				selectedPlayer = null;
				selectedPlayerDestinations = null;
			} else {
				selectedPlayer = player;	//si es otro -> cambiamos
				selectedPlayerDestinations = player.spot.bridges;
				if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
				teamInfo.SelectPlayer (player);
				player.Select ();
			}
		}
	}
	public void SelectEnemy(Enemy enemy) {
		teamInfo.ClearSelectEnemy ();
		teamInfo.SelectEnemy (enemy);
		if (selectedPlayer != null) {
			selectedEnemy = enemy;
			//TODO show actions
			selectedPlayer.GetActions();
			selectedEnemy.ShowActions();
		}
	}
	public void SelectSpot(Spot spot) {
		if (!playerTurn) return;
		if (selectedPlayer == null || selectedPlayerDestinations == null) return;
		if (selectedPlayer.actionsLeft <= 0) return;
		bool found = false;
		for (int i = 0; !found && i < selectedPlayerDestinations.Count; ++i) {
			if (spot == selectedPlayerDestinations [i]) {
				UnmarkBridges (selectedPlayer);
				teamInfo.ClearSelectPlayer ();
				selectedPlayer.Move (selectedPlayer, spot.transform.position + Vector3.up, 1f, curveMovement, EndAction);
				//occupation
				selectedPlayer.spot.occupied = false;  	//old
				selectedPlayer.spot.occupation = null; 	//old
				selectedPlayer.spot.Leave();			//old
				selectedPlayer.spot = spot;			   	//new
				spot.occupied = true;					//new
				spot.occupation = selectedPlayer;		//new

				--selectedPlayer.actionsLeft;
				found = true;
				selectedPlayer = null;
				selectedPlayerDestinations = null;
			}
		}
	}

	void MarkBridges(Player player) {
		List<Spot> bridges = player.spot.bridges;
		List<LineRenderer> bridgesLines = player.spot.lines;
		for (int i = 0; i < bridges.Count; ++i) {
			//if (!bridges [i].occupied) {
				bridgesLines[i].material = bridgeMarkedMaterial;
			//}
		}
	}
	void UnmarkBridges(Player player) {
		List<Spot> bridges = player.spot.bridges;
		List<LineRenderer> bridgesLines = player.spot.lines;
		for (int i = 0; i < bridges.Count; ++i) {
			//if (!bridges [i].occupied) {
				bridgesLines[i].material = bridgeNormalMaterial;
			//}
		}
	}
}
