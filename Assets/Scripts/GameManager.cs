using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public float spawnPeriod = 2.0f;
	float nextSpawnRemaining;
	bool startSpawn;

	public string myHeroName;
	public string userName;
	public GameManager.team myTeam;
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

		userName = PlayerPrefs.GetString ("userName");
		myHeroName = PlayerPrefs.GetString ("heroName");
		myTeam = (GameManager.team)PlayerPrefs.GetInt ("userTeam");
	
		Debug.Log(userName + " " + myHeroName + " " + myTeam);
		GameObject obj = SpawnHero ();
		setPlayer ();
		GetComponent<ObjectPool> ().Initialize (myTeam);
		gameObject.GetComponent<UIManager> ().initializeUI ();
		gameStart ();
	}

	public void gameStart()
	{
		startSpawn = true;
		StartCoroutine ("SpawnMinion");
	}

	public void setPlayer()
	{
		for (int i=0; i < heros.Count; i++) {
			if (heros [i].name == myHeroName + "(Clone)") {
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

	GameObject SpawnHero()
	{
		Vector3 startPos, destPos;
		
		if (myTeam == GameManager.team.BETA) {
			startPos = betaHome.transform.position;
			destPos = alphaHome.transform.position;
		} else {
			startPos = alphaHome.transform.position;
			destPos = betaHome.transform.position;
		}
		
		Vector3 direction = (destPos - startPos).normalized;
		
		GameObject heroObj = PhotonNetwork.Instantiate (myHeroName, 
		                                                startPos, Quaternion.LookRotation (direction), 0);
		
		heroObj.GetComponent<HeroController> ().init_hero (myTeam, startPos, destPos);
		
		addHeroObj (heroObj);
		
		return heroObj;
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
