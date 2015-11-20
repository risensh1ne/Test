using UnityEngine;
using System.Collections;

public class MinionController : Photon.MonoBehaviour, IPlayer {

	public GameObject gm;
    
	public enum CharacterState {STATE_MOVING, STATE_ATTACKING, STATE_DEAD};
	public CharacterState curr_state;

	public float health = 50.0f;
	[System.NonSerialized]
	public float lookRange = 4.0f;
	public float attackRange = 1.0f;

	public Vector3 startPos;
	public Vector3 endPos;
	public float walkSpeed;
	
	public GameManager.team attachedTeam;

	public Vector3 realPosition;
	public Quaternion realRotation = Quaternion.identity;
	private bool gotFirstUpdate;
	
	private int originalWidth = 1024;
	private int originalHeight = 600;
	private Vector3 guiScale;

	private float my_exp_val = 100.0f;
	
	GameObject targetEnemy;

	bool bMoving;
	bool bAttacking;
	bool bDead;
	
	private GameObject lastAttackedBy;

	public Animator anim;

	public GameManager.team checkTeam() {
		return attachedTeam;
	}

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

	public void SetAttacker(GameObject attacker)
	{
		lastAttackedBy = attacker;
	}

	// Use this for initialization
	void Start () {

		guiScale.x = (float)Screen.width / (float)originalWidth; // calculate hor scale
		guiScale.y = (float)Screen.height / (float)originalHeight; // calculate vert scale
		guiScale.z = 1.0f;
	}

	public void initState(GameManager.team team) {

		if (gm == null)
			gm = GameObject.Find ("_GM").gameObject;
		if (anim == null) 
			anim = GetComponent<Animator> ();

		attachedTeam = team;

		if (team == GameManager.team.BETA) {
			startPos = gm.GetComponent<GameManager>().betaHome.position;
			endPos = gm.GetComponent<GameManager>().alphaHome.position;
		} else {
			startPos = gm.GetComponent<GameManager>().alphaHome.position;
			endPos = gm.GetComponent<GameManager>().betaHome.position;
		}

		resetState ();
	}

	public void resetState() {

		isDead = false;
		isMoving = false;
		isAttacking = false;
		
		lastAttackedBy = null;
		
		health = 50.0f;
		walkSpeed = 2.0f;
		
		changeStateTo (CharacterState.STATE_MOVING);
		
		Vector3 direction = (endPos - transform.position).normalized;
		transform.position = startPos;
		transform.rotation = Quaternion.LookRotation (direction);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) 
	{
		//Debug.Log (photonView.viewID + "-->" + info.photonView.viewID + ":" + health);

		if (stream.isWriting) {
			stream.SendNext (gameObject.transform.position);
			stream.SendNext (gameObject.transform.rotation);
			stream.SendNext (health);
			
			stream.SendNext (isMoving);
			stream.SendNext (isAttacking);
			stream.SendNext (isDead);
		} else {
			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
			health = (float)stream.ReceiveNext();

			isMoving = (bool)stream.ReceiveNext();
			isAttacking = (bool)stream.ReceiveNext();
			isDead = (bool)stream.ReceiveNext();

			if(gotFirstUpdate == false) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				gotFirstUpdate = true;
			}

			//if (health == 0) {
			//	StartCoroutine("Die");
			//}
		}		
	}

	public void changeStateTo(CharacterState newState)
	{
		if (newState == CharacterState.STATE_ATTACKING) {
			if (curr_state == CharacterState.STATE_MOVING) {
				isAttacking = true;
			} 
		} else if (newState == CharacterState.STATE_MOVING) {
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} 
		} else if (newState == CharacterState.STATE_DEAD) {
			isDead = true;
			if (curr_state == CharacterState.STATE_ATTACKING) {
				isAttacking = false;
			} 
			targetEnemy = null;
		}
		
		curr_state = newState; 
	}

	public void OnNormalAttack() {
		if (targetEnemy != null) {
			if (targetEnemy.tag == "Player" || targetEnemy.tag == "minion" || targetEnemy.tag == "defender" ) {
				IPlayer ip = targetEnemy.GetComponent<IPlayer>();
				if (!ip.isDead) {
					//targetEnemy.GetComponent<PhotonView> ().RPC ("DamageSync", PhotonTargets.All, 10.0f);
					targetEnemy.GetComponent<MinionController>().damage(10.0f);
				}
			}
		}
	}

	[PunRPC]
	void DamageSync(float d)
	{
		damage (d);
	}

	public void damage(float d)
	{
		if (isDead)
			return;

		health -= d;
		if (health <= 0) {
			if (curr_state != CharacterState.STATE_DEAD)
				StartCoroutine("Die");
		}
	}
	
	IEnumerator Die()
	{
		//isDead = true;
		//isMoving = false;
		//isAttacking = false;

		changeStateTo (CharacterState.STATE_DEAD);

        if (lastAttackedBy && lastAttackedBy.tag == "Player") {
			lastAttackedBy.GetComponent<HeroController>().GainExp(my_exp_val);
			lastAttackedBy.GetComponent<HeroController>().GainGold(20);

            GameObject coinObj = ObjectPool.instance.GetObjectForType("coin", true);
            if (coinObj != null)
            {
                coinObj.transform.parent = gameObject.transform;
                coinObj.transform.rotation = Quaternion.identity;
                coinObj.transform.localPosition = new Vector3(0, 1, 0);
            }

            GameObject statObj = ObjectPool.instance.GetObjectForType("FloatingDigit", true);
            if (statObj != null)
            {
                statObj.transform.SetParent (gm.GetComponent<GameManager>().UI.transform);
                statObj.GetComponent<GUIText>().text = "+20";

				statObj.GetComponent<GUIText>().fontSize = (int)(20 * guiScale.y);

                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                
                pos.x = Mathf.Clamp((pos.x - 10) / Screen.width, 0.05f, 0.95f);
                pos.y = Mathf.Clamp((pos.y + 10) / Screen.height, 0.05f, 0.9f);
                pos.z = 1;
                statObj.GetComponent<RectTransform>().position = pos;
            }
        
            yield return new WaitForSeconds(0.5f);
            ObjectPool.instance.PoolObject(coinObj);   

			yield return new WaitForSeconds(0.5f);
			ObjectPool.instance.PoolObject(statObj);
        }

		yield return new WaitForSeconds (3.0f);

		NetworkObjectPool.instance.DespawnObject(gameObject);

		yield return null;
	}


    void OnParticleCollision(GameObject other)
	{
		if (other.name == "Fireball") {
            if (other.GetComponent<FireballData>().team != attachedTeam)
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

		if (anim == null) 
			return;

		if (photonView.isMine) {
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
						changeStateTo(CharacterState.STATE_ATTACKING);
					} else {
						changeStateTo(CharacterState.STATE_MOVING);

						direction = (cldrs [enemyIndex].transform.position - transform.position).normalized;
						transform.position += direction * walkSpeed * Time.fixedDeltaTime;
						transform.rotation = Quaternion.LookRotation (direction);
					}
				} else {
					changeStateTo(CharacterState.STATE_MOVING);

					direction = (endPos - transform.position).normalized;
					transform.position += direction * walkSpeed * Time.fixedDeltaTime;
					transform.rotation = Quaternion.LookRotation (direction);
				}
			} else {
				changeStateTo(CharacterState.STATE_MOVING);

				direction = (endPos - transform.position).normalized;
				transform.position += direction * walkSpeed * Time.fixedDeltaTime;
				transform.rotation = Quaternion.LookRotation (direction);
			}
		} else {
			transform.position = realPosition; //Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = realRotation; //Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
		}

	}
}
