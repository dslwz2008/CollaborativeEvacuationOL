using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PrefabDictionaryEntry
{
    public string key;
    public GameObject value;
}

/// <summary>
/// Generator.
/// 生成与可视化
/// </summary>
public class Generator : MonoBehaviour
{
    public bool isPause = false;
    public bool hasFinished = false;
    public List<PrefabDictionaryEntry> PlayerPrefabs;
    public GameObject parent;
	public float frameTime = 0.3f;
	
	private Frame lastFrame = null;
    private Players players;
    private FrameBuffer frameBuffer;
    private Dictionary<string, GameObject> Avatars;

    void Start ()
    {
        frameBuffer = GetComponent<FrameBuffer>();
        if (!frameBuffer)
        {
            Debug.Log("Frame Buffer Error！");
            return;
        }

        players = GetComponent<Players>();
        if (!players)
        {
            Debug.Log("Player Dictionary Error！");
            return;
        }

        //生成该次实验的化身数据
        Avatars = new Dictionary<string, GameObject>();
        GenerateAvatars();
        InvokeRepeating("GenerateOneFrame", 1.5f, frameTime);
        LeanTween.init(800);
	}

    private void GenerateAvatars()
    {
        foreach (KeyValuePair<string, Player> keyValuePair in players.PlayerDictionary)
        {
            string username = keyValuePair.Key;
            //根据个人属性生成对应的对象
            Player player = keyValuePair.Value;
            int groupID = player.GroupID;
            int gender = player.Gender;
            string prefabKey = groupID.ToString("D2") + gender.ToString();
            GameObject goPrefab = FindPlayerPrefabByName(prefabKey);
            GameObject goAvatar = Instantiate(goPrefab, parent.transform);
            goAvatar.name = username;
            Avatars.Add(username, goAvatar);
            //刚开始不可用
            goAvatar.SetActive(false);
        }   
    }

    private GameObject FindPlayerPrefabByName(string prefabKey)
    {
        GameObject goPrefab = null;
        // 查找化身prefab
        for (int i = 0; i < PlayerPrefabs.Count; i++)
        {
            if (PlayerPrefabs[i].key == prefabKey)
            {
                goPrefab = PlayerPrefabs[i].value;
                break;
            }
        }
        return goPrefab;
    }

    void Update() {
    }
	
	void GenerateOneFrame()
	{
	    if (isPause) return;

		Queue<Frame> queue = frameBuffer.queue;
		Frame thisFrame = null;

        //数据已经没有了
		if(queue.Count == 0){
            Debug.Log("Simulation Finished!");
            CancelInvoke("GenerateOneFrame");
            return;
		}
		thisFrame = queue.Dequeue();

        //更新化身的位置属性
	    foreach (KeyValuePair<string, PlayerMove> pair in thisFrame.PlayerMoves)
	    {
	        GameObject go = Avatars[pair.Key];
	        PlayerMove oneMove = pair.Value;
	        if (!go.activeSelf)//第一次出现
            {
                go.SetActive(true);
                go.transform.localPosition = new Vector3(oneMove.PosX, oneMove.PosY, oneMove.PosZ);
                go.transform.localEulerAngles = new Vector3(0.0f, oneMove.RotY, 0.0f);
                GameObject goArrow = Utilities.GetChildObjectWithName(go,"arrow");
                goArrow.transform.localEulerAngles = new Vector3(oneMove.RotX, 0.0f, 0.0f);
            }
	        else//要插值
	        {
                LeanTween.move(go, new Vector3(oneMove.PosX, oneMove.PosY, oneMove.PosZ), frameTime)
                    .setEase(LeanTweenType.linear);
	            LeanTween.rotateY(go, oneMove.RotY, frameTime).setEase(LeanTweenType.linear);
                GameObject goArrow = Utilities.GetChildObjectWithName(go, "arrow");
	            LeanTween.rotateX(goArrow, oneMove.RotX, frameTime).setEase(LeanTweenType.linear);
	        }
	    }
	}
    
}
