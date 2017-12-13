using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishManager : MonoBehaviour
{
    public Text GroupEvacTimeText;
    public Text UserEvacTimeText;
    public Text StatusResultsText;
    public Text StatusLogfileText;
    public Button QuitButton;
    public string serverIP = "192.168.1.2";
    public string port = "8090";
    public string uploadResultPath = "/updateResults";
    public string uploadLogfilePath = "/uploadLogfile";

    // Use this for initialization
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        QuitButton.interactable = false;
        float groupEvacTime = PlayerPrefs.GetFloat("GroupEvacTime");
        float userEvacTime = PlayerPrefs.GetFloat("UserEvacTime");
        GroupEvacTimeText.text = groupEvacTime.ToString();
        UserEvacTimeText.text = userEvacTime.ToString();

        //上传结果参数
        StartCoroutine(UploadResults());
        //上传日志文件
        StartCoroutine(UploadLogfile());
        QuitButton.interactable = true;
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

    IEnumerator UploadLogfile()
    {
        StatusLogfileText.text = "正在上传日志文件...";
        
        int groupID = PlayerPrefs.GetInt("GroupID");
        string username = PlayerPrefs.GetString("Username");
        string filename = PlayerPrefs.GetString("LogFile");
        string fullFilePath = Application.streamingAssetsPath + "/Log/" + filename;
        string uploadLogfileURL = "http://" + serverIP + ":" + port + uploadLogfilePath;
        string fileData = File.ReadAllText(fullFilePath, Encoding.UTF8);
        WWWForm form = new WWWForm();
        form.AddField("fileName", filename);
        form.AddField("fileData", fileData);
        using (UnityWebRequest www = UnityWebRequest.Post(uploadLogfileURL, form))
        {
            yield return www.Send();
            if (www.isError)
            {
                StatusLogfileText.text = www.error;
            }
            else
            {
                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
                // sucess
                if (result.code == 0)
                {
                    StatusLogfileText.text = "上传日志成功！";
                }
                else
                {
                    StatusLogfileText.text = result.msg;
                }
            }
        }
    }

    IEnumerator UploadResults()
    {
        StatusResultsText.text = "正在上传结果参数...";

        int groupID = PlayerPrefs.GetInt("GroupID");
        string username = PlayerPrefs.GetString("Username");
        WWWForm form1 = new WWWForm();
        form1.AddField("groupID", groupID);
        form1.AddField("username", username);
        form1.AddField("userEvacTime", UserEvacTimeText.text);
        form1.AddField("groupEvacTime", GroupEvacTimeText.text);
        string uploadResultURL = "http://" + serverIP + ":" + port + uploadResultPath;
        using (UnityWebRequest www = UnityWebRequest.Post(uploadResultURL, form1))
        {
            yield return www.Send();

            if (www.isError)
            {
                StatusResultsText.text = www.error;
            }
            else
            {
                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
                // sucess
                if (result.code == 0)
                {
                    StatusResultsText.text = "上传结果参数成功！";
                }
                else
                {
                    StatusResultsText.text = result.msg;
                }
            }
        }
    }
}
