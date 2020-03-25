using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static audioManager;
using static poolManager;

public class crosshair : MonoBehaviour
{
    public GameObject crosshairs;
    public GameObject spieler;
    private Vector3 target;
    private float lastShot=0;

    public float weaponDamage = 100f;
    public float feuerRateMin = 900f;
    public float ammoCapacity = 30f;
    public float magCapacity = 2f;
    public float reloadTime = 0.5f;
    public float bulletSpeed = 3000f;
    public bool isMelee;
    public string itemId = "1";
    public string useSoundName = "1";
    public string reloadSoundName = "1reload";

    public GameObject ammoCapacityText;
    private magazin ammoCapacityTextMagazin;

    //only used in enemy.cs for hearing
    public bool canShoot;

    private Vector3 casingVelocity;

    // Start is called before the first frame update
    void Start()
    {
        ammoCapacityTextMagazin = ammoCapacityText.GetComponent<magazin>();
        Cursor.visible = false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //crosshairPosition
        Vector3 mouspos = Input.mousePosition;
        mouspos = Camera.main.ScreenToWorldPoint(mouspos);
        target = new Vector3(mouspos.x, 1f, mouspos.z);
        crosshairs.transform.position = target;
        //crosshairPosition ENDE

        //rotatePlayer
        Vector3 differnce = target - spieler.transform.position;
        float rotationZ = Mathf.Atan2(differnce.x, differnce.z)*Mathf.Rad2Deg;
        spieler.transform.rotation = Quaternion.Euler(0.0f, rotationZ, 0.0f);
        //rotatePlayer ENDE

        //schießen
        if (Input.GetMouseButton(0))
        {
            if (!isMelee)
            {   
                 if((lastShot-(Time.time*1000)) <= -(60000/ feuerRateMin))
                 {
                    if (!ammoCapacityTextMagazin.checkMagEmpty())
                    {
                        //mag not empty
                        canShoot = true;
                        audioManager.playClipOnObject(Resources.Load<AudioClip>("audio/itemSounds/" + useSoundName),spieler);//shoot sound effect

                        float distance = differnce.magnitude;
                        Vector3 direction = differnce / distance;
                        direction.Normalize();
                        
                        //SpawnBullet
                        GameObject bullet = poolManager.spawnObject(0);
                        bullet.transform.position = spieler.transform.position + (differnce.normalized / 2);
                        bullet.transform.rotation = Quaternion.Euler(90, rotationZ, 90f);
                        bullet.GetComponent<Rigidbody>().velocity = spieler.transform.TransformDirection(0f, 0f, bulletSpeed) * Time.deltaTime;
                        bulletCollision collison = bullet.GetComponent<bulletCollision>();
                        collison.enemyBullet = false;
                        bullet.SetActive(true);
                        collison.Start();

                        //Remove Bullet from Magazine and set lastShot to now
                        ammoCapacityTextMagazin.removeBulletFromMag(1);
                        lastShot = Time.time * 1000;

                        //Spawn bulletCasing
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
                        bulletCasing.transform.position = spieler.transform.position;
                        bulletCasing.transform.rotation = Quaternion.Euler(90, rotationZ + Random.Range(-20, 20), 90f);
                        bulletCasing.GetComponent<Rigidbody>().velocity = casingVelocity;
                        bulletCasing.SetActive(true);
                    }
                    else
                    {
                        //mag is empty
                        if (!ammoCapacityTextMagazin.checkMagsEmpty())
                            //There are Magazines left
                            ammoCapacityTextMagazin.reload();
                    }
                 }else
                    canShoot = false;
            }
        };
        //schießen ENDE
    }
}
