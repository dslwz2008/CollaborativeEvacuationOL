using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class FinishTrigger : MonoBehaviour
{
    public MainManager manager;

    void Start()
    {
        manager.sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        string cmd = (string)evt.Params["cmd"];
        SFSObject dataObject = (SFSObject)evt.Params["params"];

        switch (cmd)
        {
            case "GroupFinished":
            {
                float groupEvacTime = dataObject.GetFloat("GroupEvacTime");
                PlayerPrefs.SetFloat("GroupEvacTime", groupEvacTime);
                    reset();
                    manager.sfs.Disconnect();
                    SceneManager.LoadScene("Scenes/Finish");
                break;
            }
            case "WaitGroupMembers":
                {
                    manager.StatusText.text = "等待其他用户疏散完成，完成后自动跳转...";
                    break;
                }
            default:
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        //本地用户
        if (manager.sfs.MySelf.Name == other.gameObject.name)
        {
            //停止动画，原地等待
            List<UserVariable> userVariables = new List<UserVariable>();
            userVariables.Add(new SFSUserVariable("Animation", "StopRun"));
            manager.sfs.Send(new SetUserVariablesRequest(userVariables));
            other.gameObject.GetComponentInChildren<Animator>().SetBool("Run", false);
            other.gameObject.GetComponent<RigidbodyFirstPersonController>().enabled = false;
            //发送个人逃生的时间，等待同组伙伴
            SendPersonalTime();
        }
        else//远程用户，停止动画
        {
            other.gameObject.GetComponentInChildren<Animator>().SetBool("Run", false);
        }

    }

    private void SendPersonalTime()
    {
        //计算花费时间
        float timeCost = manager.TotalTime - manager.timer.timeLeft;
        PlayerPrefs.SetFloat("UserEvacTime", timeCost);
        SFSObject sfsObject = SFSObject.NewInstance();
        sfsObject.PutFloat("EvacTime", timeCost);
        manager.sfs.Send(new ExtensionRequest("UserFinish", sfsObject, manager.sfs.LastJoinedRoom));
    }

    private void reset()
    {
        // Remove SFS2X listeners
        manager.sfs.RemoveAllEventListeners();
    }
}
