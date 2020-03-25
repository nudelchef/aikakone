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
    string objectId;
    string objectTextureName;
    string objectTextureDeadName;//TODO USE THIS TEXTURE IF OBJECT GETS DESTORYED
    string objectDieSoundName;//TODO PLAY THIS SOUND IF OBJECT GETS DESTORYED

    public static GameObject spawnObject(string objectId, Vector3 position, float rotation) //function to prepare object-spawn
    {
        GameObject b = Instantiate(Resources.Load("Prefabs/object")) as GameObject; //Creates Object
        b.transform.rotation = Quaternion.Euler(0f, rotation, 0f); //Sets random rotation
        b.name = "o" + objectId; //Sets objectname
        b.GetComponent<Renderer>().material = Resources.Load<Material>("objectTextures/" + objectId); //Sets texture
        return b;
    }
    // Start is called before the first frame update
    void Start()
    {
        objectId = this.name.Substring(1);
        //loading objectsJson and setting stats
        string pathToObjectsJson = Application.dataPath + "/objects.json";
        objectsJSON = JSON.Parse(File.ReadAllText(pathToObjectsJson));
        objectsHealth = float.Parse(objectsJSON[objectId]["objectsHealth"]);
        string objectsName = objectsJSON[objectId]["objectsName"];
        objectTextureName = objectsJSON[objectId]["textureName"];
        objectTextureDeadName = objectsJSON[objectId]["textureDeadName"];
        objectDieSoundName = objectsJSON[objectId]["dieSoundName"];
        this.GetComponent<Renderer>().material = Resources.Load<Material>("objectTextures/" + objectTextureName); //Setzt Texture
    }

    // Update is called once per frame
    void Update()
    {
        //if object has 0hp, it gets destroyed
        if (objectsHealth <= 0)
            Destroy(gameObject);
    }
}
