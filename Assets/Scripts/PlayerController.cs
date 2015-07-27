using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	
	public bool attackSelection = false;
	
	public RectTransform healthBarTransform;
	public Image visualHealth;
	public Text healthText;

	private float cachedY;
	private float minXValue;
	private float maxXValue;
	public float maxHealth;
	
	// Use this for initialization
	void Start () {
		cachedY = healthBarTransform.position.y;
		maxXValue = healthBarTransform.position.x;
		minXValue = healthBarTransform.position.x - healthBarTransform.rect.width;
	}

	// Update is called once per frame
	void FixedUpdate () {


		/*
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
		*/
	}

	public void updateHealthBar()
	{
		GameObject player = gameObject.GetComponent<GameManager> ().player;
		if (player == null)
			return;
		maxHealth = player.GetComponent<HeroController> ().maxHealth;

		healthText.text = "Player";
		float currentHealth = player.GetComponent<HeroController> ().health;
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
	
	public void attackTargetSelectionMode(bool mode)
	{
		GameObject player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			return;
		}
		attackSelection = mode;
		player.transform.Find("attack_range").gameObject.SetActive (mode);
	}
	
}
