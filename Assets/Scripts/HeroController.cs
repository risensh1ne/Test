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

	public Vector3 nextPos, destinationPos;
	public GameObject targetEnemy;

	public GameObject gm;

	public bool autoAttack;

	public bool skillTargetSelectionMode;
	public int selectedSkill;
	public float skillRange;

	public void ToggleSkillTargetMode(int skill, float range)
	{
		Transform obj = transform.Find ("attack_range");

		if (skill == 0) {
			selectedSkill = 0;
			obj.gameObject.SetActive (false);
			skillTargetSelectionMode = false;
		} else if (skillTargetSelectionMode) {
			if (skill == selectedSkill) {
				selectedSkill = 0;
				skillRange = 0;
				obj.gameObject.SetActive (false);
				skillTargetSelectionMode = false;
			} else {
				selectedSkill = skill;
				skillRange = range;
				skillTargetSelectionMode = true;

				Vector3 newScale = new Vector3 ();
				newScale.x = skillRange;
				newScale.y = 0.01f;
				newScale.z = skillRange;
				obj.transform.localScale = newScale;
			}
		} else {
			selectedSkill = skill;
			skillRange = range;

			Vector3 newScale = new Vector3 ();
			newScale.x = skillRange;
			newScale.y = 0.01f;
			newScale.z = skillRange;
			obj.transform.localScale = newScale;
			
			obj.gameObject.SetActive (true);
			skillTargetSelectionMode = true;
		}


	}
	
	public bool skill1start = false;
	public bool skill2start = false;
	public bool skill3start = false;

	public float skill1AttackRange = 2.0f;
	public float skill2AttackRange = 10.0f;

	public Transform firePoint;

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
			damage (40.0f);
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

		targetEnemy = null;
		autoAttack = false;

		selectedSkill = 0;
		skillTargetSelectionMode = false;

		if (isMine) 
			nextPos = - Vector3.one;
		else 
			nextPos = destinationPos;

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

		firePoint = transform.Find ("FirePoint");

		InitState ();
	}

	public void SetAutoAttackMode(bool _autoAttack)
	{
		autoAttack = _autoAttack;
		
		if (autoAttack) {
			GameObject target = getNearestAttackTarget();
			if (target != null) {
				targetEnemy = target;
				nextPos = targetEnemy.transform.position;
			}
		}
	}

	public GameObject getNearestAttackTarget()
	{
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

			if (enemyIndex >= 0)
				return cldrs[enemyIndex].gameObject;
		}

		return null;
	}

	public GameObject getClickedAttackTarget()
	{
		RaycastHit hitInfo = new RaycastHit ();
		bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
		
		if (hit && 
		    (hitInfo.transform.gameObject.tag == "Player" || hitInfo.transform.gameObject.tag == "minion" || hitInfo.transform.gameObject.tag == "defender")) {
			
			IPlayer ip = hitInfo.transform.gameObject.GetComponent<IPlayer>();
			if (attachedTeam != ip.checkTeam()) {
				targetEnemy = hitInfo.transform.gameObject;
				return targetEnemy;
			}
		}

		return null;
	}



	// Update is called once per frame
	void Update () 
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

				GameObject targetEnemy = getClickedAttackTarget();
				if (targetEnemy != null) {
					nextPos = targetEnemy.transform.position;
				} else {
					nextPos = eventPos;
					return;
				}
				if (skillTargetSelectionMode) {

					if (targetEnemy) {
						float enemyDistance = Vector3.Distance(targetEnemy.transform.position, transform.position);
						IPlayer ip = targetEnemy.GetComponent<IPlayer> ();

						if (!ip.isDead) {
							Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
							transform.rotation = Quaternion.LookRotation (direction);

							if (enemyDistance <= skillRange) {
								fireSkill();
							} else {
								transform.position += direction * moveSpeed * Time.deltaTime;
							}
						} else {
							targetEnemy = null;
							if (isAttacking) {
								isAttacking = false;
							}
						}

					} else {
						Vector3 direction = (eventPos - transform.position).normalized;
						transform.position += direction * moveSpeed * Time.deltaTime;
						transform.rotation = Quaternion.LookRotation (direction);
						
						targetEnemy = null;
						if (isAttacking) {
							isAttacking = false;
						}
					}
				} else { //skillTargetSelectionMode == false
					if (targetEnemy != null) {
						if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
							isMoving = false;
							isAttacking = true;
						} else {
							isMoving = true;
							isAttacking = false;
						}
					} else {

						if (isAttacking)
							isAttacking = false;

						if (nextPos != -Vector3.one) {
							isMoving = true;

							Vector3 dir = (nextPos - transform.position).normalized;
							transform.rotation = Quaternion.LookRotation (dir);
							transform.position += dir * Time.deltaTime * moveSpeed;
						}
					
					}						
				}
			} else { // Mine, not a click

				if (skill2start) 
					return;

				if (autoAttack) {
					if (targetEnemy != null) {
						IPlayer ip = targetEnemy.GetComponent<IPlayer> ();
						
						if (!ip.isDead) {
							float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
							Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
							transform.rotation = Quaternion.LookRotation (dir);

							if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
								isMoving = false;
								isAttacking = true;
							} else {
								isMoving = true;
								isAttacking = false;
								transform.position += dir * Time.deltaTime * moveSpeed;
							}
						} else {
							targetEnemy = null;
						}
					} else {
						if (nextPos != Vector3.zero) {
							Vector3 direction = (nextPos - transform.position).normalized;
							transform.position += direction * moveSpeed * Time.deltaTime;
							transform.rotation = Quaternion.LookRotation (direction);
							
							isMoving = true;
							isAttacking = false;
						} else {
							GameObject target = getNearestAttackTarget();

							if (target != null) {
								targetEnemy = target;
								nextPos = targetEnemy.transform.position;

								float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
								Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
								transform.rotation = Quaternion.LookRotation (dir);
								
								if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
									isMoving = false;
									isAttacking = true;
								} else {
									transform.position += dir * Time.deltaTime * moveSpeed;
								}
							}
						}
					}
				} else {
					if (targetEnemy != null) {
						float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
						Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
						transform.rotation = Quaternion.LookRotation (dir);

						IPlayer ip = targetEnemy.GetComponent<IPlayer> ();
						
						if (!ip.isDead) {

							if (distance < attackRange) {
								isMoving = false;
								if (!isAttacking)
									isAttacking = true;
							} else {
								if (isAttacking) {
									isAttacking = false;
									isMoving = true;
									targetEnemy = null;

								} else {
									if (distance > 0.1)
										transform.position += dir * Time.deltaTime * moveSpeed;
								}
							}
						} else {
							targetEnemy = null;
							nextPos = -Vector3.one;
							isMoving = true;
							if (isAttacking)
								isAttacking = false;
						}
					} else {
						if (nextPos != -Vector3.one) {
							float distance = Vector3.Distance (transform.position, nextPos);

							if (distance > 0.1) {
								Vector3 dir = (nextPos - transform.position).normalized;
								transform.rotation = Quaternion.LookRotation (dir);
								transform.position += dir * Time.deltaTime * moveSpeed;
								isMoving = true;
							} else {
								isMoving = false;
							}
						} else {
							isMoving = false;
						}
					}
				} 
			}
		} else { // Not mine
			if (targetEnemy != null) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer> ();

				if (!ip.isDead) {
					float enemyDistance = Vector3.Distance(targetEnemy.transform.position, transform.position);

					if (enemyDistance <= attackRange) {
						if (!isAttacking) {
							isAttacking = true;
						}
						isMoving = false;
					} else {
						if (isAttacking) {
							isAttacking = false;
						} 
						isMoving = true;
						targetEnemy = null;
						nextPos = destinationPos;
					}
				} else {
					targetEnemy = null;
					nextPos = destinationPos;
					isMoving = true;
					if (isAttacking)
						isAttacking = false;
				}
			} else {
				GameObject target = getNearestAttackTarget();
				
				if (target != null) {
					targetEnemy = target;
					nextPos = targetEnemy.transform.position;
					
					float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
					Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
					transform.rotation = Quaternion.LookRotation (dir);
					
					if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
						isMoving = false;
						isAttacking = true;
					} else {
						transform.position += dir * Time.deltaTime * moveSpeed;
					}
				} else {
					isAttacking = false;

					if (nextPos != Vector3.zero) {
						Vector3 direction = (nextPos - transform.position).normalized;
						transform.position += direction * moveSpeed * Time.deltaTime;
						transform.rotation = Quaternion.LookRotation (direction);
					
						isMoving = true;
					}
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

	public void fireSkill()
	{
		if (selectedSkill == 1) {
			if (skill1start)
				return;

			if (targetEnemy != null) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer>();
				if (!ip.isDead) {
					Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
					transform.rotation = Quaternion.LookRotation (dir);
					
					skill1start = true;
					anim.SetBool ("skill01", true);
				}
			}

		} else if (selectedSkill == 2) {
			if (skill2start)
				return;
			
			if (targetEnemy != null) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer>();
				if (!ip.isDead) {
					isMoving = false;
					Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
					transform.rotation = Quaternion.LookRotation (dir);
					
					skill2start = true;
					anim.SetBool ("skill02", true);

					StartCoroutine ("Thunder", targetEnemy.transform.position);


				}
			}
		}

		skillTargetSelectionMode = false;
		ToggleSkillTargetMode (0, 0);

	}

	IEnumerator Thunder(Vector3 enemyPos) {
		yield return new WaitForSeconds(1.0f);

		GameObject obj = ObjectPool.instance.GetObjectForType ("Thunder", true);
		if (obj != null) {
			obj.transform.position = enemyPos;
			obj.GetComponent<ParticleSystem> ().Play ();
		}
		
		yield return new WaitForSeconds (1.0f);
		ObjectPool.instance.PoolObject(obj);
		skill2start = false;
		targetEnemy = null;
		nextPos = -Vector3.one;
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

		if (isMine && targetEnemy != null) {
			Vector3 enemyPos = targetEnemy.transform.position;
			pos.y += 1;
			Vector3 pos2 = Camera.main.WorldToScreenPoint (enemyPos);
			GUI.Label (new Rect (pos2.x, pos2.y, 50, 50), targetEnemy.name);
		}           
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
                                                                                                                                                                                                                                          