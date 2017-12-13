using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

public class FrameBuffer : MonoBehaviour
{
    public Queue<Frame> queue = null;
    void Awake(){
//        //  获取.NET版本，无关
//        Debug.Log(System.Environment.Version);
//        Type type = Type.GetType("Mono.Runtime");
//        if (type != null)
//        {
//            MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
//            if (displayName != null)
//                Debug.Log(displayName.Invoke(null, null));
//        }
//        //
        queue = new Queue<Frame>();
	}
}
