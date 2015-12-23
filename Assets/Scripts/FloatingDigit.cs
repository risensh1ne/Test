using UnityEngine;
using System.Collections;

public class FloatingDigit : MonoBehaviour {

    Color color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    float scroll = 0.05f;  // scrolling velocity
    float duration = 1.5f; // time to die
    float alpha;

	// Use this for initialization
	void Start () {

        GetComponent<GUIText>().color = color; // set text color
        alpha = 1;
    }
	
	// Update is called once per frame
	void Update () {
        if (alpha > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + scroll * Time.deltaTime, transform.position.z);
                        
            alpha -= Time.deltaTime / duration;
            GetComponent<GUIText>().material.color = new Color(color.r, color.g, color.b, alpha);
        }
    }
}
