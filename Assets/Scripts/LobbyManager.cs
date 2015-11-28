using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	string userName;
	string userID;
	public string heroName;

	public GameManager.team team;
	
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



	public GameObject chatInputRoom;
	public UILabel chatLabel;
	public GameObject chatAreaRoom;

	bool connecting;

	private string selectedRoomName;
	
	// Use this for initialization
	void Start () {

		/*
		for (int i=0; i < 15; i++) {
			GameObject roomInfo = Instantiate(roomPrefab) as GameObject;
			
			GameObject label = roomInfo.transform.FindChild("roomName").gameObject;		
			label.GetComponent<UILabel>().text = "room" + i;
			GameObject numPlayer = roomInfo.transform.FindChild("numPlayer").gameObject;		
			numPlayer.GetComponent<UILabel>().text =  i + "/4";

			NGUITools.AddChild(tableRoom, roomInfo);
			
			tableRoom.GetComponent<UITable>().Reposition();
		}
*/
		Connect ();
	}

	public void Connect() {

		userID = PlayerPrefs.GetString("userID");
		userName = PlayerPrefs.GetString("userName");

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

	public void StartGame()
    {
        GetComponent<PhotonView>().RPC("RealStartGame", PhotonTargets.All);
	}

	public void SetRoomSelected(string roomName) {
	
		Color dftColor = new Color();
		ColorUtility.TryParseHtmlString("#3C250FFF", out dftColor);

		for (int i=0; i < tableRoom.transform.childCount; i++) {
			GameObject roomInfo = tableRoom.transform.GetChild (i).gameObject;
			string _roomName = roomInfo.transform.FindChild("roomName").GetComponent<UILabel>().text;

			if (roomName != _roomName) 
				roomInfo.transform.FindChild("Button").GetComponent<UIButton>().defaultColor = dftColor;
			else 
				roomInfo.transform.FindChild("Button").GetComponent<UIButton>().defaultColor = Color.red;
			roomInfo.transform.FindChild("Button").GetComponent<UIButton>().UpdateColor(true);
		}

		selectedRoomName = roomName;
        buttonJoinGame.GetComponent<UIButton>().isEnabled = true;
    }

	public void SelectHero(string name) {
		heroName = name;

		if (team == GameManager.team.ALPHA) 
			alphaHeroSprite.GetComponent<UISprite> ().spriteName = name;
		else 
			betaHeroSprite.GetComponent<UISprite> ().spriteName = name;

        Debug.Log(name);

        string msg = team + ":" + heroName;
        GetComponent<PhotonView>().RPC("UpdateHeroSelection", PhotonTargets.AllBuffered, msg);
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

		buttonCreateGame.SetActive (true);
		buttonJoinGame.SetActive(true);
		roomCtrl.SetActive (true);

        buttonJoinGame.GetComponent<UIButton>().isEnabled = false;
        selectedRoomName = "";
	}
	
	public void OnChat(string chatText) {
		chatInputRoom.GetComponent<UIInput> ().value = "";
		chatInputRoom.GetComponent<UIInput> ().isSelected = true;

		string chatMessage = "[" + userName + "] " + chatText;
		GetComponent<PhotonView> ().RPC ("SendChatMessage", PhotonTargets.AllBuffered, chatMessage);
	}

	void OnReceivedRoomListUpdate() {
		Debug.Log ("OnReceivedRoomListUpdate");

		if (PhotonNetwork.insideLobby) {
			int childCount = tableRoom.transform.childCount;
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
            
			buttonStartGame.GetComponent<UIButton>().isEnabled = false;
            

            string chatMessage = userName + " has joined the room";
            GetComponent<PhotonView>().RPC("SendChatMessage", PhotonTargets.Others, chatMessage);

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
		chatAreaRoom.GetComponent<UITextList> ().Add (chatMessage);
	}

    [PunRPC]
    public void UpdateHeroSelection(string txtInfo)
    {
        string[] infoList = txtInfo.Split(':');
        string team = infoList[0];
        string heroName = infoList[1];

        Debug.Log(team + ":" + heroName);
        if (team == "ALPHA")
        {
            alphaHeroSprite.GetComponent<UISprite>().spriteName = heroName;
        }
        else
        {
            betaHeroSprite.GetComponent<UISprite>().spriteName = heroName;
        }

        if (alphaHeroSprite.GetComponent<UISprite>().spriteName != "select_hero" &&
            betaHeroSprite.GetComponent<UISprite>().spriteName != "select_hero")
        {
            buttonStartGame.GetComponent<UIButton>().isEnabled = true;
        }
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
            alphaHeroSprite.GetComponent<UISprite>().spriteName = "select_hero";
        } else {
			betaHeroNameLabel.GetComponent<UILabel> ().text = "";
            betaHeroSprite.GetComponent<UISprite>().spriteName = "select_hero";
        }

        buttonStartGame.GetComponent<UIButton>().isEnabled = false;

    }

    [PunRPC]
    public void RealStartGame()
    {
        PlayerPrefs.SetString("userName", userName);
        PlayerPrefs.SetString("heroName", heroName);
        PlayerPrefs.SetInt("userTeam", (int)team);

        //Application.LoadLevel("scene_main");
        //PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.LoadLevel("scene_main");
    }

    // Update is called once per frame
    void Update () {
	
	}
}
