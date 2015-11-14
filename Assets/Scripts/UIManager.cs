using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public GameObject player;
    public GameObject gm;

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

    private Texture myteamHeroTexture;
    private Texture enemyteamHeroTexture;

    private int originalWidth = 1024;
    private int originalHeight = 600;
    private Vector3 guiScale;

    // Use this for initialization
    void Start () {
        //gm = transform.Find("_GM").gameObject;
        guiScale.x = (float)Screen.width / (float)originalWidth; // calculate hor scale
        guiScale.y = (float)Screen.height / (float)originalHeight; // calculate vert scale
        guiScale.z = 1.0f;
    }

	public void initializeUI()
    {
        player = gm.GetComponent<GameManager>().player;
        if (player == null)
        {
            Debug.Log("Can't find Player object");
            return;
        }

        userName = PlayerPrefs.GetString("userName");
        myHeroName = PlayerPrefs.GetString("heroName");
        myTeam = (GameManager.team)PlayerPrefs.GetInt("userTeam");

		if (myHeroName == "Gion") { 
			skill1_btn.GetComponent<Image> ().sprite = skillIcons [0];
			skill2_btn.GetComponent<Image> ().sprite = skillIcons [1];
			skill3_btn.GetComponent<Image> ().sprite = skillIcons [2]; 
        } else if (myHeroName == "SwordMaster") { 
			skill1_btn.GetComponent<Image> ().sprite = skillIcons [2];
			skill2_btn.GetComponent<Image> ().sprite = skillIcons [1];
			skill3_btn.GetComponent<Image> ().sprite = skillIcons [0];
        }
        myteamHeroTexture = heroIcons[getHeroIconIndex(myHeroName)].texture; 
    }
	
    private int getHeroIconIndex(string heroName)
    {
        int idx = -1;
        if (heroName == "Gion")
            idx = 0;
        else if (heroName == "SwordMaster")
            idx = 1;

        return idx;
    }


	public void OnSkill1BtnClicked()
	{
        if (player == null)
        {
            Debug.Log("Can't find Player object");
            return;
        }
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
        if (player == null)
            return;

        Matrix4x4 saveMat = GUI.matrix;

        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiScale);

        //for DEBUG 
        /*
        {
            string txt = "";
            GameObject[] _playersInGame = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject networkplayer in _playersInGame)
            {
                object[] data = networkplayer.GetComponent<PhotonView>().instantiationData;
                txt += (string)data[0];
            }

            GUIStyle stl = new GUIStyle();
            stl.fontSize = 15;
            stl.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(300, 100, 200, 50), _playersInGame.Length + "," + txt, stl);
        }
        */
        
        GameObject[] playersInGame = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject networkplayer in playersInGame)
        {
            object[] data = networkplayer.GetComponent<PhotonView>().instantiationData;
            string heroName = (string)data[0];
            GameManager.team _team = (GameManager.team)data[1];

            //Debug.Log(heroName + "," + _team.ToString() + "," + myTeam.ToString());
            if (_team != myTeam)
                enemyteamHeroTexture = heroIcons[getHeroIconIndex(heroName)].texture;
        }

        GUIStyle gStyle = new GUIStyle();
        gStyle.fontSize = 8;
        gStyle.fontStyle = FontStyle.Bold;


        GUI.DrawTexture(new Rect(20, 80, 60, 60), myteamHeroTexture);
        GUI.DrawTexture(new Rect(20, 140, 50, 10), progressBarBack);
        GUI.DrawTexture(new Rect(20, 140, 50 * (player.GetComponent<HeroController>().health / 100), 10), progressBarHealth);
        if (player.GetComponent<IPlayer>().isDead)
        {
            GUI.Label(new Rect(25, 100, 100, 20), "Respawning", gStyle);
        }

        if (enemyteamHeroTexture != null)
        {
            GUI.DrawTexture(new Rect(Screen.width - 70, 80, 60, 60), enemyteamHeroTexture);
            GUI.DrawTexture(new Rect(Screen.width - 70, 140, 50, 10), progressBarBack);
            GUI.DrawTexture(new Rect(Screen.width - 70, 140, 50 * (player.GetComponent<HeroController>().health / 100), 10), progressBarHealth);

            if (player.GetComponent<IPlayer>().isDead)
            {
                GUI.Label(new Rect(Screen.width - 65, 70, 100, 20), "Respawning", gStyle);
            }
        }

        GUI.matrix = saveMat;
	}


	// Update is called once per frame
	void Update () {
		//player = gm.GetComponent<GameManager> ().player;
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
