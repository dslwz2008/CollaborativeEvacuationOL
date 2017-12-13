package com.vge.collaboration;

import com.smartfoxserver.v2.entities.User;

import java.util.ArrayList;

/**
 * Created by VGE on 2017/8/8.
 * 服务器端保存一份分组的数据
 */
public class Group {
    private boolean evacuateFinished;

    public Group(){
        this.size = 0;
        this.users = new ArrayList<>();
        this.evacuatedUsers = new ArrayList<>();
        this.evacuationTime = 0.0f;
    }
    public int size;

    //已就绪的用户列表
    public ArrayList<User> users;
    public float evacuationTime;
    //已疏散的用户列表
    public ArrayList<User> evacuatedUsers;

    //获取当前组中的用户数量
    public int getReadyNum() {
        return this.users.size();
    }

    //是否已经疏散完成
    public boolean getEvacuateFinished() {
        return this.evacuatedUsers.size() == this.users.size();
    }

    //返回群组的疏散时间，即最长的用户耗时
    public float getEvacuateTime() {
        float maxTime = 0.0f;
        for (int i = 0; i < users.size(); i++){
            User user = users.get(i);
            float temp = (float)user.getProperty("EvacTime");
            if(temp > maxTime){
                maxTime = temp;
            }
        }
        return maxTime;
    }

    //删除该用户
    public void RemoveUserFromReady(User user) {
        this.users.remove(user);
        this.size--;
    }

    public boolean hasEvacuated(User user){
        return this.evacuatedUsers.contains(user);
    }

    public void RemoveUserFromEvacuated(User user){
        this.evacuatedUsers.remove(user);
    }
}
