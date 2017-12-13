package com.vge.collaboration;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by VGE on 2017/8/24.
 */
public class UserPrepareHandler extends BaseClientRequestHandler {
    @Override
    public void handleClientRequest(User user, ISFSObject isfsObject) {
        ZoneExtension ext = (ZoneExtension)getParentExtension();
        int userCount = ext.getParentZone().getUserCount();
        List<User> users = new ArrayList(ext.getParentZone().getUserList());

        // 发送已登陆的用户名字
        List<String> nameList = new ArrayList<>();
        for (int i = 0; i < users.size(); i++){
            nameList.add(users.get(i).getName());
        }
        String names = String.join(",", nameList);
        SFSObject sfsObject = SFSObject.newInstance();
        sfsObject.putUtfString("Names", names);
        send("LoginUsers", sfsObject, users);

        //人数够了的话，给所有人发消息
        if(userCount == ext.userNum){
            send("AllLogin", SFSObject.newInstance(), users);
        }
    }
}
