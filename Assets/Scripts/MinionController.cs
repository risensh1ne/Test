using UnityEngine;
using System.Collections;

public class MinionController : MonoBehaviour {

	public float health = 50.0f;
	public float lookRange = 5.0f;
	public float attackRange = 1.0f;

	public Vector3 startPos;
	public Vector3 endPos;
	public float walkSpeed;
	
	public GameManager.team attachedTeam;

	GameObject alphaHome;
	GameObject betaHome;

	GameObject targetEnemy;

	bool isWalking;
	bool isAttacking;
	bool isDead;

	Animator anim;

	// Use this for initialization
	void Start () {

		InitState ();
	}

	public void setTeam(GameManager.team team) {
		alphaHome = GameObject.Find ("targetPosition_alpha");	
		betaHome = GameObject.Find ("targetPosition_beta");

		attachedTeam = team;

		if (team == GameManager.team.ALPHA) {
			startPos = alphaHome.transform.position;
			endPos = betaHome.transform.position;
		} else if (team == GameManager.team.BETA) {
			startPos = betaHome.transform.position;
			endPos = alphaHome.transform.position;
		}

		Vector3 direction = (endPos - transform.position).normalized;
		transform.position = startPos;
		transform.rotation = Quaternion.LookRotation (direction);
	}

	public void InitState() {
		isDead = false;
		isWalking = true;
		isAttacking = false;
		health = 50.0f;
		walkSpeed = 5.0f;

		anim = GetComponent<Animator> ();
		anim.SetBool ("isWalking", true);
		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isDead", false);
	}
	public void OnSpawn() {
		InitState ();
	}

	public void OnNormalAttack() {
		if (targetEnemy.tag == "minion" && targetEnemy != null) {

			targetEnemy.GetComponent<MinionController> ().damage (10.0f);
		}

	}

	public void damage(float d)
	{
		health -= d;


		if (health <= 0) {
			Die();
		}
	}

	void Die()
	{
		isDead = true;
		anim.SetBool ("isDead", true);

		//yield return new WaitForSeconds (5.0f);
		//ObjectPool.instance.PoolObject (gameObject);
		Destroy (gameObject, 5.0f);
	}


	// Update is called once per frame
	void Update () {
		if (startPos == null || endPos == null)
			return;

		if (isDead)
			return;
	}

	void FixedUpdate() {
		if (startPos == null || endPos == null)
			return;

		if (isDead)
			return;

		int enemyIndex = -1;
		float enemyDistance = 0;
		Vector3 direction;
		Collider[] cldrs = Physics.OverlapSphere (transform.position, lookRange);
		if (cldrs.Length > 0) {

			for (int i=0; i < cldrs.Length; i++) {

				bool isTarget = false;
				if (cldrs [i].tag == "Player") {
					isTarget = true;
				} else if (cldrs [i].tag == "minion") {
					if (attachedTeam != cldrs [i].GetComponent<MinionController>().attachedTeam)
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

			if (enemyDistance <= attackRange) {
				if (!isAttacking) {
					isAttacking = true;
					anim.SetBool ("isAttacking", true);
				}
			} else {
				if (isAttacking) {
					isAttacking = false;
					anim.SetBool ("isAttacking", false);
				} else {
					direction = (cldrs [enemyIndex].transform.position - transform.position).normalized;
					transform.position += direction * walkSpeed * Time.fixedDeltaTime;
					transform.rotation = Quaternion.LookRotation (direction);
				}
			}
		} else {
			direction = (endPos - transform.position).normalized;
			transform.position += direction * walkSpeed * Time.fixedDeltaTime;
			transform.rotation = Quaternion.LookRotation (direction);

			targetEnemy = null;
			if (isAttacking) {
				isAttacking = false;
				anim.SetBool ("isAttacking", false);
			}


		}

	}
}
