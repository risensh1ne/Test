using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject player;
	
	public float skillCooldown;

	public GameObject skill1_btn;
	public GameObject skill1_cooldown_obj;

	public GameObject skill2_btn;
	public GameObject skill2_cooldown_obj;

	public GameObject skill3_btn;
	public GameObject skill3_cooldown_obj;

	public float skill1Cooldown = 5.0f;
	public float skill1CooldownCurrent;

	public float skill2Cooldown = 5.0f;
	public float skill2CooldownCurrent;

	public float skill3Cooldown = 5.0f;
	public float skill3CooldownCurrent;

	public Sprite[] heroIcons;

	public Texture2D progressBarBack, progressBarHealth;

	// Use this for initialization
	void Start () {
		player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}

	}
	
	public void OnSkill1BtnClicked()
	{
		player.GetComponent<HeroController> ().OnSkill1 ();
		skill1CooldownCurrent = skill1Cooldown;
		skill1_btn.GetComponent<Button> ().interactable = false;
	}

	public void OnSkill2BtnClicked()
	{
		player.GetComponent<HeroController> ().ToggleSkillTargetMode(2, 10.0f);
		//player.GetComponent<HeroController> ().OnSkill2 ();
	}

	public void OnSkill3BtnClicked()
	{
		player.GetComponent<HeroController> ().OnSkill3 ();
		skill3CooldownCurrent = skill3Cooldown;
		skill3_btn.GetComponent<Button> ().interactable = false;
	}

	public void OnSkill2Fire()
	{
		skill2CooldownCurrent = skill2Cooldown;
		skill2_btn.GetComponent<Button> ().interactable = false;
	}

	void OnGUI()
	{
		Texture t1 = heroIcons [0].texture;

		GUI.DrawTexture (new Rect (20, 50, 50, 50), t1);
		if (player.GetComponent<IPlayer> ().isDead) {
			GUIStyle stl = new GUIStyle();
			stl.fontSize = 8;
			stl.fontStyle = FontStyle.Bold;

			GUI.Label (new Rect(25, 70, 100, 20), "Respawning", stl);
		}
		GUI.DrawTexture (new Rect (20, 100, 50, 10), progressBarBack);
		GUI.DrawTexture (new Rect (20, 100, 50 * (player.GetComponent<HeroController>().health / 100), 10), progressBarHealth);

		Texture t2 = heroIcons [1].texture;
		GUI.DrawTexture (new Rect (Screen.width - 70, 50, 50, 50), t2);
		GameObject hero = gameObject.GetComponent<GameManager> ().getHeroObj ("SwordMaster");
		if (hero != null) {
			if (hero.GetComponent<IPlayer> ().isDead) {
				GUIStyle stl = new GUIStyle ();
				stl.fontSize = 8;
				stl.fontStyle = FontStyle.Bold;
				
				GUI.Label (new Rect (Screen.width - 65, 70, 100, 20), "Respawning", stl);
			}
			GUI.DrawTexture (new Rect (Screen.width - 70, 100, 50, 10), progressBarBack);
			GUI.DrawTexture (new Rect (Screen.width - 70, 100, 50 * (hero.GetComponent<HeroController> ().health / 100), 10), progressBarHealth);
		}
	}


	// Update is called once per frame
	void Update () {
		player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}

		if (player.GetComponent<HeroController>().targetEnemy == null)
			skill1_btn.GetComponent<Button> ().interactable = false;

		if (skill1CooldownCurrent > 0) {
			skill1CooldownCurrent -= Time.deltaTime;
			skill1_cooldown_obj.GetComponent<Image> ().fillAmount = (skill1Cooldown - skill1CooldownCurrent) / skill1Cooldown;
		} else {
			skill1CooldownCurrent = 0;
			skill1_cooldown_obj.GetComponent<Image> ().fillAmount = 0;

			if (player.GetComponent<HeroController>().targetEnemy != null)
				skill1_btn.GetComponent<Button> ().interactable = true;
		}

		if (skill2CooldownCurrent > 0) {
			skill2CooldownCurrent -= Time.deltaTime;
			skill2_cooldown_obj.GetComponent<Image> ().fillAmount = (skill2Cooldown - skill2CooldownCurrent) / skill2Cooldown;
		} else {
			skill2CooldownCurrent = 0;
			skill2_cooldown_obj.GetComponent<Image> ().fillAmount = 0;
			skill2_btn.GetComponent<Button> ().interactable = true;
		}

		if (skill3CooldownCurrent > 0) {
			skill3CooldownCurrent -= Time.deltaTime;
			skill3_cooldown_obj.GetComponent<Image> ().fillAmount = (skill3Cooldown - skill3CooldownCurrent) / skill3Cooldown;
		} else {
			skill3CooldownCurrent = 0;
			skill3_cooldown_obj.GetComponent<Image> ().fillAmount = 0;
			skill3_btn.GetComponent<Button> ().interactable = true;
		}



	}
}
