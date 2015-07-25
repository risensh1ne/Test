using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject player;

	public GameObject skill1_btn;

		// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}
	}

	public void OnNormalAttackBtnClicked()
	{
		player.GetComponent<PlayerController> ().SelectAttackTarget ();
	}

	public void OnSkill1BtnClicked()
	{
		player.GetComponent<PlayerController> ().OnSkill1 ();
	}

	void OnGUI()
	{
		/*
		if (GUI.Button (new Rect (50, 50, 100, 50), "Fire")) {
			Debug.Log ("BUTTON!!");
			player.GetComponent<PlayerController>().OnSpecialAttack3();
		}
		*/
	}


	// Update is called once per frame
	void Update () {
		GameObject targetEnemy = player.GetComponent<PlayerController> ().targetEnemy;

		if (targetEnemy != null) {
			float dist = Vector3.Distance (player.transform.position, targetEnemy.transform.position);
			if (dist <= player.GetComponent<PlayerController> ().skill1AttackRange)
				skill1_btn.GetComponent<Button> ().interactable = true;
			else 
				skill1_btn.GetComponent<Button> ().interactable = false;
		} else
			skill1_btn.GetComponent<Button> ().interactable = false;
	}
}
