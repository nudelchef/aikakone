using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;
using System.IO;

public class objects : MonoBehaviour
{
    JSONNode objects;
    public string objectsName;
    public int objectsHealth;

    // Start is called before the first frame update
    void Start()
    {
        //loading objectsJson and setting stats
        string pathToObjectsJson = Application.dataPath + "/objects.json";
        objects = JSON.Parse(File.ReadAllText(pathToObjectsJson));
        objectsHealth = float.Parse(objectsJSON[0]["objectsHealth"]);
        objectsName = string.Parse(pathToObjectsJson[0]["objectsName"]);
    }

    // Update is called once per frame
    void Update()
    {
        //if object gets under 1hp, it gets destroyed
        if (objectsHealth == 0)
            Destroy(gameObject);
    }
}
