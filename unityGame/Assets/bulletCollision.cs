using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 12.5f);
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        if(hitInfo.name!="boden"&& hitInfo.name != "spieler" && hitInfo.name != "bullet(Clone)" && hitInfo.tag != "item")
        {
            try
            {
                hitInfo.GetComponent<enemy>().health = hitInfo.GetComponent<enemy>().health - GameObject.Find("spieler").GetComponent<crosshair>().weaponDamage;
            }
            catch { }
            Destroy(gameObject);
        }
    }
}
