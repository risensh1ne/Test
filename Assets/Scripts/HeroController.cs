using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HeroController : MonoBehaviour, IPlayer {

	public enum CharacterState {STATE_IDLE, STATE_MOVING, STATE_ATTACKING};
	public CharacterState curr_state;
	public float moveSpeed = 5.0f;
	public float health;
	public float mana;
	public float healthRegenRate, manaRegenRate;
	public int level;
	public float max_exp_val, curr_exp_val, my_exp_val;
	public float maxHealth, maxMana;

	Vector2 healthBarSize = new Vector2(50, 5);

	public float attackRange = 1.5f;
	public float lookRange = 3.0f;

	public GameManager.team attachedTeam;

	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	public Vector3 destinationPos;
	public GameObject targetEnemy;

	public GameObject gm;

	public bool autoAttack;

	public bool skillTargetSelectionMode;
	public int selectedSkill;
	public float skillRange;

	private GameObject lastAttackedBy;

	public void ToggleSkillTargetMode(int skill, float range)
	{
		Transform obj = transform.Find ("attack_range");

		if (skill == 0) {
			selectedSkill = 0;	
			obj.gameObject.SetActive (false);
		} else if (skillTargetSelectionMode && skill == selectedSkill) {
			selectedSkill = 0;

			obj.gameObject.SetActive (false);
		} else {
			selectedSkill = skill;
			skillRange = range;

			Vector3 newScale = new Vector3 ();
			newScale.x = skillRange;
			newScale.y = 0.01f;
			newScale.z = skillRange;
			obj.transform.localScale = newScale;
			
			obj.gameObject.SetActive (true);
		}

		skillTargetSelectionMode = !skillTargetSelectionMode;
	}

	public void SetAttacker(GameObject attacker)
	{
		lastAttackedBy = attacker;
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

	public void changeStateTo(CharacterState newState)
	{
		if (newState == CharacterState.STATE_ATTACKING) {
			if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
				isAttacking = true;
			} else if (curr_state == CharacterState.STATE_IDLE) {
				isAttacking = true;
			} 
		} else if (newState == CharacterState.STATE_MOVING) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isMoving = true;
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_IDLE) {
				isMoving = true;
			} 
		} else if (newState == CharacterState.STATE_IDLE) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
			} 
			destinationPos = -Vector3.one;
		} 
		curr_state = newState; 
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
		if (health <= 0) 
			health = 0;

		gm.GetComponent<PlayerController> ().updateHealthBar ();

		if (health == 0) {
			StartCoroutine("Die");
		}
	}

	public void useMana(float d)
	{
		if (isDead)
			return;

		mana -= d;

		if (mana < 0)
			mana = 0;
		gm.GetComponent<PlayerController> ().updateManaBar ();
	}

	public void regenHealthMana()
	{
		if (health < maxHealth)
			health += healthRegenRate;
		else {
			health = maxHealth;
		}
		if (mana < maxMana)
			mana += manaRegenRate;
		else {
			mana = maxMana;
		}
	}

	void OnParticleCollision(GameObject other)
	{
		if (other.name == "Fireball") {
			damage (40.0f);
		}
	}

	public void GainExp(float exp_val)
	{
		targetEnemy = null;

		curr_exp_val += exp_val;
		if (curr_exp_val >= max_exp_val)
			LevelUp ();

		gm.GetComponent<PlayerController> ().updateLevelText ();
		
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
		autoAttack = false;

		curr_state = CharacterState.STATE_IDLE;
		targetEnemy = null;
		lastAttackedBy = null;

		updateMaxStats ();
		ResetStats ();

		gm.GetComponent<PlayerController> ().updateHealthBar ();
		gm.GetComponent<PlayerController> ().updateManaBar ();
		gm.GetComponent<PlayerController> ().updateLevelText ();

		if (attachedTeam == GameManager.team.ALPHA)
			transform.position = gm.GetComponent<GameManager> ().alphaHome.position;
		else 
			transform.position = gm.GetComponent<GameManager> ().betaHome.position;
	}

	void updateMaxStats()
	{
		maxHealth = 100.0f + level * 50;
		maxMana = 50.0f + level * 30;
		
		healthRegenRate = 1.0f + level * 0.2f;
		manaRegenRate = 1.0f + level * 0.1f;
	}

	void ResetStats()
	{
		health = maxHealth;
		mana = maxMana;
	}

	void LevelUp()
	{
		level++;
		max_exp_val = 500.0f + (level - 1) * 500.0f;
		curr_exp_val = 0;
		
		updateMaxStats ();
		ResetStats ();

		if (isMine) {
			gm.GetComponent<PlayerController> ().updateHealthBar ();
			gm.GetComponent<PlayerController> ().updateManaBar ();
			gm.GetComponent<PlayerController> ().updateLevelText ();
		}
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		gm = GameObject.Find ("_GM");

		firePoint = transform.Find ("FirePoint");

		curr_exp_val = 0;
		max_exp_val = 500.0f;
		level = 1;

		InitState ();
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
					if (!ip.isDead && attachedTeam != ip.checkTeam ())
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

	public GameObject[] getAttackTargetList(Vector3 targetPos, float range)
	{
		Collider[] cldrs = Physics.OverlapSphere (targetPos, range);

		List<GameObject> objList = new List<GameObject> ();

		for (int i=0; i < cldrs.Length; i++) {
			if (cldrs [i].tag == "Player" || cldrs [i].tag == "minion" || cldrs [i].tag == "defender") {
				IPlayer ip = cldrs [i].gameObject.GetComponent<IPlayer> ();
				if (!ip.isDead && attachedTeam != ip.checkTeam ())
					objList.Add (cldrs [i].transform.gameObject);
			}
		}

		return objList.ToArray();
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

	private bool IsPointerOverUIObject() {
		// Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
		// the ray cast appears to require only eventData.position.
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if (anim == null)
			return;

		if (isDead)
			return;

		if (skill2start || skill3start)
			return;

		if (Input.GetMouseButtonDown(0)) {
		/*
			bool isPointerOverGameObject = false;

			if(EventSystem.current.currentInputModule is TouchInputModule) {
				for(int i=0; i<Input.touchCount; i++) {
					Touch touch = Input.touches[i];
					if( touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended) {
						if(EventSystem.current.IsPointerOverGameObject( Input.touches[i].fingerId )) {
							isPointerOverGameObject = true;
							break;
						}
					}
				}
			} else {
				isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
			}
*/

			if (IsPointerOverUIObject()) {
				return;
			}

			Vector3 eventPos = ScreenToWorld (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			eventPos.y = transform.position.y;

			if (skillTargetSelectionMode) {
				if (Vector3.Distance (transform.position, eventPos) < skillRange) {
					OnSkill2 (eventPos);
					gm.GetComponent<UIManager>().OnSkill2Fire();
					ToggleSkillTargetMode(0, 0);
					skill2start = true;
				} else {
					ToggleSkillTargetMode(0, 0);
				}

			} else {
				destinationPos = eventPos;

				targetEnemy = getClickedAttackTarget();
			}
		}
		
		if (destinationPos != -Vector3.one) {
			Vector3 direction = (destinationPos - transform.position).normalized;

			if (Vector3.Distance (transform.position, destinationPos) < 0.1) {
				changeStateTo (CharacterState.STATE_IDLE);
				destinationPos = -Vector3.one;
			} else {
				transform.position += direction * moveSpeed * Time.fixedDeltaTime;
				transform.rotation = Quaternion.LookRotation (direction);
				changeStateTo (CharacterState.STATE_MOVING);	
			}
		} else {
			if (targetEnemy != null) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer> ();
				if (!ip.isDead) {
					if (Vector3.Distance (transform.position, targetEnemy.transform.position) < attackRange) {
						changeStateTo (CharacterState.STATE_ATTACKING);
					} else {
						Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
						transform.position += direction * moveSpeed * Time.fixedDeltaTime;
						transform.rotation = Quaternion.LookRotation (direction);	
						changeStateTo (CharacterState.STATE_MOVING);
					}
				} else {
					changeStateTo (CharacterState.STATE_IDLE);
				}
			} else {
				targetEnemy = getNearestAttackTarget ();
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
				ip.SetAttacker(this.gameObject);
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
			//Vector3 dir = (destinationPos - transform.position).normalized;
			//transform.rotation = Quaternion.LookRotation (dir);

			skill1start = true;
			anim.SetBool ("skill01", true);
			mana -= 20.0f;
		}
	}

	public void OnSkill2(Vector3 targetPos)
	{
		if (skill2start)
			return;

		Vector3 dir = (targetPos - transform.position).normalized;
		transform.rotation = Quaternion.LookRotation (dir);
		skill2start = true;
		anim.SetTrigger ("skill02");
		StartCoroutine ("FireExplosion", targetPos);

	}

	public void OnSkill3()
	{
		if (skill3start)
			return;

		skill3start = true;
		anim.SetTrigger ("skill03");
		StartCoroutine ("Cyclone");
		
	}
	
	IEnumerator FireExplosion(Vector3 targetPos) {
		yield return new WaitForSeconds(1.0f);

		GameObject obj = ObjectPool.instance.GetObjectForType ("Explosion", true);
		if (obj != null) {
			GameObject[] attackedList = getAttackTargetList(targetPos, 2.0f);

			Debug.Log (attackedList.Length);
			for (int i=0; i < attackedList.Length; i++) {

				attackedList[i].GetComponent<IPlayer>().SetAttacker(this.gameObject);
			}

			obj.transform.position = targetPos;
			obj.GetComponent<ParticleSystem> ().Play ();
		}
		mana -= 50.0f;
		skill2start = false;

		yield return new WaitForSeconds (3.0f);
		ObjectPool.instance.PoolObject (obj);
	}

	IEnumerator Cyclone()
	{
		float origHealthRegenRate = healthRegenRate;
		float origManaRegenRate = manaRegenRate;
		
		healthRegenRate *= 10;
		manaRegenRate *= 10;

		GameObject obj = ObjectPool.instance.GetObjectForType ("Cyclone", true);
		if (obj != null) {
			obj.transform.position = transform.position;
			obj.GetComponent<ParticleSystem> ().Play ();

		}
		mana -= 50.0f;
		skill3start = false;
		
		yield return new WaitForSeconds (10.0f);
		healthRegenRate = origHealthRegenRate;
		manaRegenRate = origManaRegenRate;
		ObjectPool.instance.PoolObject (obj);
	}

	void OnNormalAttack()
	{
		if (targetEnemy == null)
			return;

		float distance = Vector3.Distance (transform.position, targetEnemy.transform.position);
		if (distance < attackRange)
			DamageEnemy (40.0f);
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
			enemyPos.y += 1;
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
                                                                                                                                                                                                                                          