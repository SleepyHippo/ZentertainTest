using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mars : MonoBehaviour {

	public int width;
	public int height;
	public int[,] map;
	public List<Rover> rovers = new List<Rover>();

	public TextAsset configFile;
	public GameObject floorPrefab;
	public GameObject roverPrefab;

	public bool HasObstacle(int _x, int _y)
	{
		if(IsOutOfBoundry(_x, _y))
			return false;
		return map[_x, _y] == 1;
	}

	public bool IsOutOfBoundry(int _x, int _y)
	{
		if(_x < 0 || _x >= width || _y < 0 || _y >= height)
			return true;
		return false;
	}

	// Use this for initialization
	void Start () {
		InitMars();
		CreateFloors();
//		Debug.Log(width);
//		Debug.Log(height);
//		Debug.Log(rovers.Count);
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
				width = int.Parse(trimedLine.Split(',')[0]);
				height = int.Parse(trimedLine.Split(',')[1]);
				map = new int[width, height];
				for(int i = 0; i < width; ++i)
					for(int j = 0; j < height; ++j)
						map[i, j] = 0;
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
					newRover.name = "Rover" + (rovers.Count+1);
					Rover newRoverScript = newRover.GetComponent<Rover>();
					newRoverScript.DrawRover(x, y, dir);
					rovers.Add(newRoverScript);
					map[x, y] = 1;
				}
				else
				{
					Rover.Command[] commands = new Rover.Command[trimedLine.Length];
					for(int i = 0; i < trimedLine.Length; ++i)
					{
						//						Debug.Log(trimedLine[i]);
						commands[i] = (Rover.Command)Enum.Parse(typeof(Rover.Command), trimedLine[i].ToString());
					}
					rovers[rovers.Count-1].SetRoverCommand(commands);
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
		for(int i = 0; i < width; ++i)
		{
			for(int j = 0; j < height; ++j)
			{
				GameObject floor = Instantiate(floorPrefab, new Vector3(i, 0, j), Quaternion.identity) as GameObject;
				floor.transform.parent = floorGourp.transform;
			}
		}
	}

	void UpdateMars()
	{
		for(int i = 0; i < rovers.Count; ++i)
		{
			Rover rover = rovers[i];
			if(IsOutOfBoundry(rover.PosX, rover.PosY))
				rover.Broken();
			else
			{
				Rover.Command nowCommand = rover.PeekCommand();
				switch(nowCommand)
				{
				case Rover.Command.M:
					if ((rover.Dir == Rover.Direction.N && !HasObstacle(rover.PosX, rover.PosY+1)) ||
					    (rover.Dir == Rover.Direction.E && !HasObstacle(rover.PosX+1, rover.PosY)) ||
					    (rover.Dir == Rover.Direction.W && !HasObstacle(rover.PosX-1, rover.PosY)) ||
					    (rover.Dir == Rover.Direction.S && !HasObstacle(rover.PosX, rover.PosY-1)))
					{
						map[rover.PosX, rover.PosY] = 0;
						rover.DoMove();
						if(!IsOutOfBoundry(rover.PosX, rover.PosY))
							map[rover.PosX, rover.PosY] = 1;
					}	
					break;
				case Rover.Command.L:
					rover.DoTurnL();
					break;
				case Rover.Command.R:
					rover.DoTurnR();
					break;
				case Rover.Command.B:
					if ((rover.Dir == Rover.Direction.N && !HasObstacle(rover.PosX, rover.PosY-1)) ||
					    (rover.Dir == Rover.Direction.E && !HasObstacle(rover.PosX-1, rover.PosY)) ||
					    (rover.Dir == Rover.Direction.W && !HasObstacle(rover.PosX+1, rover.PosY)) ||
					    (rover.Dir == Rover.Direction.S && !HasObstacle(rover.PosX, rover.PosY+1)))
					{
						map[rover.PosX, rover.PosY] = 0;
						rover.DoBack();
						if(!IsOutOfBoundry(rover.PosX, rover.PosY))
							map[rover.PosX, rover.PosY] = 1;
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


}
