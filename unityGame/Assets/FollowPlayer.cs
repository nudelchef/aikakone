using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform spieler;
    public Transform kamera;
    public Vector3 offset;
    public float smoothSpeed;
    public float maxZoom;

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(kamera.transform.position);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Freeview
            Vector3 mouspos = Input.mousePosition;
            mouspos = Camera.main.ScreenToWorldPoint(mouspos);

            if (mouspos.x < -maxZoom + spieler.transform.position.x && mouspos.z > maxZoom + spieler.transform.position.z)
            {
                //Upper Left Corner
                smoothCamera(new Vector3(-maxZoom + spieler.transform.position.x, 3, maxZoom + spieler.transform.position.z) + offset);
            }
            else { 
            if (mouspos.x < -maxZoom + spieler.transform.position.x && mouspos.z < -maxZoom + spieler.transform.position.z)
            {
                //Under Left Corner
                smoothCamera(new Vector3(-maxZoom + spieler.transform.position.x, 3, -maxZoom + spieler.transform.position.z) + offset);
            }
            else { 
            if (mouspos.x > maxZoom+ spieler.transform.position.x && mouspos.z < -maxZoom+ spieler.transform.position.z)
            {
                //Under Right Corner
                smoothCamera(new Vector3(maxZoom+ spieler.transform.position.x, 3, -maxZoom+ spieler.transform.position.z) + offset);
            }
            else { 
            if (mouspos.x > maxZoom+ spieler.transform.position.x && mouspos.z> maxZoom+ spieler.transform.position.z)
            {
                //Upper Right Corner
                smoothCamera(new Vector3(maxZoom+ spieler.transform.position.x, 3, maxZoom+ spieler.transform.position.z) + offset);
            }
            else
            {
                //X-AXIS
                if ((mouspos.x - spieler.transform.position.x) < maxZoom && (mouspos.x - spieler.transform.position.x) > -maxZoom && (mouspos.z - spieler.transform.position.z) < maxZoom && (mouspos.z - spieler.transform.position.z) > -maxZoom)
                {
                    smoothCamera(mouspos + offset);
                }

                if ((mouspos.x - spieler.transform.position.x) > maxZoom)
                {
                    //Top
                    smoothCamera(new Vector3(spieler.transform.position.x + maxZoom, 3, mouspos.z) + offset);
                }

                if ((mouspos.x - spieler.transform.position.x) < -maxZoom)
                {
                    //Bottom
                    smoothCamera(new Vector3(spieler.transform.position.x - maxZoom, 3, mouspos.z) + offset);
                }
                //X-AXIS ende

                //Z-AXIS
                if ((mouspos.z - spieler.transform.position.z) > maxZoom)
                {
                    //Top
                    smoothCamera(new Vector3(mouspos.x, 3, spieler.transform.position.z + maxZoom) + offset);
                }
                if ((mouspos.z - spieler.transform.position.z) < -maxZoom)
                {
                    //Bottom
                    smoothCamera(new Vector3(mouspos.x, 3, spieler.transform.position.z - maxZoom) + offset);
                }
                //Z-AXIS ende
            }
            }
            }
            }
        }
        else
        {
            //NO Freeview
            smoothCamera(spieler.transform.position + offset);
        }
    }
    void smoothCamera(Vector3 endPosition) 
    {
        Vector3 smoothedPosition = Vector3.Lerp(kamera.transform.position, endPosition, smoothSpeed);
        kamera.transform.position = smoothedPosition;
    }
}
