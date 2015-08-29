using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	bool connecting;

	public GameManager gm;
	public string selectedHeroName;
	public GameManager.team selectedTeam;

	void Start () {

		gm = gameObject.GetComponent<GameManager> ();

		PhotonNetwork.player.name = PlayerPrefs.GetString ("username", "risenshine");
		connecting = false;
	}

	void OnDestroy() {
		PlayerPrefs.SetString("username", PhotonNetwork.player.name);
	}

	void Connect(string heroName, GameManager.team team) {
		PhotonNetwork.ConnectUsingSettings( "risenhine games 001" );

		selectedHeroName = heroName;
		selectedTeam = team;
	}

	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		PhotonNetwork.JoinRandomRoom();
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("OnPhotonRandomJoinFailed");
		PhotonNetwork.CreateRoom( null );
	}

	void OnJoinedRoom() {
		Debug.Log ("OnJoinedRoom");
		connecting = false;

		GameObject obj = SpawnHero (selectedHeroName, selectedTeam);
		//gm.player = obj;
		gm.setPlayer (selectedHeroName);

		gameObject.GetComponent<UIManager> ().initializeUI ();
	}

	void OnGUI()
	{
		GUILayout.Label( PhotonNetwork.connectionStateDetailed.ToString() );
		
		if(PhotonNetwork.connected == false && connecting == false ) {
			// We have not yet connected, so ask the player for online vs offline mode.
			GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Username: ");
			PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name);
			GUILayout.EndHorizontal();
			
			if( GUILayout.Button("Gion") ) {
				connecting = true;
				//PhotonNetwork.offlineMode = true;
				Connect ("Gion", GameManager.team.BETA);
			}
			
			if( GUILayout.Button("Sword Master") ) {
				connecting = true;
				Connect ("SwordMaster", GameManager.team.ALPHA);
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	
	GameObject SpawnHero(string heroName, GameManager.team team)
	{
		Vector3 startPos, destPos;

		if (team == GameManager.team.BETA) {
			startPos = gm.betaHome.transform.position;
			destPos = gm.alphaHome.transform.position;
		} else {
			startPos = gm.alphaHome.transform.position;
			destPos = gm.betaHome.transform.position;
		}

		Vector3 direction = (destPos - startPos).normalized;

		GameObject heroObj = PhotonNetwork.Instantiate (heroName, 
		                                                startPos, Quaternion.LookRotation (direction), 0);
		heroObj.GetComponent<HeroController> ().attachedTeam = team;
		heroObj.GetComponent<HeroController> ().startPos = startPos;
		heroObj.GetComponent<HeroController> ().endPos = destPos;
		gm.addHeroObj (heroObj);


		return heroObj;

	}




	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.connected) {

		}
	}
}
