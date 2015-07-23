using UnityEngine;
using System.Collections;

public class TowerController : MonoBehaviour {

	public float attackRange = 5.0f;
	public float attackRate = 3.0f;
	public bool fired = false;

	public Transform firePoint;

	// Use this for initialization
	void Start () {
		firePoint = transform.Find ("TowerFirePoint");
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		Collider[] cldrs = Physics.OverlapSphere (firePoint.position, attackRange);
		if (cldrs.Length > 0 && !fired) {

			GameObject nearest = cldrs[0].gameObject;

			if (nearest.tag == "Player") {
				Debug.Log (nearest.transform.position);
				Vector3 dir = (nearest.transform.position - firePoint.transform.position).normalized;
				
				StartCoroutine("Fire", dir);
			}
		}
	}

	IEnumerator Fire(Vector3 dir) {
		GameObject fireballObj = ObjectPool.instance.GetObjectForType ("Fireball", true);
		if (fireballObj != null) {
			fireballObj.transform.position = firePoint.position;
			fireballObj.transform.rotation = Quaternion.LookRotation (dir);
			fired = true;
		}

		yield return new WaitForSeconds(attackRate);
		fired = false;
		ObjectPool.instance.PoolObject(fireballObj);
	}
	
}
