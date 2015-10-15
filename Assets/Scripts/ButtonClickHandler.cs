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

	public void OnJoinGameButtonClick()
	{
		lm.GetComponent<LobbyManager> ().JoinGame ();
	}

	public void OnRoomClick(string roomName)
	{
		lm.GetComponent<LobbyManager> ().SetRoomSelected(roomName);
	}

	public void OnHeroButtonClick(string name)
	{
        Debug.Log(name + "clicked!!");
		lm.GetComponent<LobbyManager>().SelectHero(name);
	}

	public void OnStartGameButtonClick()
	{
		lm.GetComponent<LobbyManager> ().StartGame ();

    }

	public void OnExitRoomClick()
	{
		lm.GetComponent<LobbyManager> ().ExitRoom ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
