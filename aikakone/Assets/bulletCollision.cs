using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCollision : MonoBehaviour
{
    public bool enemyBullet = false;

    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(RemoveAfterSeconds(12.5f, gameObject));
    }

    void OnTriggerEnter(Collider hitInfo)
    {
        if (enemyBullet)
        {
            if (hitInfo.name != "boden" && hitInfo.name[0] != 'e' && hitInfo.name != "bullet(Clone)" && hitInfo.tag != "item" && hitInfo.name != "bulletCasing(Clone)")
            {
                try
                {
                    hitInfo.GetComponent<enemy>().health = hitInfo.GetComponent<enemy>().health - GameObject.Find("spieler").GetComponent<crosshair>().weaponDamage;
                }
                catch { }
                try
                {
                    hitInfo.GetComponent<objects>().objectsHealth = hitInfo.GetComponent<objects>().objectsHealth - GameObject.Find("spieler").GetComponent<crosshair>().weaponDamage;
                }
                catch { }
                //TODO SPIELER LEBEN ABZIEHEN SIEHE OBEN
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (hitInfo.name != "boden" && hitInfo.name != "spieler" && hitInfo.name != "bullet(Clone)" && hitInfo.tag != "item" && hitInfo.name != "bulletCasing(Clone)")
            {
                try
                {
                    hitInfo.GetComponent<enemy>().health = hitInfo.GetComponent<enemy>().health - GameObject.Find("spieler").GetComponent<crosshair>().weaponDamage;
                }
                catch { }
                try
                {
                    hitInfo.GetComponent<objects>().objectsHealth = hitInfo.GetComponent<objects>().objectsHealth - GameObject.Find("spieler").GetComponent<crosshair>().weaponDamage;
                }
                catch { }
                gameObject.SetActive(false);
            }
        }
    }
    IEnumerator RemoveAfterSeconds(float seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}