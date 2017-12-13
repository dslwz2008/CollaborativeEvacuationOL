using UnityEngine;
using System.Collections;
using System.IO;

public class ParseDataFromFiles : MonoBehaviour
{
    public string dataPath = "/data/0826/";
    public string participantsData = "participants.csv";
    public string expData = "exp1.csv";
    
    private FrameBuffer frameBuffer;
    private Players players;
    
    void Awake()
    {
        //读csv文件，分成单帧数据，写入FrameBuffer
        frameBuffer = GetComponent<FrameBuffer>();
        if(!frameBuffer)
        {
            Debug.Log("Frame Buffer Error！");
            return;
        }

        players = GetComponent<Players>();
        if (!players)
        {
            Debug.Log("Player Dictionary Error！");
            return;
        }

        //参与者信息的csv
        string participantsPath = Application.streamingAssetsPath + dataPath + participantsData;
        string[] lines = File.ReadAllLines(participantsPath);
        for(int i = 0; i < lines.Length; i++)
        {
            //跳过列名
            if (i == 0) continue;
            string[] items = lines[i].TrimEnd().Split(new char[] {','});
            players.PlayerDictionary.Add(items[1],new Player(items));
        }

        //参与者运动数据
        string expDataPath = Application.streamingAssetsPath + dataPath + expData;
        lines = File.ReadAllLines(expDataPath);
        int frameCount = -1;
        Frame frame = null;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] items = lines[i].TrimEnd().Split(new char[] { ',' });
            int frameID = int.Parse(items[1]);
            if (frameID != frameCount)
            {
                if(frame != null) frameBuffer.queue.Enqueue(frame);
                frameCount++;
                frame = new Frame();
                frame.id = frameCount;
            }
            frame.PlayerMoves.Add(items[2], new PlayerMove(items));
        }
        
        ////单独读取第一行
        //ArrayList firstLine = dt[0] as ArrayList;
        //linesPerFrame = int.Parse(firstLine[1].ToString());

        //int frameCount = (dt.Count - 1) / linesPerFrame;
        ////每一帧
        //for (int i = 0; i < frameCount; i++)
        //{
        //    Frame frame = FramePool.GetInstance().GetFrame();
        //    frame.id = i;
        //    //每一行
        //    for (int j = 0; j < linesPerFrame; j++)
        //    {
        //        int idx = i * linesPerFrame + 1 + j;
        //        ArrayList line = dt[idx] as ArrayList;

        //        //"id:{0}, people_id:{1}, people_status:{2}, posx:{3}, posy:{4}"
        //        //"posz:{5}, velx:{6}, vely:{7}",
        //        int peopleID = int.Parse(line[1].ToString());
        //        int peopleRole = -1;//int.Parse(data[2].ToString());
        //        int peopleStatus = int.Parse(line[2].ToString());
        //        float posx = float.Parse(line[3].ToString());
        //        float posy = float.Parse(line[5].ToString());//posz
        //        float posz = float.Parse(line[4].ToString());//posy
        //        float orntx = float.Parse(line[6].ToString());
        //        float ornty = float.Parse(line[7].ToString());

        //        People p = PeoplePool.GetInstance().GetPeople();
        //        p.m_id = peopleID;
        //        p.m_role = peopleID == 1 ? 0 : peopleRole;
        //        p.m_status = (PeopleStatus)peopleStatus;
        //        p.m_position = new Vector3(posx, posy, posz);
        //        p.m_orientation = new Vector3(orntx, 0, ornty);
        //        p.m_animSpeed = p.m_animBaseSpeed + (Random.value - 0.5f) * 0.4f;
        //        p.m_figureScale = Vector3.one;
        //        frame.peoples.Add(peopleID, p);

        //    }
        //    FrameBuffer.GetInstance().GetQueue().Enqueue(frame);
        //}
    }

}
