using UnityEngine;
using System.Collections;

public class TowerController : Photon.MonoBehaviour, IPlayer {

    public float maxHealth;
	public float health;
	public float attackRange = 8.0f;
	public float attackRate = 3.0f;
	public bool bAttacking = false;
	public bool bDead = false;
	public Transform firePoint;

	public GameManager.team attachedTeam;
	public GameObject targetEnemy;
	private GameObject lastAttackedBy;

    private Color origObjColor;

    private GameObject destructionEffect;

    private float elapsedTimeOfDistruction = 0;
    private float durationOfDistruction = 2.0f;
    private float heightOfDistruction = 1.0f;
    private float alphaOfDistruction = 1.0f;

    Vector2 healthBarSize = new Vector2(50, 5);

    private int originalWidth = 1024;
    private int originalHeight = 600;
    private Vector3 guiScale;

    public Texture2D progressBarBack;
	public Texture2D progressBarHealth;

	public GameManager.team checkTeam() {
		return attachedTeam;
	}

	public bool isMoving 
	{
		get { return false; }
		set {  }
	}
	
	public bool isAttacking 
	{
		get { return bAttacking; }
		set { bAttacking = value; }
	}
	
	public bool isDead 
	{
		get { return bDead; }
		set { bDead = value; }
	}

	public void SetAttacker(GameObject attacker)
	{
		lastAttackedBy = attacker;
	}

    [PunRPC]
    void DamageSync(float d)
    {
        damage(d);
    }

    public void damage(float d)
	{
		if (isDead)
			return;

		if (health - d < 0)
			health = 0;
		else
			health -= d;

		if (health <= 0)
        {
            DestructTower();
        }
			
	}

   
    void DestructTower()
    {
        Color origObjColor = GetComponent<MeshRenderer>().material.color;

        destructionEffect = ObjectPool.instance.GetObjectForType("Destruction01", true);
        if (destructionEffect != null)
        { 
            destructionEffect.transform.position = transform.position;
            destructionEffect.GetComponent<ParticleSystem>().Play();
        }
        isDead = true;
    }

	// Use this for initialization
	void Start () {
		firePoint = transform.Find ("TowerFirePoint");

        guiScale.x = (float)Screen.width / (float)originalWidth; // calculate hor scale
        guiScale.y = (float)Screen.height / (float)originalHeight; // calculate vert scale
        guiScale.z = 1.0f;

        maxHealth = 200.0f;
        health = maxHealth;
    }

	void OnGUI()
	{
        Matrix4x4 saveMat = GUI.matrix;

        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, guiScale);

        Vector3 worldPos = transform.position;
        worldPos.y += 5;

        Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);

        GUI.BeginGroup (new Rect(pos.x - 30, Screen.height - pos.y, healthBarSize.x, healthBarSize.y));
        GUI.DrawTexture (new Rect (0, 0, healthBarSize.x, healthBarSize.y), progressBarBack);

		GUI.BeginGroup (new Rect(0, 0, healthBarSize.x * (health / 100), healthBarSize.y));
		GUI.DrawTexture (new Rect (0, 0, healthBarSize.x * (health / 100), healthBarSize.y), progressBarHealth);
		GUI.EndGroup();
		GUI.EndGroup ();

        GUI.matrix = saveMat;
    }

	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {

		int enemyIndex = -1;
		float enemyDistance = 0;
		Vector3 direction;

        if (isDead)
        {
            float newY = transform.position.y - heightOfDistruction * Time.fixedDeltaTime;
                
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            alphaOfDistruction -= Time.fixedDeltaTime / durationOfDistruction;
            if (alphaOfDistruction < 0)
                alphaOfDistruction = 0;
            
            GetComponent<MeshRenderer>().material.color = new Color(origObjColor.r, origObjColor.g, origObjColor.b, alphaOfDistruction);

            elapsedTimeOfDistruction += Time.fixedDeltaTime;


            if (elapsedTimeOfDistruction >= durationOfDistruction)
            {
                ObjectPool.instance.PoolObject(destructionEffect);
                Destroy(gameObject);
            }
        }
        else
        {

            Collider[] cldrs = Physics.OverlapSphere(firePoint.position, attackRange);
            if (cldrs.Length > 0 && !isAttacking)
            {
                for (int i = 0; i < cldrs.Length; i++)
                {

                    bool isTarget = false;
                    if (cldrs[i].tag == "Player" || cldrs[i].tag == "minion")
                    {
                        IPlayer ip = cldrs[i].gameObject.GetComponent<IPlayer>();
                        if (attachedTeam != ip.checkTeam() && !ip.isDead)
                            isTarget = true;
                    }

                    if (isTarget)
                    {
                        float dist = Vector3.Distance(cldrs[i].transform.position, transform.position);
                        if (enemyDistance == 0 || dist < enemyDistance)
                        {
                            enemyDistance = dist;
                            enemyIndex = i;
                        }
                    }
                }
            }

            if (enemyIndex >= 0)
            {
                targetEnemy = cldrs[enemyIndex].gameObject;
                IPlayer ip = targetEnemy.GetComponent<IPlayer>();
                if (!ip.isDead)
                    gameObject.GetComponent<PhotonView>().RPC("attack", PhotonTargets.All, targetEnemy.transform.position);
            }
        }
	}

	[PunRPC]
	void attack(Vector3 targetPos) {
		StartCoroutine ("Fire", targetPos);
	}

	IEnumerator Fire(Vector3 targetPos) {
		GameObject fireballObj = ObjectPool.instance.GetObjectForType ("Fireball", true);
		if (fireballObj != null) {
			fireballObj.GetComponent<FireballData>().team = attachedTeam;
			Vector3 dir = (targetPos - firePoint.transform.position).normalized;
			fireballObj.transform.position = firePoint.position;
			fireballObj.transform.rotation = Quaternion.LookRotation (dir);
			isAttacking = true;
		}

		yield return new WaitForSeconds(attackRate);
		isAttacking = false;
		ObjectPool.instance.PoolObject(fireballObj);
    }
	
}
