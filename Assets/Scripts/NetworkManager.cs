using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	bool connecting;
	// Use this for initialization
	void Start () {
		PhotonNetwork.player.name = PlayerPrefs.GetString ("username", "risenshine");
		connecting = false;
	}

	void OnDestroy() {
		PlayerPrefs.SetString("username", PhotonNetwork.player.name);
	}

	void Connect() {
		PhotonNetwork.ConnectUsingSettings( "risenhine games 001" );
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
			
			if( GUILayout.Button("Single Player") ) {
				connecting = true;
				PhotonNetwork.offlineMode = true;
				OnJoinedLobby();
			}
			
			if( GUILayout.Button("Multi Player") ) {
				connecting = true;
				Connect ();
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
			Debug.Log ("connected!!!");
		}
	}
}
