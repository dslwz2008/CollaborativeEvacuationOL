using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class TestWWW : MonoBehaviour {

    public string serverIP = "192.168.1.2";
    public string port = "8090";
    public string uploadResultPath = "/updateResults";
    public string uploadLogfilePath = "/uploadLogfile";

    // Use this for initialization
    void Start ()
	{
//	    StartCoroutine(UploadResults());
	    StartCoroutine(UploadLogfile());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator UploadLogfile()
    {
        string uploadLogfileURL = "http://" + serverIP + ":" + port + uploadLogfilePath;
        string fullFilePath = Application.streamingAssetsPath + "/Log/2_test4_Controller.txt";
        string fileData = File.ReadAllText(fullFilePath, Encoding.UTF8);
        WWWForm form = new WWWForm();
        form.AddField("fileName", "2_test4_Controller.txt");
        form.AddField("fileData", fileData);
        using (UnityWebRequest www = UnityWebRequest.Post(uploadLogfileURL, form))
        {
            yield return www.Send(); if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
                // sucess
                if (result.code == 0)
                {
                    Debug.Log("上传日志成功！");
                }
                else
                {
                    Debug.Log(result.msg);
                }
            }
        }
        //        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        ////        Debug.Log(fileData);
        //        formData.Add(new MultipartFormFileSection(fileData, "2_test4_Controller.txt"));
        //
        //        using (UnityWebRequest www = UnityWebRequest.Post(uploadLogfileURL, formData))
        //        {
        //            yield return www.Send();
        //            if (www.isError)
        //            {
        //                Debug.Log(www.error);
        //            }
        //            else
        //            {
        //                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
        //                // sucess
        //                if (result.code == 0)
        //                {
        //                    Debug.Log("上传日志成功！");
        //                }
        //                else
        //                {
        //                    Debug.Log(result.msg);
        //                }
        //            }
        //        }
    }

    IEnumerator UploadResults()
    {
        WWWForm form1 = new WWWForm();
        form1.AddField("groupID", 1);
        form1.AddField("username", "test1");
        form1.AddField("userEvacTime", "1.123");
        form1.AddField("groupEvacTime", "12.34");
        string uploadResultURL = "http://" + serverIP + ":" + port + uploadResultPath;
        using (UnityWebRequest www = UnityWebRequest.Post(uploadResultURL, form1))
        {
            yield return www.Send(); if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                ResponseUser result = JsonUtility.FromJson<ResponseUser>(www.downloadHandler.text);
                // sucess
                if (result.code == 0)
                {
                    Debug.Log("上传结果参数成功！");
                }
                else
                {
                    Debug.Log(result.msg);
                }
            }
        }
    }
}
