using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SimpleJSON;
using System.IO;
using static countdown;


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



    //public vars for sight
    public float viewAngel = 120f;
    public float viewRadius = 10f;
    public float viewRememberTime = 3000f;

    //Melee or Distance
    public bool isEnemyMelee = false;
    public float meleeRadius;
    private float enemyDamage;

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
        timeWonInSeconds = int.Parse(enemyJSON[enemyID]["timeWonInSeconds"]);
        string itemId = enemyJSON[enemyID]["itemId"];
        enemyDamage = float.Parse(items[itemId]["weaponDamage"]);
        range = float.Parse(items[itemId]["range"]);
        this.GetComponent<Renderer>().material = Resources.Load<Material>("enemyTextures/" + enemyJSON[enemyID]["textureId"]); //Sets texture

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

                            float spielerEntfernung = Mathf.Sqrt(Mathf.Pow((spieler.transform.position.x - objects.transform.position.x), 2) + Mathf.Pow((spieler.transform.position.y - objects.transform.position.y), 2));

                            //Enemy in fight range
                            if (spielerEntfernung < meleeRadius)
                            {
                                //spielerLeben = spielerLeben - enemyDamage;

                                return;
                            }
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

            targetPosition = spieler.transform.position;
            agent.SetDestination(targetPosition);
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
}
