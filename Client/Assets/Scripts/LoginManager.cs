using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public InputField inputUsername;
    public InputField inputPasswrod;
    public Text statusText;
    public string serverIP="192.168.1.2";
    public string port="8090";
    public string loginPath="/login";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Submit()
    {
        StartCoroutine(ValidLogin());
    }

    IEnumerator ValidLogin()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", inputUsername.text);
        form.AddField("password", inputPasswrod.text);
        string loginURL = "http://"+serverIP+":"+port+loginPath;
        Debug.Log(loginURL);
        using (UnityWebRequest www = UnityWebRequest.Post(loginURL, form))
        {
            yield return www.Send();

            if (www.isError)
            {
                statusText.text = www.error;
            }
            else
            {
                //                Debug.Log(www.ToString());
                //                Debug.Log(www.downloadHandler.text);
                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
//                Debug.Log(result.code);
//                Debug.Log(result.msg);
//                Debug.Log(result.data);
                // sucess
                if (result.code == 0)
                {
                    statusText.text = "";
                    //供下面的场景使用
                    PlayerPrefs.SetString("Username", inputUsername.text);
                    PlayerPrefs.SetInt("GroupID", result.groupID);
                    PlayerPrefs.SetInt("GroupSize", result.groupSize);
                    PlayerPrefs.SetInt("Leader", result.leader);
                    PlayerPrefs.SetInt("Gender", result.gender);
                    PlayerPrefs.SetFloat("Speed", result.speed);
                    PlayerPrefs.SetInt("KnowTruth", result.knowTruth);
                    SceneManager.LoadScene("Scenes/GetReady");
                }
                else
                {
                    statusText.text = result.msg;
                }
            }
        }
    }
}
