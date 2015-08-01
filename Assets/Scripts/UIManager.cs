using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject player;

	public bool autoAttack;
	public float skillCooldown;


	public Sprite attackSprite;
	public Sprite moveSprite;

	public GameObject autoAttackBtn;
	public GameObject skill1_btn;
	public GameObject skill1_cooldown_obj;

	public GameObject skill2_btn;
	public GameObject skill2_cooldown_obj;

	public float skill1Cooldown = 5.0f;
	public float skill1CooldownCurrent;

	public float skill2Cooldown = 5.0f;
	public float skill2CooldownCurrent;

	// Use this for initialization
	void Start () {
		player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}

		autoAttack = false;
	}

	public void OnNormalAttackBtnClicked()
	{
		if (autoAttack) {
			autoAttackBtn.GetComponent<Image> ().sprite = moveSprite;
		} else {
			autoAttackBtn.GetComponent<Image> ().sprite = attackSprite;
		}
		autoAttack = !autoAttack;
		player.GetComponent<HeroController> ().SetAutoAttackMode (autoAttack);
	}

	public void OnSkill1BtnClicked()
	{
		player.GetComponent<HeroController> ().ToggleSkillTargetMode(1, 2.0f);
		//skill1CooldownCurrent = skill1Cooldown;
	}

	public void OnSkill2BtnClicked()
	{
		player.GetComponent<HeroController> ().ToggleSkillTargetMode(2, 10.0f);
		//player.GetComponent<HeroController> ().OnSkill2 ();
	}

	void OnGUI()
	{

	}

	void SetSkillCooldown(int skill)
	{
		if (skill == 1) {
			skill1CooldownCurrent = skill1Cooldown;
		} else if (skill == 2) {
			skill2CooldownCurrent = skill2Cooldown;
		}

	}

	// Update is called once per frame
	void Update () {
		player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			Debug.Log ("Can't find Player object");
			return;
		}

		if (skill1CooldownCurrent > 0) {
			skill1CooldownCurrent -= Time.deltaTime;
			skill1_cooldown_obj.GetComponent<Image> ().fillAmount = (skill1Cooldown - skill1CooldownCurrent) / skill1Cooldown;
			skill1_btn.GetComponent<Button> ().interactable = true;
		} else {
			skill1CooldownCurrent = 0;
			skill1_cooldown_obj.GetComponent<Image> ().fillAmount = 0;
			skill1_btn.GetComponent<Button> ().interactable = true;
		}

		if (skill2CooldownCurrent > 0) {
			skill2CooldownCurrent -= Time.deltaTime;
			skill2_cooldown_obj.GetComponent<Image> ().fillAmount = (skill2Cooldown - skill2CooldownCurrent) / skill2Cooldown;
			skill2_btn.GetComponent<Button> ().interactable = false;
		} else {
			skill2CooldownCurrent = 0;
			skill2_cooldown_obj.GetComponent<Image> ().fillAmount = 0;
			skill2_btn.GetComponent<Button> ().interactable = true;
		}


	}
}
