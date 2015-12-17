 using UnityEngine;
using System.Collections;

public class DestroyObj_MagicAttack1 : MonoBehaviour {
	
	public float timer = 0.5f;

	void Start () {
		
	}

	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0){
			Object.Destroy(gameObject);
		}
	}
}
