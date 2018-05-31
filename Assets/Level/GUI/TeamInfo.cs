using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamInfo : MonoBehaviour {

	public RectTransform playerVerticalLayout;
	public GameObject playerPanel;
	public RectTransform enemyVerticalLayout;
	public GameObject enemyPanel;

    [System.Serializable]
	public class Relation { public Entity entity; public EntityInfo info; }
	public List<Relation> playersInfo;
	public List<Relation> enemiesInfo;

	public void Init(List<Player> players, List<Enemy> enemies = null) {
		playersInfo = new List<Relation> ();
		for (int i = 0; i < players.Count; ++i) {
			Relation r = new Relation ();
			r.entity = players [i];
			r.info = Instantiate (playerPanel, playerVerticalLayout).GetComponent<EntityInfo> ();
			r.info.Init (r.entity.Name, r.entity.image, r.entity.actionsPerTurn);
			playersInfo.Add (r);
		}
		if (enemies != null) {
			enemiesInfo = new List<Relation> ();
			for (int i = 0; i < enemies.Count; ++i) {
				Relation r = new Relation ();
				r.entity = enemies [i];
				r.info = Instantiate (enemyPanel, enemyVerticalLayout).GetComponent<EntityInfo> ();
				r.info.Init (r.entity.Name, r.entity.image, r.entity.actionsPerTurn);
				enemiesInfo.Add (r);
			}
		}
	}
	public void RestartActions() {
		Debug.Log ("RestartActions");
		for (int i = 0; i < playersInfo.Count; ++i) {
			if (playersInfo [i].entity != null)
				playersInfo [i].info.RecoverActions (playersInfo [i].entity.actionsPerTurn);
		}
		if(enemiesInfo != null)
			for (int i = 0; i < enemiesInfo.Count; ++i) {
				if (enemiesInfo [i].entity != null)
					enemiesInfo [i].info.RecoverActions (enemiesInfo [i].entity.actionsPerTurn);
			}
	}

	EntityInfo selectedPlayer;
	Relation relationPlayer;
	public void SelectPlayer(Player player) {
		for (int i = 0; i < playersInfo.Count; ++i) {
			if (player == playersInfo [i].entity) {
				playersInfo [i].info.Select ();
				selectedPlayer = playersInfo [i].info;
				relationPlayer = playersInfo [i];
				return;
			}
		}
	}
	public void ClearSelectPlayer() {
		selectedPlayer.Unselect ();
		selectedPlayer = null;
		relationPlayer = null;
	}
	public void KillPlayer(Player player) {

	}
	public void UseActionPlayer() {
		relationPlayer.info.UseAction ();
	}

	EntityInfo selectedEnemy;
	Relation relationEnemy;
	public void SelectEnemy(Enemy enemy) {
		for (int i = 0; i < enemiesInfo.Count; ++i) {
			if (enemy == enemiesInfo [i].entity) {
				enemiesInfo [i].info.Select ();
				selectedEnemy = enemiesInfo [i].info;
				relationEnemy = enemiesInfo [i];
				return;
			}
		}
	}
	public void ClearSelectEnemy() {
		if (selectedEnemy == null) return;
		selectedEnemy.Unselect ();
		selectedEnemy = null;
		relationEnemy = null;
	}
	public void KillEnemy(Enemy enemy) {

	}
	public void UseActionEnemy() {
		relationEnemy.info.UseAction ();
	}
}
