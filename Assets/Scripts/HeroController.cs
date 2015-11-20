using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HeroController : Photon.MonoBehaviour, IPlayer {

	public enum CharacterState {STATE_IDLE, STATE_MOVING, STATE_ATTACKING, STATE_SKILL, STATE_DEAD};
	public CharacterState curr_state;
	[System.NonSerialized]
	public float moveSpeed = 3.0f;
	public float health;
	public float mana;
	public float healthRegenRate, manaRegenRate;
	public int level;
	public float max_exp_val, curr_exp_val, my_exp_val;
	public float maxHealth, maxMana;
	public float statAttack, statDefense;
    public string heroName;

	public Transform range_obj;

	Vector2 healthBarSize = new Vector2(50, 5);

	public float attackRange = 1.5f;
	public float lookRange = 3.0f;

	public int goldAmount = 100;
	private float goldIncreaseRatePerSec = 5.0f;
	private float goldTimer =0;

	public GameManager.team attachedTeam;

	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	public Vector3 startPos, endPos;
	public Vector3 destinationPos;
	public GameObject targetEnemy;

	public Vector3 realPosition;
	public Quaternion realRotation = Quaternion.identity;

	public GameObject gm;
	public GameObject UI;

	public bool autoAttack;

	public int skill1Level = 0;
	public int skill2Level = 0;
	public int skill3Level = 0;

	public int skillUpgradeCnt = 1;
	
	public bool skillTargetSelectionMode;
	public int selectedSkill;
	public float skillRange;

	private GameObject lastAttackedBy;

	private bool gotFirstUpdate;
	
	Animator anim;

    [PunRPC]
	public void init(object[] param)
	{
		attachedTeam = (GameManager.team)param[0];
		startPos = (Vector3)param[1];
		endPos = (Vector3)param[2];
	}

	[PunRPC]
	void DamageSync(float d)
	{
		damage (d);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
	{
		if (anim == null)
			anim = GetComponent<Animator> ();

		if (stream.isWriting) {
			stream.SendNext (gameObject.transform.position);
			stream.SendNext (gameObject.transform.rotation);
			stream.SendNext (health);

			stream.SendNext (isMoving);
			stream.SendNext (isAttacking);
			stream.SendNext (isDead);

			stream.SendNext (anim.GetBool ("skill01"));
			stream.SendNext (anim.GetBool ("skill02"));
			stream.SendNext (anim.GetBool ("skill03"));

		} else {
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
			health = (float)stream.ReceiveNext();

			isMoving = (bool)stream.ReceiveNext();
			isAttacking = (bool)stream.ReceiveNext();
			isDead = (bool)stream.ReceiveNext();

			anim.SetBool ("skill01", (bool)stream.ReceiveNext());
			anim.SetBool ("skill02", (bool)stream.ReceiveNext());
			anim.SetBool ("skill03", (bool)stream.ReceiveNext());

			if(gotFirstUpdate == false) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				gotFirstUpdate = true;
			}

			if (health == 0) {
				StartCoroutine("Die");
			}
		}
	}

	public void ToggleSkillTargetMode(int skill, float range)
	{
		if (skill == 0) {
			selectedSkill = 0;	
			//obj.gameObject.SetActive (false);
			skillTargetSelectionMode = false;
		} else if (skillTargetSelectionMode && skill == selectedSkill) {
			selectedSkill = 0;
			//obj.gameObject.SetActive (false);
			skillTargetSelectionMode = false;
		} else {
			selectedSkill = skill;
			skillRange = range;
			//obj.gameObject.SetActive (true);
			skillTargetSelectionMode = true;
		}

		//skillTargetSelectionMode = !skillTargetSelectionMode;
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

	public void init_hero(GameManager.team team, Vector3 startPos, Vector3 destPos)
	{
		object[] param = new object[3]{team, startPos, destPos};

		gameObject.GetComponent<PhotonView> ().RPC ("init", PhotonTargets.AllBuffered, param);

	}

	public void changeStateTo(CharacterState newState)
	{
		if (newState == CharacterState.STATE_ATTACKING) {
			if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
				isAttacking = true;
			} else if (curr_state == CharacterState.STATE_IDLE) {
				isAttacking = true;
			} else if (curr_state == CharacterState.STATE_SKILL) {
				isAttacking = true;
			} 
		} else if (newState == CharacterState.STATE_MOVING) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isMoving = true;
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_IDLE) {
				isMoving = true;
			} else if (curr_state == CharacterState.STATE_SKILL) {
				isMoving = true;
			} 
		} else if (newState == CharacterState.STATE_IDLE) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
			} 
			destinationPos = -Vector3.one;
		} else if (newState == CharacterState.STATE_SKILL) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
			}
			destinationPos = -Vector3.one;
		} else if (newState == CharacterState.STATE_DEAD) {
			isDead = true;
			targetEnemy = null;

			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} else if (curr_state == CharacterState.STATE_MOVING) {
				isMoving = false;
			}
		}

		curr_state = newState; 
	}

	public GameManager.team checkTeam() {
		return attachedTeam;
	}
	
	public void damage(float d)
	{
		if (isDead)
			return;

		health -= d;
		if (health <= 0) 
			health = 0;

		gm.GetComponent<PlayerController> ().updateHealthBar (gameObject);

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
		gm.GetComponent<PlayerController> ().updateManaBar (gameObject);
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
			damage (30.0f);
		} else if (other.name == "Explosion") {
			damage (30.0f);
		} else if (other.name == "energyBlast") {
			damage (20.0f);
		}
	}

	public void GainExp(float exp_val)
	{
		targetEnemy = null;
		changeStateTo (CharacterState.STATE_IDLE);

		curr_exp_val += exp_val;
		if (curr_exp_val >= max_exp_val)
			LevelUp ();

		gm.GetComponent<PlayerController> ().updateLevelText (gameObject);	
	}

	public void GainGold(int _goldAmount)
	{
		goldAmount += _goldAmount;
	}

	IEnumerator Die()
	{
		changeStateTo (CharacterState.STATE_DEAD);

		if (photonView.isMine) {
			UI.GetComponent<UIManager>().respawnLayer.SetActive (true);
		}

		yield return new WaitForSeconds (4.0f);

		if (photonView.isMine) {
			UI.GetComponent<UIManager>().respawnLayer.SetActive (false);
		}

        InitState ();
	}

	void InitState()
	{
		isDead = false;
		isMoving = false;
		isAttacking = false;
		autoAttack = false;

		skillTargetSelectionMode = false;

		transform.position = startPos;
		if (!photonView.isMine)
			destinationPos = endPos;

		changeStateTo (CharacterState.STATE_IDLE);

		range_obj = transform.Find ("attack_range");
		Vector3 newScale = new Vector3 ();
		newScale.x = 10.0f;
		newScale.y = 0.1f;
		newScale.z = 10.0f;
		range_obj.transform.localScale = newScale;

		targetEnemy = null;
		lastAttackedBy = null;

		updateMaxStats ();
		ResetStats ();

		gm.GetComponent<PlayerController> ().updateHealthBar (gameObject);
		gm.GetComponent<PlayerController> ().updateManaBar (gameObject);
		gm.GetComponent<PlayerController> ().updateLevelText (gameObject);
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
		skillUpgradeCnt++;
		max_exp_val = 500.0f + (level - 1) * 500.0f;
		curr_exp_val = 0;
		
		updateMaxStats ();
		ResetStats ();

		if (photonView.isMine) {
			gm.GetComponent<PlayerController> ().updateHealthBar (gameObject);
			gm.GetComponent<PlayerController> ().updateManaBar (gameObject);
			gm.GetComponent<PlayerController> ().updateLevelText (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();

		gm = GameObject.Find ("_GM");
		UI = GameObject.Find ("UI");

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

        if (photonView.isMine) {
			if (isDead)
				return;

			goldTimer += Time.fixedDeltaTime;
			if (goldTimer / 1.0f * goldIncreaseRatePerSec > 1.0f) {
				goldAmount++;
				goldTimer = 0;
			}

			if (skillTargetSelectionMode)
				range_obj.gameObject.SetActive(true);
			else
				range_obj.gameObject.SetActive(false);

            if (skill1start || skill2start || skill3start)
				return;

			if (Input.GetMouseButtonDown (0)) {

				if (IsPointerOverUIObject ()) {
					return;
				}

				Vector3 eventPos = ScreenToWorld (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
				eventPos.y = transform.position.y;

				if (skillTargetSelectionMode) {
					if (Vector3.Distance (transform.position, eventPos) < 5.0f) {
						gameObject.GetComponent<PhotonView> ().RPC ("OnSkill2", PhotonTargets.All, eventPos);
						UI.GetComponent<UIManager> ().OnSkill2Fire ();
						ToggleSkillTargetMode (0, 0);
						skill2start = true;
					} else {
						ToggleSkillTargetMode (0, 0);
					}

				} else {
					destinationPos = eventPos;

					targetEnemy = getClickedAttackTarget ();
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
		} else {
			transform.position = realPosition; //Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = realRotation; //Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}
	
	}

	void DamageEnemy(float damage)
	{
		if (targetEnemy != null) {
			IPlayer ip = targetEnemy.gameObject.GetComponent<IPlayer> ();

			if (!ip.isDead) {
				ip.SetAttacker(this.gameObject);
				targetEnemy.GetComponent<PhotonView> ().RPC ("DamageSync", PhotonTargets.All, damage);
			}
		}
	}

	void Skill1Damage()
	{
		DamageEnemy (40.0f);
		skill1start = false;
	}

	[PunRPC]
	public void OnSkill1()
	{
        if (skill1start)
			return;

		if (targetEnemy != null) {

			if (name == "Gion(Clone)") {
				Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
				transform.rotation = Quaternion.LookRotation (dir);

				skill1start = true;
				anim.SetBool ("skill01", true);
				useMana(20.0f);
			} else if (name == "SwordMaster(Clone)") {
				Vector3 dir = (targetEnemy.transform.position - transform.position).normalized;
				transform.rotation = Quaternion.LookRotation (dir);
				
				skill1start = true;
				anim.SetBool ("skill01", true);
				useMana(20.0f);
			}
		}
	}

	[PunRPC]
	public void OnSkill2(Vector3 targetPos)
	{
		if (skill2start)
			return;

		changeStateTo (CharacterState.STATE_SKILL);

		if (name == "Gion(Clone)") {
			Vector3 dir = (targetPos - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation (dir);
			skill2start = true;
			anim.SetTrigger ("skill02");
			StartCoroutine ("FireExplosion", targetPos);
		} else if (name == "SwordMaster(Clone)") {
			Vector3 dir = (targetPos - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation (dir);
			skill2start = true;
			anim.SetTrigger ("skill02");
			StartCoroutine ("OnSkill2End", targetPos);
		} 
	}

	IEnumerator OnSkill2End(Vector3 targetPos)
	{
		yield return new WaitForSeconds(1.4f);

		GameObject obj = ObjectPool.instance.GetObjectForType ("energyBlast", true);
		if (obj != null) {
			GameObject[] attackedList = getAttackTargetList(targetPos, 2.0f);
			
			for (int i=0; i < attackedList.Length; i++) {
				attackedList[i].GetComponent<IPlayer>().SetAttacker(this.gameObject);
			}

			obj.transform.position = targetPos;
			obj.GetComponent<ParticleSystem> ().Play ();
		}
		useMana (10.0f);
		skill2start = false;

		yield return new WaitForSeconds (2.0f);
		ObjectPool.instance.PoolObject (obj);
	}

	[PunRPC]
	public void OnSkill3()
	{
		if (skill3start)
			return;

		changeStateTo (CharacterState.STATE_SKILL);
		skill3start = true;
		anim.SetTrigger ("skill03");
		StartCoroutine ("Cyclone");
		
	}
	
	IEnumerator FireExplosion(Vector3 targetPos) {
		yield return new WaitForSeconds(1.0f);

		GameObject obj = ObjectPool.instance.GetObjectForType ("Explosion", true);
		if (obj != null) {
			GameObject[] attackedList = getAttackTargetList(targetPos, 2.0f);

			for (int i=0; i < attackedList.Length; i++) {
				attackedList[i].GetComponent<IPlayer>().SetAttacker(this.gameObject);
				attackedList[i].GetComponent<IPlayer>().damage(30.0f);
			}

			obj.transform.position = targetPos;
			obj.GetComponent<ParticleSystem> ().Play ();
		}
		useMana (50.0f);
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
		changeStateTo (CharacterState.STATE_IDLE);

		yield return new WaitForSeconds (5.0f);
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
                                                                                                                                                                                                                                          