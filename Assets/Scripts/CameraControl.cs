using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Transform target;

	int DistanceAway = 15;
	// Use this for initialization
	void Start () {
		//Vector3 PlayerPOS = GameObject.Find("Player").transform.transform.position;	
		//transform.position = new Vector3(PlayerPOS.x, 5, PlayerPOS.z - DistanceAway);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 PlayerPOS = GameObject.Find("Player").transform.transform.position;
		transform.position = new Vector3(PlayerPOS.x, 5, PlayerPOS.z - DistanceAway);

	}
}
