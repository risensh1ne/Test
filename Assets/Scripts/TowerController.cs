using UnityEngine;
using System.Collections;

public class TowerController : MonoBehaviour, IPlayer {

	public float health = 500.0f;
	public float attackRange = 8.0f;
	public float attackRate = 3.0f;
	public bool bAttacking = false;
	public bool bDead = false;
	public Transform firePoint;

	public GameManager.team attachedTeam;
	public GameObject targetEnemy;

	public GameManager.team checkTeam() {
		return attachedTeam;
	}

	public bool isMoving 
	{
		get { return false; }
		set {  }
	}
	
	public bool isAttacking 
	{
		get { return bAttacking; }
		set { bAttacking = value; }
	}
	
	public bool isDead 
	{
		get { return bDead; }
		set { bDead = value; }
	}

	public void damage(float d)
	{
		if (isDead)
			return;

		if (health - d < 0)
			health = 0;
		else
			health -= d;
		
		if (health <= 0) 
			Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		firePoint = transform.Find ("TowerFirePoint");
	}

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {

		int enemyIndex = -1;
		float enemyDistance = 0;
		Vector3 direction;

		Collider[] cldrs = Physics.OverlapSphere (firePoint.position, attackRange);
		if (cldrs.Length > 0 && !isAttacking) {
			
			for (int i=0; i < cldrs.Length; i++) {
				
				bool isTarget = false;
				if (cldrs [i].tag == "Player" || cldrs [i].tag == "minion") {
					IPlayer ip = cldrs [i].gameObject.GetComponent<IPlayer>();
					if (attachedTeam != ip.checkTeam())
						isTarget = true;
				}
				
				if (isTarget) {
					float dist = Vector3.Distance (cldrs [i].transform.position, transform.position);
					if (enemyDistance == 0 || dist < enemyDistance) {
						enemyDistance = dist;
						enemyIndex = i;
					}
				}
			}
		}
		
		if (enemyIndex >= 0) {
			targetEnemy = cldrs [enemyIndex].gameObject;
			IPlayer ip = targetEnemy.GetComponent<IPlayer>();
			if (!ip.isDead)
				StartCoroutine ("Fire");
		}

	}

	IEnumerator Fire() {
		GameObject fireballObj = ObjectPool.instance.GetObjectForType ("Fireball", true);
		if (fireballObj != null) {
			Vector3 dir = (targetEnemy.transform.position - firePoint.transform.position).normalized;
			fireballObj.transform.position = firePoint.position;
			fireballObj.transform.rotation = Quaternion.LookRotation (dir);
			isAttacking = true;
		}

		yield return new WaitForSeconds(attackRate);
		isAttacking = false;
		ObjectPool.instance.PoolObject(fireballObj);
	}
	
}
