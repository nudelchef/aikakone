using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleJSON;
using static audioManager;
using System.Threading;
using System.Globalization;
using static enemy;
using static objects;

public class item : MonoBehaviour
{
    string pathToItemJson = "";
    JSONNode items;
    public GameObject itemPrefab;

    public GameObject spieler;
    public float pickupRange = 1;
    private bool itemInHand = false;
    private GameObject[] allItems = new GameObject[1];
    private float[] allItemsDistancesToPlayer = new float[1];

    private int smallestDistanceIndex;
    private float playerToItemDistance;
    private float smallestDistance;

    public string pickupSound = "pickup";
    public string textureName = "1";

    public string itemInHandId = "";
    public string itemInHandType = "";
    // Start is called before the first frame update
    void Start()
    {
        enemy.spawnEnemy("1", new Vector3(0, 0, 0), 0f); //temporär -> Muss später in Level-Init
        enemy.spawnEnemy("2", new Vector3(0, 0, 0), 0f); //temporär -> Muss später in Level-Init
        objects.spawnObject("0", new Vector3(0, 0, 0), 0f); //temporär -> Muss später in Level-Init

        items = JSON.Parse((Resources.Load("items") as TextAsset).text);

        addMeleeToInventory("0");
    }

    // Update is called once per frame
    void Update()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

        if (Input.GetMouseButtonDown(1))
            {
                  pickUpAndDrop();
            }
    }

    void pickUpAndDrop()
    {
        allItems = new GameObject[100000];
        List<float> allItemsDistancesToPlayer = new List<float>();


        //Finde alle items
        int index = 0;
        int smallestDistanceIndex = 0;
        smallestDistance = 2147f;
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("item"))
        {
            playerToItemDistance = Vector3.Distance(spieler.transform.position, item.transform.position);
            allItemsDistancesToPlayer.Add(playerToItemDistance);
            if (playerToItemDistance < smallestDistance)
            {
                smallestDistanceIndex = index;
                smallestDistance = playerToItemDistance;
            }
            allItems[index] = item;
            index = index + 1;
        }


        //Hebe nähestes Item auf wenn in pickUpRange
        if (smallestDistance < pickupRange)
        {
            string temp = allItems[smallestDistanceIndex].name.Substring(1);
            itemInHandType = items[temp]["itemType"];
            if (itemInHandType == "gun")
            {
                dropItem(itemInHandId);
                addGunToInventory(temp);
                audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + pickupSound), spieler);//pickup sound effect
                GameObject.Find("ammoCapacityText").GetComponent<magazin>().ammoLeft = allItems[smallestDistanceIndex].GetComponent<itemStats>().ammoLeft;
                GameObject.Find("ammoCapacityText").GetComponent<magazin>().magLeft = allItems[smallestDistanceIndex].GetComponent<itemStats>().magLeft;
                GameObject.Find("ammoCapacityText").GetComponent<magazin>().updateAmmoCount();
            }
            else if (itemInHandType == "melee")
            {
                dropItem(itemInHandId);
                addMeleeToInventory(temp);
                audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + pickupSound), spieler);//pickup sound effect
            }


            Destroy(allItems[smallestDistanceIndex]);
        }
        else
        {
            dropItem(itemInHandId);
        }
    }

    public GameObject spawnItem(string itemId,Vector3 position)
    {
        GameObject b = Instantiate(itemPrefab) as GameObject;//Erstellt Objekt 
        b.transform.position = position; //Setzt position
        b.transform.rotation = Quaternion.Euler(0f, Random.Range(-360f, 360f), 0f); //Setzt zufällige rotation
        b.name = "i"+itemId; //Setzt Objektname
        b.GetComponent<Renderer>().material = Resources.Load<Material>("itemTextures/" + textureName); //Setzt Texture
        return b;
    }

    public void dropItem(string itemId)
    {
        if (itemInHandId != "0")
        {
            audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/drop"), spieler);//drop sound effect
            //Stop current Reload
            GameObject.Find("ammoCapacityText").GetComponent<magazin>().stopReload();
            //Create Item Object
            GameObject itemObject = spawnItem(itemId, spieler.transform.position); //Spawn item
            itemObject.GetComponent<itemStats>().ammoLeft = GameObject.Find("ammoCapacityText").GetComponent<magazin>().ammoLeft; //Setzt übrige Munition des Item auf derzeitige übrige Munition
            itemObject.GetComponent<itemStats>().magLeft = GameObject.Find("ammoCapacityText").GetComponent<magazin>().magLeft; //Setzt übrige Magazine des Item auf derzeitige übrige Magazine
                                                                                                                                
            addMeleeToInventory("0");//Setzt Current Weapon to "Hands"
            itemInHand = false;
        }
    }

    public void addGunToInventory(string itemId)
    {
        //Set all Weapon stats
        spieler.GetComponent<crosshair>().weaponDamage = float.Parse(items[itemId]["weaponDamage"]);
        spieler.GetComponent<crosshair>().feuerRateMin = float.Parse(items[itemId]["feuerRateMin"]);
        spieler.GetComponent<crosshair>().ammoCapacity = float.Parse(items[itemId]["ammoCapacity"]);
        spieler.GetComponent<crosshair>().magCapacity = float.Parse(items[itemId]["magCapacity"]);
        spieler.GetComponent<crosshair>().reloadTime = float.Parse(items[itemId]["reloadTime"]);
        spieler.GetComponent<crosshair>().bulletSpeed = float.Parse(items[itemId]["bulletSpeed"]);
        spieler.GetComponent<crosshair>().itemType = items[itemId]["itemType"].ToString().ToLower().Trim('"');
        spieler.GetComponent<crosshair>().itemId = itemId;
        spieler.GetComponent<crosshair>().useSoundName = items[itemId]["useSoundName"];
        spieler.GetComponent<crosshair>().reloadSoundName = items[itemId]["reloadSoundName"];

        pickupSound = items[itemId]["pickupSoundName"];
        textureName = items[itemId]["textureName"];

        itemInHandId = itemId;
        itemInHand = true;

        //Update Magazin Info
        GameObject.Find("ammoCapacityText").GetComponent<magazin>().Start();
    }
    public void addMeleeToInventory(string itemId)
    {
        //Set all Weapon stats
        spieler.GetComponent<melee>().weaponDamage = float.Parse(items[itemId]["weaponDamage"]);
        spieler.GetComponent<melee>().meleeRange = float.Parse(items[itemId]["range"]);
        spieler.GetComponent<melee>().meleeRateMin = float.Parse(items[itemId]["meleeRateMin"]);
        spieler.GetComponent<melee>().itemId = itemId;
        spieler.GetComponent<melee>().useSoundName = items[itemId]["useSoundName"];
        spieler.GetComponent<crosshair>().itemType = items[itemId]["itemType"].ToString().ToLower().Trim('"');

        pickupSound = items[itemId]["pickupSoundName"];
        textureName = items[itemId]["textureName"];

        itemInHandId = itemId;
        itemInHand = true;

        //Update Magazin Info
        GameObject.Find("ammoCapacityText").GetComponent<magazin>().Start();
    }
}
