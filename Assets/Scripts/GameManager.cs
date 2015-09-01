using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float spawnPeriod = 2.0f;
	float nextSpawnRemaining;
	bool startSpawn;

	public GameObject[] heroPrefabs;
	public List<GameObject> heros;

	public enum team { ALPHA, BETA };

	public GameObject player;

	public Transform alphaHome;
	public Transform betaHome;

	// Use this for initialization
	void Start () {

		heros = new List<GameObject>();

		startSpawn = false;
		nextSpawnRemaining = spawnPeriod;

	}

	public void gameStart()
	{
		startSpawn = true;
		StartCoroutine ("SpawnMinion");
	}

	public void setPlayer(string heroName)
	{
		for (int i=0; i < heros.Count; i++) {
			if (heros [i].name == heroName + "(Clone)") {
				player = heros[i].gameObject;
				heros [i].GetComponent<HeroController> ().destinationPos = - Vector3.one;
				break;
			}
		}
	}

	public void addHeroObj(GameObject obj)
	{
		heros.Add(obj);
	}

	public GameObject getHeroPrefab(string heroName)
	{
		for (int i=0; i < heroPrefabs.Length; i++) {
			if (heroPrefabs [i].name == heroName)
				return heroPrefabs [i];
		}
		return null;
	}

	public GameObject getHeroObj(string heroName)
	{
		for (int i=0; i < heros.Count; i++) {
			if (heros [i].name == heroName)
				return heros [i];
		}
		return null;
	}

	IEnumerator SpawnMinion() {
	
		yield return new WaitForSeconds (1.0f);

		if (player.GetComponent<HeroController> ().checkTeam () == GameManager.team.ALPHA) {
			GameObject minionAlpha = PhotonNetwork.Instantiate ("minion_alpha", 
			                           alphaHome.position, Quaternion.identity, 0);

			if (minionAlpha != null) {
				minionAlpha.GetComponent<MinionController> ().initState (GameManager.team.ALPHA);
				minionAlpha.GetComponent<MinionController> ().resetState ();
			}
		}

		if (player.GetComponent<HeroController> ().checkTeam () == GameManager.team.BETA) {
			GameObject minionBeta = PhotonNetwork.Instantiate ("minion_beta", 
			                                                    betaHome.position, Quaternion.identity, 0);

			if (minionBeta != null) {
				minionBeta.GetComponent<MinionController> ().initState (GameManager.team.BETA);
				minionBeta.GetComponent<MinionController> ().resetState ();
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (startSpawn) {
			nextSpawnRemaining -= Time.deltaTime;

			if (nextSpawnRemaining <= 0) {
				nextSpawnRemaining = spawnPeriod;
				StartCoroutine ("SpawnMinion");
			}
		}

	}
}
