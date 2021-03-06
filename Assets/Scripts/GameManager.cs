﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public GameObject UI;

	public float spawnPeriod = 10.0f;
	float nextSpawnRemaining;
	private int minionSpawnCount = 3;
	private bool startSpawn;

	public string myHeroName;
	public string userName;
	public GameManager.team myTeam;

	public enum team { ALPHA, BETA };

	public GameObject player;

	public Transform alphaHome;
	public Transform betaHome;


    /*
    private void TestConnect()
    {
        PhotonNetwork.player.name = "seo";
        PhotonNetwork.ConnectUsingSettings("risenhine games 001");
    }

    void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom("room1", roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        PlayerPrefs.SetString("userName", "seo");
        PlayerPrefs.SetString("heroName", "SwordMaster");
        PlayerPrefs.SetInt("userTeam", (int)GameManager.team.ALPHA);

        userName = PlayerPrefs.GetString("userName");
        myHeroName = PlayerPrefs.GetString("heroName");
        myTeam = (GameManager.team)PlayerPrefs.GetInt("userTeam");

        gameStart();
    }
    */



	// Use this for initialization
	void Start () {
        startSpawn = false;
		nextSpawnRemaining = spawnPeriod;

        //TestConnect();
		//gameStart ();
	}


	public void gameStart()
	{
        Debug.Log("game start");


        userName = PlayerPrefs.GetString("userName");
        myHeroName = PlayerPrefs.GetString("heroName");
        myTeam = (GameManager.team)PlayerPrefs.GetInt("userTeam");
        
		//Debug.Log ("------>" + userName + "," + myHeroName + "," + myTeam);

        GameObject obj = SpawnHero();

        player = obj;
        obj.GetComponent<HeroController>().heroName = myHeroName;
        obj.GetComponent<HeroController>().destinationPos = -Vector3.one;
        
        GetComponent<ObjectPool>().Initialize(myTeam);
        GameObject.Find("UI").GetComponent<UIManager>().initializeUI();

        startSpawn = true;
		StartCoroutine ("SpawnMinion");
    }

    /*
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
    */

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

        Debug.Log(myHeroName + " Spawned!!");
		Vector3 direction = (destPos - startPos).normalized;

        object[] data = new object[2];
        data[0] = (string)myHeroName;
        data[1] = myTeam;

		GameObject heroObj = PhotonNetwork.Instantiate (myHeroName, 
		                                                startPos, Quaternion.LookRotation (direction), 0, data);
		
		heroObj.GetComponent<HeroController> ().init_hero (myHeroName, myTeam, startPos, destPos);
		
		//addHeroObj (heroObj);
		
		return heroObj;
	}

	IEnumerator SpawnMinion() {
	
		for (int i=0; i < minionSpawnCount; i++) {

			yield return new WaitForSeconds (1.0f);

			NetworkObjectPool.instance.SpawnObject ("minion_alpha", alphaHome.position);
			NetworkObjectPool.instance.SpawnObject ("minion_beta", betaHome.position);

		}
	}
	
	// Update is called once per frame
	void Update () {

		if (startSpawn) {
			nextSpawnRemaining -= Time.deltaTime;

			if (nextSpawnRemaining <= 0) {
				nextSpawnRemaining = spawnPeriod;
				//StartCoroutine ("SpawnMinion");
            }
		}

	}
}
