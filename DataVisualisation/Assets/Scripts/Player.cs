using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerAnimation{
	Idle=0,
	Run,
	StopRun,
    Greet
}

/// <summary>
/// Player info
/// </summary>
public class Player
{
    public int UserID;
    public string Username;
    public string Password;
    public int GroupID;
    public int GroupSize;
    public int Leader;
    public int Gender;
    public float Speed;
    public float UserEvacTime;
    public float GroupEvacTime;
    public int KnowTruth;

    public Player(string[] items)
    {
        UserID = int.Parse(items[0]);
        Username = items[1];
        Password = items[2];
        GroupID = int.Parse(items[3]);
        GroupSize = int.Parse(items[4]);
        Leader = int.Parse(items[5]);
        Gender = int.Parse(items[6]);
        Speed = float.Parse(items[7]);
        UserEvacTime = Utilities.StrToFloatDef(items[8],0.0f);
        GroupEvacTime = Utilities.StrToFloatDef(items[9],0.0f);
        KnowTruth = int.Parse(items[10]);
    }
}

/// <summary>
/// PlayerMove.
///  Store the variant in one moment.
/// </summary>
public class PlayerMove
{
    public PlayerMove(string[] items)
    {
        TimeStamp = items[0];
        FrameID = int.Parse(items[1]);
        Username = items[2];
        PosX = float.Parse(items[3]);
        PosY = float.Parse(items[4]);
        PosZ = float.Parse(items[5]);
        RotY = float.Parse(items[6]);//×¢ÒâË³Ðò
        RotX = float.Parse(items[7]);
        Anim = items[8];
    }

    public string TimeStamp;
    public int FrameID;
    public string Username;
    public float PosX;
    public float PosY;
    public float PosZ;
    public float RotY;
    public float RotX;
    public string Anim;

	public override string ToString ()
	{
		return string.Format("[PlayerMove{0}]:{1} {2} {3}", Username, PosX, PosY, PosZ);
	}
}
