using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crosshair : MonoBehaviour
{
    public GameObject crosshairs;
    public GameObject spieler;
    public GameObject bulletPrefab;
    private Vector3 target;
    private float lastShot=0;

    public float weaponDamage = 100f;
    public float feuerRateMin = 900f;
    public float ammoCapacity = 30f;
    public float magCapacity = 2f;
    public float reloadTime = 0.5f;
    public float bulletSpeed = 3000f;
    public bool isMelee;


    //only used in enemy.cs for hearing
    public bool canShoot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //crosshairPosition
        Vector3 mouspos = Input.mousePosition;
        mouspos = Camera.main.ScreenToWorldPoint(mouspos);
        target = new Vector3(mouspos.x, 3.5f, mouspos.z);
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
                    if (!GameObject.Find("ammoCapacityText").GetComponent<magazin>().checkMagEmpty())
                    {
                        //mag not empty
                        canShoot = true;

                        float distance = differnce.magnitude;
                        Vector3 direction = differnce / distance;
                        direction.Normalize();
                        GameObject b = Instantiate(bulletPrefab) as GameObject;
                        b.transform.position = spieler.transform.position + (differnce.normalized / 2);
                        b.transform.rotation = Quaternion.Euler(90, rotationZ, 90f);
                        b.GetComponent<Rigidbody>().velocity = spieler.transform.TransformDirection(0f, 0f, bulletSpeed) * Time.deltaTime;
                        GameObject.Find("ammoCapacityText").GetComponent<magazin>().removeBulletFromMag(1);
                        lastShot = Time.time * 1000;
                    }
                    else
                    {
                        //mag is empty
                        if (!GameObject.Find("ammoCapacityText").GetComponent<magazin>().checkMagsEmpty())
                            //There are Magazines left
                            GameObject.Find("ammoCapacityText").GetComponent<magazin>().reload();
                    }
                 }else
                    canShoot = false;
            }
        };
        //schießen ENDE
    }
}
