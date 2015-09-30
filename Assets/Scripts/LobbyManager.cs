using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	string userName;
	public GameObject userNameBox;
	public GameObject inputUserName;
	public GameObject buttonLogin;
	public GameObject windowChat;
	public GameObject buttonCreateGame;

	public GameObject roomCtrl;
	public GameObject tableRoom;
	public GameObject roomPrefab;

	bool connecting;

	// Use this for initialization
	void Start () {

	}

	public void Connect() {
		if (inputUserName.GetComponent<UIInput> ().label.text.Length == 0) {
			return;
		}
		userName = inputUserName.GetComponent<UIInput> ().label.text;

		PhotonNetwork.player.name = userName;

		PhotonNetwork.ConnectUsingSettings( "risenhine games 001" );
	}

	public void CreateGame() {


		RoomOptions roomOptions = new RoomOptions() { isVisible = true, isOpen = true, maxPlayers = 4 };
		bool result = PhotonNetwork.JoinOrCreateRoom (userName, roomOptions, TypedLobby.Default);
		if (!result)
			Debug.Log ("createRoom error");
	}

	public void JoinGame() {


	}

	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		connecting = false;

		userNameBox.SetActive (false);
		windowChat.SetActive (true);
		buttonCreateGame.SetActive (true);
		roomCtrl.SetActive (true);

		for (int i=0; i < 3; i++) {
			GameObject roomInfo = Instantiate(roomPrefab) as GameObject;
			GameObject label = roomInfo.transform.FindChild("roomName").gameObject;		
			label.GetComponent<UILabel>().text = "test" + i;
			NGUITools.AddChild(tableRoom, roomInfo);
			
		}
		
		tableRoom.GetComponent<UITable>().Reposition();
	}

	void OnReceivedRoomListUpdate() {
		if (PhotonNetwork.insideLobby) {
			foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
				GameObject roomInfo = Instantiate(roomPrefab) as GameObject;

				
				GameObject label = roomInfo.transform.FindChild("roomName").gameObject;
				
				label.GetComponent<UILabel>().text = room.name;
				//label.GetComponent<UILabel>().width = 300;
				roomInfo.transform.SetParent (tableRoom.transform);

				roomInfo.transform.localPosition = new Vector3(20, -20, 0);
				roomInfo.transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}

	void OnPhotonRandomJoinFailed() {
		Debug.Log ("OnPhotonRandomJoinFailed");
	}

	void OnCreatedRoom() {
		Debug.Log ("OnCreatedRoom");	
	}

	                   
	void OnJoinedRoom() {
		Debug.Log ("OnJoinedRoom");	
		connecting = false;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
