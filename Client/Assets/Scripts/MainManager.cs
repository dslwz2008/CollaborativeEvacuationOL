using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

[Serializable]
public class PrefabDictionaryEntry
{
    public string key;
    public GameObject value;
}

public class MainManager : MonoBehaviour
{
    // UI and Scene Elements
    public Image GroupFlag;
    public Text GroupSizeText;
    public Text RoleText;
    public Text CountDownText;
    public Text WaitCountDownText;
    public Text WaitSecondsText;
    public int TotalTime = 300;
    public int CountDownTime = 10;
    public Text StatusText;
    public Text TruthText;
    public GameObject BarrierAB;
    public GameObject BarrierCD;
    public Text NetworkLagText;

    // Data
    public GameObject GoParent;
    public List<PrefabDictionaryEntry> PlayerPrefabs;
    public Transform[] SpawnPositions;
    public Timer timer;
    public float UserVariableSendInterval = 0.25f;
    private float DealtaTime = 0.0f;

    public SmartFox sfs;

    private GameObject localPlayer;
    private RigidbodyFirstPersonController localPlayerController;
    private Dictionary<SFSUser, GameObject> remotePlayers = new Dictionary<SFSUser, GameObject>();
    private Dictionary<SFSUser, UserData> remoteUserData = new Dictionary<SFSUser, UserData>();

    private const string RoleLeader = "队长";
    private const string RoleGroupMemeber = "队员";
    private string RightExitString;
    private string WrongExitString;

    void Start()
    {
        if (!SmartFoxConnection.IsInitialized)
        {
            SceneManager.LoadScene("Scenes/GetReady");
            return;
        }

        sfs = SmartFoxConnection.Connection;

        // Register callback delegates
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        sfs.AddEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, OnProximityListUpdate);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        sfs.EnableLagMonitor(true);
        sfs.AddEventListener(SFSEvent.PING_PONG, OnPingPongEvent);

        FillUI();
        // spawn local player
        SpawnLocalPlayer();

        // 创建同步的场景元素
        //CreateMMOItems();

        sfs.Send(new ExtensionRequest("EnterMain", SFSObject.NewInstance(),sfs.LastJoinedRoom));
    }

    private void FillUI()
    {
        int leader = PlayerPrefs.GetInt("Leader");
        int groupSize = PlayerPrefs.GetInt("GroupSize");
        Color color = new Color(PlayerPrefs.GetFloat("R"), PlayerPrefs.GetFloat("G"), PlayerPrefs.GetFloat("B"));
        GroupFlag.material.SetColor("_Color1out", color);
        GroupSizeText.text = groupSize.ToString();
        RoleText.text = leader == 1 ? RoleLeader : RoleGroupMemeber;
        int rightExit = PlayerPrefs.GetInt("RightExit");
        //AB出口逃生
        if (rightExit == 0)
        {
            BarrierAB.SetActive(false);
            RightExitString = "AB";
            WrongExitString = "CD";
        }
        else
        {
            BarrierCD.SetActive(false);
            RightExitString = "CD";
            WrongExitString = "AB";
        }
        if (PlayerPrefs.GetInt("KnowTruth") == 1)
        {
            TruthText.enabled = true;
            TruthText.text = string.Format("地铁{0}出口着火，请从{1}出口逃生！", WrongExitString, RightExitString);
        }
    }

    void FixedUpdate()
    {
        if (sfs != null)
        {
            sfs.ProcessEvents();

            DealtaTime += Time.deltaTime;
            if (DealtaTime < UserVariableSendInterval)
            {
                return;
            }
            DealtaTime = 0.0f;

            //处理位置、旋转、动画的变化
            if (localPlayer != null && localPlayerController != null)
            {
                List<UserVariable> userVariables = new List<UserVariable>();
                if (localPlayerController.MovementDirty)
                {
                    userVariables.Add(new SFSUserVariable("PosX", (double) localPlayer.transform.position.x));
                    userVariables.Add(new SFSUserVariable("PosY", (double) localPlayer.transform.position.y));
                    userVariables.Add(new SFSUserVariable("PosZ", (double) localPlayer.transform.position.z));
                    localPlayerController.MovementDirty = false;
                }
                if (localPlayerController.RotationDirty)
                {
                    //旋转的时候只有这两个数值改变
                    //可视化时只使用RotY，RotX记在日志中
                    userVariables.Add(new SFSUserVariable("RotY",
                        (double) localPlayer.transform.localRotation.eulerAngles.y));
                    userVariables.Add(new SFSUserVariable("RotX",
                        (double) Camera.main.transform.localRotation.eulerAngles.x));
                    localPlayerController.RotationDirty = false;
                }
                if (localPlayerController.RunAnimationDirty)
                {
                    if (localPlayerController.RunAnimation)
                    {
                        userVariables.Add(new SFSUserVariable("Animation", "Run"));
                    }
                    else
                    {
                        userVariables.Add(new SFSUserVariable("Animation", "StopRun"));
                    }
                    localPlayerController.RunAnimationDirty = false;
                }
                if (localPlayerController.GreetAnimation)
                {
                    userVariables.Add(new SFSUserVariable("Animation", "Greet"));
                    localPlayerController.GreetAnimation = false;
                }
                if (userVariables.Count != 0)
                {
                    sfs.Send(new SetUserVariablesRequest(userVariables));
                }
            }

            //如果MMOItem变化了，发送请求
            //UpdateMMOItems();
        }
    }

    //----------------------------------------------------------
    // SmartFoxServer event listeners
    //----------------------------------------------------------

    /**
	 * This is where we receive events about people in proximity (AoI).
	 * We get two lists, one of new users that have entered the AoI and one with users that have left our proximity area.
	 */

    public void OnProximityListUpdate(BaseEvent evt)
    {
        var addedUsers = (List<User>) evt.Params["addedUsers"];
        var removedUsers = (List<User>) evt.Params["removedUsers"];

        // Handle all new Users
        foreach (User user in addedUsers)
        {
            Vector3 pos = new Vector3(user.AOIEntryPoint.FloatX, user.AOIEntryPoint.FloatY, user.AOIEntryPoint.FloatZ);
//            Quaternion rot = Quaternion.Euler(0, (float) user.GetVariable("RotY").GetDoubleValue(), 0);
            Quaternion rot = Quaternion.Euler(0, 0, 0);
            SpawnRemotePlayer((SFSUser) user, pos, rot);
        }

        // Handle removed users
        foreach (User user in removedUsers)
        {
            RemoveRemotePlayer((SFSUser) user);
        }
    }

    public void OnConnectionLost(BaseEvent evt)
    {
        // Reset all internal states so we kick back to login screen
        sfs.RemoveAllEventListeners();
        SceneManager.LoadScene("Scenes/GetReady");
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string) evt.Params["cmd"];
        SFSObject dataObject = (SFSObject) evt.Params["params"];

        switch (cmd)
        {
            case "UserDisconnect":
            {
                string username = dataObject.GetUtfString("Username");
                StatusText.text = string.Format("同组的用户 {0} 掉线了，其他人请继续！", username);
                break;
            }
            case "AllEntered":
            {
                // 开始倒计时
                timer = GetComponent<Timer>();
                timer.stopAtZero = true;
                timer.StartTimer(CountDownTime);
                timer.OnTimerTicked += () => WaitCountDownText.text = timer.timeLeft.ToString("F0");
                timer.OnTimerFinished += Timer_OnCountDownTimerFinished;
                break;
            }
            default:
                break;
        }
    }

    private void Timer_OnCountDownTimerFinished()
    {
        WaitSecondsText.enabled = false;
        WaitCountDownText.enabled = false;

        //控制器放开
        float speed = PlayerPrefs.GetFloat("Speed");
        localPlayerController.movementSettings.ForwardSpeed = speed;
        //侧向、后向速度减半
        localPlayerController.movementSettings.BackwardSpeed = speed / 2;
        localPlayerController.movementSettings.StrafeSpeed = speed / 2;
        //添加日志记录组件
        localPlayer.AddComponent<ControllerLogger>();

        //整体疏散倒计时
        timer.stopAtZero = false;
        timer.StartTimer(TotalTime);
        timer.OnTimerTicked += () => CountDownText.text = timer.timeLeft.ToString("F0");
    }

    /**
	 * When user variable is updated on any client within the AoI, then this event is received.
	 * This is where most of the game logic for this example is contained.
	 */

    public void OnUserVariableUpdate(BaseEvent evt)
    {
        ArrayList changedVars = (ArrayList)evt.Params["changedVars"];
        SFSUser user = (SFSUser)evt.Params["user"];
        if (user == sfs.MySelf) return;

        try
        {
            UserData userData = remoteUserData[user];
            userData.Tt += Time.deltaTime;
            // 收到新的位置数据，移动或者转向
            if (changedVars.Contains("PosX") || changedVars.Contains("PosY") ||
                changedVars.Contains("PosZ"))
            {
                Vector3 newPos = new Vector3((float) user.GetVariable("PosX").GetDoubleValue(),
                    (float) user.GetVariable("PosY").GetDoubleValue(),
                    (float) user.GetVariable("PosZ").GetDoubleValue());
                //第一个点
                if (userData.State == 0)
                {
                    userData.Pm1 = newPos;
//                    Debug.Log(userData.State);
//                    Debug.Log(newPos);
                    remotePlayers[user].GetComponent<SimpleRemoteInterpolation>().SetPosition(newPos, true);
                    userData.State = 1;
                }
                //只有一个位置，进行简单预测
                else if (userData.State == 1)
                {
                    userData.P0 = newPos;
                    Vector3 v0 = 0.1f*(userData.P0 - userData.Pm1)/Time.deltaTime;
                    Vector3 combined = userData.P0 + v0*Time.deltaTime;
                    userData.Pm1 = userData.P0;
                    userData.Vm1 = v0;
                    userData.P0 = combined;
//                    Debug.Log(userData.State);
//                    Debug.Log(combined);
                    remotePlayers[user].GetComponent<SimpleRemoteInterpolation>().SetPosition(combined, true);
                    userData.State = 2;
                }
                else
                {
                    float tNorm = 0.8f; // 取值越大，越接近真实轨迹，但不平滑
                    //1.从server收到新位置数据，作为last known，求出新的速度、加速度
                    Vector3 v0 = (userData.P0 - userData.Pm1) / Time.deltaTime;
                    Vector3 p0Prime = newPos;
                    Vector3 v0Prime = p0Prime - userData.Pm1;
                    Vector3 a0Prime = (v0Prime - userData.Vm1)/Time.deltaTime;
                    //2.velocity blending
                    Vector3 vb = v0 + (v0Prime - v0)*tNorm;
                    //3.从当前位置，根据混合速度和新加速度，计算预测位置Pt
                    Vector3 pt = userData.P0 + vb*Time.deltaTime +
                                 0.5f*a0Prime*Time.deltaTime*Time.deltaTime;
                    //4.从last known位置，根据新速度和加速度，计算预测位置Pt'
                    Vector3 ptPrime = p0Prime + v0Prime*Time.deltaTime +
                                      0.5f*a0Prime*Time.deltaTime*Time.deltaTime;
                    //5.结合两个预测位置，计算目标位置
                    Vector3 combined = pt + (ptPrime - pt)*tNorm;
//                    Debug.Log(userData.State);
//                    Debug.Log(combined);
                    //6.移动到目标位置
                    remotePlayers[user].GetComponent<SimpleRemoteInterpolation>().SetPosition(combined, true);
                    //7.准备下一次迭代
                    userData.Pm1 = userData.P0;
                    userData.Vm1 = v0;
                    userData.P0 = combined;
                }

                // clear
                userData.Tt = 0.0f;
                //验证服务器发过来的数据是否正确
                //            Debug.Log(string.Format("{0},{1},{2}", (float)user.GetVariable("PosX").GetDoubleValue(),
                //                (float)user.GetVariable("PosY").GetDoubleValue(),
                //                (float)user.GetVariable("PosZ").GetDoubleValue()));
            }

            if (changedVars.Contains("RotY"))
            {
                remotePlayers[user].GetComponent<SimpleRemoteInterpolation>().SetQuaternion(
                    Quaternion.Euler(0, (float)user.GetVariable("RotY").GetDoubleValue(), 0),
                    true
                );
            }

            //播放同步动画
            if (changedVars.Contains("Animation"))
            {
                string anim = user.GetVariable("Animation").GetStringValue();
                if (anim.Equals("Run"))
                {
                    remotePlayers[user].GetComponentInChildren<Animator>().SetBool("Run", true);
                }
                else if (anim.Equals("StopRun"))
                {
                    remotePlayers[user].GetComponentInChildren<Animator>().SetBool("Run", false);
                }
                else if (anim.Equals("Greet"))
                {
                    remotePlayers[user].GetComponentInChildren<Animator>().SetTrigger(anim);
                }
//                else
//                {
//                    remotePlayers[user].GetComponentInChildren<Animator>().SetTrigger(anim);
//                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void OnPingPongEvent(BaseEvent evt)
    {
        int lagValue = (int)evt.Params["lagValue"];//milliseconds
        NetworkLagText.text = lagValue.ToString();
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------
    public void Disconnect() {
		sfs.Disconnect();
	}
	
	//----------------------------------------------------------
	// Private player helper methods
	//----------------------------------------------------------
	private void SpawnLocalPlayer()
	{
        //取本地数据
        string username = PlayerPrefs.GetString("Username");
        int groupID = PlayerPrefs.GetInt("GroupID");
        int leader = PlayerPrefs.GetInt("Leader");
        int groupSize = PlayerPrefs.GetInt("GroupSize");
        int enterIndex = PlayerPrefs.GetInt("EnterIndex");
	    int gender = PlayerPrefs.GetInt("Gender");
        float speed = PlayerPrefs.GetFloat("Speed");

        string prefabKey = groupID.ToString("D2") + gender.ToString();
	    GameObject goPrefab = FindPlayerPrefabByName(prefabKey);
	    if (goPrefab == null)
	    {
	        Debug.Log("Can not find prefab");
	        return;
	    }

        //计算自己的生成位置
	    Vector3 basePos = SpawnPositions[groupID].position;
	    if (groupSize == 1 || enterIndex == 1)
	    {
            localPlayer = Instantiate(goPrefab, basePos, Quaternion.identity, GoParent.transform);
	    }
	    else
        {
            int interval = 360 / (groupSize - 1);
            localPlayer = Instantiate(goPrefab, basePos, Quaternion.identity, GoParent.transform);
            localPlayer.transform.Rotate(Vector3.up, interval * (enterIndex-1));
            localPlayer.transform.Translate(localPlayer.transform.forward, Space.World);
        }
	    localPlayer.name = username;
	    localPlayerController = localPlayer.GetComponent<RigidbodyFirstPersonController>();
        //先不让走动
	    localPlayerController.movementSettings.ForwardSpeed = 0f;
        //侧向、后向速度减半
	    localPlayerController.movementSettings.BackwardSpeed = 0f;
	    localPlayerController.movementSettings.StrafeSpeed = 0f;

        Vec3D pos = new Vec3D(localPlayer.transform.position.x, localPlayer.transform.position.y, localPlayer.transform.position.z);
        sfs.Send(new SetUserPositionRequest(pos));

        //处理旗帜
	    GameObject goFlag = Utilities.GetChildObjectWithName(localPlayer, "Flag");
        //不是leader不渲染flag
	    if (leader == 0)
	    {
	        goFlag.SetActive(false);
	    }
	    else
	    {
	        Color color = new Color(PlayerPrefs.GetFloat("R"), PlayerPrefs.GetFloat("G"), PlayerPrefs.GetFloat("B"));
	        goFlag.GetComponent<Renderer>().sharedMaterial.SetColor("_Color1out", color);
	    }
        
	    // 设置用户变量
	    List<UserVariable> userVariables = new List<UserVariable>();
	    userVariables.Add(new SFSUserVariable("PosX", (double)localPlayer.transform.position.x));
	    userVariables.Add(new SFSUserVariable("PosY", (double)localPlayer.transform.position.y));
	    userVariables.Add(new SFSUserVariable("PosZ", (double)localPlayer.transform.position.z));
	    userVariables.Add(new SFSUserVariable("RotY", (double)localPlayer.transform.localRotation.eulerAngles.y));
        userVariables.Add(new SFSUserVariable("RotX", (double)Camera.main.transform.localRotation.eulerAngles.x));
        userVariables.Add(new SFSUserVariable("Animation", "Idle"));
	    // Send request
	    sfs.Send(new SetUserVariablesRequest(userVariables));
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

    private void SpawnRemotePlayer(SFSUser user, Vector3 pos, Quaternion rot)
    {
		// 已经存在的话先删除一下
		if (remotePlayers.ContainsKey(user) && remotePlayers[user] != null) {
			Destroy(remotePlayers[user]);
			remotePlayers.Remove(user);
		}
        if (remoteUserData.ContainsKey(user) && remoteUserData[user] != null)
        {
            remoteUserData.Remove(user);
        }

        int groupID = user.GetVariable("GroupID").GetIntValue();
        int gender = user.GetVariable("Gender").GetIntValue();
        int leader = user.GetVariable("Leader").GetIntValue();

        Debug.Log("Spawning " + user.Name);
        //        string prefabKey = groupID.ToString("D2") + gender.ToString();
        string prefabKey;
        int localGroupID = PlayerPrefs.GetInt("GroupID");
        if (groupID == localGroupID) //使用同组化身
        {
            prefabKey = groupID.ToString("D2") + gender.ToString();
        }
        else//使用其他组的化身
        {
            prefabKey = "0" + gender.ToString();
        }
        GameObject goPrefab = FindPlayerPrefabByName(prefabKey);
        GameObject remotePlayer = Instantiate(goPrefab, pos, rot, GoParent.transform);
        remotePlayer.name = user.Name;
        //添加插值组件，禁用控制器
		remotePlayer.AddComponent<SimpleRemoteInterpolation>();
		remotePlayer.GetComponent<SimpleRemoteInterpolation>().SetTransform(pos, rot, false);
        Destroy(remotePlayer.GetComponent<RigidbodyFirstPersonController>());
        remotePlayer.GetComponent<Rigidbody>().isKinematic = true;
        //保留碰撞检测，不删除
        //Destroy(remotePlayer.GetComponent<CapsuleCollider>());
        //Destroy(remotePlayer.GetComponent<Rigidbody>());
        GameObject camera = Utilities.GetChildObjectWithName(remotePlayer, "MainCamera");
        camera.SetActive(false);

        //处理队旗
        GameObject goFlag = Utilities.GetChildObjectWithName(remotePlayer, "Flag");
        //只有同组的leader才渲染flag
        if (groupID == localGroupID && leader == 1)
        {
            float[] colorArray = user.GetVariable("GroupColor").GetSFSArrayValue().GetFloatArray(0);
            Color color = new Color(colorArray[0], colorArray[1], colorArray[2]);
            goFlag.GetComponent<Renderer>().sharedMaterial.SetColor("_Color1out", color);
        }
        else
        {
            goFlag.SetActive(false);
        }

        // Lets track the dude
        remotePlayers.Add(user, remotePlayer);
        UserData data = new UserData();
        data.P0 = pos;
        data.State = 1;
        remoteUserData.Add(user, data);
        Debug.Log("Spawned " + user.Name);
    }
	
	private void RemoveRemotePlayer(SFSUser user) {
		if (user == sfs.MySelf) return;
		
		if (remotePlayers.ContainsKey(user)) {
			Destroy(remotePlayers[user]);
			remotePlayers.Remove(user);
		}
	    if (remoteUserData.ContainsKey(user))
	    {
	        remoteUserData.Remove(user);
	    }
	}

    void OnApplicationQuit()
    {
        sfs.RemoveAllEventListeners();
        sfs.Disconnect();
    }

    //    private void CreateMMOItems()
    //    {
    //        SFSObject sfsObj = SFSObject.NewInstance();
    //        foreach (GameObject item in mmoItems)
    //        {
    //            SFSObject itemData = SFSObject.NewInstance();
    //            itemData.PutFloat("PosX", item.transform.position.x);
    //            itemData.PutFloat("PosY", item.transform.position.y);
    //            itemData.PutFloat("PosZ", item.transform.position.z);
    //            itemData.PutFloat("RotX", item.transform.rotation.eulerAngles.x);
    //            itemData.PutFloat("RotY", item.transform.rotation.eulerAngles.y);
    //            itemData.PutFloat("RotZ", item.transform.rotation.eulerAngles.z);
    //            sfsObj.PutSFSObject(item.name, itemData);
    //        }
    //        sfs.Send(new ExtensionRequest("CreateMMOItems",sfsObj,sfs.LastJoinedRoom));
    //    }
    //
    //    private void UpdateMMOItems()
    //    {
    //        List<Transform> changedItems = new List<Transform>();
    //        foreach (GameObject item in mmoItems)
    //        {
    //            CheckTransformChanged ctc = item.GetComponent<CheckTransformChanged>();
    //            if (ctc.hasChanged)
    //            {
    //                changedItems.Add(item.transform);
    //                ctc.hasChanged = false;
    //            }
    //        }
    //        if (changedItems.Count == 0)
    //        {
    //            return;
    //        }
    //
    //        SFSObject sfsObj = SFSObject.NewInstance();
    //        foreach (Transform item in changedItems)
    //        {
    //            SFSObject itemData = SFSObject.NewInstance();
    //            itemData.PutFloat("PosX", item.position.x);
    //            itemData.PutFloat("PosY", item.position.y);
    //            itemData.PutFloat("PosZ", item.position.z);
    //            itemData.PutFloat("RotX", item.rotation.eulerAngles.x);
    //            itemData.PutFloat("RotY", item.rotation.eulerAngles.y);
    //            itemData.PutFloat("RotZ", item.rotation.eulerAngles.z);
    //            sfsObj.PutSFSObject(item.name, itemData);
    //        }
    //        sfs.Send(new ExtensionRequest("UpdateMMOItems", sfsObj, sfs.LastJoinedRoom));
    //    }

}

public class UserData
{
    public float Tt = 0.0f; // sinace last update
    public int State = -1;
    public Vector3 P0 = Vector3.zero; // P(0)
    public Vector3 Pm1 = Vector3.zero; // P(-1)
    public Vector3 Vm1 = Vector3.zero; // V(-1)
}
