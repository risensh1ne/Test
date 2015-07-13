using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarControl : MonoBehaviour {

	public float maxHealth = 100.0f;
	public float currentHealth;

	public RectTransform healthBarTransform;
	public Image visualHealth;
	public Text healthText;


	private float cachedY;
	private float minXValue;
	private float maxXValue;
		
	private float CurrentHealth
	{
		get { return currentHealth; }
		set { 
			currentHealth = value;
			HandleHealth ();
		}
	}

	public HealthBarControl(float sizeX, float sizeY, float _maxHealth)
	{
		maxHealth = _maxHealth;
		sizeX = minXValue;
		sizeY = maxXValue;
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

	// Use this for initialization
	void Start () {
		healthBarTransform = GetComponent<RectTransform> ();
		visualHealth = GetComponent<Image> ();

		cachedY = healthBarTransform.position.y;
		maxXValue = healthBarTransform.position.x;
		minXValue = healthBarTransform.position.x - healthBarTransform.rect.width;
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
