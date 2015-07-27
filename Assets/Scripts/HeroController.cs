using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class HeroController : MonoBehaviour, IPlayer {

	public float moveSpeed = 5.0f;
	public float health;
	public float maxHealth = 100.0f;
	Vector2 healthBarSize = new Vector2(50, 5);

	public float attackRange = 1.5f;
	public float lookRange = 10.0f;

	public GameManager.team attachedTeam;

	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	public Vector3 destinationPos;
	public GameObject targetEnemy;

	public GameObject gm;

	public bool skill1start = false;
	public bool skil2start = false;
	public bool skill3start = false;

	public float skill1AttackRange;

	bool bMoving;
	bool bAttacking;
	bool bDead;

	public bool isMine;

	public bool isMoving 
	{
		get { return bMoving; }
		set { 
			bMoving = value; 
			anim.SetBool ("isMoving", value);
		}
	}
	
	public bool isAttacking 
	{
		get { return bAttacking; }
		set { 
			bAttacking = value; 
			anim.SetBool ("isAttacking", value);
		}
	}
	
	public bool isDead 
	{
		get { return bDead; }
		set { 
			bDead = value; 
			anim.SetBool ("isDead", value);
		}
	}
	
	public GameManager.team checkTeam() {
		return attachedTeam;
	}

	Animator anim;

	public void damage(float d)
	{
		if (isDead)
			return;

		health -= d;
		gm.GetComponent<PlayerController> ().updateHealthBar ();

		if (health <= 0) {
			StartCoroutine("Die");
		}
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.name == "Fireball") {
			damage (20.0f);
		}
	}

	IEnumerator Die()
	{
		isDead = true;

		yield return new WaitForSeconds (4.0f);

		InitState ();
	}

	void InitState()
	{
		isDead = false;
		isMoving = false;
		isAttacking = false;

		health = maxHealth;
		gm.GetComponent<PlayerController> ().updateHealthBar ();

		if (attachedTeam == GameManager.team.ALPHA)
			transform.position = gm.GetComponent<GameManager> ().alphaHome.position;
		else 
			transform.position = gm.GetComponent<GameManager> ().betaHome.position;
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		gm = GameObject.Find ("_GM");

		InitState ();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if (anim == null)
			return;

		if (isDead)
			return;

		if (isMine) {
		
			if (Input.GetMouseButtonDown (0)) {

				if (EventSystem.current.IsPointerOverGameObject ())
					return;
					
				Vector3 eventPos = ScreenToWorld (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
				eventPos.y = transform.position.y;

				if (gm.GetComponent<PlayerController> ().attackSelection) {
					int enemyIndex = -1;
					float enemyDistance = 0;
					Vector3 direction;

					Collider[] cldrs = Physics.OverlapSphere (transform.position, lookRange);
					if (cldrs.Length > 0) {
						for (int i=0; i < cldrs.Length; i++) {
							bool isTarget = false;
							if (cldrs [i].tag == "Player" || cldrs [i].tag == "minion" || cldrs [i].tag == "defender") {
								IPlayer ip = cldrs [i].gameObject.GetComponent<IPlayer> ();
								if (attachedTeam != ip.checkTeam ())
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
						IPlayer ip = targetEnemy.GetComponent<IPlayer> ();
						
						if (!ip.isDead) {
							if (enemyDistance <= attackRange) {
								if (!isAttacking) {
									direction = (targetEnemy.transform.position - transform.position).normalized;
									transform.position += direction * moveSpeed * Time.fixedDeltaTime;
									transform.rotation = Quaternion.LookRotation (direction);
									
									isAttacking = true;
								}
							} else {

								direction = (cldrs [enemyIndex].transform.position - transform.position).normalized;
								transform.position += direction * moveSpeed * Time.fixedDeltaTime;
								transform.rotation = Quaternion.LookRotation (direction);

								destinationPos = cldrs [enemyIndex].transform.position;
							}
						} else {
							direction = (eventPos - transform.position).normalized;
							transform.position += direction * moveSpeed * Time.fixedDeltaTime;
							transform.rotation = Quaternion.LookRotation (direction);
							
							targetEnemy = null;
							if (isAttacking) {
								isAttacking = false;
								anim.SetBool ("isAttacking", false);
							}
						}
					} else {
						direction = (eventPos - transform.position).normalized;
						transform.position += direction * moveSpeed * Time.fixedDeltaTime;
						transform.rotation = Quaternion.LookRotation (direction);
						
						targetEnemy = null;
						if (isAttacking) {
							isAttacking = false;
							anim.SetBool ("isAttacking", false);
						}
					}
					gm.GetComponent<PlayerController> ().attackTargetSelectionMode (false);
					
				} else { //attackSelection == false
					RaycastHit hitInfo = new RaycastHit ();
					bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
					
					if (hit && (hitInfo.transform.gameObject.tag == "Player" || hitInfo.transform.gameObject.tag == "minion" || hitInfo.transform.gameObject.tag == "defender")) {
						
						targetEnemy = hitInfo.transform.gameObject;
						
						if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
							isMoving = false;
							isAttacking = true;
						} else {
							isMoving = true;
							isAttacking = false;
							destinationPos = targetEnemy.transform.position;
						}
					} else {
						targetEnemy = null;
						float _distance = Vector3.Distance (transform.position, eventPos);
						if (_distance > 0.1) {
							isMoving = true;
							
							if (isAttacking) 
								isAttacking = false;
							
							destinationPos = eventPos;
						}
					}
				}
			} else {
				if (isMoving || isAttacking) {
					float distance = Vector3.Distance (transform.position, destinationPos);

					if (targetEnemy != null) {

						if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
							isMoving = false;
							isAttacking = true;
						} else {

							Vector3 dir = (destinationPos - transform.position).normalized;
							
							transform.position += dir * Time.fixedDeltaTime * moveSpeed;
							transform.rotation = Quaternion.LookRotation (dir);
						}
					} else {
						if (distance > 0.1) {
							Vector3 dir = (destinationPos - transform.position).normalized;
							
							transform.position += dir * Time.fixedDeltaTime * moveSpeed;
							transform.rotation = Quaternion.LookRotation (dir);
						} else {
							isMoving = false;
							isAttacking = false;
						}
					}
				} 
			}
		} else {
			int enemyIndex = -1;
			float enemyDistance = 0;
			Vector3 direction;

			Collider[] cldrs = Physics.OverlapSphere (transform.position, lookRange);
			if (cldrs.Length > 0) {
				for (int i=0; i < cldrs.Length; i++) {
					bool isTarget = false;
					if (cldrs [i].tag == "Player" || cldrs [i].tag == "minion" || cldrs [i].tag == "defender") {
						IPlayer ip = cldrs [i].gameObject.GetComponent<IPlayer> ();
						if (attachedTeam != ip.checkTeam ())
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
				IPlayer ip = targetEnemy.GetComponent<IPlayer> ();
				
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
							transform.position += direction * moveSpeed * Time.fixedDeltaTime;
							transform.rotation = Quaternion.LookRotation (direction);
						}
					}
				} else {
					direction = (destinationPos - transform.position).normalized;
					transform.position += direction * moveSpeed * Time.fixedDeltaTime;
					transform.rotation = Quaternion.LookRotation (direction);
					
					targetEnemy = null;
					if (isAttacking) {
						isAttacking = false;
						anim.SetBool ("isAttacking", false);
					}
				}
			} else {
				direction = (destinationPos - transform.position).normalized;
				transform.position += direction * moveSpeed * Time.fixedDeltaTime;
				transform.rotation = Quaternion.LookRotation (direction);

				isMoving = true;
				targetEnemy = null;
				if (isAttacking) {
					isAttacking = false;
					anim.SetBool ("isAttacking", false);
				}
			}
		}


		
	}

	void DamageEnemy(float damage)
	{
		if (targetEnemy != null) {
			IPlayer ip = targetEnemy.gameObject.GetComponent<IPlayer> ();
			
			if (ip.isDead) {
				targetEnemy = null;
				isAttacking = false;
				isMoving = false;
				anim.SetBool ("isAttacking", false);
				anim.SetBool ("isMoving", false);
			} else {
				ip.damage (damage);
			}
		}
	}

	void OnSkill1Start()
	{
		DamageEnemy (40.0f);
		skill1start = false;
	}
	
	public void OnSkill1()
	{
		if (skill1start)
			return;
		
		if (targetEnemy != null) {
			Vector3 dir = (destinationPos - transform.position).normalized;
			
			skill1start = true;
			anim.SetBool ("skill01", true);
		}
	}

	void OnNormalAttack()
	{
		if (targetEnemy == null)
			return;

		float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
		if (distance < attackRange)
			DamageEnemy (10.0f);
	}

	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		
		GUI.BeginGroup (new Rect(pos.x - 30, Screen.height - pos.y - 90, healthBarSize.x, healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x, healthBarSize.y), progressBarBack);
		
		GUI.BeginGroup (new Rect(0, 0, healthBarSize.x * (health / 100), healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x * (health / 100), healthBarSize.y), progressBarHealth);
		GUI.EndGroup();
		GUI.EndGroup ();	
	}

	Vector3 ScreenToWorld( Vector2 screenPos )
	{
		// Create a ray going into the scene starting 
		// from the screen position provided 
		Ray ray = Camera.main.ScreenPointToRay( screenPos );
		RaycastHit hit;
		
		// ray hit an object, return intersection point
		if( Physics.Raycast( ray, out hit ) )
			return hit.point;
		
		// ray didn't hit any solid object, so return the 
		// intersection point between the ray and 
		// the Y=0 plane (horizontal plane)
		float t = -ray.origin.y / ray.direction.y;
		return ray.GetPoint( t );
	}
}
                                                                                                                                                                                                                                          