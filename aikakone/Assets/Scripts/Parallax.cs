using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

    private float x;
    public float speed;
    private Vector3 mouseX;
    private Vector3 StartPos;
    public float limitx1;
    public float limitx2;
    public bool isActive;

    // Use this for initialization
    void Start () {
        StartPos = gameObject.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isActive)
        {
            return;
        }
        mouseX = Input.mousePosition;
        x = mouseX.x -= Screen.width / 2;
        transform.position = new Vector3 (Mathf.Clamp(x * speed, limitx1, limitx2), transform.position.y);
    }
}
