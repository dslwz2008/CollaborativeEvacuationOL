using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;
using UnityEngine.SceneManagement;

public class GetReadyManager : MonoBehaviour {

	//----------------------------------------------------------
	// Editor public properties
	//----------------------------------------------------------

	[Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
	public string GameHost = "127.0.0.1";
	
	[Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
	public int TcpPort = 9933;
	
	[Tooltip("Name of the SmartFoxServer 2X zoneName to join")]
	public string ZoneName = "BasicExamples";

    public string RoomName = "Evacuation";

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------
    const string RoleLeader = "队长";
    const string RoleGroupMemeber = "队员";
    const string PromptLeader = "疏散过程中照顾好组内成员是队长的责任！";
    const string PromptGroupMemeber = "疏散过程中请紧跟自己的队长，以免走失落单！";

	public Text UsernameText;
    public Text GroupIdText;
    public Text GroupSizeText;
    public Text LeaderText;
    public Image GroupFlag;
    public Text GroupAvataText;
    public RawImage FemaleRawImage;
    public RawImage MaleRawImage;
    public Text PromptText;
    public Text StatusText;
	public Button GetReadyButton;
    public Color[] GroupColors;
    public Text LoginUserListText;
    public Text PreparedUserListText;

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private SmartFox sfs;
	
	//----------------------------------------------------------
	// Unity calback methods
	//----------------------------------------------------------

	void Start()
	{
	    string username = PlayerPrefs.GetString("Username");
        int groupID = PlayerPrefs.GetInt("GroupID");
	    int leader = PlayerPrefs.GetInt("Leader");
	    int groupSize = PlayerPrefs.GetInt("GroupSize");

	    UsernameText.text = username;
	    GroupIdText.text = groupID.ToString();
	    GroupSizeText.text = groupSize.ToString();
	    LeaderText.text = leader == 1 ? RoleLeader : RoleGroupMemeber;
	    Color color;
        //单独成组的情况
	    if (groupID >= 17)
	    {
	        color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value); ;//随机给一个
	    }
	    else
	    {
	        color = GroupColors[groupID - 1];
	    }
            //保存颜色，Main场景使用
	    PlayerPrefs.SetFloat("R", color.r);
	    PlayerPrefs.SetFloat("G", color.g);
	    PlayerPrefs.SetFloat("B", color.b);
	    GroupFlag.material.SetColor("_Color1out", color);
	    string texturePath = "Characters/" + groupID + "/";
	    if (groupID < 17)
	    {
            FemaleRawImage.texture = Resources.Load(texturePath + "female") as Texture;
            MaleRawImage.texture = Resources.Load(texturePath + "male") as Texture;
	    }
	    else//单人组不显示组内化身形象的图片
	    {
	        GroupAvataText.text = "单人组";
	        FemaleRawImage.enabled = false;
	        MaleRawImage.enabled = false;
	    }
        PromptText.text = leader == 1 ? PromptLeader : PromptGroupMemeber;

        //先禁用
        GetReadyButton.interactable = false;

        //连接服务器
        ConfigData cfg = new ConfigData();
        cfg.Host = GameHost;
        cfg.Port = TcpPort;
        cfg.Zone = ZoneName;
        cfg.UseBlueBox = false;
        sfs = new SmartFox();

        // Set ThreadSafeMode explicitly, or Windows Store builds will get a wrong default value (false)
        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        sfs.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);

        // Connect to SFS2X
        sfs.Connect(cfg);
    }

    void Update() {
		if (sfs != null)
			sfs.ProcessEvents();
	}

	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------
	public void OnLoginButtonClick()
	{
        sfs.Send(new JoinRoomRequest(RoomName));
	    GetReadyButton.interactable = false;
	}

    private void OnUserExitRoom(BaseEvent evt)
    {
        Room room = (Room)evt.Params["room"];
        User user = (User)evt.Params["user"];
        Debug.Log("User: " + user.Name + " has just left Room: " + room.Name); 
    }

    private void reset()
    {
        GetReadyButton.interactable = false;
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
	}

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	private void OnConnection(BaseEvent evt) {
		if ((bool)evt.Params["success"]) {
			// Save reference to the SmartFox instance in a static field, to share it among different scenes
			SmartFoxConnection.Connection = sfs;

			// Login
            sfs.Send(new LoginRequest(UsernameText.text, "", "BasicExamples"));
		}
		else {
			// Remove SFS2X listeners and re-enable interface
			reset();
			
			// Show error message
			StatusText.text = "Connection failed. Server is running?";
		}
	}
	
	private void OnConnectionLost(BaseEvent evt) {
		// Remove SFS2X listeners and re-enable interface
		reset();
		
		string reason = (string) evt.Params["reason"];
		
		if (reason != ClientDisconnectionReason.MANUAL) {
            // Show error message
		    StatusText.text = "Connection failed because : " + reason;
		}
    }
	
	private void OnLogin(BaseEvent evt)
    {
//        ISFSObject data = evt.Params["data"] as ISFSObject;
//        int index = data.GetInt("SpawnIndex");
//        PlayerPrefs.SetInt("SpawnIndex", index);
        StatusText.text = "登录服务器成功！";
        sfs.Send(new ExtensionRequest("Prepare", SFSObject.NewInstance()));
	}
	
	private void OnLoginError(BaseEvent evt) {
		// Disconnect
		sfs.Disconnect();
		
		// Remove SFS2X listeners and re-enable interface
		reset();
		
		// Show error message
		StatusText.text = "Login failed: " + (string) evt.Params["errorMessage"];
	}
	
	private void OnRoomJoin(BaseEvent evt)
	{
        SFSObject sfsObj = SFSObject.NewInstance();
        sfsObj.PutInt("GroupID", PlayerPrefs.GetInt("GroupID"));
        sfsObj.PutInt("GroupSize", PlayerPrefs.GetInt("GroupSize"));
        sfsObj.PutInt("Leader", PlayerPrefs.GetInt("Leader"));
        sfsObj.PutInt("Gender", PlayerPrefs.GetInt("Gender"));
        sfsObj.PutFloatArray("GroupColor",new float[] {PlayerPrefs.GetFloat("R"),PlayerPrefs.GetFloat("G"),PlayerPrefs.GetFloat("B")});
        sfs.Send(new ExtensionRequest("Ready", sfsObj, sfs.LastJoinedRoom));
    }

    private void OnRoomJoinError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();

        // Show error message
        StatusText.text = "Join room failed: " + (string) evt.Params["errorMessage"];
	}

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        SFSObject dataObject = (SFSObject)evt.Params["params"];

        switch (cmd)
        {
            case "AllLogin":
            {
                GetReadyButton.interactable = true;
                break;
            }
            case "LoginUsers":
            {
                string loginUsernames = dataObject.GetUtfString("Names");
                LoginUserListText.text = loginUsernames;
                break;
            }
            case "Start":
            {
                //给主场景使用
                PlayerPrefs.SetInt("EnterIndex", dataObject.GetInt("EnterIndex"));
                //正确出口
                PlayerPrefs.SetInt("RightExit", dataObject.GetInt("RightExit"));
                // Remove SFS2X listeners and re-enable interface before moving to the main game scene
                reset();

                // Go to main game scene
                SceneManager.LoadScene("Main");
                break;
            }
            case "Wait":
            {
                int curUserCount = dataObject.GetInt("UserCount");
                StatusText.text = string.Format("正在等待其他用户......\n已加入 {0} 名用户\n", curUserCount);
                string preparedUsernames = dataObject.GetUtfString("Names");
                PreparedUserListText.text = preparedUsernames;
                break;
            }
            case "HasStarted":
            {
                StatusText.text = "疏散已经开始，请等待下次机会！";
                break;
            }
            default:
                break;
        }
    }
}