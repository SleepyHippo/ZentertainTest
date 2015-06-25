using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mars : MonoBehaviour {

	public const int FLOOR = 0;
	public const int ROVER = 1;

	public int Width
	{
		get;
		private set;
	}
	public int Height
	{
		get;
		private set;
	}
	public int[,] Map
	{
		get;
		private set;
	}
	public List<Rover> Rovers
	{
		get;
		private set;
	}

	public TextAsset configFile;
	public GameObject floorPrefab;
	public GameObject roverPrefab;

	// Use this for initialization
	void Start () {
		Rovers = new List<Rover>();
		InitMars();
		CreateFloors();
	}
	
	// Update is called once per frame
	void OnGUI () {
		if(GUILayout.Button("NextStep"))
		{
			UpdateMars();
		}
	}

	void InitMars()
	{
		Debug.Log(configFile.text);
		
		bool hasReadFirstLine = false;
		bool hasNewRover = false;
		foreach(var line in configFile.text.Split('\n'))
		{
//			Debug.Log("line:" + line);
			string trimedLine = line.Trim();
			if(!hasReadFirstLine)
			{
				Width = int.Parse(trimedLine.Split(',')[0]);
				Height = int.Parse(trimedLine.Split(',')[1]);
				Map = new int[Width, Height];
				for(int i = 0; i < Width; ++i)
					for(int j = 0; j < Height; ++j)
						Map[i, j] = FLOOR;
				hasReadFirstLine = true;
			}
			else 
			{
				if(!hasNewRover)
				{
					hasNewRover = true;
					string[] roverDetails = trimedLine.Split(',');
					int x = int.Parse(roverDetails[0]);
					int y = int.Parse(roverDetails[1]);
					Rover.Direction dir = (Rover.Direction)Enum.Parse(typeof(Rover.Direction), roverDetails[2]);
					
					GameObject newRover = Instantiate(roverPrefab) as GameObject;
					newRover.name = "Rover" + (Rovers.Count+1);
					Rover newRoverScript = newRover.GetComponent<Rover>();
					newRoverScript.DrawRover(x, y, dir);
					Rovers.Add(newRoverScript);
					Map[x, y] = ROVER;
				}
				else
				{
					Rover.Command[] commands = new Rover.Command[trimedLine.Length];
					for(int i = 0; i < trimedLine.Length; ++i)
					{
						//						Debug.Log(trimedLine[i]);
						commands[i] = (Rover.Command)Enum.Parse(typeof(Rover.Command), trimedLine[i].ToString());
					}
					Rovers[Rovers.Count-1].SetRoverCommands(commands);
					hasNewRover = false;
				}
			}
		}
		
		if(hasNewRover)
		{
			Debug.LogError("Config file format error!");
			return;
		}
	}

	void CreateFloors()
	{
		GameObject floorGourp = new GameObject("floorGroup");
		for(int i = 0; i < Width; ++i)
		{
			for(int j = 0; j < Height; ++j)
			{
				GameObject floor = Instantiate(floorPrefab, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
				floor.transform.parent = floorGourp.transform;
			}
		}
	}

	void UpdateMars()
	{
		for(int i = 0; i < Rovers.Count; ++i)
		{
			Rover rover = Rovers[i];
			if(IsOutOfBoundry(rover.PosX, rover.PosY))
				rover.Broken();
			else
			{
				Rover.Command nowCommand = rover.PeekCommand();
				switch(nowCommand)
				{
				case Rover.Command.M:
					if ((rover.Dir == Rover.Direction.N && !HasObstacle(rover.PosX, 	rover.PosY+1)) 	||
					    (rover.Dir == Rover.Direction.E && !HasObstacle(rover.PosX+1, 	rover.PosY)) 	||
					    (rover.Dir == Rover.Direction.W && !HasObstacle(rover.PosX-1, 	rover.PosY)) 	||
					    (rover.Dir == Rover.Direction.S && !HasObstacle(rover.PosX, 	rover.PosY-1)))
					{
						Map[rover.PosX, rover.PosY] = FLOOR;
						rover.DoMove();
						if(!IsOutOfBoundry(rover.PosX, rover.PosY))
							Map[rover.PosX, rover.PosY] = ROVER;
					}	
					break;
				case Rover.Command.L:
					rover.DoTurnL();
					break;
				case Rover.Command.R:
					rover.DoTurnR();
					break;
				case Rover.Command.B:
					if ((rover.Dir == Rover.Direction.N && !HasObstacle(rover.PosX, 	rover.PosY-1)) 	||
					    (rover.Dir == Rover.Direction.E && !HasObstacle(rover.PosX-1, 	rover.PosY)) 	||
					    (rover.Dir == Rover.Direction.W && !HasObstacle(rover.PosX+1, 	rover.PosY)) 	||
					    (rover.Dir == Rover.Direction.S && !HasObstacle(rover.PosX, 	rover.PosY+1)))
					{
						Map[rover.PosX, rover.PosY] = FLOOR;
						rover.DoBack();
						if(!IsOutOfBoundry(rover.PosX, rover.PosY))
							Map[rover.PosX, rover.PosY] = ROVER;
					}	
					break;
				case Rover.Command.NULL:
					Debug.Log("No more command");
					break;
				}
			}
			rover.LogState();
		}
	}

	bool HasObstacle(int _x, int _y)
	{
		if(IsOutOfBoundry(_x, _y))
			return false;
		return Map[_x, _y] == 1;
	}
	
	bool IsOutOfBoundry(int _x, int _y)
	{
		if(_x < 0 || _x >= Width || _y < 0 || _y >= Height)
			return true;
		return false;
	}
}
