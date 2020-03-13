using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DungeonGenerator.createDungeon(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
