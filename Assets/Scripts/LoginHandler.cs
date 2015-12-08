using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;

public class LoginHandler : MonoBehaviour {

	public GameObject woodenPanel;
	public GameObject loginButton;
	public GameObject registerButton;
	public GameObject joinButton;
	public GameObject enterButton;
	public GameObject backButton;

	public InputField login_inputID;
	public GameObject login_inputPass;

	public InputField register_inputID;
	public GameObject register_inputPass;
	public GameObject register_inputPass_confirm;

	public GameObject loginPanel;
	public GameObject registerPanel;

	public GameObject messageBox;
	public Text messageText;

	// Use this for initialization
	void Start () {
		Input.imeCompositionMode = IMECompositionMode.On;
	}

	public void OnLoginButtonClicked()
	{
		loginButton.SetActive (false);
		registerButton.SetActive (false);

		loginPanel.SetActive (true);

		Vector3 scale = new Vector3((float)(woodenPanel.transform.localScale.x * 1.5),
		                            (float)(woodenPanel.transform.localScale.y * 1.3),
		                            woodenPanel.transform.localScale.z);
		woodenPanel.transform.localScale = scale;
	}

	public void OnRegisterButtonClicked()
	{
		loginButton.SetActive (false);
		registerButton.SetActive (false);

		registerPanel.SetActive (true);

		Vector3 scale = new Vector3((float)(woodenPanel.transform.localScale.x * 1.5),
		                            (float)(woodenPanel.transform.localScale.y * 1.3),
		                            woodenPanel.transform.localScale.z);
		woodenPanel.transform.localScale = scale;
	}

	public void OnEnterButtonClicked()
	{
		string input_id = login_inputID.GetComponent<InputField>().text;
		if (input_id == "") {
			MessageBox("아이디를 입력하세요.");
			return;
		}

		string input_pass = login_inputPass.GetComponent<InputField>().text;
		if (input_pass == "") {
			MessageBox("패스워드를 입력하세요.");
			return;
		}

		WWWForm form = new WWWForm();
		form.AddField("id", input_id);
		form.AddField("pass", input_pass);
		WWW www = new WWW("http://risenshine-games.pe.kr/login_user.php", form);
		StartCoroutine(CheckLoginResponse(www));

	}

	public void OnJoinButtonClicked()
	{
		string input_id = register_inputID.GetComponent<InputField>().text;
		if (input_id == "") {
			MessageBox("아이디를 입력하세요.");
			return;
		}
		
		string input_pass = register_inputPass.GetComponent<InputField>().text;
		if (input_pass == "") {
			MessageBox("패스워드를 입력하세요.");
			return;
		}

		string input_pass_confirm = register_inputPass_confirm.GetComponent<InputField>().text;
		if (input_pass_confirm == "") {
			MessageBox("패스워드 확인을 입력하세요.");
			return;
		}	

		if (input_pass != input_pass_confirm) {
			MessageBox("패스워드 확인이 일치하지 않습니다.");
			return;
		}	

		WWWForm form = new WWWForm();
		form.AddField("id", input_id);
		form.AddField("pass", input_pass);
		form.AddField("name", input_id);
		form.AddField("email", "");
		WWW www = new WWW("http://risenshine-games.pe.kr/register_user.php", form);
		StartCoroutine(CheckRegisterResponse(www));
	}

	IEnumerator CheckRegisterResponse(WWW www)
	{
		yield return www;

		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}

	IEnumerator CheckLoginResponse(WWW www)
	{
		yield return www;
		
		if (www.error == null)
		{
			Debug.Log("WWW Ok!: " + www.text);

			JSONNode node = JSON.Parse (www.text);

			Debug.Log (node["data"]);

			if (node["data"] != null) {

				PlayerPrefs.SetString("userID", node["data"]["id"]);
				PlayerPrefs.SetString("userName", node["data"]["name"]);
				PlayerPrefs.SetInt("userLevel", System.Int32.Parse(node["data"]["level"]));
				PlayerPrefs.SetInt("userCash", System.Int32.Parse(node["data"]["cash"]));

				Application.LoadLevel ("lobby");
			} else {
				MessageBox("로그인 정보를 찾을 수 없습니다.");
			}

		} else {
			Debug.Log("WWW Error: "+ www.error);
		}    
	}

	public void OnBackButtonCliked()
	{
		loginButton.SetActive (true);
		registerButton.SetActive (true);

		loginPanel.SetActive (false);	
		registerPanel.SetActive (false);

		messageBox.SetActive(false);

		Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
		woodenPanel.transform.localScale = scale;
	}

	public void MessageBox(string msg)
	{
		messageText.text = msg;
		messageBox.SetActive(true);
	}

	public void OnMessageBoxOkButtonClicked()
	{
		messageBox.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
