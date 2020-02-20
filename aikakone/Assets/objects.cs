using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;
using System.IO;

public class objects : MonoBehaviour
{
    JSONNode objectsJSON;
    public string objectsName;
    public float objectsHealth;
    string objectID;

    // Start is called before the first frame update
    void Start()
    {
        objectID = this.name.Substring(1);
        //loading objectsJson and setting stats
        string pathToObjectsJson = Application.dataPath + "/objects.json";
        objectsJSON = JSON.Parse(File.ReadAllText(pathToObjectsJson));
        objectsHealth = float.Parse(objectsJSON[objectID]["objectsHealth"]);
        string objectsName = objectsJSON[objectID]["objectsName"];
        this.GetComponent<Renderer>().material = Resources.Load<Material>("objectTextures/" + objectID); //Setzt Texture
    }

    // Update is called once per frame
    void Update()
    {
        //if object has 0hp, it gets destroyed
        if (objectsHealth <= 0)
            Destroy(gameObject);
    }
}
