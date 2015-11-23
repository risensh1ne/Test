using UnityEngine;
using System.Collections;

public class LoginHandler : MonoBehaviour {

	public GameObject woodenPanel;
	public GameObject loginButton;
	public GameObject joinButton;
	public GameObject backButton;

	public GameObject labelID;
	public GameObject labelPass;
	public GameObject inputID;
	public GameObject inputPass;

	// Use this for initialization
	void Start () {
		Input.imeCompositionMode = IMECompositionMode.On;
	}

	public void OnLoginButtonClicked()
	{
		loginButton.SetActive (false);
		joinButton.SetActive (false);

		labelID.SetActive(true);
		labelPass.SetActive(true);
		inputID.SetActive (true);
		inputPass.SetActive(true);
		backButton.SetActive (true);

		Vector3 scale = new Vector3((float)(woodenPanel.transform.localScale.x * 1.5),
		                            (float)(woodenPanel.transform.localScale.y * 1.3),
		                            woodenPanel.transform.localScale.z);
		woodenPanel.transform.localScale = scale;
	}

	public void OnJoinButtonClicked()
	{
		loginButton.SetActive (false);
		joinButton.SetActive (false);

		Vector3 scale = new Vector3((float)(woodenPanel.transform.localScale.x * 1.5),
		                            (float)(woodenPanel.transform.localScale.y * 1.3),
		                            woodenPanel.transform.localScale.z);
		woodenPanel.transform.localScale = scale;
	}

	public void OnBackButtonCliked()
	{
		loginButton.SetActive (true);
		joinButton.SetActive (true);

		backButton.SetActive (false);
		labelID.SetActive(false);
		labelPass.SetActive(false);
		inputID.SetActive (false);
		inputPass.SetActive(false);

		Vector3 scale = new Vector3((float)(woodenPanel.transform.localScale.x * 2/3),
		                            (float)(woodenPanel.transform.localScale.y * 2/3),
		                            woodenPanel.transform.localScale.z);
		woodenPanel.transform.localScale = scale;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
