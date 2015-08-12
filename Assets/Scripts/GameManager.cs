using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public float spawnPeriod = 2.0f;
	float nextSpawnRemaining;

	public GameObject[] heroPrefabs;
	public GameObject[] heros;

	public enum team { ALPHA, BETA };

	public GameObject player;

	public Transform alphaHome;
	public Transform betaHome;

	// Use this for initialization
	void Start () {

		heros = new GameObject[1];

		GameObject heroObj = Instantiate (heroPrefabs [0]) as GameObject;
		heroObj.GetComponent<HeroController>().isMine = true;
		heroObj.GetComponent<HeroController> ().attachedTeam = team.BETA;
		heroObj.transform.position = betaHome.transform.position;
		heroObj.GetComponent<HeroController> ().destinationPos = - Vector3.one;
		heroObj.name = "Gion";
		player = heroObj;
		heros [0] = heroObj;

		/*
		heroObj = Instantiate (heroPrefabs [1]) as GameObject;
		heroObj.GetComponent<HeroController>().isMine = false;
		heroObj.GetComponent<HeroController> ().attachedTeam = team.ALPHA;
		heroObj.transform.position = alphaHome.transform.position;
		heroObj.GetComponent<HeroController>().destinationPos = betaHome.transform.position;
		heroObj.name = "SwordMaster";
		heros [1] = heroObj;
		*/

		nextSpawnRemaining = spawnPeriod;
		StartCoroutine ("SpawnMinion");

	}

	public GameObject getHeroObj(string heroName)
	{
		for (int i=0; i < heros.Length; i++) {
			if (heros [i].name == heroName)
				return heros [i];
		}
		return null;
	}

	IEnumerator SpawnMinion() {

		yield return new WaitForSeconds (1.0f);

		GameObject minionAlpha = ObjectPool.instance.GetObjectForType ("minion_alpha", true);
		if (minionAlpha != null) {
			minionAlpha.GetComponent<MinionController> ().OnSpawn ();
		}

		GameObject minionBeta = ObjectPool.instance.GetObjectForType ("minion_beta", true);
		if (minionBeta != null) {
			minionBeta.GetComponent<MinionController>().OnSpawn();
		}

	}
	
	// Update is called once per frame
	void Update () {

		nextSpawnRemaining -= Time.deltaTime;

		if (nextSpawnRemaining <= 0) {
			nextSpawnRemaining = spawnPeriod;
			StartCoroutine ("SpawnMinion");
		}

	}
}
