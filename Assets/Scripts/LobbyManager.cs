using UnityEngine;
using System.Collections;

public class LobbyManager : MonoBehaviour {

	string userName;
	string userID;
	public string heroName;

	public int teamSeedNumber;
	public int globalSeedNumber;
	public int currentSeedNumber;

	public GameManager.team team;
	
	public GameObject buttonCreateGame;
	public GameObject buttonJoinGame;
	public GameObject buttonStartGame;
	public GameObject buttonReady;
	public GameObject buttonCreateRoom;
	public GameObject createRoomDlg;

	public GameObject inputRoomName;
	public GameObject selectRoomCapacity;

	public GameObject roomPanel;

	public GameObject roomCtrl;
	public GameObject tableRoom;
	public GameObject roomPrefab;

	public Sprite[] heroSprites;

	public GameObject alphaGroupObj;
	public GameObject betaGroupObj;

	public GameObject heroSpriteObj;
	public GameObject heroNameObj;
	public GameObject heroSelectView;

	public bool selectionStarted;

	public float selectionTimer;

	public GameObject chatInputRoom;
	public UILabel chatLabel;
	public GameObject chatAreaRoom;

	bool connecting;

	string selectedRoomName;
	string currentRoomName;
	int roomCapacity;
	int roomPlayerCnt;

	// Use this for initialization
	void Start () {
		selectionStarted = false;

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
		createRoomDlg.SetActive(true);

	}

	public void CreateRoom() {

		currentRoomName = inputRoomName.GetComponent<UIInput>().value;
		roomCapacity = System.Int32.Parse(selectRoomCapacity.GetComponent<UIPopupList>().value);

		ExitGames.Client.Photon.Hashtable htRoomProps = new ExitGames.Client.Photon.Hashtable();
		htRoomProps.Add("userName", userName);

		RoomOptions roomOptions = new RoomOptions() { isVisible = true, isOpen = true, customRoomProperties = htRoomProps, 
			maxPlayers = (byte)roomCapacity };

		bool result = PhotonNetwork.JoinOrCreateRoom (currentRoomName, roomOptions, TypedLobby.Default);
		if (!result) {
			Debug.Log ("createRoom error");
		}

		createRoomDlg.SetActive(false);
	}

	public void JoinGame() {
		PhotonNetwork.JoinRoom(selectedRoomName);
	}

	public void StartGame()
    {
        GetComponent<PhotonView>().RPC("OnStartGame", PhotonTargets.All);
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
	
	public void OnSelectHero(string name) {
		heroName = name;

		heroSpriteObj.GetComponent<UISprite> ().spriteName = name;
		heroNameObj.GetComponent<UILabel>().text = userName;

        string msg = team + ":" + userName + ":" + heroName + ":" + globalSeedNumber;
        GetComponent<PhotonView>().RPC("UpdateHeroSelection", PhotonTargets.AllBuffered, msg);
    }
	
	public void ExitRoom(){
		string msg = team + ":::" + teamSeedNumber;
		GetComponent<PhotonView> ().RPC ("UpdateHeroSelection", PhotonTargets.AllBuffered, msg);

		//GetComponent<PhotonView> ().RPC ("ExitPlayerInfo", PhotonTargets.Others, msg);

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

				Debug.Log (room.customProperties);
				Debug.Log (room);
				GameObject labelName = roomInfo.transform.FindChild("roomName").gameObject;		
				labelName.GetComponent<UILabel>().text = room.name;
				GameObject labelCapacity = roomInfo.transform.FindChild("numPlayer").gameObject;		
				labelCapacity.GetComponent<UILabel>().text = room.playerCount.ToString () + "/" + room.maxPlayers.ToString ();

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
			string _userName = PhotonNetwork.room.customProperties["userName"].ToString();
			currentRoomName = PhotonNetwork.room.name;
			roomCapacity = PhotonNetwork.room.maxPlayers;

			roomPanel.SetActive (true);
            
			if (PhotonNetwork.isMasterClient) {
				buttonStartGame.SetActive (true);
				buttonStartGame.GetComponent<UIButton>().isEnabled = false;
			}

            string chatMessage = _userName + " has joined the room";
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
			teamSeedNumber = (int)Mathf.Ceil((float)numPlayersInRoom / 2); 
			globalSeedNumber = numPlayersInRoom;


			alphaGroupObj.transform.FindChild("alpha_1").gameObject.SetActive(true);
			betaGroupObj.transform.FindChild("beta_1").gameObject.SetActive(true);
			if (roomCapacity >= 4) {
				alphaGroupObj.transform.FindChild("alpha_2").gameObject.SetActive(true);
				betaGroupObj.transform.FindChild("beta_2").gameObject.SetActive(true);
			}
			if (roomCapacity >= 6) {
				alphaGroupObj.transform.FindChild("alpha_3").gameObject.SetActive(true);
				betaGroupObj.transform.FindChild("beta_3").gameObject.SetActive(true);
			}

			GameObject heroSlot = null;
			if (team == GameManager.team.ALPHA) {
				heroSlot = alphaGroupObj.transform.FindChild("alpha_" + teamSeedNumber).gameObject;
			} else {
				heroSlot = betaGroupObj.transform.FindChild("beta_" + teamSeedNumber).gameObject;
			}
			heroSpriteObj = heroSlot.transform.FindChild("hero_sprite").gameObject;
			heroNameObj = heroSlot.transform.FindChild("hero_name").gameObject;

			string msg = team + ":" + userName + "::" + teamSeedNumber;
			GetComponent<PhotonView> ().RPC ("UpdateHeroSelection", PhotonTargets.AllBuffered, msg);

			if (PhotonNetwork.isMasterClient && numPlayersInRoom == roomCapacity) {
				buttonStartGame.GetComponent<UIButton>().isEnabled = false;
			}

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
        string _team = infoList[0];
		string _playerName = infoList[1];
        string _heroName = infoList[2];
		int _seedNumber = System.Int32.Parse(infoList[3]);
        
		Debug.Log(_team + ":" + _heroName + ":" + _playerName + ":" + _seedNumber);

		GameObject heroSlot = null;

        if (_team == "ALPHA")
        {
			heroSlot = alphaGroupObj.transform.FindChild("alpha_" + _seedNumber).gameObject;
        }
        else
        {
			heroSlot = betaGroupObj.transform.FindChild("beta_" + _seedNumber).gameObject;
        }

		heroSpriteObj = heroSlot.transform.FindChild("hero_sprite").gameObject;
		heroNameObj = heroSlot.transform.FindChild("hero_name").gameObject;

		heroSpriteObj.GetComponent<UISprite>().spriteName = _heroName;
		heroNameObj.GetComponent<UILabel>().text = _playerName;

		if (PhotonNetwork.isMasterClient) {


		}
		/*
        if (alphaHeroSprite.GetComponent<UISprite>().spriteName != "select_hero" &&
            betaHeroSprite.GetComponent<UISprite>().spriteName != "select_hero")
        {
            buttonStartGame.GetComponent<UIButton>().isEnabled = true;
        }
        */
    }

	/*
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
*/

    [PunRPC]
    public void OnStartGame()
    {
		buttonStartGame.SetActive (false);
		buttonReady.SetActive (true);
		buttonReady.GetComponent<UIButton>().isEnabled = false;
		heroSelectView.GetComponent<UIScrollView>().enabled = false;
		selectionStarted = true;
		currentSeedNumber = 1;

		checkMyTurn();
    }

	[PunRPC]
	public void checkMyTurn()
	{
		selectionTimer = 10.0f;

		if (currentSeedNumber == globalSeedNumber) {
			buttonReady.GetComponent<UIButton>().isEnabled = true;	
			heroSelectView.GetComponent<UIScrollView>().enabled = true;
		}
	}

	public void RealStartGame()
	{
		PlayerPrefs.SetString("userName", userName);
		PlayerPrefs.SetString("heroName", heroName);
		PlayerPrefs.SetInt("userTeam", (int)team);
		
		//Application.LoadLevel("scene_main");
		//PhotonNetwork.automaticallySyncScene = true;
		PhotonNetwork.LoadLevel("scene_main");

	}

    void FixedUpdate () {
	
	}
}
