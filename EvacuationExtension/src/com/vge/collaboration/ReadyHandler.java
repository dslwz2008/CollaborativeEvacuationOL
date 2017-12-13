package com.vge.collaboration;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSArray;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.SFSUserVariable;
import com.smartfoxserver.v2.exceptions.SFSVariableException;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Random;

/**
 * Created by VGE on 2017/8/1.
 */
public class ReadyHandler extends BaseClientRequestHandler {
    @Override
    public void handleClientRequest(User user, ISFSObject isfsObject) {
        EvacuationExtension evcExt = (EvacuationExtension) getParentExtension();
        try {
            //如果已经开始了，只能等待下次机会
            if(evcExt.hasStarted){
                send("HasStarted", new SFSObject(), user);
                return;
            }

            int gid = isfsObject.getInt("GroupID");
            user.setVariable(new SFSUserVariable("GroupID", gid));
            int gSize = isfsObject.getInt("GroupSize");
            user.setVariable(new SFSUserVariable("GroupSize", gSize));
            user.setVariable(new SFSUserVariable("Gender",isfsObject.getInt("Gender")));
            user.setVariable(new SFSUserVariable("Leader",isfsObject.getInt("Leader")));
            SFSArray colorArray = new SFSArray();
            colorArray.addFloatArray(isfsObject.getFloatArray("GroupColor"));
            user.setVariable(new SFSUserVariable("GroupColor",colorArray));

            //看是第几个入组的
            Group group = evcExt.groups[gid];
            group.size = gSize;
            int readyNum = group.getReadyNum();
            int entryIndex = readyNum + 1;
            user.setVariable(new SFSUserVariable("EnterIndex", entryIndex));
            group.users.add(user);

            List<User> users = evcExt.getParentRoom().getUserList();
            //房间满员，开始疏散，给每位用户发送进入的顺序，本地用户用来定初始化位置
            if(evcExt.getParentRoom().getSize().getUserCount() == evcExt.userNum){
                //写日志,记录本次实验相关的信息
                String timeString = LocalDateTime.now().toString();
                String logString = String.format("%s,%s,%d,%d",
                        "EvacuationStarted",timeString,evcExt.userNum,evcExt.rightExit);
                trace(ExtensionLogLevel.INFO, logString);

                for (int i = 0; i < users.size(); i++){
                    User u = users.get(i);
                    ISFSObject resObj = new SFSObject();
                    resObj.putInt("EnterIndex", u.getVariable("EnterIndex").getIntValue());
                    resObj.putInt("RightExit", evcExt.rightExit);
                    send("Start", resObj, u);
                }
                evcExt.hasStarted = true;
            }else{//继续等待
                List<String> nameList = new ArrayList<>();
                for (int i = 0; i < users.size(); i++){
                    nameList.add(users.get(i).getName());
                }
                String names = String.join(",", nameList);
                ISFSObject resObj = new SFSObject();
                resObj.putInt("UserCount", evcExt.getParentRoom().getSize().getUserCount());
                resObj.putUtfString("Names", names);
                send("Wait", resObj, evcExt.getParentRoom().getUserList());
            }
        } catch (SFSVariableException e) {
            e.printStackTrace();
        }
    }
}
