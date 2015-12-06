using UnityEngine;
using System.Collections;

public class ButtonClickHandler : MonoBehaviour {

	public GameObject lm;

	// Use this for initialization
	void Start () {
		lm = GameObject.Find ("_LM");
	}

	public void OnLoginButtonClick()
	{
		lm.GetComponent<LobbyManager> ().Connect ();
	}

	public void OnCreateGameButtonClick()
	{
		lm.GetComponent<LobbyManager> ().CreateGame ();
	}

	public void OnCreateRoomButtonClick()
	{
		lm.GetComponent<LobbyManager> ().CreateRoom ();
	}

	public void OnJoinGameButtonClick()
	{
		lm.GetComponent<LobbyManager> ().JoinGame ();
	}

	public void OnRoomClick(GameObject roomInfo)
	{
		string roomName = roomInfo.GetComponent<UILabel>().text;
		lm.GetComponent<LobbyManager> ().SetRoomSelected(roomName);
	}

	public void OnReadyButtonClick()
	{
		lm.GetComponent<LobbyManager>().OnReadyGame();
	}

	public void OnHeroButtonClick(string name)
	{
        //Debug.Log(name + "clicked!!");
		lm.GetComponent<LobbyManager>().OnSelectHero(name);
	}

	public void OnStartGameButtonClick()
	{
		lm.GetComponent<LobbyManager> ().StartGame ();
    }

	public void OnCreateRoomDlgClose()
	{
		lm.GetComponent<LobbyManager> ().OnCreateRoomDlgClose ();
	}

	public void OnExitRoomClick()
	{
		lm.GetComponent<LobbyManager> ().ExitRoom ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
