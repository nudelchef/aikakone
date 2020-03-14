using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class DungeonGenerator
{
    private const byte region_size = 40; // 100

    private const byte minRoomSize = 20, maxRoomSize = 30; // 40 80

    public static SpriteAtlas atlas = Resources.Load<SpriteAtlas>("dungeonAssets/purple");
    private static GameObject cubePrefab = Resources.Load<GameObject>("Prefabs/primitiveCube");

    struct Position2D
    {
        public int x;
        public int y;

        public Position2D(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void setPosition(Position2D pos)
        {
            this.x = pos.x;
            this.y = pos.y;
        }

        public void setPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public static void createDungeon(Dungeon dungeon, byte numOfRooms = 15)
    {
        /*
         * regions are used to place rooms without overlapping.
         */

        // ----- region size -----
        int num_region = (int)Mathf.Sqrt(numOfRooms) + 1;

        //int[] regions = new int[size * size];
        List<Position2D> free_region_list = new List<Position2D>();
        for (ushort y = 0; y < num_region; y++)
        {
            for (ushort x = 0; x < num_region; x++)
            {
                free_region_list.Add(new Position2D(x, y));
            }
        }

        ushort[,] tiles = new ushort[region_size * num_region, region_size * num_region];
        /* / / improtant - float aarray.
         * [ 0    ] floor
         * [ 1    ] tl wall
         * [ 2    ] t  wall
         * [ 3    ] tr wall
         * [ 4    ] l  wall
         * [ 5    ] allwall
         * [ 6    ] r  wall
         * [ 7    ] bl wall
         * [ 8    ] b  wall
         * [ 9    ] br wall
         * [ 10   ] |  door
         * [ 11   ] -  door
         * [ 20+  ] roomId (floor)
         */

        // roomId's start from 20 (to 65535)
        const ushort const_roomId = 20;
        ushort roomId = 0;

        // -- create room's --
        for (int __i__ = 0; __i__ < numOfRooms; __i__++) // __i__ (unused counter)
        {
            ushort free_index = (ushort)(Random.Range(0, free_region_list.Count));

            // ----- find a random free region ----- to create a room.
            Position2D region = free_region_list[free_index];
            free_region_list.RemoveAt(free_index);

            // ----- create room -----

            // even roomWidth & roomHeight
            ushort roomWidth = (ushort)(Random.Range(minRoomSize / 2, maxRoomSize / 2) * 2);
            ushort roomHeight = (ushort)(Random.Range(minRoomSize / 2, maxRoomSize / 2) * 2);

            // evenly placed roomX & roomY
            ushort roomX = (ushort)(Random.Range(0, (region_size - roomWidth) / 2) * 2);
            ushort roomY = (ushort)(Random.Range(0, (region_size - roomHeight) / 2) * 2);

            for (ushort y = 0; y < roomHeight; y++)
            {
                for (ushort x = 0; x < roomWidth; x++)
                {
                    ushort tileId = const_roomId;
                    tileId += roomId;

                    if (x == 0)
                    {
                        if (y == 0)
                        { // tl
                            tileId = 1;
                        }
                        else
                        if (y == roomHeight - 1)
                        { // bl
                            tileId = 7;
                        }
                        else
                        { // l
                            tileId = 4;
                        }
                    }
                    else
                    if (y == 0)
                    {
                        if (x == roomWidth - 1)
                        { // tr
                            tileId = 3;
                        }
                        else
                        { // t
                            tileId = 2;
                        }
                    }
                    else
                    if (y == roomHeight - 1)
                    {
                        if (x == roomWidth - 1)
                        { // br
                            tileId = 9;
                        }
                        else
                        { // r
                            tileId = 8;
                        }
                    }
                    else
                    if (x == roomWidth - 1)
                    { // r
                        tileId = 6;
                    }

                    tiles[region_size * region.y + y + roomY, region_size * region.x + x + roomX] = tileId;
                }
            }

            //Debug.Log("--- Room [ " + roomId + " (" + (roomId + const_roomId) + ") ] ---");
            for (ushort y = 0; y < region_size; y++)
            {
                string row = "";
                for (ushort x = 0; x < region_size; x++)
                {
                    string tileId = "" + tiles[region_size * region.y + y, region_size * region.x + x];
                    row += (tileId).PadLeft(5);
                }
             //   Debug.Log(row);
            }
           // Debug.Log("---  ---");
           // Debug.Log("");


            roomId++;
        }

        // clear room id's
        for (int y = 0; y < region_size * num_region; y++)
        {
            for (int x = 0; x < region_size * num_region; x++)
            {
                tiles[y, x] = tiles[y, x] >= const_roomId ? (ushort) 0 : tiles[y, x];
            }
        }




        {// instantiate
            for (int y = 0; y < region_size * num_region; y++)
            {
                for (int x = 0; x < region_size * num_region; x++)
                {
                    Vector3 position = new Vector3(x, 0, region_size * num_region-y);//Quaternion.Euler(0, 0, 0) * Vector3.right;

                    createTile(tiles[y, x], position, Quaternion.Euler(0, 180, 0));
                }
            }

            
        }// instantiate
    }

    public static void createDungeon(ushort[,] tiles)
    {
        for (int y = 0; y < tiles.GetLength(0); y++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                createTile(tiles[y, x], new Vector3(x, 0, y), Quaternion.Euler(0, 0, 0));
            }
        }
    }

    private static Object createTile(ushort tileId, Vector3 position, Quaternion rotation)
    {
        GameObject go = null;

        if (tileId == 0)
        {
            go = GameObject.Instantiate(cubePrefab) as GameObject;
            go.transform.position = position + new Vector3(0f, -0.502f, 0f);
            go.transform.rotation = Quaternion.Euler(90f+ rotation.x, 0 + rotation.y, 0 + rotation.z);
            go.name = "boden";

            var rend = go.GetComponent<SpriteRenderer>();
            rend.sprite = atlas.GetSprite("floor_basic");
        }
        else if (tileId >= 1 && tileId <= 9)
        {
            go = GameObject.Instantiate(cubePrefab) as GameObject;
            go.transform.position = position + new Vector3(0f, -0.502f, 0f);
            go.transform.rotation = Quaternion.Euler(90f + rotation.x, 0 + rotation.y, 0 + rotation.z);
            go.name = "wall";

            var rend = go.GetComponent<SpriteRenderer>();
            rend.sprite = atlas.GetSprite("wall_" + tileId);

            var coll = go.GetComponent<BoxCollider>();
            coll.size = new Vector3(0.32f, 0.32f, 1f);
        }

        return go;
    }
}
