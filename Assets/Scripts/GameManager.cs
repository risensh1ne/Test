using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public float spawnPeriod = 2.0f;
	float nextSpawnRemaining;

	public enum team { ALPHA, BETA };

	// Use this for initialization
	void Start () {

		nextSpawnRemaining = spawnPeriod;
		StartCoroutine ("SpawnMinion");
	}

	IEnumerator SpawnMinion() {

		yield return new WaitForSeconds (1.0f);
/*
		GameObject minionAlpha = ObjectPool.instance.GetObjectForType ("minion_alpha", true);
		if (minionAlpha != null) {
			minionAlpha.GetComponent<MinionController>().OnSpawn();
		}
*/
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
