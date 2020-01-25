using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform spieler;
    public Rigidbody spielerRB;
    public float speed = 50f;
    public Vector3 spawnPunkt;

    void FixedUpdate()
    {
        Vector3 spielerPositon = spieler.transform.position;
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        spielerRB.GetComponent<Rigidbody>().velocity = movement * speed * Time.deltaTime;
        //controls ENDE
    }
}