package com.vge.collaboration;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by ShenShen on 2017/5/15.
 */
public class UserLoginHandler extends BaseServerEventHandler{
    @Override
    public void handleServerEvent(ISFSEvent isfsEvent) throws SFSException {
//        // 返回spawn position数据
//        ZoneExtension ext = (ZoneExtension)getParentExtension();
//        int userCount = ext.getParentZone().getUserCount();
//        //人数够了的话，给所有人发消息
//        if(userCount == ext.userNum){
//            List<User> users = new ArrayList(ext.getParentZone().getUserList());
//            send("Prepare", SFSObject.newInstance(), users);
//        }
//        ISFSObject outData = (ISFSObject) isfsEvent.getParameter(SFSEventParam.LOGIN_OUT_DATA);
//        if(ext.spawnIndexQueue.size()!=0) {
//            int idx = ext.spawnIndexQueue.remove();
//            outData.putInt("SpawnIndex", idx);
//        }else{
//            outData.putUtfString("Error","Room is full.");
//        }
    }
}
