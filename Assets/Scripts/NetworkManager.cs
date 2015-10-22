using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	bool connecting;

	public GameObject gm;
	public string selectedHeroName;
	public GameManager.team selectedTeam;

	void Start () {
	}

	void OnDestroy() {
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

        PlayerPrefs.SetString("userName", "seo");
        PlayerPrefs.SetString("heroName", selectedHeroName);
        PlayerPrefs.SetInt("userTeam", (int)selectedTeam);

        gameObject.GetComponent<GameManager>().gameStart();
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
		
	// Update is called once per frame
	void Update () {
		if (PhotonNetwork.connected) {

		}
	}
}
