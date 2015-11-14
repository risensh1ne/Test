using UnityEngine;
using System.Collections;

public class FireballData : MonoBehaviour {

	GameManager.team attachedTeam;

	public GameManager.team team 
	{
		get { return attachedTeam; }
		set { 
			attachedTeam = value; 
		}
	}


}
