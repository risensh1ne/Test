using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public bool attackSelection = false;
	
	public RectTransform healthBarTransform;
	public RectTransform manaBarTransform;
	public Image visualHealth, visualMana;
	public Text healthText, manaText, levelText;

	private float healthbar_cachedY;
	private float healthbar_minXValue;
	private float healthbar_maxXValue;
	public float healthbar_maxHealth;

	private float manabar_cachedY;
	private float manabar_minXValue;
	private float manabar_maxXValue;
	public float manabar_maxMana;

	public float regenTimer;

    private int originalWidth = 1024;
    private int originalHeight = 600;
    private Vector3 guiScale;

    // Use this for initialization
    void Start () {
		healthbar_cachedY = healthBarTransform.position.y;
		healthbar_maxXValue = healthBarTransform.position.x;
		healthbar_minXValue = healthBarTransform.position.x - healthBarTransform.rect.width;

		manabar_cachedY = manaBarTransform.position.y;
		manabar_maxXValue = manaBarTransform.position.x;
		manabar_minXValue = manaBarTransform.position.x - manaBarTransform.rect.width;

		regenTimer = 1.0f;

        guiScale.x = (float)Screen.width / (float)originalWidth; // calculate hor scale
        guiScale.y = (float)Screen.height / (float)originalHeight; // calculate vert scale
        guiScale.z = 1.0f;
    }

	// Update is called once per frame
	void FixedUpdate () {

		GameObject player = gameObject.GetComponent<GameManager> ().player;
		if (player == null)
			return;

		IPlayer ip = player.GetComponent<IPlayer> ();
		if (ip.isDead)
			return;
		regenTimer -= Time.fixedDeltaTime;
		if (regenTimer <= 0) {
			player.GetComponent<HeroController>().regenHealthMana();
			regenTimer = 1.0f;
		}
	}

	public void updateHealthBar(GameObject player)
	{
        healthbar_maxHealth = player.GetComponent<HeroController> ().maxHealth;
	
		float currentHealth = player.GetComponent<HeroController> ().health;
		float currentXValue = MapValues (currentHealth, 0, healthbar_maxHealth, healthbar_minXValue, healthbar_maxXValue);

		healthText.text = (int)currentHealth + "/" + healthbar_maxHealth;
		healthBarTransform.position = new Vector3 (currentXValue, healthbar_cachedY);

		if (currentHealth > healthbar_maxHealth / 2) {
			visualHealth.color = new Color32((byte)MapValues(currentHealth, healthbar_maxHealth/2, healthbar_maxHealth, 255, 0), 255, 0, 255);
		} else {
			visualHealth.color = new Color32(255, (byte)MapValues(currentHealth, 0, healthbar_maxHealth/2, 0, 255),0, 255);
		}
    }

	public void updateManaBar(GameObject player)
	{
        manabar_maxMana = player.GetComponent<HeroController> ().maxMana;

		float currentMana = player.GetComponent<HeroController> ().mana;
		float currentXValue = MapValues (currentMana, 0, manabar_maxMana, manabar_minXValue, manabar_maxXValue);

		manaText.text = (int)currentMana + "/" + manabar_maxMana;
		manaBarTransform.position = new Vector3 (currentXValue, manabar_cachedY);
    }

	public void updateLevelText(GameObject player)
	{
        levelText.text = player.GetComponent<HeroController> ().level + " " + player.GetComponent<HeroController> ().curr_exp_val + "/" 
			+ player.GetComponent<HeroController> ().max_exp_val;
  
    }

    void OnGUI()
    {
        GameObject player = gameObject.GetComponent<GameManager>().player;
        if (player == null)
            return;

        Matrix4x4 saveMat = GUI.matrix;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiScale);

        updateHealthBar(player);
        updateManaBar(player);
        updateLevelText(player);

        GUI.matrix = saveMat;
    }

	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}

	
}
