using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// store frame data. need new object
/// </summary>
public class Frame {
	public Frame(){
		Initialize();
	}

	public void Initialize(){
		this.id = 0;
		this.PlayerMoves = new Dictionary<string, PlayerMove>();
	}
	
	/// <summary>
	/// frame id
	/// </summary>
	public int id;
	
	/// <summary>
	/// The people.
	/// </summary>
	public Dictionary<string, PlayerMove> PlayerMoves = null; 

	public override string ToString ()
	{
		return string.Format("Frame:{0}\n", id);
	}
}
