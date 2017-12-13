using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FastFileLog;

public class ControllerLogger : MonoBehaviour
{
    public float logInterval = 0.3f;
    private float time = 0f;
    private Transform transCamera;
	// Use this for initialization
	void Start ()
    {
        transCamera = Utilities.GetChildTransformWithName(gameObject, "MainCamera");
        int groupID = PlayerPrefs.GetInt("GroupID");
        string username = PlayerPrefs.GetString("Username");
        string filename = string.Format("{0}_{1}_Controller_{2}.csv", groupID, username, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
        PlayerPrefs.SetString("LogFile",filename);
		LogManager.Register(gameObject.name, filename, false, true);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    time += Time.deltaTime;
	    if (time > logInterval)
	    {
	        string logString = string.Format("{0},{1},{2},{3},{4},{5}", 
                Time.time, transform.localPosition.x, transform.localPosition.y, transform.localPosition.z,
                transform.localRotation.y, transCamera.localRotation.x);
            LogManager.Log(gameObject.name, logString);

	        time = 0;
	    }
	}
}
