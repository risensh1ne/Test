using UnityEngine;
using System.Collections;

public class CursorHandler : MonoBehaviour {
	
	public GameObject arrow;
	public float animationSpeed = 5;
	
	void Start () {
		
		arrow.GetComponent<Animator>().speed = animationSpeed;
	}
	
	public LayerMask groundLayer;
	
	void Update () {

		if( Input.GetMouseButtonDown(0)){
			
			RaycastHit hit;
			if (Physics.Raycast	(Camera.main.ScreenPointToRay (Input.mousePosition),out hit, Mathf.Infinity, groundLayer)) {
				arrow.SetActive(true);
				transform.position = hit.point;
				arrow.GetComponent<Animator>().SetTrigger("move");
				//arrow.GetComponent<Animation>().Play("Play", PlayMode.StopAll);
			}
		}
	}

}
