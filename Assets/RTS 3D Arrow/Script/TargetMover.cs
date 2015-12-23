using UnityEngine;
using System.Collections;

public class TargetMover : MonoBehaviour {

	public GameObject arrow;
	public float animationSpeed = 4;
	 
	void Start () {
		
		arrow.GetComponent<Animation>()["Play"].speed = animationSpeed;
	}
	
	public LayerMask groundLayer;

	void Update () {
		
		if( Input.GetMouseButtonDown(0)){
			
			RaycastHit hit;
			if (Physics.Raycast	(Camera.main.ScreenPointToRay (Input.mousePosition),out hit, Mathf.Infinity, groundLayer)) {
				
				transform.position = hit.point;
				arrow.GetComponent<Animation>().Rewind("Play");
				arrow.GetComponent<Animation>().Play("Play", PlayMode.StopAll);
			}
		}
	}
}
