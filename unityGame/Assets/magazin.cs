using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using TMPro;

public class magazin : MonoBehaviour
{
    public TextMeshProUGUI textMesh; //UI Text Object
    public TextMeshProUGUI textMeshMags; //UI Text Object
    public bool magEmpty = false;

    public GameObject spieler;
    private float ammoCapacity;
    private float magCapacity;
    public float ammoLeft;
    public float magLeft;

    private float reloadTime;

    public void Start()
    {
        ammoCapacity = GameObject.Find("spieler").GetComponent<crosshair>().ammoCapacity;
        magCapacity = GameObject.Find("spieler").GetComponent<crosshair>().magCapacity;
        reloadTime = GameObject.Find("spieler").GetComponent<crosshair>().reloadTime;

         magLeft = magCapacity;
         ammoLeft = ammoCapacity;
         updateAmmoCount();
    }
    public bool checkMagsEmpty()
    {
        if (magLeft < 1)
            return true;
        return false;
    }
    public bool checkMagEmpty()
    {
        if (ammoLeft < 1)
        {
            magEmpty = true;
        }
        else
        {
            magEmpty = false;
        }
            
        return magEmpty;
    }

    public void removeBulletFromMag(int amount)
    {
        checkMagEmpty();

        ammoLeft = ammoLeft - 1;
            updateAmmoCount();

        checkMagEmpty();
    }

    public void reload()
    {
        StartCoroutine("reloadEnum");
    }
    public void stopReload()
    {
        StopCoroutine("reloadEnum");
    }
    IEnumerator reloadEnum()
    {
        yield return new WaitForSeconds(reloadTime/1000f);
        if (magLeft > 0)
        {
            ammoLeft = ammoCapacity;
            magLeft--;
            updateAmmoCount();
            magEmpty = false;
        }
        StopCoroutine("reloadEnum");
    }

    public void updateAmmoCount()
    {
        if (spieler.GetComponent<crosshair>().isMelee)
        {
            textMeshMags.text = "";
            textMesh.text = "";
        }
        else
        {
            textMeshMags.text = "/" + magLeft;
            textMesh.text = ammoLeft + "/" + ammoCapacity;
            if (ammoLeft < 10)
            {
                textMesh.transform.position = new Vector3(62.5f, 31f, 0f) * 1.5f;
            }
            else
            {
                textMesh.transform.position = new Vector3(55f, 31f, 0f) * 1.5f;
            }
        }

    }
}
