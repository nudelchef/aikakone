using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class poolManager : MonoBehaviour
{
    //"prefabs" List and "amountToPool" List always have to have the same length!
    public List<GameObject> prefabs = new List<GameObject>();
    public List<uint> amountToPool = new List<uint>();

    public static List<List<GameObject>> listOfGameObjectLists = new List<List<GameObject>>();
    public static List<int> listOfGameObjectIndexes = new List<int>();

    private static List<uint> amountToPool2 = new List<uint>();

    void Start()
    {
        amountToPool2 = amountToPool;

        if (prefabs.Count!=amountToPool.Count)
            throw new System.ArgumentException("prefabs List and amountToPool List have to have the same Count!");

        for (int i = 0;i < prefabs.Count;i++)
        {
            listOfGameObjectLists.Add(new List<GameObject>());
            listOfGameObjectIndexes.Add(0);
            for (int j = 0; j < amountToPool[i]; j++)
            {
                GameObject b = Instantiate(prefabs[i]) as GameObject;
                b.SetActive(false);
                listOfGameObjectLists[i].Add(b);
            }
        }
    }

    public static GameObject spawnObject(int GameObjectIndex)
    {
        if (listOfGameObjectIndexes[GameObjectIndex] == amountToPool2[GameObjectIndex]-1)
            listOfGameObjectIndexes[GameObjectIndex] = 0;
        else
            listOfGameObjectIndexes[GameObjectIndex]++;

        return listOfGameObjectLists[GameObjectIndex][listOfGameObjectIndexes[GameObjectIndex]];
    }
}