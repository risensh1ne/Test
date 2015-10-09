using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	string userName;
	public string heroName;

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

	public Sprite[] heroSprites;

	public GameObject alphaHeroSprite;
	public GameObject betaHeroSprite;

	public GameObject alphaHeroNameLabel;
	public GameObject betaHeroNameLabel;

	public GameObject buttonStartGame;

	public GameObject chatInput;
	public UILabel chatLabel;
	public GameObject chatArea;

	bool connecting;

	private string selectedRoomName;
	
	// Use this for initialization
	void Start () {
		inputUserName.GetComponent<UIInput> ().isSelected = true;
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

	public void StartGame() {

		PlayerPrefs.SetString ("userName", userName);
		PlayerPrefs.SetString ("heroName", heroName);
		PlayerPrefs.SetInt ("userTeam", (int)team);

		Application.LoadLevel ("scene_main");
	}

	public void SetRoomSelected(string roomName) {
		selectedRoomName = roomName;
	}

	public void SelectHero(string name) {
		heroName = name;

		if (team == GameManager.team.ALPHA)
			alphaHeroSprite.GetComponent<UISprite> ().spriteName = name;
		else 
			betaHeroSprite.GetComponent<UISprite> ().spriteName = name;
	}
	
	public void ExitRoom(){
		string msg = team + ":" + userName;
		GetComponent<PhotonView> ().RPC ("ExitPlayerInfo", PhotonTargets.Others, msg);

		string chatMessage = userName + " has left the room";
		GetComponent<PhotonView> ().RPC ("SendChatMessage", PhotonTargets.Others, chatMessage);

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

	public void OnChat(string chatText) {
		chatInput.GetComponent<UIInput> ().value = "";
		chatInput.GetComponent<UIInput> ().isSelected = true;

		string chatMessage = "[" + userName + "] " + chatText;
		GetComponent<PhotonView> ().RPC ("SendChatMessage", PhotonTargets.AllBuffered, chatMessage);
	}

	void OnReceivedRoomListUpdate() {

		if (PhotonNetwork.insideLobby) {
		
			int childCount = tableRoom.transform.childCount;
			Debug.Log (childCount);
			if (childCount > 0) {
				for (int i = childCount - 1; i >= 0; i--)
				{
					Transform childTransform = tableRoom.transform.GetChild(i);
					if (childTransform == null)
						continue;

					GameObject child = childTransform.gameObject;
					if (child == null)
						continue;

					NGUITools.DestroyImmediate(child);
				}
			}

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
			//buttonStartGame.GetComponent<UIButton>().isEnabled = false;


			int numPlayersInRoom = 0;
			foreach(PhotonPlayer player in PhotonNetwork.playerList) {
				numPlayersInRoom++;
			}
				
			if (numPlayersInRoom % 2 == 1) {
				team = GameManager.team.ALPHA;
			} else {
				team = GameManager.team.BETA;
			}			

			string msg = team + ":" + userName;
			GetComponent<PhotonView> ().RPC ("JoinPlayerInfo", PhotonTargets.AllBuffered, msg);
		}
	}

	[PunRPC]
	public void SendChatMessage(string chatMessage) {
		chatArea.GetComponent<UITextList> ().Add (chatMessage);
	}

	[PunRPC]
	public void JoinPlayerInfo(string txtInfo) {

		string[] infoList = txtInfo.Split (':');
		string team = infoList [0];
		string playerName = infoList [1];

		if (team == "ALPHA") {
			alphaHeroNameLabel.GetComponent<UILabel> ().text = playerName;
		} else {
			betaHeroNameLabel.GetComponent<UILabel> ().text = playerName;
		}

	}

	[PunRPC]
	public void ExitPlayerInfo(string txtInfo) {
		
		string[] infoList = txtInfo.Split (':');
		string team = infoList [0];
		string playerName = infoList [1];
		
		if (team == "ALPHA") {
			alphaHeroNameLabel.GetComponent<UILabel> ().text = "";
		} else {
			betaHeroNameLabel.GetComponent<UILabel> ().text = "";
		}
		
	}


	// Update is called once per frame
	void Update () {
	
	}
}
