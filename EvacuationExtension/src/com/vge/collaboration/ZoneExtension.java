package com.vge.collaboration;

import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.extensions.SFSExtension;

import java.util.Collections;
import java.util.LinkedList;

/**
 * Created by ShenShen on 2017/5/15.
 *
 */
public class ZoneExtension extends SFSExtension {
//    public LinkedList<Integer> spawnIndexQueue;
    public int userNum;

    @Override
    public void init() {
        userNum = Integer.parseInt(getConfigProperties().get("USER_NUM").toString());
//        addEventHandler(SFSEventType.USER_LOGIN, UserLoginHandler.class);
        addRequestHandler("Prepare", UserPrepareHandler.class);
    }

//    private void setupGame() {
//        spawnIndexQueue = new LinkedList<>();
//        for (int i = 0; i < Settings.USER_NUM; i++){
//            spawnIndexQueue.push(i);
//        }
//        Collections.shuffle(spawnIndexQueue);
//    }

}
