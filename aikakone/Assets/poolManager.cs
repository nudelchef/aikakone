using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class poolManager : MonoBehaviour
{
    public GameObject bulletCasingPrefab;
    private static int maxCasingObjects = 20; //If this number of Casings in the List is reached the oldest will be reused
    private static List<GameObject> casings = new List<GameObject>();


    public GameObject bulletPrefab;
    private static int maxBulletObjects = 50; //If this number of Bullets in the List is reached the oldest will be reused
    private static List<GameObject> bullets = new List<GameObject>();


    void Start()
    {
        //Spawn BulletCasing Prefabs and Disable them for later use
        for (int i = 0; i < maxCasingObjects; i++)
        {
            GameObject b = Instantiate(bulletCasingPrefab) as GameObject;
            b.SetActive(false);
            casings.Add(b);
        }

        //Spawn Bullet Prefabs and Disable them for later use
        for (int i = 0; i < maxBulletObjects; i++)
        {
            GameObject b = Instantiate(bulletPrefab) as GameObject;
            b.SetActive(false);
            bullets.Add(b);
        }
    }

    static int casingCounter = 0;
    public static GameObject spawnBulletCasing(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        casings[casingCounter].transform.position = position;
        casings[casingCounter].transform.rotation = rotation;
        casings[casingCounter].GetComponent<Rigidbody>().velocity = velocity;
        casings[casingCounter].SetActive(true);

        if (casingCounter == maxCasingObjects-1)
            casingCounter = 0;
        else
            casingCounter++;

        return casings[casingCounter];
    }


    static int bulletCounter = 0;
    public static GameObject spawnBullet(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        bullets[bulletCounter].transform.position = position;
        bullets[bulletCounter].transform.rotation = rotation;
        bullets[bulletCounter].GetComponent<Rigidbody>().velocity = velocity;
        bullets[bulletCounter].SetActive(true);
        bullets[bulletCounter].GetComponent<bulletCollision>().Start();

        if (bulletCounter == maxBulletObjects-1)
            bulletCounter = 0;
        else
            bulletCounter++;

        return bullets[bulletCounter];
    }
}
