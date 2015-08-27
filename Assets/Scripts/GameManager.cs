using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float spawnPeriod = 2.0f;
	float nextSpawnRemaining;

	public GameObject[] heroPrefabs;
	public List<GameObject> heros;

	public enum team { ALPHA, BETA };

	public GameObject player;

	public Transform alphaHome;
	public Transform betaHome;

	// Use this for initialization
	void Start () {

		heros = new List<GameObject>();

		nextSpawnRemaining = spawnPeriod;
		//StartCoroutine ("SpawnMinion");

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
			//StartCoroutine ("SpawnMinion");
		}

	}
}
