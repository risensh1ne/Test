using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour {

	public float moveSpeed = 2.0f;
	public float currentHealth = 100.0f;
	Vector2 healthBarSize = new Vector2(50, 5);

	public float attackDistance = 1.0f;
	public float lookDistance = 10.0f;

	public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	bool isMoving = false;
	bool isAttacking = false;

	Animator anim;

	public void damage(float d)
	{
		currentHealth -= d;

		if (currentHealth <= 0) {

			Die ();
		}
	}

	void Die()
	{
		Destroy (gameObject);
	}

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (anim == null)
			return;

		GameObject player = GameObject.FindGameObjectWithTag("Player");

		float distance = Vector3.Distance (transform.position, player.transform.position);

		if (distance < lookDistance) {
			Vector3 dir = (player.transform.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation (dir);
	
			isMoving = true;
			anim.SetBool ("isMoving", true);
		
			if (isAttacking == false)
				transform.position += dir * moveSpeed * Time.deltaTime;


			if (distance < attackDistance) {
				if (isAttacking == false) {
					if (isMoving) {
						isMoving = false;
						anim.SetBool ("isMoving", false);
					}

					anim.SetBool ("isAttacking", true);
					isAttacking = true;
				}
			} else {
				if (isAttacking == true) {
					anim.SetBool ("isAttacking", false);
					isAttacking = false;
				}
			}
		} else {
			if (isMoving) {
				isMoving = false;
				anim.SetBool ("isMoving", false);
			}
		}

	}

	void OnNormalAttack()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");

		float distance = Vector3.Distance (transform.position, player.transform.position);
		if (distance < attackDistance)
			player.GetComponent<PlayerController> ().damage (10.0f);
	}

	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

		GUI.BeginGroup (new Rect(pos.x - 30, Screen.height - pos.y - 70, healthBarSize.x, healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x, healthBarSize.y), progressBarBack);
		
		GUI.BeginGroup (new Rect(0, 0, healthBarSize.x * (currentHealth / 100), healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x * (currentHealth / 100), healthBarSize.y), progressBarHealth);
		GUI.EndGroup();
		GUI.EndGroup ();

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		float distance = Vector3.Distance (transform.position, player.transform.position);
		GUI.Label (new Rect (100, 100, 50, 20), distance.ToString ());
	}
}
                                                                                                                                                                                                                                          