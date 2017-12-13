using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour {
    public Dictionary<string, Player> PlayerDictionary;

    // Use this for initialization
    void Awake () {
        PlayerDictionary = new Dictionary<string, Player>();
	}
	
}
