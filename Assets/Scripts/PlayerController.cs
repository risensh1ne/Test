using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour, IPlayer {
	
	float moveSpeed = 3.0f;
	float attackRange = 1.0f;
	float lookRange = 5.0f;

	public float skill1AttackRange = 1.5f;

	public GameManager.team attachedTeam;

	bool attackSelection = false;
	bool skill1start = false;
	bool skil2start = false;
	public bool skill3start = false;

	private Vector3 destinationPos;
	public GameObject targetEnemy;

	public Transform fireballPrefab;

	Vector2 healthBarSize = new Vector2(50, 5);
	
	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	private Transform firePoint;

	public RectTransform healthBarTransform;
	public Image visualHealth;
	public Text healthText;

	private float cachedY;
	private float minXValue;
	private float maxXValue;
	public float maxHealth = 100.0f;
	public float currentHealth;
	
	private float CurrentHealth
	{
		get { return currentHealth; }
		set { 
			currentHealth = value;
			HandleHealth ();
		}
	}

	private Animator anim;

	bool bWalking;
	bool bAttacking;
	bool bDead;
	
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

	public GameManager.team checkTeam() {
		return attachedTeam;
	}

	// Use this for initialization
	void Start () {
		destinationPos = transform.position;
		
		anim = GetComponent<Animator> ();

		firePoint = transform.Find ("FirePoint");
		if (firePoint == null) {
			Debug.Log ("Can't find firepoint");
			return;
		}

		attachedTeam = GameManager.team.BETA;

		cachedY = healthBarTransform.position.y;
		maxXValue = healthBarTransform.position.x;
		minXValue = healthBarTransform.position.x - healthBarTransform.rect.width;
		currentHealth = maxHealth;
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.name == "Fireball") {
			damage (5.0f);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (Input.GetMouseButtonDown (0)) {

			if (EventSystem.current.IsPointerOverGameObject())
				return;

			Vector3 eventPos = ScreenToWorld (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			eventPos.y = transform.position.y;

			if (attackSelection) {

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
								direction = (targetEnemy.transform.position - transform.position).normalized;
								transform.position += direction * moveSpeed * Time.fixedDeltaTime;
								transform.rotation = Quaternion.LookRotation (direction);

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

				attackSelection = false;
				transform.Find("attack_range").gameObject.SetActive (false);

			} else {
				RaycastHit hitInfo = new RaycastHit ();
				bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);

				if (hit && (hitInfo.transform.gameObject.tag == "minion" || hitInfo.transform.gameObject.tag == "defender")) {

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
		}
		
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
		updatePlayerState ();
	}

	private void HandleHealth()
	{
		healthText.text = "Player";
		float currentXValue = MapValues (currentHealth, 0, maxHealth, minXValue, maxXValue);

		healthBarTransform.position = new Vector3 (currentXValue, cachedY);

		if (currentHealth > maxHealth / 2) {
			visualHealth.color = new Color32((byte)MapValues(currentHealth, maxHealth/2, maxHealth, 255, 0), 255, 0, 255);
		} else {
			visualHealth.color = new Color32(255, (byte)MapValues(currentHealth, 0, maxHealth/2, 0, 255),0, 255);
		}
	}


	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}

	public void damage(float d)
	{
		if (isDead)
			return;

		if (CurrentHealth - 5 < 0)
			CurrentHealth = 0;
		else
			CurrentHealth -= 5;

		if (currentHealth <= 0) 
			Die ();
	}

	void Die()
	{
		Destroy (gameObject);
	}

	void updatePlayerState()
	{
		if (isMoving) {
			anim.SetBool ("isMoving", true);
			anim.SetBool ("isAttacking", false);
		} else if (isAttacking) {
			anim.SetBool ("isMoving", false);
			anim.SetBool ("isAttacking", true);
		} else {
			anim.SetBool ("isMoving", false);
			anim.SetBool ("isAttacking", false);
		}
	}
	
	void OnNormalAttack()
	{
		DamageEnemy (10.0f);
	}

	/*


	void OnSpecialAttack1End()
	{
		float kickDistance = 4.0f;
		Vector3 dir = (destinationPos - transform.position).normalized;

		transform.position += dir * kickDistance;
		specialAttack1start = false;
	}
	
	void OnSpecialAttack2()
	{
		DamageEnemy (20.0f);
		specialAttack2start = false;
	}

	void OnSpecialAttack2End()
	{
		specialAttack2start = false;
	}
*/
	public void SelectAttackTarget()
	{
		attackSelection = true;
		transform.Find("attack_range").gameObject.SetActive (true);

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
	
	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		
		GUI.BeginGroup (new Rect(pos.x - 30, Screen.height - pos.y - 90, healthBarSize.x, healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x, healthBarSize.y), progressBarBack);
		
		GUI.BeginGroup (new Rect(0, 0, healthBarSize.x * (currentHealth / 100), healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x * (currentHealth / 100), healthBarSize.y), progressBarHealth);
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
