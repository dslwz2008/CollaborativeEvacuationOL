using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFileLog;
using UnityStandardAssets.Characters.FirstPerson;

public class ControllerLogger : MonoBehaviour
{
    public float logInterval = 0.3f;
    private float time = 0f;
    private GameObject Head;
	// Use this for initialization
	void Start ()
	{
	    Head = GetComponent<RigidbodyFirstPersonController>().Head;
        int groupID = PlayerPrefs.GetInt("GroupID");
        string username = PlayerPrefs.GetString("Username");
        string filename = string.Format("{0}_{1}_Controller_VR_{2}.csv", groupID, username, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        PlayerPrefs.SetString("LogFile",filename);
		LogManager.Register(gameObject.name, filename, false, true);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    time += Time.deltaTime;
	    if (time > logInterval)
	    {
            //注意！
            //1.比PC端的多记录一个参数
            //2.后三个都是Camera对象的参数
            //3.为了与PC端保持一致，记录顺序为yxz
	        string logString = string.Format("{0},{1},{2},{3},{4},{5},{6}", 
                Time.time, transform.localPosition.x, transform.localPosition.y, transform.localPosition.z,
                Head.transform.localEulerAngles.y, Head.transform.localEulerAngles.x, Head.transform.localEulerAngles.z);
            LogManager.Log(gameObject.name, logString);

	        time = 0;
	    }
	}
}
