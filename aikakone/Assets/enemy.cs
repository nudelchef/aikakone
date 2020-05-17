using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;
using System.IO;
using static countdown;
using static audioManager;
using static poolManager;
using System.Threading;
using System.Globalization;
using System.Reflection;
using static userInterface;
using UnityEngine;

public class enemy : MonoBehaviour
{
    JSONNode enemyJSON;
	JSONNode perks;
    JSONNode items;
    JSONNode difficulty;
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
    public bool droppable;
    public bool eliteEnemy;
    public string itemId;
    public string itemType;
    public string textureDeadName;
    public bool justDied = true;
    public float difficultyDamageMultiplier;
    public float difficultyTimeMultiplier;

    //vars for elite-enemies
    public int perk;
    public int perkValue;
    private float lastUsed = 0;
    public float perkUsageRate;
    public string perkName;
    public string perkID;
    public int perkValueMin;
    public int perkValueMax;
    public int perkWeaponDamage;
    public float perkFireRateMin;
    public float perkMaxAmmoCapacity;
    public float perkMovementSpeed;
    public float perkRunningSpeed;
    public int perkConjureSlots = 0;
    public int perkBonusConjureSlots = -1;
    private float maxHealth;
    public string perkType;



   //vars for shooting
    private Vector3 casingVelocity;
    private Vector3 target;
    private float lastShot = 0;
    public float bulletSpeed = 3000f;
    public float feuerRateMin;
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
    public float meleeRateMin = 120f;
    public int weaponDamage;

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


    //////////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                              //
    //           Got it from dotnet-snippets.de/snippet/runden-vor-dem-komma/12063                  //
    //   All Rights belong to the respected Owner. (aka I try to not take credit for this method)   //                                                                 //
    //                                                                                              //
    //////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Method to round down a digit.
    /// </summary>
    /// <param name="input"> Object to round </param>
    /// <param name="length"> Length you want to round down. 
    /// <para>1 = Tens-Digi Digits </para> 
    /// <para>2 = Hundrets-Digits </para>
    /// <para>3 = Thousands-Digit</para> </param>
    /// <returns></returns>

    public static int overround(object input, int length = 0)
    {
        if (input is int | input is double | input is decimal | input is float)
        {
            double value = System.Convert.ToDouble(input);
            int div = (int)System.Math.Pow(10, length);
            return ((int)System.Math.Round(value / div, 0) * div);
        }
        throw new System.ArgumentException("Input is not numeric");
    }




    void Start()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

        spieler = GameObject.Find("spieler");

        //loading difficulty.json
        difficulty = JSON.Parse((Resources.Load("difficulty") as TextAsset).text);
        difficultyDamageMultiplier = float.Parse(difficulty["1"]["difficultyDamageMultiplier"]);
        difficultyTimeMultiplier = float.Parse(difficulty["1"]["difficultyTimeMultiplier"]);


        //loading item.json
        items = JSON.Parse((Resources.Load("items") as TextAsset).text);


        enemyID = this.name.Substring(1);

        rb = this.GetComponent<Rigidbody>();

        //loading enemy.json and setting stats
        enemyJSON = JSON.Parse((Resources.Load("enemy") as TextAsset).text);

        itemId = enemyJSON[enemyID]["itemId"];
        eliteEnemy = enemyJSON[enemyID]["eliteEnemy"];
        droppable = items[itemId]["droppable"];
        movementSpeed = float.Parse(enemyJSON[enemyID]["movementSpeed"]);
        runingSpeed = float.Parse(enemyJSON[enemyID]["runningSpeed"]);
        health = float.Parse(enemyJSON[enemyID]["enemyHealth"]);
        textureDeadName = enemyJSON[enemyID]["textureDeadName"];
        enemyPrecision = float.Parse(enemyJSON[enemyID]["enemyPrecision"]);
        timeWonInSeconds = int.Parse(enemyJSON[enemyID]["timeWonInSeconds"]);
        reloadSoundName = items[itemId]["reloadSoundName"];
        this.GetComponent<Renderer>().material = Resources.Load<Material>("enemyTextures/" + enemyJSON[enemyID]["textureName"]); //Sets texture																   
        itemType = items[itemId]["itemType"];
        if (itemType == "melee")
        {
            weaponDamage = int.Parse(items[itemId]["weaponDamage"]);
            range = float.Parse(items[itemId]["range"]);
        }
        else if (itemType == "gun")
        {
            reloadTime = float.Parse(items[itemId]["reloadTime"]);
            maxAmmoCapacity = float.Parse(items[itemId]["ammoCapacity"]);
            ammoCapacity = maxAmmoCapacity;
            weaponDamage = int.Parse(items[itemId]["weaponDamage"]);
			feuerRateMin = int.Parse(items[itemId]["feuerRateMin"]);
            range = float.Parse(items[itemId]["range"]);
        }

        //difficulty-multiplier gets applied
        weaponDamage = (weaponDamage * (int)(difficultyDamageMultiplier*100))/100;
        timeWonInSeconds = (timeWonInSeconds * (int)(difficultyTimeMultiplier*100))/100;

        //If it's an elite-enemy
        eliteEnemy = true;
        if (eliteEnemy)
        {
            perks = JSON.Parse((Resources.Load("perks") as TextAsset).text);
			
            //get a random "perk" by perks.txt
            perkID = Random.Range(1, 2).ToString();
            perkName = perks[perkID]["perkName"];
            perkValueMin = int.Parse(perks[perkID]["perkValueMin"]);
            perkValueMax = int.Parse(perks[perkID]["perkValueMax"]);
            perkUsageRate = float.Parse(perks[perkID]["perkUsageRate"]);
            perkMovementSpeed = float.Parse(perks[perkID]["perkMovementSpeed"]);
            perkRunningSpeed = float.Parse(perks[perkID]["perkRunningSpeed"]);
            perkMaxAmmoCapacity = float.Parse(perks[perkID]["perkMaxAmmoCapacity"]);
            perkFireRateMin = float.Parse(perks[perkID]["perkFireRateMin"]);
            perkWeaponDamage = int.Parse(perks[perkID]["perkWeaponDamage"]);
            perkType = perks[perkID]["perkType"];

            if(perkType == "fight")
            {
                if(perkName == "conjure")
                {
                    perkBonusConjureSlots++;
                }
            }

            //perk values and buffs/nerfs are getting set
            perkValue = Random.Range(perkValueMin, perkValueMax);
            perkValue = enemy.overround(perkValue, 1);
            this.health += perkValue;
            this.movementSpeed += perkMovementSpeed;
            this.runingSpeed += perkRunningSpeed;
            this.weaponDamage += perkWeaponDamage;
            this.maxAmmoCapacity += perkMaxAmmoCapacity;
            this.feuerRateMin += perkFireRateMin;
        }

        maxHealth = health;
        Debug.Log("Max Health:" + this.health.ToString());

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
            if (justDied) //if it just died
            {
                this.GetComponent<Renderer>().material = Resources.Load<Material>("enemyTextures/" + enemyJSON[enemyID]["textureDeadName"]); //sets corpse-texture
                agent.Stop(); //let's it stop moving

                if (droppable)
                {
                    GameObject itemObject = GameObject.Find("spieler").GetComponent<item>().spawnItem(this.itemId.ToString(), (this.transform.position + new Vector3(Random.Range(-3, 3), 0)), Random.Range(-360f, 360f)); //Spawn item mit der itemId 1 und den Koordinaten X:0 Y:1.5 Z:0
                    itemObject.GetComponent<itemStats>().ammoLeft = ammoCapacity; //Ammo the enemy had in stock while dying
                    itemObject.GetComponent<itemStats>().magLeft = Random.Range(1, 3) + Random.Range(0, 1); //Random Magazine-Count
                }

                countdown.timeLeft = countdown.timeLeft + timeWonInSeconds; //adds reward-time if enemy is killed
                userInterface.highscore = userInterface.highscore + timeWonInSeconds; //adds reward-time - which also are the actual points - to the highscore
                userInterface.enemiesDefeated++;


                //If it was a conjured "Lesser Demon" which died, the Conjurer gets a ConjureSlot back
                if (enemyID == "5")
                {
                    perkConjureSlots--;
                }
                justDied = false;
            }
            else //if it didn't just died, there's nothing to do for it
            {
                return;
            }
        }

        //perks which are used if they have usage without being in a fight
        if (perkType == "non-fight")
        {
            Invoke(perkName, 0f);
            //Debug.Log("Health:" + this.health.ToString());
        }
		
        //enemy sight
        Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, viewRadius);
        foreach (Collider objects in objectsInRadius)
        {
            //Check if Player or Bullet in sight Radius
            if (objects.name == "spieler" || objects.name == "bullet(Clone)")
            {
                if (Input.GetMouseButton(0) && spieler.GetComponent<crosshair>().itemType == "gun" && spieler.GetComponent<crosshair>().canShoot)
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
                    if (Physics.Raycast(transform.position, dirToTarget, out hit, Mathf.Infinity, ~((1 << 11) | (1 << 12) ) ))
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
            agent.speed = runingSpeed;

            //Check if Player or Bullet not obstructed by wall
            Vector3 dirToTarget = (spieler.transform.position - transform.position).normalized;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dirToTarget, out hit, Mathf.Infinity, ~((1 <<11) | (1 << 12)) ))
            {
                if (hit.collider.name == "spieler")
                {
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
                        }
                        //normal speed if both ok
                        else
                        {
                            agent.speed = runingSpeed;
                        }

                        Vector3 differnce = this.transform.position - spieler.transform.position;

                        differnce.Normalize();
                        differnce *= 1f;
                        agent.SetDestination(differnce + spieler.transform.position);

                        if ((lastShot - (Time.time * 1000)) <= -(60000 / meleeRateMin))
                        {
                            float spielerEntfernung = Mathf.Sqrt(Mathf.Pow((spieler.transform.position.x - this.transform.position.x), 2) + Mathf.Pow((spieler.transform.position.y - this.transform.position.y), 2));
                            // Debug.Log("Spielerentfernung:" + spielerEntfernung.ToString());
                            // Debug.Log("Range:" + range.ToString());
                            if (spielerEntfernung <= range)
                            {
                                audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + itemId), gameObject);
                                //TODO
                                //Player looses lifepoints
                                //hitInfo[i].GetComponent<PlayerHealth>().currentHealth = hitInfo[i].GetComponent<PlayerHealth>().currentHealth - weaponDamage; //TODO
                                Debug.Log("Gegner macht Schaden!");


                            }
                            lastShot = Time.time * 1000;
                        }
                    }
                    else
                    {
                        agent.updateRotation = false;

                        
                        Vector3 differnce = this.transform.position - spieler.transform.position;

                        differnce.Normalize();
                        differnce *= range;
                        agent.SetDestination(differnce + spieler.transform.position);
                        Vector3 lookAtCords = spieler.transform.position + new Vector3(((Random.Range(-100f, 100f) * (1f - enemyPrecision)) / 100f), 0, ((Random.Range(-100f, 100f) * (1f - enemyPrecision)) / 100f));

                        transform.LookAt(lookAtCords);

                        if (isReloading)
                        {
                            return;
                        }
                        if (ammoCapacity >= 1)
                        {

                            if ((lastShot - (Time.time * 1000)) <= -(60000 / feuerRateMin))
                            {
                                //shooting
                                audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + itemId), gameObject);//shoot sound effect

                                Vector3 unterschied = lookAtCords - this.transform.position;
                                float rotationZ = Mathf.Atan2(unterschied.x, unterschied.z) * Mathf.Rad2Deg;
                                float distance = unterschied.magnitude;
                                Vector3 direction = unterschied / distance;//TODO 20.04.2020 evtl. nicht mehr verwendet
                                direction.Normalize();//TODO 20.04.2020 evtl. nicht mehr verwendet

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
                        else
                        {
                            StartCoroutine(reload());
                        }
                    }
                }
                else
                {
                    agent.updateRotation = true;
                    agent.SetDestination(spieler.transform.position);
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
        agent.updateRotation = true;
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
        agent.updateRotation = true;
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
	void regenerate()
    {
        if ((lastUsed - (Time.time * 1000)) <= -(60000 / perkUsageRate))
        {
            if (this.health < this.maxHealth)
            {
                if ((this.health + perks[perkID]["perkRegenRate"]) <= this.maxHealth)
            {
                    this.health += perks[perkID]["perkRegenRate"];
                }
                else
                {
                    this.health = this.maxHealth;
                }
            }
	    lastUsed = Time.time * 1000;
        }

    }
    void conjuring()
    {
        if ((lastUsed - (Time.time * 1000)) <= -(60000 / perkUsageRate))
        {
            if (perkConjureSlots <= 2)
            {
                for (int x = perkConjureSlots; x <= 3 * perkBonusConjureSlots; x++)
                {
                enemy.spawnEnemy("5", (this.transform.position + new Vector3(Random.Range(1f, 2f), 0f, Random.Range(1f, 2f))), 0f);
                }
            }
	    lastUsed = Time.time * 1000;
        }
    }
}
