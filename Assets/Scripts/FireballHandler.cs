using UnityEngine;
using System.Collections;

public class FireballHandler : MonoBehaviour {

    GameManager.team attachedTeam;

	public GameManager.team team 
	{
		get { return attachedTeam; }
		set { 
			attachedTeam = value; 
		}
	}
    
	public GameObject ImpactEffect1;

    void Start() {
    
    }

	void OnTriggerEnter(Collider col){
 
		if (col.tag == "Player") {
			GameObject clone1 = Instantiate(ImpactEffect1, transform.position, Quaternion.identity) as GameObject;
            col.GetComponent<HeroController>().damage(30.0f);
		} else if (col.tag == "minion") {
            GameObject clone1 = Instantiate(ImpactEffect1, transform.position, Quaternion.identity) as GameObject;
            col.GetComponent<MinionController>().damage(30.0f);
        }
	}
}
