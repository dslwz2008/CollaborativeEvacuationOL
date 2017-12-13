package com.vge.collaboration;

import com.smartfoxserver.v2.core.ISFSEvent;
import com.smartfoxserver.v2.core.SFSEventParam;
import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSObject;
import com.smartfoxserver.v2.entities.variables.UserVariable;
import com.smartfoxserver.v2.exceptions.SFSException;
import com.smartfoxserver.v2.exceptions.SFSVariableException;
import com.smartfoxserver.v2.extensions.BaseServerEventHandler;
import com.smartfoxserver.v2.extensions.ExtensionLogLevel;
import com.smartfoxserver.v2.mmo.Vec3D;

import java.time.LocalDateTime;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

/**
 * Created by ShenShen on 2017/5/14.
 */

public class UserVariablesHandler extends BaseServerEventHandler
{
    @Override
    public void handleServerEvent(ISFSEvent event) throws SFSException {
        try{

            EvacuationExtension ext = (EvacuationExtension)getParentExtension();

            List<UserVariable> variables = (List<UserVariable>) event.getParameter(SFSEventParam.VARIABLES);
            User user = (User) event.getParameter(SFSEventParam.USER);

            //把List转换成Map
            Map<String, UserVariable> varMap = new HashMap<String, UserVariable>();
            for (UserVariable var : variables)
            {
                varMap.put(var.getName(), var);
            }

            //更新位置信息
            if (varMap.containsKey("PosX") && varMap.containsKey("PosY") &&
                    varMap.containsKey("PosZ"))
            {
                Vec3D pos = new Vec3D
                        (
                                varMap.get("PosX").getDoubleValue().floatValue(),
                                varMap.get("PosY").getDoubleValue().floatValue(),
                                varMap.get("PosZ").getDoubleValue().floatValue()
                        );

                ext.mmoApi.setUserPosition(user, pos, ext.getParentRoom());
            }

            //写日志
            String timeString = LocalDateTime.now().toString();
            String logString = String.format("%s,%s,%d,%s,%f,%f,%f,%f,%f,%s",
                    "UserVariablesUpdate",timeString,user.getId(),user.getName(),
                    user.getVariable("PosX").getDoubleValue().floatValue(),
                    user.getVariable("PosY").getDoubleValue().floatValue(),
                    user.getVariable("PosZ").getDoubleValue().floatValue(),
                    user.getVariable("RotY").getDoubleValue().floatValue(),
                    user.getVariable("RotX").getDoubleValue().floatValue(),
                    user.getVariable("Animation").getStringValue());

            //每次更新存储该用户的数据
            trace(ExtensionLogLevel.INFO, logString);
        }catch (NullPointerException e) {
            e.printStackTrace();
        }
    }
}