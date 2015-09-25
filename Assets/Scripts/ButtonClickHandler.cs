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

	// Update is called once per frame
	void Update () {
	
	}
}
