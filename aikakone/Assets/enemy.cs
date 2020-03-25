using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;
using System.IO;
using static countdown;
using static audioManager;
using static poolManager;


public class enemy : MonoBehaviour
{
    JSONNode enemyJSON;
    JSONNode items;
    public GameObject spieler;
    public int spielerLeben = 100;
    private float spielerEntferung;
    private Rigidbody rb;
    private Vector3 movement;
    private float lastSeen = float.MinValue;
    public float movementSpeed = 5f;
    public float runingSpeed = 20f;
    public Vector3[] patrolPoints;
    public float enemyPrecision;
    public float range;
    public float health = 100f;
    public string enemyID;
    public int timeWonInSeconds;
    public int highscore = 0;
    public string itemId;
    public string itemType;
    public string textureDeadName; //TODO USE THIS TEXTURE IF ENEMY DIES

    //vars for shooting
    public float distanceToPlayer = 10;
    private Vector3 casingVelocity;
    private Vector3 target;
    private float lastShot = 0;
    public float bulletSpeed = 3000f;
    public float feuerRateMin = 450f;
    public float maxAmmoCapacity;
    public float reloadTime;
    public float ammoCapacity;
    private bool isReloading = false;
    public string reloadSoundName;

    //public vars for sight
    public float viewAngel = 120f;
    public float viewRadius = 10f;
    public float viewRememberTime = 3000f;

    //vars for melee
    public bool isEnemyMelee = false;
    public float meleeRadius;
    public float meleeRange = 1f;
    public float meleeRateMin = 120f;
    private int weaponDamage;

    private Vector3 targetPosition;
    private bool searchRunning = false;
    private bool patrolRunning = false;

    //private vars for patrol
    private int patrolPos = 0;
    private bool startSide = true;

    public NavMeshAgent agent;

    public static GameObject spawnEnemy(string enemyId, Vector3 position, float rotation) //Function to prepare enemy-spawn
    {
        GameObject b = Instantiate(Resources.Load("Prefabs/enemy")) as GameObject; //Creates object 
        b.transform.position = position; //Sets position
        b.transform.rotation = Quaternion.Euler(0f, rotation, 0f); //Sets random rotation
        b.name = "e" + enemyId; //Sets objectname
        b.GetComponent<Renderer>().material = Resources.Load<Material>("enemyTextures/" + enemyId); //Sets texture
        return b;
    }

    void Start()
    {
        spieler = GameObject.Find("spieler");

        //loading item.json
        string pathToItemJson = Application.dataPath + "/items.json";
        items = JSON.Parse(File.ReadAllText(pathToItemJson));

        //loading enemy.json and setting stats
        enemyID = this.name.Substring(1);
        string pathToEnemyJson = Application.dataPath + "/enemy.json";
        enemyJSON = JSON.Parse(File.ReadAllText(pathToEnemyJson));
        rb = this.GetComponent<Rigidbody>();
        movementSpeed = float.Parse(enemyJSON[enemyID]["movementSpeed"]);
        runingSpeed = float.Parse(enemyJSON[enemyID]["runningSpeed"]);
        health = float.Parse(enemyJSON[enemyID]["enemyHealth"]);
        enemyPrecision = float.Parse(enemyJSON[enemyID]["enemyPrecision"]);
        timeWonInSeconds = int.Parse(enemyJSON[enemyID]["timeWonInSeconds"]);
        itemId = enemyJSON[enemyID]["itemId"];
        textureDeadName = enemyJSON[enemyID]["textureDeadName"];
        itemType = items[itemId]["itemType"];
        maxAmmoCapacity = float.Parse(items[itemId]["ammoCapacity"]);
        reloadTime = float.Parse(items[itemId]["reloadTime"]);
        ammoCapacity = maxAmmoCapacity;
        weaponDamage = int.Parse(items[itemId]["weaponDamage"]);
        range = float.Parse(items[itemId]["range"]);
        reloadSoundName = items[itemId]["reloadSoundName"];
        this.GetComponent<Renderer>().material = Resources.Load<Material>("enemyTextures/" + enemyJSON[enemyID]["textureName"]); //Sets texture

        //If there are no patrol points, prepare to start searchrunning
        if (patrolPoints.Length < 1)
        {
            searchRunning = false;
            patrolRunning = false;
        }
    }

    void FixedUpdate()
    {
        //die
        if (health < 1)
        {
            Destroy(gameObject);
            countdown.timeLeft = countdown.timeLeft + timeWonInSeconds; //adds reward-time if enemy is killed
            highscore = highscore + timeWonInSeconds; //adds reward-time - which also are the actual points - to the highscore -> MUSS ICH NOCH ALS ANZEIGE IN-GAME EINFÜGEN UND DAS DANN ORDENTLICH VERKNÜPFEN
        }

        //enemy sight
        Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, viewRadius);
        foreach (Collider objects in objectsInRadius)
        {
            //Check if Player or Bullet in sight Radius
            if (objects.name == "spieler" || objects.name == "bullet(Clone)")
            {
                if (Input.GetMouseButton(0) && !spieler.GetComponent<crosshair>().isMelee && spieler.GetComponent<crosshair>().canShoot)
                {
                    //Enemy hears shot
                    lastSeen = Time.time * 1000;
                }

                Vector3 dirToTarget = (objects.transform.position - transform.position).normalized;
                //Check if Player or Bullet in sight Angle
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngel / 2)
                {
                    RaycastHit hit;
                    //Check if Player or Bullet not obstructed by wall
                    if (Physics.Raycast(transform.position, dirToTarget, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.name == "spieler" || hit.collider.name == "bullet(Clone)")
                        {
                            //Enemy sees Player or Bullet
                            lastSeen = Time.time * 1000;

                        }
                    }
                }
            }
        }
        //enemy sight ENDE

        if (((Time.time * 1000) - lastSeen) <= viewRememberTime)
        {
            //enemy saw player
            patrolRunning = false;
            StopCoroutine("enemySearch");
            searchRunning = false;

            if (itemType == "melee")
            {
                //Checks how daring the enemy can be
                //Less daring if near death
                if (health <= 25f)
                {
                    agent.speed = runingSpeed / 2f;
                }
                //More daring if player's near death
                else if (spielerLeben <= 25f)
                {
                    agent.speed = runingSpeed * 1.2f;
                    meleeRadius = meleeRadius - 1f;
                }
                //normal speed if both ok
                else
                {
                    agent.speed = runingSpeed;
                }

                agent.SetDestination(spieler.transform.position);
                if ((lastShot - (Time.time * 1000)) <= -(60000 / meleeRateMin))
                {
                    audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + itemId), gameObject);
                    Collider[] hitInfo = Physics.OverlapSphere(this.transform.position + this.transform.TransformDirection(new Vector3(0f, 0f, meleeRange)), meleeRange);
                    int i = 0;
                    while (i < hitInfo.Length)
                    {
                        if (hitInfo[i].name == "spieler")
                        {
                            //Player looses lifepoints
                            //hitInfo[i].GetComponent<PlayerHealth>().currentHealth = hitInfo[i].GetComponent<PlayerHealth>().currentHealth - weaponDamage; //TODO
                            Debug.Log("Gegner macht Schaden!");
                        }
                        i++;
                    }
                    lastShot = Time.time * 1000;
                }
            }
            else
            {

                float spielerEntfernung = Mathf.Sqrt(Mathf.Pow((spieler.transform.position.x - this.transform.position.x), 2) + Mathf.Pow((spieler.transform.position.y - this.transform.position.y), 2));

                Vector3 differnce = this.transform.position - spieler.transform.position;

                differnce.Normalize();
                differnce *= distanceToPlayer;
                agent.SetDestination(differnce + spieler.transform.position);
                transform.LookAt(spieler.transform.position);
                if (isReloading)
                {
                    return;
                }
                if (ammoCapacity >= 1)
                {

                    if ((lastShot - (Time.time * 1000)) <= -(60000 / feuerRateMin))
                    {
                        //shooting
                        Vector3 dirToTarget = (spieler.transform.position - transform.position).normalized;
                        RaycastHit hit;
                        //Check if Player or Bullet not obstructed by wall
                        if (Physics.Raycast(transform.position, dirToTarget, out hit, Mathf.Infinity))
                        {
                            if (hit.collider.name == "spieler")
                            {
                                audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + itemId), gameObject);//shoot sound effect

                                Vector3 unterschied = spieler.transform.position - this.transform.position;
                                float rotationZ = Mathf.Atan2(unterschied.x, unterschied.z) * Mathf.Rad2Deg;
                                float distance = unterschied.magnitude;
                                Vector3 direction = unterschied / distance;
                                direction.Normalize();

                                //SpawnBullet
                                GameObject bullet = poolManager.spawnObject(0);
                                bullet.transform.position = this.transform.position + (unterschied.normalized / 2);
                                bullet.transform.rotation = Quaternion.Euler(90f, rotationZ, 90f);
                                bullet.GetComponent<Rigidbody>().velocity = this.transform.TransformDirection(0f, 0f, bulletSpeed) * Time.deltaTime;
                                bulletCollision collison = bullet.GetComponent<bulletCollision>();
                                collison.enemyBullet = true;
                                bullet.SetActive(true);
                                collison.Start();

                                lastShot = Time.time * 1000;
                                ammoCapacity--;
                                //BULLETCASING
                                //TEMPORÄR 4 IF STATEMENTS TODO 
                                if (rotationZ > 0 && rotationZ <= 90)
                                {
                                    casingVelocity = new Vector3(1f - (rotationZ / 90), 0, -(rotationZ / 90)) * 90 * Random.Range(50, 100) * Time.deltaTime;
                                }
                                else if (rotationZ > 90 && rotationZ <= 180)
                                {
                                    casingVelocity = new Vector3(1f - (rotationZ / 90), 0, -1f + (rotationZ / 180)) * 90 * Random.Range(50, 100) * Time.deltaTime;
                                }
                                else if (rotationZ <= 0 && rotationZ >= -90)
                                {
                                    casingVelocity = new Vector3(1f + (rotationZ / 90), 0, -(rotationZ / 90)) * 90 * Random.Range(50, 100) * Time.deltaTime;
                                }
                                else if (rotationZ <= -90 && rotationZ >= -180)
                                {
                                    casingVelocity = new Vector3(1f + (rotationZ / 90), 0, 1 + (rotationZ / 180)) * 90 * Random.Range(50, 100) * Time.deltaTime;
                                }
                                //TEMPORÄR 4 IF STATEMENTS TODO ende
                                GameObject bulletCasing = poolManager.spawnObject(1);
                                bulletCasing.transform.position = this.transform.position;
                                bulletCasing.transform.rotation = Quaternion.Euler(90, rotationZ + Random.Range(-20, 20), 90f);
                                bulletCasing.GetComponent<Rigidbody>().velocity = casingVelocity;
                                bulletCasing.SetActive(true);
                                //BULLETCASING ende
                            }
                        }
                    }
                }
                else
                {
                    StartCoroutine(reload());
                }
            }
        }
        else
        {
            //enemy doesnt see player
            if (!patrolRunning)
            {
                //start search if not patroling
                if (!searchRunning)
                {
                    agent.speed = movementSpeed;
                    searchRunning = true;
                    StartCoroutine("enemySearch");
                }
            }
        }

        if (patrolRunning)
        {
            //start patrol if set to true
            enemyPatrol(patrolPoints);
        }
    }
    void enemyPatrol(Vector3[] points)
    {
        if (transform.position != points[patrolPos])
        {
            agent.SetDestination(points[patrolPos]);
        }
        else
        {
            if (startSide)
            {
                patrolPos++;
                if (patrolPos == points.Length - 1)
                {
                    startSide = false;
                }
            }
            else
            {
                patrolPos--;
                if (patrolPos == 0)
                {
                    startSide = true;
                }
            }
        }
    }
    IEnumerator enemySearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2.5f, 3f));
            agent.SetDestination(transform.position + new Vector3(Random.Range(-7.5f, 7.5f), 0f, Random.Range(-7.5f, 7.5f)));
        }
    }
    IEnumerator reload()
    {
        if (!isReloading)
        {
            audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + reloadSoundName), gameObject);
        }
        isReloading = true;
        yield return new WaitForSeconds(reloadTime / 1000f);
        ammoCapacity = maxAmmoCapacity;
        isReloading = false;
    }
}
