using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	string userName;
	
	public GameManager.team team;
	
	public GameObject userNameBox;
	public GameObject inputUserName;
	public GameObject buttonLogin;
	public GameObject windowChat;
	public GameObject buttonCreateGame;
	public GameObject buttonJoinGame;

	public GameObject roomPanel;

	public GameObject roomCtrl;
	public GameObject tableRoom;
	public GameObject roomPrefab;

	bool connecting;

	private string selectedRoomName;
	
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

		PhotonNetwork.JoinRoom(selectedRoomName);
	}

	public void SetRoomSelected(string roomName) {
		selectedRoomName = roomName;

	}
	
	public void ExitRoom(){
		PhotonNetwork.LeaveRoom();
		roomPanel.SetActive (false);
	}
	
	void OnJoinedLobby() {
		Debug.Log ("OnJoinedLobby");
		connecting = false;

		userNameBox.SetActive (false);
		windowChat.SetActive (true);
		buttonCreateGame.SetActive (true);
		buttonJoinGame.SetActive(true);
		roomCtrl.SetActive (true);
	}

	void OnReceivedRoomListUpdate() {
		if (PhotonNetwork.insideLobby) {
			foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
				GameObject roomInfo = Instantiate(roomPrefab) as GameObject;

				GameObject label = roomInfo.transform.FindChild("roomName").gameObject;		
				label.GetComponent<UILabel>().text = room.name;
				NGUITools.AddChild(tableRoom, roomInfo);
					
				tableRoom.GetComponent<UITable>().Reposition();
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
		
		if (PhotonNetwork.inRoom) {
			roomPanel.SetActive (true);
				
			int numPlayersInRoom = 0;
			foreach(PhotonPlayer player in PhotonNetwork.playerList) {
				numPlayersInRoom++;
			}
				
			if (numPlayersInRoom % 2 == 1)
				team = GameManager.team.ALPHA;
			else
				team = GameManager.team.BETA;
				
			Debug.Log(team);
		}
			
	}


	// Update is called once per frame
	void Update () {
	
	}
}
