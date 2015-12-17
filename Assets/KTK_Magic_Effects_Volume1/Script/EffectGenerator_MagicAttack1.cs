 using UnityEngine;
using System.Collections;

public class EffectGenerator_MagicAttack1 : MonoBehaviour {
	
	public float hSliderValue = 0.5F;
	public Material FloorMat;

	private GameObject Eff_Point;

	public GameObject[] Effect_List;

	private Vector2 scrollViewVector = Vector2.zero;
	
	private Rect scrollViewRect = new Rect(0, 20, 170, 500);
	
	private Rect scrollViewAllRect = new Rect (40, 70, 100, 1000);


	void Awake() {
		Eff_Point = GameObject.Find("Eff_Point");
	}

	void Update() {
		if(FloorMat != null){
			FloorMat.color = new Color(hSliderValue, hSliderValue, hSliderValue, 1.0f);
		}
	}

	void OnGUI() {

		hSliderValue = GUI.HorizontalSlider(new Rect(170, 20, 100, 30), hSliderValue, 0.0F, 1.0F);
		GUI.Label(new Rect(170, 50,  200, 20), "FloorBrightness: " + hSliderValue);

		scrollViewVector = GUI.BeginScrollView(scrollViewRect , scrollViewVector, scrollViewAllRect);
		
		for(int i = 0; i < Effect_List.Length; i++)
		{
			if(Effect_List[i] != null){
				if (GUI.Button(new Rect(50, 70 + i * 40, 140, 30), Effect_List[i].name))
				{
					GameObject clone1 = Instantiate(Effect_List[i], Eff_Point.transform.position, Quaternion.identity) as GameObject;
					clone1.transform.rotation = Quaternion.Euler(0,  0 , 0 );
					clone1.transform.parent = Eff_Point.transform;
				}
			}
		}
		
		
		GUI.EndScrollView();

		/*
		if (GUI.Button(new Rect(10, 10, 150,  50), "Effect1"))
		{
 			GameObject clone1 = Instantiate(Effect1, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone1.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone1.transform.parent = Eff_Point.transform;
		}

		if (GUI.Button(new Rect(10, 70, 150, 50), "Effect2"))
		{	
			GameObject clone2 = Instantiate(Effect2, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone2.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 ); 
			clone2.transform.parent = Eff_Point.transform;
		}

		if (GUI.Button(new Rect(10, 130, 150, 50), "Effect3"))
		{	
			GameObject clone3 = Instantiate(Effect3, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone3.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone3.transform.parent = Eff_Point.transform;
		}
		 
		if (GUI.Button(new Rect(10, 190, 150, 50), "Effect4"))
		{	
			GameObject clone4 = Instantiate(Effect4, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone4.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone4.transform.parent = Eff_Point.transform;
		}

		if (GUI.Button(new Rect(10, 250, 150, 50), "Effect5"))
		{	
			GameObject clone5 = Instantiate(Effect5, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone5.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone5.transform.parent = Eff_Point.transform;
		}
		 
		if (GUI.Button(new Rect(10, 310, 150, 50), "Effect6"))
		{	
			GameObject clone6 = Instantiate(Effect6, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone6.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone6.transform.parent = Eff_Point.transform;
		}

		if (GUI.Button(new Rect(10, 370, 150, 50), "Effect7"))
		{	
			GameObject clone7 = Instantiate(Effect7, Eff_Point.transform.position, Quaternion.identity) as GameObject;
			clone7.transform.rotation = Quaternion.Euler(0, Random.Range(0.0F,   360.0F), 0 );
			clone7.transform.parent = Eff_Point.transform;
		}
		 
		 */
	}

}
