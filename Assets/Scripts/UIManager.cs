using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {

	public GameObject player;


	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (50, 50, 100, 50), "Fire")) {
			Debug.Log ("BUTTON!!");
			player.GetComponent<PlayerController>().OnSpecialAttack3();
		}

	}


	// Update is called once per frame
	void Update () {
	
	}
}
