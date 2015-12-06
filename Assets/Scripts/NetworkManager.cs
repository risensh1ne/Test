using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {

	bool connecting;

	public GameObject gm;
	public string selectedHeroName;
	public GameManager.team selectedTeam;

    private int originalWidth = 1024;
    private int originalHeight = 600;
    private Vector3 guiScale;

    void Start () {
        guiScale.x = (float)Screen.width / (float)originalWidth; // calculate hor scale
        guiScale.y = (float)Screen.height / (float)originalHeight; // calculate vert scale
        guiScale.z = 1.0f;

		//Debug.Log (PlayerPrefs.GetString ("userName") + "#" + PlayerPrefs.GetString ("heroName") + "#" + PlayerPrefs.GetInt ("userTeam"));

		selectedHeroName = PlayerPrefs.GetString ("heroName");
		selectedTeam = (GameManager.team)PlayerPrefs.GetInt ("userTeam");
		gameObject.GetComponent<GameManager>().gameStart();
    }

	void OnDestroy() {
	}

/*
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
*/

	void OnGUI()
    {
		/*
		if(PhotonNetwork.connected == false && connecting == false ) {
           
            Matrix4x4 saveMat = GUI.matrix;

            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiScale);

            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

            // We have not yet connected, so ask the player for online vs offline mode.
            GUILayout.BeginArea( new Rect(0, 0, originalWidth, originalHeight) );
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

            GUI.matrix = saveMat;
        }
		*/

	}
		
	// Update is called once per frame
	void Update () {

	}
}
