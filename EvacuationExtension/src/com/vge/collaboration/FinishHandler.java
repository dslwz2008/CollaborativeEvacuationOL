package com.vge.collaboration;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;

import java.time.LocalDateTime;

/**
 * Created by VGE on 2017/8/8.
 */
public class FinishHandler extends BaseClientRequestHandler {
    @Override
    public void handleClientRequest(User user, ISFSObject isfsObject) {
        EvacuationExtension evcExt = (EvacuationExtension) getParentExtension();
        float evacTime = isfsObject.getFloat("EvacTime");
        user.setProperty("EvacTime", evacTime);

        int groupId = user.getVariable("GroupID").getIntValue();
        //已疏散人数加1
        Group group = evcExt.groups[groupId];
        group.evacuatedUsers.add(user);

        //写日志
        String timeString = LocalDateTime.now().toString();
        String logString = String.format("%s,%s,%d,%d,%s,%f",
                "UserFinish",timeString,groupId,user.getId(),user.getName(),evacTime);
        trace(ExtensionLogLevel.INFO, logString);

        //看一下组员是否全部疏散完
        //疏散完成，计算小组所用时间
        if(group.getEvacuateFinished()){
            float groupTime = group.getEvacuateTime();
            ISFSObject resObj = new SFSObject();
            resObj.putFloat("GroupEvacTime", groupTime);
            //发送给组内用户
            send("GroupFinished", resObj, group.users);

            //写日志
            String groupTimeString = LocalDateTime.now().toString();
            String groupLogString = String.format("%s,%s,%d,%f",
                    "GroupFinish",groupTimeString,groupId,groupTime);
            trace(ExtensionLogLevel.INFO, groupLogString);

            //释放group资源，不然下次会积累
            group = new Group();
        }else{//没疏散完呢，再等会
            ISFSObject resObj = new SFSObject();
            send("WaitGroupMembers", resObj, user);
        }
    }
}
