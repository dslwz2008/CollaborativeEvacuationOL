package com.vge.collaboration;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;
import com.smartfoxserver.v2.util.ClientDisconnectionReason;

import java.time.LocalDateTime;
import java.util.List;

/**
 * Created by VGE on 2017/8/14.
 */
public class UserDisconnectHandler extends BaseServerEventHandler {
    @Override
    public void handleServerEvent(ISFSEvent isfsEvent) throws SFSException {
        EvacuationExtension ext = (EvacuationExtension) getParentExtension();
        User user = (User) isfsEvent.getParameter(SFSEventParam.USER);
        ClientDisconnectionReason reason = (ClientDisconnectionReason) isfsEvent.getParameter(SFSEventParam.DISCONNECTION_REASON);

        //写日志
        String timeString = LocalDateTime.now().toString();
        String logString = String.format("%s,%s,%d,%s",
                "UserDisconnect", timeString, user.getId(), user.getName());
        trace(ExtensionLogLevel.INFO, logString);

        //更新组信息，通知该组人员
        int groupID = user.getVariable("GroupID").getIntValue();
        Group group = ext.groups[groupID];
        group.RemoveUserFromReady(user);
        //如果该用户已疏散完成
        if (group.hasEvacuated(user)) {
            group.RemoveUserFromEvacuated(user);
        }

        //看一下其他人是否全部疏散完
        //疏散完成，计算小组所用时间
        if(group.getEvacuateFinished()){
            float groupTime = group.getEvacuateTime();
            ISFSObject resObj = SFSObject.newInstance();
            resObj.putFloat("GroupEvacTime", groupTime);
            //发送给组内用户
            send("GroupFinished", resObj, group.users);

            //写日志
            String groupTimeString = LocalDateTime.now().toString();
            String groupLogString = String.format("%s,%s,%d,%f",
                    "GroupFinish",groupTimeString,groupID,groupTime);
            trace(ExtensionLogLevel.INFO, groupLogString);

            //释放group资源，不然下次会积累
            group = new Group();
        }else{//没疏散完，告诉他们有人掉线了
            ISFSObject sfsObject = SFSObject.newInstance();
            sfsObject.putUtfString("Username", user.getName());
            send("UserDisconnect", sfsObject, group.users);
        }
    }
}
