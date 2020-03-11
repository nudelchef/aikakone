using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class despawnManager : MonoBehaviour
{
    private int maxGameObjects = 1000; //If this number of GameObjects in the List is reached the oldest GameObject will be destroyed

    public static class global
    {
        public static List<GameObject> objects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (despawnManager.global.objects.Count >= maxGameObjects)
        {
            Destroy(despawnManager.global.objects[0]);
            despawnManager.global.objects.RemoveAt(0);
        }
    }
}
