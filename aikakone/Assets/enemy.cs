using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemy : MonoBehaviour
{
    public Transform spieler;
    private Rigidbody rb;
    private Vector3 movement;
    private float lastSeen = float.MinValue;
    public float moveSpeed = 5f;
    public float runingSpeed = 20f;
    public Vector3[] patrolPoints;

    public float health = 100f;


    //public vars for sight
    public float viewAngel = 90f;
    public float viewRadius = 10f;
    public float viewRememberTime = 3000f;


    private Vector3 targetPosition;
    private bool searchRunning = false;
    private bool patrolRunning = true;

    //private vars for patrol
    private int patrolPos = 0;
    private bool startSide = true;


    public NavMeshAgent agent;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        //die
        if(health<1)
            Destroy(gameObject);

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

        if (((Time.time * 1000) - lastSeen ) <= viewRememberTime)
        {
            //enemy saw player
            patrolRunning = false;
            StopCoroutine("enemySearch");
            searchRunning = false;
            agent.speed = runingSpeed;
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
                    agent.speed = moveSpeed;
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
            agent.SetDestination(transform.position+new Vector3(Random.Range(-7.5f, 7.5f), 0f, Random.Range(-7.5f, 7.5f)));
        }
    }
}
