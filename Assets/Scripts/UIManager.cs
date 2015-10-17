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

	public Sprite[] skillIcons;

	public Texture2D progressBarBack, progressBarHealth;

    private GameManager.team myTeam;
    private string myHeroName;
    private string userName;

    private Texture alphaHeroTexture;
    private Texture betaHeroTexture;

    // Use this for initialization
    void Start () {
        
    }

	public void initializeUI()
    {
        player = gameObject.GetComponent<GameManager>().player;
        if (player == null)
        {
            //Debug.Log("Can't find Player object");
            return;
        }

        userName = PlayerPrefs.GetString("userName");
        myHeroName = PlayerPrefs.GetString("heroName");
        myTeam = (GameManager.team)PlayerPrefs.GetInt("userTeam");

        int heroTextureIdx = 0;

		if (myHeroName == "Gion") { 
			skill1_btn.GetComponent<Image> ().sprite = skillIcons [0];
			skill2_btn.GetComponent<Image> ().sprite = skillIcons [1];
			skill3_btn.GetComponent<Image> ().sprite = skillIcons [2];

            heroTextureIdx = 0;
            
        } else if (myHeroName == "SwordMaster") { 
			skill1_btn.GetComponent<Image> ().sprite = skillIcons [2];
			skill2_btn.GetComponent<Image> ().sprite = skillIcons [1];
			skill3_btn.GetComponent<Image> ().sprite = skillIcons [0];

            heroTextureIdx = 1;
        }

        if (myTeam == GameManager.team.ALPHA)
            alphaHeroTexture = heroIcons[heroTextureIdx].texture;
        else
            betaHeroTexture = heroIcons[heroTextureIdx].texture;
    }
	
	public void OnSkill1BtnClicked()
	{
		player.GetComponent<PhotonView> ().RPC ("OnSkill1", PhotonTargets.All);
		skill1CooldownCurrent = skill1Cooldown;
		skill1_btn.GetComponent<Button> ().interactable = false;
	}

	public void OnSkill2BtnClicked()
	{
		player.GetComponent<HeroController> ().ToggleSkillTargetMode(2, 10.0f);
	}

	public void OnSkill3BtnClicked()
	{
		player.GetComponent<PhotonView> ().RPC ("OnSkill3", PhotonTargets.All);
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
        //for DEBUG 
        {
            string txt = "";
            System.Collections.Generic.HashSet<GameObject> playersInGame = PhotonNetwork.FindGameObjectsWithComponent(typeof(HeroController));
            foreach (GameObject networkplayer in playersInGame)
            {
                txt += networkplayer.GetComponent<HeroController>().heroName;
            }

            txt = playersInGame.

            GUIStyle stl = new GUIStyle();
            stl.fontSize = 15;
            stl.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(300, 100, 200, 50), txt, stl);
        }

        if (player == null)
            return;

        GUIStyle gStyle = new GUIStyle();
        gStyle.fontSize = 8;
        gStyle.fontStyle = FontStyle.Bold;

        if (myTeam == GameManager.team.ALPHA)
        {
            GUI.DrawTexture(new Rect(20, 80, 60, 60), alphaHeroTexture);
            GUI.DrawTexture(new Rect(20, 140, 50, 10), progressBarBack);
            GUI.DrawTexture(new Rect(20, 140, 50 * (player.GetComponent<HeroController>().health / 100), 10), progressBarHealth);
        } 
        else
        {
            GUI.DrawTexture(new Rect(Screen.width - 70, 80, 60, 60), betaHeroTexture);
            GUI.DrawTexture(new Rect(Screen.width - 70, 140, 50, 10), progressBarBack);
            GUI.DrawTexture(new Rect(Screen.width - 70, 140, 50 * (player.GetComponent<HeroController>().health / 100), 10), progressBarHealth);
        }
            
        if (player.GetComponent<IPlayer> ().isDead) {
            if (myTeam == GameManager.team.ALPHA)
                GUI.Label (new Rect(25, 100, 100, 20), "Respawning", gStyle);
            else
                GUI.Label(new Rect(Screen.width - 65, 70, 100, 20), "Respawning", gStyle);
        }
		

        /*
		GUI.DrawTexture (new Rect (Screen.width - 70, 80, 60, 60), betaHeroTexture);
        if (player.GetComponent<IPlayer>().isDead)
        {
            GUIStyle stl = new GUIStyle();
            stl.fontSize = 8;
            stl.fontStyle = FontStyle.Bold;

            GUI.Label(new Rect(25, 100, 100, 20), "Respawning", stl);
        }
        

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
        */
	}


	// Update is called once per frame
	void Update () {
		player = gameObject.GetComponent<GameManager> ().player;
		if (player == null) {
			//Debug.Log ("Can't find Player object");
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

			if (player.GetComponent<HeroController>().mana >= 20.0f)
				skill2_btn.GetComponent<Button> ().interactable = true;
			else
				skill2_btn.GetComponent<Button> ().interactable = false;
		}

		if (skill3CooldownCurrent > 0) {
			skill3CooldownCurrent -= Time.deltaTime;
			skill3_cooldown_obj.GetComponent<Image> ().fillAmount = (skill3Cooldown - skill3CooldownCurrent) / skill3Cooldown;
		} else {
			skill3CooldownCurrent = 0;
			skill3_cooldown_obj.GetComponent<Image> ().fillAmount = 0;
			if (player.GetComponent<HeroController>().mana >= 50.0f)
				skill3_btn.GetComponent<Button> ().interactable = true;
			else
				skill3_btn.GetComponent<Button> ().interactable = false;
		}



	}
}
