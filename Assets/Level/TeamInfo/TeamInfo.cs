using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamInfo : MonoBehaviour {

	public RectTransform playerVerticalLayout;
	public GameObject playerPanel;
	public RectTransform enemyVerticalLayout;
	public GameObject enemyPanel;

	class Relation { public Entity entity; public EntityInfo info; }
	List<Relation> playersInfo;
	List<Relation> enemiesInfo;

	public void Init(List<Player> players, List<Enemy> enemies = null) {
		playersInfo = new List<Relation> ();
		for (int i = 0; i < players.Count; ++i) {
			Relation r = new Relation ();
			r.entity = players [i];
			r.info = Instantiate (playerPanel, playerVerticalLayout).GetComponent<EntityInfo> ();
			r.info.Init (r.entity.Name, r.entity.image);
			playersInfo.Add (r);
		}
		if (enemies != null) {
			enemiesInfo = new List<Relation> ();
			for (int i = 0; i < enemies.Count; ++i) {
				Relation r = new Relation ();
				r.entity = enemies [i];
				r.info = Instantiate (enemyPanel, enemyVerticalLayout).GetComponent<EntityInfo> ();
				r.info.Init (r.entity.Name, r.entity.image);
				enemiesInfo.Add (r);
			}
		}
	}

	EntityInfo selectedPlayer;
	public void SelectPlayer(Player player) {
		for (int i = 0; i < playersInfo.Count; ++i) {
			if (player == playersInfo [i].entity) {
				playersInfo [i].info.Select ();
				selectedPlayer = playersInfo [i].info;
				return;
			}
		}
	}
	public void ClearSelectPlayer() {
		selectedPlayer.Unselect ();
		selectedPlayer = null;
	}
	public void KillPlayer(Player player) {

	}

	EntityInfo selectedEnemy;
	public void SelectEnemy(Enemy enemy) {
		for (int i = 0; i < enemiesInfo.Count; ++i) {
			if (enemy == enemiesInfo [i].entity) {
				enemiesInfo [i].info.Select ();
				selectedEnemy = playersInfo [i].info;
				return;
			}
		}
	}
	public void ClearSelectEnemy() {
		selectedEnemy.Unselect ();
		selectedEnemy = null;
	}
	public void KillEnemy(Enemy enemy) {

	}
}
