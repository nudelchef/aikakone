using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class LevelGenerator : MonoBehaviour {
	public enum gridSpace {empty, floor, wall};
	gridSpace[,] grid;
	int roomHeight, roomWidth;
	Vector2 roomSizeWorldUnits = new Vector2(100,100);
	float worldUnitsInOneGridCell = 1;
	struct walker{
		public Vector2 dir;
		public Vector2 pos;
	}
	List<walker> walkers;
	float chanceWalkerChangeDir = 0.5f, chanceWalkerSpawn = 0.05f;
	float chanceWalkerDestoy = 0.05f;
	int maxWalkers = 10;
	float percentToFill = 0.2f; 
	public SpriteAtlas atlas;
	public GameObject cubePrefab;
	void Start () {
		Setup();
		CreateFloors();
		CreateWalls();
		RemoveSingleWalls();
		addCornerWalls();
		fixSingleSpaces();
		SpawnLevel();
	}
	void Setup(){
		//find grid size
		roomHeight = Mathf.RoundToInt(roomSizeWorldUnits.x / worldUnitsInOneGridCell);
		roomWidth = Mathf.RoundToInt(roomSizeWorldUnits.y / worldUnitsInOneGridCell);
		//create grid
		grid = new gridSpace[roomWidth,roomHeight];
		//set grid's default state
		for (int x = 0; x < roomWidth-1; x++){
			for (int y = 0; y < roomHeight-1; y++){
				//make every cell "empty"
				grid[x,y] = gridSpace.empty;
			}
		}
		//set first walker
		//init list
		walkers = new List<walker>();
		//create a walker 
		walker newWalker = new walker();
		newWalker.dir = RandomDirection();
		//find center of grid
		Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth/ 2.0f),
										Mathf.RoundToInt(roomHeight/ 2.0f));
		newWalker.pos = spawnPos;
		//add walker to list
		walkers.Add(newWalker);
	}
	void CreateFloors(){
		int iterations = 0;//loop will not run forever
		do{
			//create 2x2 floor at position of every walker
			foreach (walker myWalker in walkers){
				grid[(int)myWalker.pos.x,(int)myWalker.pos.y] = gridSpace.floor;
				grid[(int)myWalker.pos.x+1, (int)myWalker.pos.y+1] = gridSpace.floor;
				grid[(int)myWalker.pos.x+1, (int)myWalker.pos.y] = gridSpace.floor;
				grid[(int)myWalker.pos.x, (int)myWalker.pos.y+1] = gridSpace.floor;
			}
			//chance: destroy walker
			int numberChecks = walkers.Count; //might modify count while in this loop
			for (int i = 0; i < numberChecks; i++){
				//only if its not the only one, and at a low chance
				if (Random.value < chanceWalkerDestoy && walkers.Count > 1){
					walkers.RemoveAt(i);
					break; //only destroy one per iteration
				}
			}
			//chance: walker pick new direction
			for (int i = 0; i < walkers.Count; i++){
				if (Random.value < chanceWalkerChangeDir){
					walker thisWalker = walkers[i];
					thisWalker.dir = RandomDirection();
					walkers[i] = thisWalker;
				}
			}
			//chance: spawn new walker
			numberChecks = walkers.Count; //might modify count while in this loop
			for (int i = 0; i < numberChecks; i++){
				//only if # of walkers < max, and at a low chance
				if (Random.value < chanceWalkerSpawn && walkers.Count < maxWalkers){
					//create a walker 
					walker newWalker = new walker();
					newWalker.dir = RandomDirection();
					newWalker.pos = walkers[i].pos;
					walkers.Add(newWalker);
				}
			}
			//move walkers
			for (int i = 0; i < walkers.Count; i++){
				walker thisWalker = walkers[i];
				thisWalker.pos += (thisWalker.dir*2);
				walkers[i] = thisWalker;				
			}
			//avoid boarder of grid
			for (int i =0; i < walkers.Count; i++){
				walker thisWalker = walkers[i];
				//clamp x,y to leave a 2 space boarder: leave room for walls
				thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth-3);
				thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight-3);
				walkers[i] = thisWalker;
			}
			//check to exit loop
			if ((float)NumberOfFloors() / (float)grid.Length > percentToFill){
				break;
			}
			iterations++;
		}while(iterations < 100000);
	}
	void CreateWalls(){
		//loop though every grid space
		for (int x = 0; x < roomWidth-1; x++){
			for (int y = 0; y < roomHeight-1; y++){
				//if theres a floor, check the spaces around it
				if (grid[x,y] == gridSpace.floor){
					//if any surrounding spaces are empty, place a wall
					if (grid[x,y+1] == gridSpace.empty){
						grid[x,y+1] = gridSpace.wall;
					}
					if (grid[x,y-1] == gridSpace.empty){
						grid[x,y-1] = gridSpace.wall;
					}
					if (grid[x+1,y] == gridSpace.empty){
						grid[x+1,y] = gridSpace.wall;
					}
					if (grid[x-1,y] == gridSpace.empty){
						grid[x-1,y] = gridSpace.wall;
					}
				}
			}
		}
	}
	void RemoveSingleWalls(){
		//loop though every grid space
		for (int x = 0; x < roomWidth-1; x++){
			for (int y = 0; y < roomHeight-1; y++){
				//if theres a wall, check the spaces around it
				if (grid[x,y] == gridSpace.wall){
					//assume all space around wall are floors
					bool allFloors = true;
					//check each side to see if they are all floors
					for (int checkX = -1; checkX <= 1 ; checkX++){
						for (int checkY = -1; checkY <= 1; checkY++){
							if (x + checkX < 0 || x + checkX > roomWidth - 1 || 
								y + checkY < 0 || y + checkY > roomHeight - 1){
								//skip checks that are out of range
								continue;
							}
							if ((checkX != 0 && checkY != 0) || (checkX == 0 && checkY == 0)){
								//skip corners and center
								continue;
							}
							if (grid[x + checkX,y+checkY] != gridSpace.floor){
								allFloors = false;
							}
						}
					}
					if (allFloors){
						grid[x,y] = gridSpace.floor;
					}
				}
			}
		}
	}
	void addCornerWalls()
	{
		//loop though every grid space
		for (int x = 0; x < roomWidth; x++)
		{
			for (int y = 0; y < roomHeight; y++)
			{
					if (grid[x, y] == gridSpace.empty && (grid.getValueOrNull(x - 1, y + 1) == gridSpace.floor || grid.getValueOrNull(x + 1, y - 1) == gridSpace.floor || grid.getValueOrNull(x - 1, y - 1) == gridSpace.floor || grid.getValueOrNull(x + 1, y + 1) == gridSpace.floor))
					{
                        if (grid.getValueOrNull(x + 1, y) == gridSpace.wall)
                        {
                            if (grid.getValueOrNull(x, y + 1) == gridSpace.wall)
                            {
								grid[x, y] = gridSpace.wall;
							}
							if (grid.getValueOrNull(x, y - 1) == gridSpace.wall)
							{
								grid[x, y] = gridSpace.wall;
							}
						}
						if (grid.getValueOrNull(x - 1, y) == gridSpace.wall)
						{
							if (grid.getValueOrNull(x, y + 1) == gridSpace.wall)
							{
								grid[x, y] = gridSpace.wall;
							}
							if (grid.getValueOrNull(x, y - 1) == gridSpace.wall)
							{
								grid[x, y] = gridSpace.wall;
							}
						}
					}
			}
		}
	}
	//TODO DOESNT WORK
	void fixSingleSpaces()
	{
		int wallCount;
		for (int x = 0; x < roomWidth; x++)
		{
			for (int y = 0; y < roomHeight; y++)
			{
				if(grid[x,y] == gridSpace.floor)
                {
					wallCount = 0;
					if (grid[x + 1, y] == gridSpace.wall)
                    {
						wallCount = wallCount + 1;
					}

					if (grid[x - 1, y] == gridSpace.wall)
                    {
						wallCount = wallCount + 1;
					}

					if (grid[x, y + 1] == gridSpace.wall)
                    {
						wallCount = wallCount + 1;
					}

					if (grid[x, y - 1] == gridSpace.wall)
                    {
						wallCount = wallCount + 1;
					}

					int emptyCount = 0;
					if (grid[x + 1, y] == gridSpace.empty)
						emptyCount = emptyCount + 1;

					if (grid[x - 1, y] == gridSpace.empty)
						emptyCount = emptyCount + 1;

					if (grid[x, y + 1] == gridSpace.empty)
						emptyCount = emptyCount + 1;

					if (grid[x, y - 1] == gridSpace.empty)
						emptyCount = emptyCount + 1;


					if (wallCount == 3)
						grid[x, y] = gridSpace.wall;

					if (wallCount == 2 && emptyCount == 1)
						grid[x, y] = gridSpace.wall;
				}
			}
		}
	}
	void SpawnLevel(){
		for (int x = 0; x < roomWidth; x++){
			for (int y = 0; y < roomHeight; y++){
				Spawn(x, y, grid[x, y]);
				/*switch(grid[x,y]){
					case gridSpace.empty:
						break;
					case gridSpace.floor:
						Spawn(x,y,floorObj);
						break;
					case gridSpace.wall:
						Spawn(x,y,wallObj);
						break;
				}*/
			}
		}
	}
	Vector2 RandomDirection(){
		//pick random int between 0 and 3
		int choice = Mathf.FloorToInt(Random.value * 3.99f);
		//use that int to chose a direction
		switch (choice){
			case 0:
				return Vector2.down;
			case 1:
				return Vector2.left;
			case 2:
				return Vector2.up;
			default:
				return Vector2.right;
		}
	}
	int NumberOfFloors(){
		int count = 0;
		foreach (gridSpace space in grid){
			if (space == gridSpace.floor){
				count++;
			}
		}
		return count;
	}
	void Spawn(int x, int y, LevelGenerator.gridSpace toSpawn){
		/*//find the position to spawn
		Vector2 offset = roomSizeWorldUnits / 2.0f;
		Vector2 spawnPos = new Vector2(x,y) * worldUnitsInOneGridCell - offset;
		//spawn object
		Instantiate(toSpawn, spawnPos, Quaternion.identity);*/
		GameObject go = null;

		if (toSpawn == gridSpace.empty)
        {
			return;
        }else if(toSpawn == gridSpace.floor)
        {
			go = GameObject.Instantiate(cubePrefab) as GameObject;
			go.transform.position = new Vector3(x, -0.502f, y);
			//go.transform.rotation = Quaternion.Euler(90f + rotation.x, 0 + rotation.y, 0 + rotation.z);
			go.name = "boden";

			var rend = go.GetComponent<SpriteRenderer>();
			rend.sprite = atlas.GetSprite("floor_basic");
		}
		else if (toSpawn == gridSpace.wall)
		{
			go = GameObject.Instantiate(cubePrefab) as GameObject;
			go.transform.position = new Vector3(x, -0.502f, y);
			//go.transform.rotation = Quaternion.Euler(90f + rotation.x, 0 + rotation.y, 0 + rotation.z);
			go.name = "wall";

			var coll = go.GetComponent<BoxCollider>();
			coll.size = new Vector3(0.32f, 0.32f, 1f);

			var rend = go.GetComponent<SpriteRenderer>();

			if (grid.getValueOrNull(x, y - 1) == gridSpace.floor)
			{
				rend.sprite = atlas.GetSprite("wall_2");
			}
			else if (grid.getValueOrNull(x, y + 1) == gridSpace.floor)
            {
				rend.sprite = atlas.GetSprite("wall_8");
			}
			else if (grid.getValueOrNull(x - 1, y) == gridSpace.floor)
			{
				rend.sprite = atlas.GetSprite("wall_6");
			}
			else if (grid.getValueOrNull(x + 1, y) == gridSpace.floor)
			{
				rend.sprite = atlas.GetSprite("wall_4");
			}


			if (grid.getValueOrNull(x + 1, y) == gridSpace.wall && grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x, y - 1) != gridSpace.wall && grid.getValueOrNull(x - 1, y) != gridSpace.wall)
			{
				if (grid.getValueOrNull(x, y - 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_13");
				}
				else
				{
					rend.sprite = atlas.GetSprite("wall_7");
				}
			}
			else if (grid.getValueOrNull(x - 1, y) == gridSpace.wall && grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x, y - 1) != gridSpace.wall && grid.getValueOrNull(x + 1, y) != gridSpace.wall)
			{
				if (grid.getValueOrNull(x, y - 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_12");
				}
				else
				{
					rend.sprite = atlas.GetSprite("wall_9");
				}
			}
			else if (grid.getValueOrNull(x + 1, y) == gridSpace.wall && grid.getValueOrNull(x, y - 1) == gridSpace.wall && grid.getValueOrNull(x, y + 1) != gridSpace.wall && grid.getValueOrNull(x - 1, y) != gridSpace.wall)
			{
				if (grid.getValueOrNull(x - 1, y) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_10");
                }
                else
                {
					rend.sprite = atlas.GetSprite("wall_1");
				}
			}
			else if (grid.getValueOrNull(x - 1, y) == gridSpace.wall && grid.getValueOrNull(x, y - 1) == gridSpace.wall && grid.getValueOrNull(x, y + 1) != gridSpace.wall && grid.getValueOrNull(x + 1, y) != gridSpace.wall)
			{
				if (grid.getValueOrNull(x + 1, y) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_11");
				}
				else
				{
					rend.sprite = atlas.GetSprite("wall_3");
				}
            }
            else
            {
                if (grid.getValueOrNull(x + 1, y) == gridSpace.wall && grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x + 1, y + 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_7");
				}
				else if (grid.getValueOrNull(x - 1, y) == gridSpace.wall && grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x - 1, y + 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_9");
				}
				else if (grid.getValueOrNull(x + 1, y) == gridSpace.wall && grid.getValueOrNull(x, y - 1) == gridSpace.wall && grid.getValueOrNull(x + 1, y - 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_1");
				}
				else if (grid.getValueOrNull(x - 1, y) == gridSpace.wall && grid.getValueOrNull(x, y - 1) == gridSpace.wall && grid.getValueOrNull(x - 1, y - 1) == gridSpace.floor)
				{
					rend.sprite = atlas.GetSprite("wall_3");
				}
			}


			if (grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x + 1, y) == gridSpace.floor && grid.getValueOrNull(x - 1, y) == gridSpace.floor)
			{
				rend.sprite = atlas.GetSprite("wall_15");
			}
			if (grid.getValueOrNull(x, y - 1) == gridSpace.wall && grid.getValueOrNull(x + 1, y) == gridSpace.floor && grid.getValueOrNull(x - 1, y) == gridSpace.floor)
			{
				rend.sprite = atlas.GetSprite("wall_16");
			}
			if (grid.getValueOrNull(x + 1, y) != gridSpace.wall && grid.getValueOrNull(x - 1, y) != gridSpace.wall && grid.getValueOrNull(x, y + 1) == gridSpace.wall && grid.getValueOrNull(x, y - 1) == gridSpace.wall)
			{
				rend.sprite = atlas.GetSprite("wall_14");
			}
		}

	}
}

public static class MyExtensions
{
	public static LevelGenerator.gridSpace? getValueOrNull(this LevelGenerator.gridSpace[,] grid,int x, int y)
	{
        try
        {
			return grid[x, y];
        }
        catch
        {
			return null;
        }
	}
}