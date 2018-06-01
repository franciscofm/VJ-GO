using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

using Audio;

public class Level : MonoBehaviour {
	
	public static Level instance;
	public static Menu.Controller menu;

	[Header("Testing")]
	public bool debug = true;
	public GameObject eventSystem;
	[Header("Scene references")]
	public TeamInfo teamInfo;
	public Chat chat;
	public CameraMovement cam;
	[Header("Entities")]
	public Transform spotsParent;
	public Transform playersParent;
	public Transform enemiesParent;
	[HideInInspector] public List<Player> players;
	[HideInInspector] public List<Enemy> enemies;
	[HideInInspector] public List<Spot> spots;
	[Header("Bridges")]
	public AnimationCurve curveMovement;
	public GameObject line;
	public Material bridgeNormalMaterial;
	public Material bridgeMarkedMaterial;

	[Header("Music & Sounds")]
	public AudioClip music;
	Sound musicSound;

	[Header("Private")]
	public uint turn;
	public bool playerTurn;
	public int teamTurn;
	public float levelDuration;

	public uint totalBonus, pickedBonus;
	public uint totalPlayers, finishedPlayers;
	public uint totalEnemies; //, killedEnemies;

	public bool teamInfoEnabled;
	public bool blocked;
	public bool gameInitialized = false;
	public bool bridgesDrawn;

	void Awake() {
		if (instance != null) Destroy (instance.gameObject);
		instance = this;
		if (!debug) {
			menu.loadedlevel = this;
			if(eventSystem != null)
				eventSystem.SetActive (false);
		}

		Entity.level = this;
		Bonus.level = this;
	}


	void Start () {
		turn = 0;
		levelDuration = 0f;
		playerTurn = true;

		spotsParent.GetComponentsInChildren<Spot> (spots);
		playersParent.GetComponentsInChildren<Player> (players);
		enemiesParent.GetComponentsInChildren<Enemy> (enemies);

		totalPlayers = (uint)players.Count;
		totalEnemies = (uint)enemies.Count;
		finishedPlayers = 0;
		teamInfoEnabled = false;
		bridgesDrawn = false;

		cam.cameraRotation = (players [0].transform.position - cam.transform.position).normalized;
		cam.zoomScale = Values.Camera.ZoomMax;
		cam.Focus (players [0].transform);

		musicSound = ManagerSound.PlaySound (music, cam.transform, true, ManagerSound.Type.Music);
		StartCoroutine (ChangeVolumeRoutine (musicSound.source, 0f, musicSound.source.volume, Values.Music.TurnOn));

		StartCoroutine(DrawBridges ());
		Start2 ();
		gameInitialized = true;
        if (debug) StartLevel();
		EndTurn ();
	}
	protected virtual void Start2() {

	}
	//Called by controller, allows movement
	public virtual void StartLevel() {
		StartCoroutine(StartLevelRoutine());
	}
	protected virtual IEnumerator StartLevelRoutine() {
		while (!gameInitialized)
			yield return null;
		blocked = false;
		if (players.Count > 1 || enemies.Count > 0) {
			teamInfoEnabled = true;
			teamInfo.Init (players, enemies);
			//TODO: HUD entra aqui
		}
	}
	protected virtual IEnumerator ChangeVolumeRoutine(AudioSource source, float a, float b, float d) {
		source.volume = a;
		float t = 0f;
		while (t < d) {
			yield return null;
			t += Time.deltaTime;
			source.volume = Mathf.Lerp (a, b, t / d);
		}
	}

	//Called by Start
	protected virtual IEnumerator DrawBridges() {
		List<Spot> drawnSpots = new List<Spot> ();
		for (int i = 0; i < spots.Count; ++i) {
			drawnSpots.Add (spots [i]);
			List<Spot> toDraw = spots [i].bridges;
			for (int n = 0; n < toDraw.Count; ++n) {
				if (!drawnSpots.Contains (toDraw [n])) {
					DrawBridge (spots [i], toDraw [n]);
				}
				if(n != toDraw.Count -1)
					yield return null;
			}
		}
		bridgesDrawn = true;
	}
	//Called by DrawBridges
	protected virtual void DrawBridge(Spot start, Spot end) {
		GameObject t = GameObject.Instantiate (this.line, start.transform);
		t.transform.position = (start.transform.position + end.transform.position) * 0.5f;
		#if UNITY_EDITOR 
		t.name = start.gameObject.name + " to " + end.gameObject.name; 
		#endif
		LineRenderer line = t.GetComponent<LineRenderer> ();
		line.positionCount = 2;
		line.material = bridgeNormalMaterial;

		Vector3 dir = end.transform.position - start.transform.position;
		dir = dir.normalized * Values.Spot.OffsetLines;
		line.SetPosition (0, start.transform.position + dir);
		line.SetPosition (1, end.transform.position - dir);

		start.AddLine (line, end);
		end.AddLine (line, start);
	}
	public virtual LineRenderer DrawBridgeVisual(Spot start, Spot end) {
		GameObject t = GameObject.Instantiate (this.line, start.transform);
		t.transform.position = (start.transform.position + end.transform.position) * 0.5f;
		#if UNITY_EDITOR 
		t.name = start.gameObject.name + " to " + end.gameObject.name; 
		#endif
		LineRenderer line = t.GetComponent<LineRenderer> ();
		line.positionCount = 2;
		line.material = bridgeNormalMaterial;

		Vector3 dir = end.transform.position - start.transform.position;
		dir = dir.normalized * Values.Spot.OffsetLines;
		line.SetPosition (0, start.transform.position + dir);
		line.SetPosition (1, end.transform.position - dir);

		return line;
	}

	//Called after select spot and moving a player
	protected virtual void EndActionPlayer(Player current) {
		if (playerTurn) {
			if (current.spot.type == Spot.Type.End) {
				++finishedPlayers;
				players.Remove (current);
				current.spot.occupied = false;
				current.spot.occupation = null;
				//TODO: 
				current.Finish(delegate {
					if (players.Count == 0) {
						EndLevel ();
						return;
					}
				});
			}
			for (int i = 0; i < players.Count; ++i)
				if (players [i].actionsLeft > 0)
					return;
			playerTurn = false;
			EndTurn ();
		}
	}
	//Called by Enemy after IA
	public virtual void EndActionEnemy(Enemy current) {
		teamInfo.UseActionEnemy ();
		teamInfo.ClearSelectEnemy ();
		if (!playerTurn) {
			for (int i = 0; i < enemies.Count; ++i)
				if (enemies [i].actionsLeft > 0) {
					EnemyIA (enemies [i]);
					return;
				}
			playerTurn = true;
			teamInfo.RestartActions ();
			if(players.Count > 0)
				cam.Focus (players [0].transform);
			EndTurn ();
		}
	}
	//Called by EndActionPlayer & EndActionEnemy
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
	//Called by EndActionPlayer
	protected virtual void EndLevel() {
		if (finishedPlayers == totalPlayers) { 
			Debug.Log ("Passed level");
			StartCoroutine (ChangeVolumeRoutine (musicSound.source, musicSound.source.volume, 0f, Values.Music.TurnOn));
			if(!debug)
				menu.FinishLevel ();
		} else Debug.Log ("Failed level");
	}

	protected virtual void EnemyIA(Enemy enemy) {
		cam.Focus (enemy.transform, delegate {
			cam.Follow(enemy.transform);
			//cam.target = enemy.transform;
			teamInfo.SelectEnemy (enemy);
			enemy.IA ();
		});
	}
		

	void Update () {
		float delta = Time.deltaTime;
		levelDuration += delta;
		Update2 (delta);
	}
	protected virtual void Update2(float delta) {

	}

	public Player selectedPlayer;
	public List<Spot> selectedPlayerDestinations;
	public Enemy selectedEnemy;
	public bool entityActing;

	public void Select<T>(T t) {
		if (entityActing || blocked) return;
		if (EventSystem.current.IsPointerOverGameObject ()) return; //Chat and GUI escape
		if (t is Player) SelectPlayer (t as Player);
		else if (t is Enemy) SelectEnemy (t as Enemy);
		else Debug.Log ("Selected entity, ERROR");
	}

	public void SelectPlayer(Player player) {
		if (selectedPlayer == null) { 	//si no hay jugador seleccionado
			selectedPlayer = player;
			selectedPlayerDestinations = player.spot.bridges;
			if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
			if(teamInfoEnabled) teamInfo.SelectPlayer (player);
			player.Select ();
		} else { 						//si hay un jugador seleccionado
			if (playerTurn) UnmarkBridges (player);
			if(teamInfoEnabled) teamInfo.ClearSelectPlayer ();
			player.Unselect ();
			if (selectedPlayer == player) { //si es el mismo -> no hay
				selectedPlayer = null;
				selectedPlayerDestinations = null;
			} else {
				selectedPlayer = player;	//si es otro -> cambiamos
				selectedPlayerDestinations = player.spot.bridges;
				if (playerTurn && player.actionsLeft > 0) MarkBridges (player);
				if(teamInfoEnabled) teamInfo.SelectPlayer (player);
				player.Select ();
			}
		}
	}
	public void SelectEnemy(Enemy enemy) {
		if (selectedEnemy == null) {
			teamInfo.SelectEnemy (enemy);
			selectedEnemy = enemy;
		} else {
			teamInfo.ClearSelectEnemy ();
			selectedEnemy = null;
		}
		if (selectedPlayer != null) {
			selectedPlayer.GetActions();
			selectedEnemy.ShowActions();
		}
	}
	public void SelectSpot(Spot spot) {
		if (!playerTurn) return;
		if (entityActing) return;
		if (selectedPlayer == null || selectedPlayerDestinations == null) return;
		if (selectedPlayer.actionsLeft <= 0) return;
		if (spot.occupied) return;
		bool found = false;
		for (int i = 0; !found && i < selectedPlayerDestinations.Count; ++i) {
			if (spot == selectedPlayerDestinations [i]) {
				UnmarkBridges (selectedPlayer);
				if (teamInfoEnabled) {
					teamInfo.UseActionPlayer ();
					teamInfo.ClearSelectPlayer ();
					teamInfo.ClearSelectEnemy ();
				}
				selectedEnemy = null;

				entityActing = true;
				cam.Follow (selectedPlayer.transform);
				selectedPlayer.Move (selectedPlayer, spot, 1f, curveMovement, delegate {
					cam.FreeCamera();
					EndActionPlayer(selectedPlayer);
					selectedPlayer = null;
					selectedPlayerDestinations = null;
					entityActing = false;
				});

				--selectedPlayer.actionsLeft;
				found = true;
			}
		}
	}

	void MarkBridges(Player player) {
		List<Spot.BridgeToLine> bridges = player.spot.bridgesToLines;
		for (int i = 0; i < bridges.Count; ++i) {
			if (!bridges [i].spot.occupied) {
				bridges[i].line.material = bridgeMarkedMaterial;
			}
		}
	}
	void UnmarkBridges(Player player) {
		if (player.spot == null) return;
		List<Spot.BridgeToLine> bridges = player.spot.bridgesToLines;
		for (int i = 0; i < bridges.Count; ++i) {
			bridges [i].line.material = bridgeNormalMaterial;
		}
	}

	public void PickBonus() {
		++pickedBonus;
		//Animacion bonus cogido
	}
	public Transform RequestTarget() {
		for (int i = 0; i < players.Count; ++i) {
			if (players [i].actionsLeft > 0)
				return players [i].transform;
		}
		return null;
	}
	public float GetCompletion() {
		return (1f + pickedBonus) / (1f + totalBonus);
	}
}
