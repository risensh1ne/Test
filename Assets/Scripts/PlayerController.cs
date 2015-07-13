using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	
	float moveSpeed = 3.0f;
	float attackDistance = 1.0f;
	bool isMoving = false;
	bool isAttacking = false;
		
	bool specialAttack1start = false;
	bool specialAttack2start = false;
	
	private Vector3 destinationPos;
	Transform targetEnemy;

	Vector2 healthBarSize = new Vector2(50, 5);
	
	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

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



	// Use this for initialization
	void Start () {
		destinationPos = transform.position;
		
		anim = GetComponent<Animator> ();

		cachedY = healthBarTransform.position.y;
		maxXValue = healthBarTransform.position.x;
		minXValue = healthBarTransform.position.x - healthBarTransform.rect.width;
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (specialAttack1start)
			return;

		if (Input.GetMouseButtonDown (0)) {
			
			Vector3 eventPos = ScreenToWorld (new Vector2 (Input.mousePosition.x, Input.mousePosition.y));
			eventPos.y = transform.position.y;
			
			RaycastHit hitInfo = new RaycastHit ();
			bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
			if (hit && hitInfo.transform.gameObject.tag == "enemy") {
				targetEnemy = hitInfo.transform;
				
				if (Vector3.Distance (transform.position, targetEnemy.position) < attackDistance) {
					isMoving = false;
					isAttacking = true;
				} else {
					isMoving = true;
					isAttacking = true;
					destinationPos = targetEnemy.position;
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
		} else if (Input.GetKeyDown (KeyCode.A)) {
			if (targetEnemy != null && specialAttack1start == false) {
				specialAttack1start = true;
				anim.SetTrigger ("special01");
				return;
			}
		} else if (Input.GetKeyDown (KeyCode.S)) {
			if (targetEnemy != null && specialAttack2start == false) {
				specialAttack2start = true;
				anim.SetTrigger ("special02");
			}
		} 
		
		if (isMoving || isAttacking) {
			float distance = Vector3.Distance (transform.position, destinationPos);
			
			if (targetEnemy != null) {
				if (Vector3.Distance (transform.position, targetEnemy.position) < attackDistance) {
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
	
	void OnSpecialAttack1()
	{
		DamageEnemy (40.0f);
	}

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
	
	void DamageEnemy(float damage)
	{
		
		targetEnemy.gameObject.GetComponent<EnemyController>().damage (damage);
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
