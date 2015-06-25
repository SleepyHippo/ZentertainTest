using UnityEngine;
using System.Collections;

public class Rover : MonoBehaviour {

	public enum Direction
	{
		N=0,
		E=1,
		S=2,
		W=3
	}
	public enum Command
	{
		NULL,
		M,
		L,
		R,
		B
	}

	public bool IsAlive
	{
		get; 
		private set;
	}

	public int PosX
	{
		get;
		private set;
	}
	public int PosY
	{
		get;
		private set;
	}
	public Direction Dir
	{
		get;
		private set;
	}

	public Command[] Commands
	{
		get;
		private set;
	}
	public int CurrentActionIndex
	{
		get;
		private set;
	}
	// Use this for initialization
	void Start () {
		IsAlive = true;
		CurrentActionIndex = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DrawRover(int _x, int _y, Direction _dir)
	{
		PosX = _x;
		PosY = _y;
		Dir = _dir;
		transform.position = new Vector3(PosX, transform.position.y, PosY);
		if(Dir == Direction.N)
			transform.localEulerAngles = Vector3.zero;
		else if(Dir == Direction.E)
			transform.localEulerAngles = new Vector3(0, 90, 0);
		else if(Dir == Direction.W)
			transform.localEulerAngles = new Vector3(0, -90, 0);
		else if(Dir == Direction.S)
			transform.localEulerAngles = new Vector3(0, 180, 0);
	}

	public void SetRoverCommand(Command[] _commands)
	{
		Commands = _commands;
	}

	public Command PeekCommand()
	{
		if(CurrentActionIndex >= Commands.Length)
			return Command.NULL;
		return Commands[CurrentActionIndex];
	}



	public void Broken()
	{
		Debug.Log("DEAD");
		IsAlive = false;
		return;
	}

	public void LogState()
	{
		Debug.Log(string.Format("{0}:[{1},{2},{3}],{4}", gameObject.name, PosX, PosY, Dir, IsAlive ? "Alive" : "Dead"));
	}

	public void DoMove()
	{
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
		PosX = (int)transform.position.x;
		PosY = (int)transform.position.z;
		SkipCommand();
	}

	public void DoTurnL()
	{
		transform.Rotate(new Vector3(0, -90, 0));
		int DirInNum = (int)Dir;
		DirInNum -= 1;
		DirInNum = DirInNum < 0 ? DirInNum+4 : DirInNum;
		Dir = (Direction)DirInNum;
		SkipCommand();
	}

	public void DoTurnR()
	{
		transform.Rotate(new Vector3(0, 90, 0));
		Dir = (Direction)(((int)Dir+1)%4);
		SkipCommand();
	}

	public void DoBack()
	{
		transform.Rotate(new Vector3(0, 180, 0));
		transform.Translate(new Vector3(0, 0, 1), Space.Self);
		PosX = (int)transform.position.x;
		PosY = (int)transform.position.z;
		Dir = (Direction)(((int)Dir+2)%4);
		SkipCommand();
	}

	void SkipCommand()
	{
		CurrentActionIndex++;
	}
}
