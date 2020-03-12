using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

    Animator anim;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();

    }
	
    public void animEnd()
    {
        anim.SetBool("Click", false);
    }

}
