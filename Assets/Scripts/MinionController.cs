using UnityEngine;
using System.Collections;

public class MinionController : MonoBehaviour, IPlayer {

	public float health = 50.0f;
	public float lookRange = 4.0f;
	public float attackRange = 1.0f;

	public Vector3 startPos;
	public Vector3 endPos;
	public float walkSpeed;
	
	public GameManager.team attachedTeam;

	GameObject alphaHome;
	GameObject betaHome;

	GameObject targetEnemy;

	bool bWalking;
	bool bAttacking;
	bool bDead;

	Animator anim;

	public GameManager.team checkTeam() {
		return attachedTeam;
	}

	public bool isMoving 
	{
		get { return bWalking; }
		set { bWalking = value; }
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



	// Use this for initialization
	void Start () {
		InitState ();
	}

	public void setTeam(GameManager.team team) {
		alphaHome = GameObject.Find ("targetPosition_alpha");	
		betaHome = GameObject.Find ("targetPosition_beta");

		attachedTeam = team;
	}

	public void InitState() {

		isDead = false;
		isMoving = false;
		isAttacking = false;

		health = 50.0f;
		walkSpeed = 2.0f;

		anim = GetComponent<Animator> ();
		anim.SetBool ("isWalking", true);
		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isDead", false);

		if (attachedTeam == GameManager.team.ALPHA) {
			startPos = alphaHome.transform.position;
			endPos = betaHome.transform.position;
		} else if (attachedTeam == GameManager.team.BETA) {
			startPos = betaHome.transform.position;
			endPos = alphaHome.transform.position;
		}

		Vector3 direction = (endPos - transform.position).normalized;
		transform.position = startPos;
		transform.rotation = Quaternion.LookRotation (direction);
	}
	public void OnSpawn() {
		InitState ();
	}

	public void OnNormalAttack() {
		if (targetEnemy != null) {
			if (targetEnemy.tag == "Player" || targetEnemy.tag == "minion" || targetEnemy.tag == "defender" ) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer>();
				if (ip.isDead) {
					targetEnemy = null;
					isAttacking = false;
					
					anim.SetBool ("isAttacking", false);
				} else {
					ip.damage (10.0f);
				}
			}
		}

	}

	public void damage(float d)
	{
		if (isDead)
			return;

		health -= d;
		if (health <= 0) {
			StartCoroutine("Die");
		}
	}

	IEnumerator Die()
	{
		isDead = true;
		isMoving = false;
		isAttacking = false;

		anim.SetBool ("isAttacking", false);
		anim.SetBool ("isWalking", false);
		anim.SetBool ("isDead", true);

		yield return new WaitForSeconds (5.0f);
		ObjectPool.instance.PoolObject (gameObject);
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.name == "Fireball") {
			damage (40.0f);
		}
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
				if (cldrs [i].tag == "Player" || cldrs [i].tag == "minion" || cldrs [i].tag == "defender") {
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

			if (!ip.isDead) {
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
