using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockTrigger : MonoBehaviour
{
    public MainManager manager;
    private string information = "前方大火，请选择其他出口逃生！";
    private string previous;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        //本地用户
        if (manager.sfs.MySelf.Name == other.gameObject.name)
        {
            previous = manager.StatusText.text;
            manager.StatusText.text = information;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //本地用户
        if (manager.sfs.MySelf.Name == other.gameObject.name)
        {
            manager.StatusText.text = previous;
        }
    }
}
