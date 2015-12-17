using UnityEngine;
using System.Collections;

public class FireballCollideHandler : MonoBehaviour {

	public GameObject ImpactEffect1;

	void OnTriggerEnter(Collider col){

		if (col.tag == "Player") {
			GameObject clone1 = Instantiate(ImpactEffect1, transform.position, Quaternion.identity) as GameObject;
			ObjectPool.instance.PoolObject(gameObject);
		}
	}
}
