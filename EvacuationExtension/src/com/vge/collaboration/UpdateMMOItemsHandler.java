package com.vge.collaboration;

import com.smartfoxserver.v2.entities.User;
import com.smartfoxserver.v2.entities.data.ISFSObject;
import com.smartfoxserver.v2.entities.data.SFSDataWrapper;
import com.smartfoxserver.v2.extensions.BaseClientRequestHandler;
import com.smartfoxserver.v2.mmo.*;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

/**
 * Created by ShenShen on 2017/5/14.
 */
public class UpdateMMOItemsHandler extends BaseClientRequestHandler {
    @Override
    public void handleClientRequest(User user, ISFSObject params) {
        for (Iterator<Map.Entry<String, SFSDataWrapper>> it = params.iterator(); it.hasNext(); ) {
            Map.Entry<String, SFSDataWrapper> entry = it.next();
            String itemName = (String) entry.getKey();
            ISFSObject itemValue = (ISFSObject) (entry.getValue().getObject());

            // Prepare the variables
            List<IMMOItemVariable> vars = new ArrayList<>();
            vars.add(new MMOItemVariable("Name", itemName));
            Vec3D pos = new Vec3D(itemValue.getFloat("PosX"), itemValue.getFloat("PosY"), itemValue.getFloat("PosZ"));
            vars.add(new MMOItemVariable("PosX", pos.floatX()));
            vars.add(new MMOItemVariable("PosY", pos.floatY()));
            vars.add(new MMOItemVariable("PosZ", pos.floatZ()));
            vars.add(new MMOItemVariable("RotX", itemValue.getFloat("RotX")));
            vars.add(new MMOItemVariable("RotY", itemValue.getFloat("RotY")));
            vars.add(new MMOItemVariable("RotZ", itemValue.getFloat("RotZ")));

            EvacuationExtension ext = (EvacuationExtension)getParentExtension();
            MMORoom room = (MMORoom) ext.getParentRoom();
            MMOItem item = (MMOItem) room.getMMOItemById(ext.mmoItems.get(itemName));

            // Set the Item in the room at specific coordinates
            ext.mmoApi.setMMOItemPosition(item, pos, getParentExtension().getParentRoom());
        }
    }
}