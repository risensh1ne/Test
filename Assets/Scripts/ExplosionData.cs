using UnityEngine;
using System.Collections;

public class ExplosionData : MonoBehaviour {

	GameManager.team attachedTeam;
	
	public GameManager.team team 
	{
		get { return attachedTeam; }
		set { 
			attachedTeam = value; 
		}
	}
}
