/**
 * Created by ShenShen on 2017/5/6.
 */
package com.vge.collaboration;

import com.smartfoxserver.v2.SmartFoxServer;
import com.smartfoxserver.v2.api.ISFSMMOApi;
import com.smartfoxserver.v2.core.SFSEventType;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.extensions.SFSExtension;

import java.io.IOException;
import java.util.*;

public class EvacuationExtension extends SFSExtension {
    public ISFSMMOApi mmoApi;
    public HashMap<String,Integer> mmoItems;
    public Group[] groups;
    public boolean hasStarted;
    public int rightExit;
    public float knowTruthProb;
    public int userNum;
    public int groupNum;
    public List<User> enteredUsers;// 存储已进入主场景的用户

    // REQUESTS FROM CLIENT
    private static final String REQ_CREATEMMOITEMS = "CreateMMOItems";
    private static final String REQ_UPDATEMMOITEMS = "UpdateMMOItems";
    private static final String REQ_READY = "Ready";
    private static final String REQ_FINISH = "UserFinish";
    private static final String REQ_ENTERMAIN = "EnterMain";

    private void setupGame() throws IOException {

    }

    @Override
    public void init()
    {
        try {
            setupGame();
            hasStarted = false;
            mmoItems = new HashMap<>();
            mmoApi = SmartFoxServer.getInstance().getAPIManager().getMMOApi();
            userNum = Integer.parseInt(getConfigProperties().get("USER_NUM").toString());
            groupNum = Integer.parseInt(getConfigProperties().get("GROUP_NUM").toString());
            enteredUsers = new ArrayList<>();

            //初始化组对象
            groups = new Group[groupNum];
            for (int i = 0; i < groups.length; i++){
                groups[i] = new Group();
            }

            //本次实验正确出口是哪个，生成0或者1
            Random rand = new Random();
            rightExit = rand.nextInt(2);

            //事件处理回调
            addEventHandler(SFSEventType.USER_VARIABLES_UPDATE, UserVariablesHandler.class);
            addEventHandler(SFSEventType.USER_DISCONNECT, UserDisconnectHandler.class);
            addRequestHandler(REQ_READY, ReadyHandler.class);
            addRequestHandler(REQ_FINISH, FinishHandler.class);
            addRequestHandler(REQ_ENTERMAIN, EnterMainHandler.class);
//        addRequestHandler(REQ_CREATEMMOITEMS, CreateMMOItemsHandler.class);
//        addRequestHandler(REQ_UPDATEMMOITEMS, UpdateMMOItemsHandler.class);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void destroy()
    {
        super.destroy();
    }
}
