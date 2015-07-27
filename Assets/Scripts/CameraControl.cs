using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public Transform target;

	private Vector2 currTouch1 = Vector2.zero,
	lastTouch1 = Vector2.zero,
	currTouch2 = Vector2.zero,
	lastTouch2 = Vector2.zero;
	
	private float currDist = 0.0f,
	lastDist = 0.0f;
	private float zoomFactor = 0.0f;

	public float zoomSpeed = 0.3f;
	float DistanceAway = 0;
	float DistanceDefault = 10;
	// Use this for initialization
	void Start () {
		Vector3 PlayerPOS = GameObject.Find("Gion(Clone)").transform.transform.position;	
		transform.position = new Vector3(PlayerPOS.x, 8, PlayerPOS.z - DistanceDefault);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.touches.Length > 1) {
			for (int i=0; i < Input.touchCount; i++)
			{
				if (Input.GetTouch(i).phase == TouchPhase.Moved)
				{
					Zoom(i);
				}
			}

		}

		Vector3 PlayerPOS = GameObject.Find("Gion(Clone)").transform.transform.position;
		transform.position =  new Vector3(PlayerPOS.x, 8, PlayerPOS.z - (DistanceDefault + DistanceAway));


	}

	void Zoom(int currTouch)
	{
		switch (currTouch) 
		{
		case 0:
			currTouch1 = Input.GetTouch (0).position;
			lastTouch1 = currTouch1 - Input.GetTouch (0).deltaPosition;
			break;
		case 1:
			currTouch2 = Input.GetTouch (1).position;
			lastTouch2 = currTouch2 - Input.GetTouch (1).deltaPosition;
			break;
		}

		currDist = Vector2.Distance (currTouch1, currTouch2);
		lastDist = Vector2.Distance (lastTouch1, lastTouch2);


		zoomFactor = Mathf.Clamp (lastDist - currDist, -5f, 5f); 

		DistanceAway = Mathf.Clamp (Mathf.Lerp (DistanceAway, zoomFactor * zoomSpeed, Time.deltaTime), -2.0f, 8.0f);

		Debug.Log ("distance:" + DistanceAway);

	}
}
